using Application.Services;
using Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EGrcoerAPI.Models;
using Nelibur.ObjectMapper;

namespace EGrocerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CartService _cartService;
        private readonly ProductService _productService;
        private readonly GetUnitNameUseCase _unitNameUseCase;

        public CartController(UserManager<IdentityUser> userManager, CartService cartService, ProductService productService, GetUnitNameUseCase unitNameUseCase)
        {
            _userManager = userManager;
            _cartService = cartService;
            _productService = productService;
            _unitNameUseCase = unitNameUseCase;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("Unauthorized");
            }

            var domaincart = _cartService.GetCart(user.UserName);
            var cart = TinyMapper.Map<Cart>(domaincart);
            return Ok(cart);
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddToCart(int qty, int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return Unauthorized();
            if (product == null) return NotFound("Product not found.");

            var cartItem = new Domain.CartItem
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = qty,
                ImagePath = product.ImagePath,
                Weight = product.Weight,
                Unit = await _unitNameUseCase.GetNameAsync(product.UnitID)
            };

            _cartService.AddToCart(cartItem, user.Id);

            return Ok(new { message = "Item added to cart successfully!" });
        }

        [HttpPost("remove/{productId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            _cartService.RemoveFromCart(productId, user.Id);
            return Ok(new { message = "Item removed from cart successfully!" });
        }

        [HttpPost("update")]
        [Authorize]
        public async Task<IActionResult> UpdateCart(int productId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            _cartService.UpdateCart(productId, quantity, user.Id);
            return Ok(new { message = "Cart updated successfully!" });
        }
    }
}
