using CoreBankingAPI.Data;
using CoreBankingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingAPI.Controllers
{
    public class BranchController : Controller
    {
        private readonly WebDbContext _context;

        public BranchController(WebDbContext context)
        {
            _context = context;
        }

        // GET: api/branches (Read All)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches()
        {
            var branches = await _context.Branches
                .OrderBy(b => b.Branchname)
                .ToListAsync();

            if (branches == null || !branches.Any())
            {
                return NotFound("No bank branches found.");
            }

            return Ok(branches);
        }

        // GET: api/branches/{branchId} (Read Single)
        [HttpGet("{branchId}")]
        public async Task<ActionResult<Branch>> GetBranch(string branchId)
        {
            var branch = await _context.Branches
                .FirstOrDefaultAsync(b => b.Branchid == branchId);

            if (branch == null)
            {
                return NotFound($"Branch with ID {branchId} not found.");
            }

            return branch;
        }

        // POST: api/branches (Create)
        [HttpPost]
        public async Task<ActionResult<Branch>> PostBranch(Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBranch), new { branchId = branch.Branchid }, branch);
        }

        // PUT: api/branches/{branchId} (Update)
        [HttpPut("{branchId}")]
        public async Task<IActionResult> PutBranch(string branchId, Branch branch)
        {
            if (branchId != branch.Branchid)
            {
                return BadRequest("Branch ID mismatch between URL and body.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(branch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Branches.Any(e => e.Branchid == branchId))
                {
                    return NotFound($"Branch with ID {branchId} not found for update.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/branches/{branchId} (Delete)
        [HttpDelete("{branchId}")]
        public async Task<IActionResult> DeleteBranch(int branchId)
        {
            var branch = await _context.Branches.FindAsync(branchId);

            if (branch == null)
            {
                return NotFound($"Branch with ID {branchId} not found.");
            }

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
