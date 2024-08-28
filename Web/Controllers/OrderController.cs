using Application.Services;
using Application.UseCases;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Threading.Tasks;
using Web.Hubs;
using Web.Models;

namespace Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<OrderController> _logger;
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private readonly OrderDetailService _orderDetailService;
        private readonly GetUnitNameUseCase _unitNameUsecase;
        private readonly IHubContext<OrderNotificationHub> _hubContext;

        public OrderController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            OrderService orderService, OrderDetailService orderDetailService, ILogger<OrderController> logger,
            ProductService productService, GetUnitNameUseCase unitNameUseCase, IHubContext<OrderNotificationHub> hubContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _logger = logger;
            _productService = productService;
            _unitNameUsecase = unitNameUseCase;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddOrder(CheckOut odrs)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id);
                if (cart != null)
                {
                    var order = new Orders
                    {
                        UserId = user.Id,
                        OrderNum = GenerateOrderNumber(),
                        OrderDate = DateTime.Now,
                        Status = "Pending",
                        TotalBill = cart.TotalPrice,
                        OrderDeliveryDate = DateTime.Now.AddDays(1),
                        PaymentMethod = "Cash on Delivery",
                        Address = odrs.Address,
                        City = odrs.City ?? "",
                        ZipCode = odrs.ZipCode,
                        State = odrs.State ?? "",
                        OrderDetails = cart.Items.Select(x => new OrderDetail
                        {
                            ProductId = x.Id,
                            Price = x.Price,
                            Quantity = x.Quantity,
                            Discount = 0,
                            TotalPrice = x.Price * x.Quantity,
                        }).ToList()
                    };

                    await _orderService.CreateOrderAsync(order);

                    foreach (var item in order.OrderDetails)
                    {
                        item.OrderId = (await _orderService.GetbyOrderNoAsync(order.OrderNum)).Id;
                        var product = await _productService.GetProductByIdAsync(item.ProductId);
                        product.Quantity -= item.Quantity;
                        await _productService.UpdateProductAsync(product);
                        await _orderDetailService.AddOrderDetailAsync(item);
                    }

                    await _hubContext.Clients.All.SendAsync("ReceiveOrderStatus", order.OrderNum, "Pending");

                    CookieHelper.ClearCartCookies(HttpContext, user.Id);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Order(string statusFilter, DateTime? startDate, DateTime? endDate)
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = (await _orderService.GetAllOrdersAsync()).ToList();
            var status = orders.Select(o => o.Status).Distinct().ToList();
            ViewBag.StatusOptions = status;
            orders = orders.Where(x => x.UserId == user?.Id).ToList();
            if (!string.IsNullOrEmpty(statusFilter))
            {
                orders = orders.Where(x => x.Status == statusFilter).ToList();
            }
            if (startDate.HasValue && endDate.HasValue)
            {
                orders = orders.Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate).ToList();
            }
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var orders = (await _orderService.GetAllOrdersAsync()).ToList();
            var order = orders.FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            order.OrderDetails = (await _orderDetailService.GetAllAsync()).Where(x => x.OrderId == id).ToList();
            foreach (var item in order.OrderDetails)
            {
                item.Product = await _productService.GetProductByIdAsync(item.ProductId);
                item.Product.UnitName = await _unitNameUsecase.GetNameAsync(item.Product.UnitID);
            }
            return View(order);
        }

        public async Task<IActionResult> OrderStatusUpdate(int id, string status)
        {
            var orders = (await _orderService.GetAllOrdersAsync()).ToList();
            var order = orders.FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            await _orderService.UpdateStatusAsync(order);

            await _hubContext.Clients.All.SendAsync("ReceiveOrderStatus", order.OrderNum, status);

            return RedirectToAction("OrderDetails", "Order", new { id = id });
        }

        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> ViewOrder(string statusFilter, DateTime? startDate, DateTime? endDate, string customerName, string ordernumber, int? Id)
        {
            var orders = (await _orderService.GetAllOrdersAsync()).ToList();
            var status = orders.Select(o => o.Status).Distinct().ToList();
            ViewBag.StatusOptions = status;

            if (!string.IsNullOrEmpty(statusFilter))
            {
                orders = orders.Where(x => x.Status == statusFilter).ToList();
            }
            else
            {
                orders = orders.Where(x => x.Status != "Cancelled").ToList();
            }
            if (startDate.HasValue && endDate.HasValue)
            {
                orders = orders.Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate).ToList();
            }
            if (!string.IsNullOrEmpty(customerName))
            {
                orders = orders.Where(x => x.UserId == customerName).ToList();
            }
            if (!string.IsNullOrEmpty(ordernumber))
            {
                orders = orders.Where(x => x.OrderNum == ordernumber).ToList();
            }

            foreach (var item in orders)
            {
                if (item.UserId != null)
                {
                    item.User = await _userManager.FindByIdAsync(item.UserId);
                }
            }

            if (Id.HasValue)
            {
                var order = orders.FirstOrDefault(x => x.Id == Id.Value);
                if (order != null)
                {
                    order.OrderDetails = (await _orderDetailService.GetAllAsync()).Where(x => x.OrderId == Id.Value).ToList();
                    foreach (var detail in order.OrderDetails)
                    {
                        detail.Product = await _productService.GetProductByIdAsync(detail.ProductId);
                    }
                    if (order.UserId != null)
                    {
                        order.User = await _userManager.FindByIdAsync(order.UserId);
                    }
                    ViewBag.Order = order;
                }
            }

            return View(orders);
        }

        public static string GenerateOrderNumber()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string guidComponent = Guid.NewGuid().ToString("N").Substring(0, 8);
            return $"{timestamp}-{guidComponent}";
        }
    }
}
