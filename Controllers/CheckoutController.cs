using Microsoft.AspNetCore.Mvc;
using WingtipToys.Services;
using PayPal.Core;
using PayPal.v1.Payments;
using BraintreeHttp;
using Microsoft.AspNetCore.Authorization;

namespace WingtipToys.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IHttpContextAccessor _context;
        private readonly CartService _cartService; 
        private readonly HttpContext _httpContext;
        private readonly string _clientId;
        private readonly string _secretKey;
        
        public CheckoutController(IHttpContextAccessor context,IConfiguration config,CartService cartService)
        {
            _context = context;
            _cartService = cartService;
            _httpContext = _context.HttpContext;
            _clientId = config["PaypalSettings:ClientID"];
            _secretKey = config["PaypalSettings:SerectKey"];
        }

        [Authorize]
        public async Task<IActionResult> NomalCheckout()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> PaypalCheckout()
        {
            var environment = new SandboxEnvironment(_clientId, _secretKey);
            var client = new PayPalHttpClient(environment);

            #region Create Paypal Order
            var itemList = new ItemList()
            {
                Items = new List<Item>()
            };
            
            var cartList = _cartService.GetCartItems();
            double total = (double)cartList.Sum(c => c.product.UnitPrice);

            // Console.WriteLine(total.ToString());
            // var total = Math.Round(Carts.Sum(p => p.ThanhTien) / TyGiaUSD, 2);
            foreach (var item in cartList)
            {
                itemList.Items.Add(new Item()
                {
                    Name = item.product.ProductName,
                    Currency = "USD",
                    Price = item.product.UnitPrice.ToString(),
                    Quantity = item.quantity.ToString(),
                    Sku = "sku",
                    Tax = "0"
                });
            }
            #endregion

            var paypalOrderId = DateTime.Now.Ticks;
            var hostname = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host}";
            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = total.ToString(),
                            Currency = "USD",
                            Details = new AmountDetails
                            {
                                Tax = "0",
                                Shipping = "0",
                                Subtotal = total.ToString()
                            }
                        },
                        ItemList = itemList,
                        Description = $"Invoice #{paypalOrderId}",
                        InvoiceNumber = paypalOrderId.ToString()
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    CancelUrl = $"{hostname}/CheckoutFail",
                    ReturnUrl = $"{hostname}/CheckoutSuccess"
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };

            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);

            try
            {
                var response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();

                var links = result.Links.GetEnumerator();
                string paypalRedirectUrl = null;
                while (links.MoveNext())
                {
                    LinkDescriptionObject lnk = links.Current;
                    if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
                    {
                        //saving the payapalredirect URL to which user will be redirected for payment  
                        paypalRedirectUrl = lnk.Href;
                    }
                }

                return Redirect(paypalRedirectUrl);
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();

                //Process when Checkout with Paypal fails
                return RedirectToAction(nameof(CheckoutFail));
            }
        }

        [Route("/ChekoutFail",Name = "CheckoutFail")]
        public IActionResult CheckoutFail()
        {
            //Tạo đơn hàng trong database với trạng thái thanh toán là "Chưa thanh toán"
            //Xóa session
            return View();
        }

        [Route("/CheckoutSuccess",Name = "CheckoutSuccess")]        
        public IActionResult CheckoutSuccess()
        {
            //Tạo đơn hàng trong database với trạng thái thanh toán là "Paypal" và thành công
            //Xóa session
            return View();
        }
    }
}