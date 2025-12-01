using CoreBankingAPI.Data;
using CoreBankingAPI.Models;
using CoreBankingAPI.View_Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;

namespace CoreBankingAPI.Controllers
{
    // DTO for Transfers
    public class TransferRequest
    {
        [Required, StringLength(15)]
        public string SourceAccountNumber { get; set; } = default!;

        [Required, StringLength(15)]
        public string DestinationAccountNumber { get; set; } = default!;

        [Range(0.01, 1000000000.00)]
        public decimal Amount { get; set; }

        [StringLength(100)]
        public string Description { get; set; } = string.Empty;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly WebDbContext _context;

        public TransactionController(WebDbContext context)
        {
            _context = context;
        }

        // ===========================================================
        //                  GET ALL RECENT TRANSACTIONS
        // ===========================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionHistoryViewModel>>> GetRecentTransactions()
        {
            var transactions = await _context.Transactions
                .OrderByDescending(t => t.Transactiontimestamp)
                .Take(100)
                .Select(t => new TransactionHistoryViewModel
                {
                    TransactionId = t.Transactionid,
                    ReferenceNumber = t.Referencenumber,
                    Amount = t.Amount,
                    TransactionType = t.Transactiontype,
                    TransactionTimestamp = t.Transactiontimestamp,
                    Status = t.Status,
                    AccountNumber = t.AccountidFkNavigation.Accountnumber,
                    AccountType = t.AccountidFkNavigation.Accounttype
                })
                .ToListAsync();

            if (!transactions.Any())
                return NotFound("No recent transactions found.");

            return Ok(transactions);
        }


        [HttpGet("TransactionReport")]
        public async Task<IActionResult> GetTransactionReport()
        {
            var transactionReport = await
            (
                from t in _context.Transactions
                join a in _context.Accounts
                    on t.AccountidFk equals a.Accountnumber

                select new View_Model.TransactionReportViewModel
                {
                    TransactionTimestamp = t.Transactiontimestamp,
                    AccountNumber = a.Accountnumber,
                    AccountType = a.Accounttype,
                    Amount = t.Amount
                }
            ).ToListAsync();

            return Ok(transactionReport);
        }





        // ===========================================================
        //                  GET: BY ACCOUNT NUMBER
        // ===========================================================
        [HttpGet("account/{accountNumber}")]
        public async Task<ActionResult<IEnumerable<TransactionHistoryViewModel>>> GetTransactionsByAccount(string accountNumber)
        {
            var transactions = await _context.Transactions
                .Where(t => t.AccountidFk == accountNumber)
                .OrderByDescending(t => t.Transactiontimestamp)
                .Select(t => new TransactionHistoryViewModel
                {
                    TransactionId = t.Transactionid,
                    ReferenceNumber = t.Referencenumber,
                    Amount = t.Amount,
                    TransactionType = t.Transactiontype,
                    TransactionTimestamp = t.Transactiontimestamp,
                    Status = t.Status,
                    AccountNumber = t.AccountidFkNavigation.Accountnumber,
                    AccountType = t.AccountidFkNavigation.Accounttype
                })
                .ToListAsync();

            if (!transactions.Any())
                return NotFound($"No transactions found for Account {accountNumber}.");

            return Ok(transactions);
        }


        // ===========================================================
        //                  POST SIMPLE TRANSACTION
        // ===========================================================
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await using var db = await _context.Database.BeginTransactionAsync();
            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Accountnumber == transaction.AccountidFk);
                if (account == null) return NotFound("Account not found.");

                if (transaction.Transactiontype == "Deposit") account.CurrentBalance += transaction.Amount;
                else if (transaction.Transactiontype == "Withdrawal")
                {
                    if (account.CurrentBalance < transaction.Amount) return BadRequest("Insufficient balance.");
                    account.CurrentBalance -= transaction.Amount;
                }
                else return BadRequest("Unsupported transaction type! Use /transfer endpoint for transfers.");

                transaction.Transactiontimestamp = DateTime.UtcNow;
                transaction.Referencenumber = Guid.NewGuid().ToString();
                _context.Transactions.Add(transaction);
                _context.Accounts.Update(account);

                await _context.SaveChangesAsync();
                await db.CommitAsync();

                return CreatedAtAction(nameof(GetTransactionsByAccount), new { accountNumber = transaction.AccountidFk }, transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ===========================================================
        //                  POST TRANSFER BETWEEN ACCOUNTS
        // ===========================================================
        [HttpPost("transfer")]
        public async Task<IActionResult> PostTransfer([FromBody] TransferRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (req.SourceAccountNumber == req.DestinationAccountNumber) return BadRequest("Same accounts not allowed.");

            await using var db = await _context.Database.BeginTransactionAsync();
            try
            {
                var source = await _context.Accounts.FirstOrDefaultAsync(a => a.Accountnumber == req.SourceAccountNumber);
                var dest = await _context.Accounts.FirstOrDefaultAsync(a => a.Accountnumber == req.DestinationAccountNumber);
                if (source == null || dest == null) return NotFound("Account not found.");

                if (source.CurrentBalance < req.Amount) return BadRequest("Insufficient funds.");

                source.CurrentBalance -= req.Amount;
                dest.CurrentBalance += req.Amount;
                source.Lastupdated = dest.Lastupdated = DateTime.UtcNow;

                var debit = new Transaction
                {
                    AccountidFk = req.SourceAccountNumber,
                    Transactiontype = "Transfer",
                    Amount = req.Amount,
                    Referencenumber = Guid.NewGuid().ToString(),
                    Status = "Completed",
                    Transactiontimestamp = DateTime.UtcNow
                };

                var credit = new Transaction
                {
                    AccountidFk = req.DestinationAccountNumber,
                    Transactiontype = "Transfer",
                    Amount = req.Amount,
                    Referencenumber = Guid.NewGuid().ToString(),
                    Status = "Completed",
                    Transactiontimestamp = DateTime.UtcNow
                };

                _context.Update(source);
                _context.Update(dest);
                _context.Transactions.AddRange(debit, credit);

                await _context.SaveChangesAsync();
                await db.CommitAsync();

                return Ok(new { Message = "Transfer Successful", DebitRef = debit.Referencenumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ===========================================================
        //        PUT – UPDATE TRANSACTION (Only Status)
        // ===========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] string status)
        {
            var txn = await _context.Transactions.FindAsync(id);
            if (txn == null)
                return NotFound("Transaction not found.");

            // Update only existing column
            txn.Status = status;
            txn.Lastupdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("Transaction status updated successfully.");
        }


        // ===========================================================
        //         DELETE – SOFT DELETE ONLY (NO RECORD REMOVED)
        // ===========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var txn = await _context.Transactions.FindAsync(id);
            if (txn == null) return NotFound("Transaction not found.");

            txn.Status = "Cancelled";     // <--- Safe delete without removing record!
            txn.Lastupdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("Transaction marked as Cancelled.");
        }
    }
}
