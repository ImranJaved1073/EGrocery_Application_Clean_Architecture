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
        private readonly ICategoryRepository _categoryRepository;

        public AdminController(ProductService productService, OrderService orderService, ICategoryRepository categoryRepository)
        {
            _productService = productService;
            _orderService = orderService;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Dashboard()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            ViewBag.TotalSales = orders.Sum(x => x.TotalBill);
            ViewBag.TotalOrders = orders.Count();
            ViewBag.TotalProducts = (await _productService.GetAllProductsAsync()).Count();
            ViewBag.TotalCategories = (await _categoryRepository.GetAsync()).Count();
            return View();
        }
    }
}
