using Newtonsoft.Json;
using WingtipToys.Models;

namespace WingtipToys.Services
{
    public class CartService
    {
        public const string CARTKEY = "cart";

    private readonly IHttpContextAccessor _context;
    private readonly ILogger<CartService> _logger;

    private readonly HttpContext HttpContext;

    public CartService(IHttpContextAccessor context,ILogger<CartService> logger)
    {
        _context = context;
        _logger = logger;
        HttpContext = context.HttpContext;
    }


    // Lấy cart từ Session (danh sách CartItem)
    public List<CartItem> GetCartItems () {

        var session = HttpContext.Session;
        string jsoncart = session.GetString (CARTKEY);
        if (jsoncart != null) {
            return JsonConvert.DeserializeObject<List<CartItem>> (jsoncart);
        }
        return new List<CartItem> ();
    }

    // Xóa cart khỏi session
    public  void ClearCart () {
        var session = HttpContext.Session;
        session.Remove (CARTKEY);
    }

    // Lưu Cart (Danh sách CartItem) vào session
    public  void SaveCartSession (List<CartItem> ls) {
        var session = HttpContext.Session;
        string jsoncart = JsonConvert.SerializeObject (ls);
        session.SetString (CARTKEY, jsoncart);
    }       
    public decimal CartItemTotal()
    {
        decimal total  = 0;
        var listItems = GetCartItems();

        foreach (var item in listItems)
        {
            total += (decimal)(item.product.UnitPrice * item.quantity);
        }
        _logger.LogInformation(total.ToString());
        return total;
    }
    }
}