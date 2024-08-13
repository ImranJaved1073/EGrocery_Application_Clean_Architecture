using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Application.Services;
using Application.UseCases;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ProductService _productService;
        private readonly GetUnitNameUseCase _unitNameUseCase;

        public CartController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ProductService productService, GetUnitNameUseCase unitNameUseCase)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _productService = productService;
            _unitNameUseCase = unitNameUseCase;
        }

        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            LoadCartFromCookies(user.Id);
            var cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id);
            if (cart == null || cart.Items.Count == 0)
            {
                cart = new Cart();
                TempData["Message"] = "Your cart is empty";
            }
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(Product p, int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (product != null)
                {
                    var cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id) ?? new Cart();

                    var cartItem = new CartItem
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = p.Quantity,
                        ImagePath = product.ImagePath,
                        Weight = product.Weight,
                        Unit = await _unitNameUseCase.GetNameAsync(product.UnitID)
                    };
                    var item = cart.Items.Find(x => x.Id == product.Id);
                    if (item == null)
                    {
                        cart.Items.Add(cartItem);
                        cart.TotalPrice += cartItem.Price * cartItem.Quantity;
                        cart.TotalQuantity += cartItem.Quantity;
                    }
                    else
                    {
                        item.Quantity += p.Quantity;
                        cart.TotalPrice += item.Price * p.Quantity;
                        cart.TotalQuantity += p.Quantity;
                    }
                    CookieHelper.SetCookie(HttpContext, "Cart", cart, 4320, user.Id);
                }
                else return NotFound();
            }
            else return RedirectToAction("Login", "Account");
            return RedirectToAction("ShopItems", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id);
            var cartItem = cart.Items.Find(x => x.Id == id);
            if (cartItem != null)
            {
                cart.TotalPrice -= cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity -= cartItem.Quantity;
                cart.Items.Remove(cartItem);
                CookieHelper.SetCookie(HttpContext, "Cart", cart, 4320, user.Id);
            }
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateCart(int id, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id);
            var cartItem = cart.Items.Find(x => x.Id == id);
            if (cartItem != null)
            {
                cart.TotalPrice -= cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity -= cartItem.Quantity;
                cartItem.Quantity = quantity;
                cart.TotalPrice += cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity += cartItem.Quantity;
                CookieHelper.SetCookie(HttpContext, "Cart", cart, 4320, user.Id);
            }
            return RedirectToAction("Cart");
        }

        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            LoadCartFromCookies(user.Id);
            var cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id);
            if (cart == null || cart.Items.Count == 0)
            {
                return RedirectToAction("Cart");
            }
            else
            {
                ViewBag.Cart = cart;
                return View();
            }
        }

        private void LoadCartFromCookies(string userId)
        {
            var cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", userId);
            if (cart != null && cart.UserId == userId)
            {
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }
        }

        private void SaveCartToCookies(string userId)
        {
            var cartData = HttpContext.Session.GetString("Cart");
            if (cartData != null)
            {
                var cart = JsonConvert.DeserializeObject<Cart>(cartData);
                if (cart != null)
                {
                    cart.UserId = userId;
                    CookieHelper.SetCookie(HttpContext, "Cart", cart, 4320, userId);
                }
            }
        }
    }
}
