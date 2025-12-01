using AutoMapper;
using CoreBankingAPI.Data;
using CoreBankingAPI.Models; // Contains Account, Customer, and ReportViewModels
using CoreBankingAPI.View_Model; // <-- Using View_Model namespace

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly WebDbContext _context;
       

        public AccountController(WebDbContext context)
        {
            _context = context;
            
        }



        // GET: api/accounts (Read All)
        // 2. Change return type to the View Model list
        [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<AccountSummaryViewModel>>> GetAccountsReport()
        {
            // Simplified LINQ Implementation: Inner Join (Account) -> Inner Join (Customer)
            // Removed the MstLoans Left Join as the entity was not successfully scaffolded
            // and its data was not required for the AccountSummaryViewModel projection.

            var query = from a in _context.Accounts // Start with the Accounts table

                            // 1. Inner Join: Account to Customer (Mandatory link)
                        join c in _context.Customers on a.Customer equals c.Customerid

                        // 2. Final Projection to the ViewModel
                        select new AccountSummaryViewModel
                        {
                            // Account Details
                            AccountNumber = a.Accountnumber,
                            AccountType = a.Accounttype,
                            CurrentBalance = a.CurrentBalance,

                            // Customer Details
                            CustomerFullName = c.Firstname + " " + c.Lastname,
                            CustomerMobileNumber = c.Mobilenumber // Assuming MobileNumber exists on the Customer entity
                        };

            var accountsSummary = await query.ToListAsync();

            if (accountsSummary == null || !accountsSummary.Any())
            {
                return NotFound("No bank accounts found in the database.");
            }

            return Ok(accountsSummary);
        }
        // GET: api/accounts/{accountNumber} (Read Single)
        [HttpGet("{accountNumber}")]
        public async Task<ActionResult<Account>> GetAccount(string accountNumber)
        {
            // This method continues to return the full Account entity for simplicity.
            var account = await _context.Accounts // Renamed to _context.Accounts from _context.Account
                .FirstOrDefaultAsync(a => a.Accountnumber == accountNumber);

            if (account == null)
            {
                return NotFound($"Account number {accountNumber} not found.");
            }

            return account;
        }

        // GET: api/accounts/customer/{customerId} (Read by Foreign Key)
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccountsByCustomer(string customerId)
        {
            var accounts = await _context.Accounts // Renamed to _context.Accounts from _context.Account
                .Where(a => a.Customer == customerId)
                .ToListAsync();

            if (accounts == null || !accounts.Any())
            {
                return NotFound($"No accounts found for Customer ID {customerId}.");
            }

            return Ok(accounts);
        }

        // POST: api/accounts (Create)
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Accounts.Add(account); // Renamed to _context.Accounts from _context.Account
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAccount), new { accountNumber = account.Accountnumber }, account);
        }

        // PUT: api/accounts/{accountNumber} (Update)
        [HttpPut("{accountNumber}")]
        public async Task<IActionResult> PutAccount(string accountNumber, Account account)
        {
            if (accountNumber != account.Accountnumber)
            {
                return BadRequest("Account number mismatch between URL and body.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Accounts.Any(e => e.Accountnumber == accountNumber)) // Renamed to _context.Accounts
                {
                    return NotFound($"Account with number {accountNumber} not found for update.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/accounts/{accountNumber} (Delete)
        [HttpDelete("{accountNumber}")]
        public async Task<IActionResult> DeleteAccount(string accountNumber)
        {
            var account = await _context.Accounts.FindAsync(accountNumber); // Renamed to _context.Accounts

            if (account == null)
            {
                return NotFound($"Account with number {accountNumber} not found.");
            }

            // Soft delete: update status to 'Closed'
            account.Status = "Closed";
            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}