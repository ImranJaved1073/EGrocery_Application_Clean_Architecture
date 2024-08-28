using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Application.Services;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : Controller
    {
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private readonly CategoryService _categoryService;

        public AdminController(ProductService productService, OrderService orderService, CategoryService categoryService)
        {
            _productService = productService;
            _orderService = orderService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            ViewBag.TotalSales = orders.Sum(x => x.TotalBill);
            ViewBag.TotalOrders = orders.Count();
            ViewBag.TotalProducts = (await _productService.GetAllProductsAsync()).Count();
            ViewBag.TotalCategories = (await _categoryService.GetAllAsync()).Count();
            return View();
        }
    }
}
