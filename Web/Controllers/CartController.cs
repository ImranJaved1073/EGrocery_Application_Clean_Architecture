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
            var cart = LoadCartFromCookies(user.Id);
            if (cart == null || cart.Items.Count == 0)
            {
                cart = new Cart();
                TempData["Message"] = "Your cart is empty";
            }
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AddToCart(int qty, int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);


            if (user != null)
            {
                if (product != null)
                {
                    var cart = LoadCartFromCookies(user.Id);

                    var cartItem = new CartItem
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = qty,
                        ImagePath = product.ImagePath,
                        //Quantity = p.Quantity > product.Quantity ? p.Quantity : product.Quantity,
                        Weight = product.Weight,
                        Unit = await _unitNameUseCase.GetNameAsync(product.UnitID)
                    };
                    if (cart == null )
                    {
                        cart = new();
                        cart.Items.Add(cartItem);
                        cart.TotalPrice = product.Price * qty;
                        cart.TotalQuantity = qty;
                    }

                    else
                    {
                        var item = cart.Items.Find(x => x.Id == product.Id);
                        if (item?.Quantity > product.Quantity)
                        {
                            cartItem.Quantity = product.Quantity;
                        }
                        if (item == null)
                        {
                            if (item?.Quantity > product.Quantity)
                            {
                                cartItem.Quantity = product.Quantity;
                            }
                            cart.Items.Add(cartItem);
                            cart.TotalPrice += cartItem.Price * cartItem.Quantity;
                            cart.TotalQuantity += cartItem.Quantity;
                        }
                        else
                        {
                            item.Quantity += qty;
                            if (item.Quantity > product.Quantity)
                            {
                                item.Quantity = product.Quantity;
                            }
                            cart.TotalPrice += item.Price * qty;
                            cart.TotalQuantity += qty;
                        }
                    }


                    SaveCartToCookies(user.Id, cart);

                    return Json(new
                    {
                        success = true,
                        totalQuantity = cart.TotalQuantity,
                        totalPrice = cart.TotalPrice,
                        message = "Item added to cart successfully!"
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Product not found." });
                }
            }
            else
            {
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account") });
            }
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

            var cart = LoadCartFromCookies(user.Id);
            var cartItem = cart.Items.Find(x => x.Id == id);
            if (cartItem != null)
            {
                cart.TotalPrice -= cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity -= cartItem.Quantity;
                cart.Items.Remove(cartItem);
                SaveCartToCookies(user.Id,cart);
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

            var cart = LoadCartFromCookies(user.Id);
            var cartItem = cart.Items.Find(x => x.Id == id);
            if (cartItem != null)
            {
                cart.TotalPrice -= cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity -= cartItem.Quantity;
                cartItem.Quantity = quantity;
                cart.TotalPrice += cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity += cartItem.Quantity;
                SaveCartToCookies(user.Id, cart);
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

        private Cart LoadCartFromCookies(string userId)
        {
            var cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", userId);
            if (cart != null && cart.UserId == userId)
            {
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }

            return cart!;
        }

        private void SaveCartToCookies(string userId, Cart cart)
        {
            if (cart != null)
            {
                CookieHelper.SetCookie(HttpContext, "Cart", cart, 4320, userId);
            }
        }
    }
}
