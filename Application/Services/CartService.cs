using Domain;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public class CartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Cart GetCart(string userId)
    {
        var cart = CookieHelper.GetCookie<Cart>(_httpContextAccessor.HttpContext, "Cart", userId);
        return cart ?? new Cart { UserId = userId };
    }

    public void SaveCart(string userId, Cart cart)
    {
        CookieHelper.SetCookie(_httpContextAccessor.HttpContext, "Cart", cart, 4320, userId);
    }

    public void AddToCart(CartItem item, string userId)
    {
        var cart = GetCart(userId);
        var existingItem = cart.Items.FirstOrDefault(x => x.Id == item.Id);

        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            cart.Items.Add(item);
        }

        cart.TotalPrice += item.Price * item.Quantity;
        cart.TotalQuantity += item.Quantity;

        SaveCart(userId, cart);
    }

    public void RemoveFromCart(int productId, string userId)
    {
        var cart = GetCart(userId);
        var cartItem = cart.Items.FirstOrDefault(x => x.Id == productId);

        if (cartItem != null)
        {
            cart.TotalPrice -= cartItem.Price * cartItem.Quantity;
            cart.TotalQuantity -= cartItem.Quantity;
            cart.Items.Remove(cartItem);

            SaveCart(userId, cart);
        }
    }

    public void UpdateCart(int productId, int quantity, string userId)
    {
        var cart = GetCart(userId);
        var cartItem = cart.Items.FirstOrDefault(x => x.Id == productId);

        if (cartItem != null)
        {
            cart.TotalPrice -= cartItem.Price * cartItem.Quantity;
            cart.TotalQuantity -= cartItem.Quantity;

            cartItem.Quantity = quantity;

            cart.TotalPrice += cartItem.Price * cartItem.Quantity;
            cart.TotalQuantity += cartItem.Quantity;

            SaveCart(userId, cart);
        }
    }
}
