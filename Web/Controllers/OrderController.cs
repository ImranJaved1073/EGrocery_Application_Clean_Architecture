using Application.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public OrderController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            OrderService orderService, OrderDetailService orderDetailService, ILogger<OrderController> logger, ProductService productService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _productService = productService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddOrder(CheckOut odrs)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                var cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id);
                if (cart != null)
                {
                    Orders order = new Orders
                    {
                        UserId = user.Id,
                        OrderNum = GenerateOrderNumber(),
                        OrderDate = DateTime.Now,
                        Status = "Pending",
                        TotalBill = cart.TotalPrice,
                        //TotalDiscount = cart.TotalDiscount,
                        CheckOut = new CheckOut
                        {
                            OrderDeliveryDate = DateTime.Today.AddDays(1),
                            PaymentMethod = "Cash on Delivery",
                            Address = odrs.Address,
                            City = odrs.City ?? "",
                            ZipCode = odrs.ZipCode,
                            State = odrs.State ?? "",
                        },
                        //PaymentStatus = "Pending",
                        OrderDetails = cart.Items.Select(x => new OrderDetail
                        {
                            ProductId = x.Id,
                            Price = x.Price,
                            Quantity = x.Quantity,
                            Discount = 0,
                            TotalPrice = x.Price * x.Quantity,
                        }).ToList()
                    };

                    _orderService.CreateOrder(order);

                    foreach (var item in order.OrderDetails)
                    {
                        item.OrderId = _orderService.GetbyOrderNo(order.OrderNum).Id;
                        _orderDetailService.AddOrderDetail(item);
                    }

                    CookieHelper.ClearCartCookies(HttpContext, user.Id);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [Authorize]
        public IActionResult Order(string statusFilter, DateTime? startDate, DateTime? endDate)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var orders = _orderService.GetAllOrders();
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

            orders = orders.ToList();


            return View(orders);

        }

        public IActionResult OrderDetails(int id)
        {
            List<Orders> orders = _orderService.GetAllOrders().ToList();
            var order = orders.Where(x => x.Id == id).FirstOrDefault();
            if (order == null)
            {
                return NotFound();
            }
            order.OrderDetails = _orderDetailService.GetAll().Where(x => x.OrderId == id).ToList();
            foreach (var item in order.OrderDetails)
            {
                item.Product = _productService.GetProductById(item.ProductId);
            }
            return View(order);
        }

        //public IActionResult OrderStatus(int id, string status)
        //{
        //    OrderRepository oR = new OrderRepository();
        //    var order = oR.Get(id);
        //    order.Status = status;
        //    oR.Update(order);
        //    return RedirectToAction("Order", "Order");
        //}

        public IActionResult OrderStatusUpdate(int id, string status)
        {
            var orders = _orderService.GetAllOrders();
            Orders? order = orders.Where(x => x.Id == id).FirstOrDefault();
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            _orderService.UpdateStatus(order);
            return RedirectToAction("OrderDetails", "Order", new { id = id });
        }

        //public IActionResult OrderStatusDelete(int id)
        //{
        //    OrderRepository oR = new OrderRepository();
        //    oR.Delete(id);
        //    return RedirectToAction("Order", "Order");
        //}

        //public IActionResult OrderStatusDeleteUpdate(int id)
        //{
        //    OrderRepository oR = new OrderRepository();
        //    var order = oR.Get(id);
        //    oR.Delete(order);
        //    return RedirectToAction("Order", "Order");
        //}

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult ViewOrder(string statusFilter, DateTime? startDate, DateTime? endDate, string customerName, string ordernumber, int? id)
        {
            var orders = _orderService.GetAllOrders();
            var status = orders.Select(o => o.Status).Distinct().ToList();
            ViewBag.StatusOptions = status;

            if (!string.IsNullOrEmpty(statusFilter))
            {
                orders = orders.Where(x => x.Status == statusFilter).ToList();
            }
            else orders = orders.Where(x => x.Status != "Cancelled").ToList();
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
                    item.User = _userManager.FindByIdAsync(item.UserId).Result;
                }
            }

            if (id.HasValue)
            {
                var order = orders.FirstOrDefault(x => x.Id == id.Value);
                if (order != null)
                {
                    order.OrderDetails = _orderDetailService.GetAll().Where(x => x.OrderId == id.Value).ToList();
                    foreach (var detail in order.OrderDetails)
                    {
                        detail.Product = _productService.GetProductById(detail.ProductId);
                    }
                    if (order.UserId != null)
                    {
                        order.User = _userManager.FindByIdAsync(order.UserId).Result;
                    }
                    ViewBag.Order = order;
                }
            }

            orders = orders.ToList();
            return View(orders);
        }


        public static string GenerateOrderNumber()
        {
            // Get the current timestamp in a specific format
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            // Generate a new GUID and take only the first 8 characters for brevity
            string guidComponent = Guid.NewGuid().ToString("N").Substring(0, 8);

            // Combine the timestamp and GUID components to form a unique order number
            return $"{timestamp}-{guidComponent}";
        }

    }
}
