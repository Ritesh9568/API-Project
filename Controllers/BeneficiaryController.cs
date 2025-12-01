using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreBankingAPI.Data;
using CoreBankingAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CoreBankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryController : Controller
    {
        private readonly WebDbContext _context;

        public BeneficiaryController(WebDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------------------------------------
        // NEW ENDPOINT: GET Beneficiary Report
        // Logic updated: Removed join to TransferLimits since the table does not exist.
        // ----------------------------------------------------------------------------------
        [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<BeneficiaryReportViewModel>>> GetBeneficiaryReportData()
        {
            var reportData = await (
                // 1. Start with Beneficiaries (The Core Entity)
                from b in _context.Beneficiaries

                    // 2. Removed the Left Join to TransferLimits to resolve database execution errors.

                    // 3. Project into the BeneficiaryReportViewModel
                select new BeneficiaryReportViewModel
                {
                    // Beneficiary Details (Direct Mapping)
                    BeneficiaryId = b.Beneficiaryid,
                    Nickname = b.Beneficiaryname,
                    ExternalAccountNumber = b.Beneficiaryaccountnum,

                    // Originating Account Details (Correlated Subquery)
                    // Finds the primary or first account associated with the customer of the beneficiary.
                    OriginatingAccountNumber = _context.Accounts
                        // Filters accounts by the customer ID linked to the beneficiary
                        .Where(a => a.Customer == b.CustomeridFk)
                        .OrderBy(a => a.Accountnumber) // Ensures a consistent account is selected
                        .Select(a => a.Accountnumber)
                        .FirstOrDefault(), // Returns the account number or null

                    OriginatingAccountType = _context.Accounts
                        .Where(a => a.Customer == b.CustomeridFk)
                        .OrderBy(a => a.Accountnumber)
                        .Select(a => a.Accounttype)
                        .FirstOrDefault(),

                    // Transfer Limits - Set to null since the source table (TransferLimits) does not exist.
                    MaxTransferLimit = null
                }
            )
            .ToListAsync();

            if (reportData == null || !reportData.Any())
            {
                return NotFound("No beneficiary report data found.");
            }

            return Ok(reportData);
        }

        // GET: api/beneficiaries/customer/{customerId} (Read by Foreign Key)
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Beneficiary>>> GetBeneficiariesByCustomer(string customerId)
        {
            var beneficiaries = await _context.Beneficiaries
                .Where(b => b.CustomeridFk == customerId)
                .ToListAsync();

            if (beneficiaries == null || !beneficiaries.Any())
            {
                return NotFound($"No beneficiaries found for Customer ID {customerId}.");
            }

            return Ok(beneficiaries);
        }

        // GET: api/beneficiaries/{beneficiaryId} (Read Single)
        [HttpGet("{beneficiaryId}")]
        public async Task<ActionResult<Beneficiary>> GetBeneficiary(int beneficiaryId)
        {
            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(b => b.Beneficiaryid == beneficiaryId);

            if (beneficiary == null)
            {
                return NotFound($"Beneficiary with ID {beneficiaryId} not found.");
            }

            return beneficiary;
        }

        // GET: api/beneficiaries (Read All)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Beneficiary>>> GetBeneficiaries()
        {
            var beneficiaries = await _context.Beneficiaries
                .ToListAsync();

            if (beneficiaries == null || !beneficiaries.Any())
            {
                return NotFound("No beneficiaries found.");
            }

            return Ok(beneficiaries);
        }

        // POST: api/beneficiaries (Create)
        [HttpPost]
        public async Task<ActionResult<Beneficiary>> PostBeneficiary(Beneficiary beneficiary)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBeneficiary), new { beneficiaryId = beneficiary.Beneficiaryid }, beneficiary);
        }

        // PUT: api/beneficiaries/{beneficiaryId} (Update)
        [HttpPut("{beneficiaryId}")]
        public async Task<IActionResult> PutBeneficiary(int beneficiaryId, Beneficiary beneficiary)
        {
            if (beneficiaryId != beneficiary.Beneficiaryid)
            {
                return BadRequest("Beneficiary ID mismatch between URL and body.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(beneficiary).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Beneficiaries.Any(e => e.Beneficiaryid == beneficiaryId))
                {
                    return NotFound($"Beneficiary with ID {beneficiaryId} not found for update.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/beneficiaries/{beneficiaryId} (Delete)
        [HttpDelete("{beneficiaryId}")]
        public async Task<IActionResult> DeleteBeneficiary(int beneficiaryId)
        {
            var beneficiary = await _context.Beneficiaries.FindAsync(beneficiaryId);

            if (beneficiary == null)
            {
                return NotFound($"Beneficiary with ID {beneficiaryId} not found.");
            }

            _context.Beneficiaries.Remove(beneficiary);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}