using CoreBankingAPI.Data;
using CoreBankingAPI.Models;
using CoreBankingAPI.View_Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq; // Required for LINQ extensions like Select and ToListAsync
using System.Threading.Tasks;

namespace CoreBankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly WebDbContext _context;

        public CustomerController(WebDbContext context)
        {
            _context = context;
        }

        // GET: api/Customer (List all customers using LINQ projection)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerBasicViewModel>>> GetCustomers()
        {
            // LINQ Projection 1: Simple projection for the list view, mapping data to CustomerBasicViewModel.
            var customers = await _context.Customers
                .Select(c => new CustomerBasicViewModel
                {
                    CustomerId = c.Customerid,
                    FullName = c.Firstname + " " + c.Lastname,
                    MobileNumber = c.Mobilenumber,
                    KycStatus = c.Kycstatus,
                    IsActive = c.Isactive
                })
                .ToListAsync();

            if (customers == null || !customers.Any())
            {
                return NotFound("No customers found.");
            }

            return Ok(customers);
        }

        // GET: api/Customer/{id} (Get detailed customer profile with nested accounts using LINQ projection)
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDetailViewModel>> GetCustomer(string id)
        {
            // LINQ Projection 2: Complex projection with a nested SELECT for child collection (Accounts).
            var customerDetail = await _context.Customers
                .Where(c => c.Customerid == id)
                .Select(c => new CustomerDetailViewModel
                {
                    // Map core Customer data
                    CustomerId = c.Customerid,
                    FirstName = c.Firstname,
                    LastName = c.Lastname,
                    // Note: Use .ToString() for clean output if DateOfBirth is DateTime
                    DateOfBirth = c.Dateofbirth.ToString("yyyy-MM-dd"),
                    Address = c.Address,
                    MobileNumber = c.Mobilenumber,
                    KycStatus = c.Kycstatus,

                    // Nested JOIN and PROJECTION into the AccountSummaryViewModel list
                    // This creates a list of AccountSummaryViewModels for the client to consume.
                    Accounts = c.Accounts.Select(a => new AccountSummaryViewModel
                    {
                        AccountNumber = a.Accountnumber,
                        AccountType = a.Accounttype,
                        CurrentBalance = a.CurrentBalance,
                        // CustomerFullName and MobileNumber are omitted here as the parent VM already contains the customer info.
                    }).ToList()
                })
                .FirstOrDefaultAsync(); // Get a single result

            if (customerDetail == null)
            {
                return NotFound($"Customer ID {id} not found.");
            }

            return Ok(customerDetail);
        }

        // POST: api/customers (Create)
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { customerId = customer.Customerid }, customer);
        }

        // PUT: api/customers/{customerId} (Update)
        [HttpPut("{customerId}")]
        public async Task<IActionResult> PutCustomer(string customerId, Customer customer)
        {
            if (customerId != customer.Customerid)
            {
                return BadRequest("Customer ID mismatch between URL and body.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Instruct EF Core to track this object for updates
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Check if the customer still exists before failing
                if (!_context.Customers.Any(e => e.Customerid == customerId))
                {
                    return NotFound($"Customer with ID {customerId} not found for update.");
                }
                else
                {
                    throw; // Re-throw other concurrency exceptions
                }
            }

            // Standard response for a successful update that returns no content
            return NoContent();
        }

        // DELETE: api/customers/{customerId} (Delete)
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(string customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);

            if (customer == null)
            {
                return NotFound($"Customer with ID {customerId} not found.");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}