using ProductCatalog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;

        public ProductsController(IConfiguration configuration, ApplicationDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        [Route("view")]
        public async Task<IActionResult> ViewCart()
        {
            if (!await IsActiveAsync())
            {
                return BadRequest("User is not signed in.");
            }

            var products = await _context.Products.ToListAsync();

            return Ok(products);
        }

        private async Task<bool> IsActiveAsync()
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            var user = await _context.Users
                                    .Where(u => u.Email == email)
                                    .FirstOrDefaultAsync();

            return user?.IsActive ?? false;
        }
    }
}
