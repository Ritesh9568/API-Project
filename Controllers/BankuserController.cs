using CoreBankingAPI.Data;
using CoreBankingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankuserController : Controller
    {
        private readonly WebDbContext _context;

        public BankuserController(WebDbContext context)
        {
            _context = context;
        }

        // GET: api/bankusers (Read All with Branch Details via LINQ Join)
        // Return type is changed to BankUserWithBranchVM to expose consolidated data.
        [HttpGet]
        public async Task<List<BankUserReportViewModel>> GetBankUserReportData()
        {
            // The LINQ query uses standard query syntax to perform an Inner Join.
            var userReportData = await (
                // 1. Start with BankUsers
                from bu in _context.Bankusers

                    // 2. Perform Inner Join with Branches
                    // FIX: The property bu.BranchId is missing. We assume the actual 
                    // database column name used for the foreign key on the BankUser table is 
                    // 'branchidfK' (lowercase to align with the rest of your model's conventions).
                    // This line must match the actual column name in the BankUser database table.
                join b in _context.Branches on bu.BranchidFk equals b.Branchid

                // 3. Project the result into the BankUserReportViewModel
                select new BankUserReportViewModel
                {
                    // Bankuser (bu) Mappings - Use lowercase properties to match the Bankuser model
                    UserId = bu.Userid,
                    Username = bu.Username,
                    Role = bu.Role,
                    IsActive = bu.Isactive,
                    CreatedOn = bu.Createdon,
                    LastLoggedIn = bu.Lastloggedin,

                    // Branch (b) Mappings - Ensure property casing matches the model
                    BranchId = b.Branchid,
                    BranchName = b.Branchname,
                    IfscCode = b.Ifsccode,
                    BranchCity = b.City
                }
            ).ToListAsync(); // Executes the query asynchronously against the database

            return userReportData;
        }

        // GET: api/bankusers/{id} (Read Single)
        [HttpGet("{id}")]
        public async Task<ActionResult<Bankuser>> GetBankUser(int id)
        {
            var user = await _context.Bankusers
                .FirstOrDefaultAsync(u => u.Userid == id); // Using scaffolded property name UserId

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return user;
        }

        // POST: api/bankusers (Create)
        [HttpPost]
        public async Task<ActionResult<Bankuser>> PostBankUser(Bankuser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // NOTE: In production, the PasswordHash MUST be securely generated before saving.
            _context.Bankusers.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBankUser), new { id = user.Userid }, user);
        }

        // PUT: api/bankusers/{id} (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankUser(int id, Bankuser user)
        {
            if (id != user.Userid)
            {
                return BadRequest("User ID mismatch between URL and body.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Note: Password changes should have a separate secure endpoint.
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bankusers.Any(e => e.Userid == id))
                {
                    return NotFound($"User with ID {id} not found for update.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/bankusers/{id} (Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankUser(int id)
        {
            var user = await _context.Bankusers.FindAsync(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            // Soft delete is highly recommended for security/audit purposes
            user.Isactive = false;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}