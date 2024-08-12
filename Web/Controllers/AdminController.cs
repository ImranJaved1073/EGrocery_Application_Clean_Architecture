using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Application.Services;

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

        public IActionResult Dashboard()
        {
            var orders = _orderService.GetAllOrders();
            ViewBag.TotalSales = orders.Sum(x => x.TotalBill);
            ViewBag.TotalOrders = _orderService.GetAllOrders().Count();
            ViewBag.TotalProducts = _productService.GetAllProducts().Count();
            ViewBag.TotalCategories = _categoryRepository.Get().Count();
            return View();
        }
    }
}
