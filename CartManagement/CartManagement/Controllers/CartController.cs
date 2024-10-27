using CartManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CartManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;

        public CartController(IConfiguration configuration, ApplicationDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        private async Task<bool> IsActiveAsync()
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return false;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            return user?.IsActive ?? false;
        }

        [Authorize]
        [HttpPost]
        [Route("view")]
        public async Task<IActionResult> ViewCart()
        {
            if (!await IsActiveAsync())
                return BadRequest("User is not signed in.");

            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var cartItems = await _context.Carts
                .Where(c => c.OwnerEmail == email)
                .Select(c => new CartItemView
                {
                    ProductName = c.ProductName,
                    Quantity = c.Quantity,
                    TotalPrice = c.TotalPrice
                })
                .ToListAsync();

            if (cartItems.Count == 0)
                return Ok("Cart is empty");

            return Ok(cartItems);
        }

        [Authorize]
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddItem(AURequest item)
        {
            if (!await IsActiveAsync())
                return BadRequest("User is not signed in.");

            var product = await _context.Products.FindAsync(item.ProdId);

            if (product == null)
                return BadRequest("Product not found");

            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var cartItem = new Cart
            {
                OwnerEmail = email,
                ProductId = product.ProdId,
                ProductName = product.ProdName,
                Quantity = item.Quantity,
                TotalPrice = product.ProdPrice * item.Quantity
            };

            _context.Carts.Add(cartItem);
            await _context.SaveChangesAsync();

            return Ok("Item added to cart");
        }

        // Other methods (delete, update, clear) can be refactored similarly...

        // Ensure isActive method is refactored accordingly...
    }
}
