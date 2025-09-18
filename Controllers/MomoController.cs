using System.Security.Cryptography;
using System.Text;
using FashionStore.Data;
using FashionStore.Entities.Dtos;
using FashionStore.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FashionStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MomoController(FStoreDbContext context) : Controller
{
    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] OrderModel order)
    {
        string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
        string partnerCode = "MOMO";
        string accessKey = "F8BBA842ECF85";
        string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";

        string orderInfo = $"Thanh toán đơn hàng {order.OrderId}";
        string redirectUrl = "http://localhost:5173/orders";
        string ipnUrl = "https://1c1549e3c4cf.ngrok-free.app/api/momo/ipn";
        string amount = order.Amount.ToString();
        string orderId = order.OrderId ?? DateTime.Now.Ticks.ToString();
        string requestId = Guid.NewGuid().ToString();
        string requestType = "captureWallet";

        // Tạo chữ ký (signature)
        string rawHash =
            $"accessKey={accessKey}&amount={amount}&extraData=&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";

        string signature;
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
        {
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
            signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        var message = new
        {
            partnerCode,
            requestId,
            amount,
            orderId,
            orderInfo,
            redirectUrl,
            ipnUrl,
            requestType,
            extraData = "",
            signature,
            lang = "vi"
        };

        using var client = new HttpClient();
        var response = await client.PostAsync(endpoint,
            new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

        var result = await response.Content.ReadAsStringAsync();

        try
        {
            // Dùng MomoCreateResponse để deserialize response từ create API
            var momoResponse = JsonConvert.DeserializeObject<MomoCreateResponse>(result);

            // Log để debug
            Console.WriteLine($"MoMo Create Response: {result}");

            return Ok(momoResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing MoMo response: {ex.Message}");
            Console.WriteLine($"Raw response: {result}");

            // Trả về raw response nếu parsing failed
            return Ok(new { RawResponse = result, Error = ex.Message });
        }
    }

    // Test endpoint
    [HttpGet("test")]
    public IActionResult TestEndpoint()
    {
        return Ok(new { message = "MoMo controller is working", timestamp = DateTime.Now });
    }

    // MoMo redirect user về sau khi thanh toán - CHỈ REDIRECT
    [HttpGet("return")]
    public async Task<IActionResult> PaymentReturn([FromQuery] Dictionary<string, string> query)
    {
        var resultCode = query.GetValueOrDefault("resultCode", "-1");
        var orderId = query.GetValueOrDefault("orderId", "");

        Console.WriteLine($"Payment Return - ResultCode: {resultCode}, OrderId: {orderId}");

        // Redirect về frontend với query parameters
        var redirectUrl = $"http://localhost:5173/orders?resultCode={resultCode}&orderId={orderId}";
        return Redirect(redirectUrl);
    }

    // MoMo gọi IPN (server-to-server) - XỬ LÝ LOGIC CHÍNH
    [HttpPost("ipn")]
    public async Task<IActionResult> PaymentNotify()
    {
        try
        {
            // Đọc raw request body
            using var reader = new StreamReader(Request.Body);
            var requestBody = await reader.ReadToEndAsync();

            Console.WriteLine($"=== IPN CALLED AT {DateTime.Now} ===");
            Console.WriteLine($"Raw body: {requestBody}");
            Console.WriteLine($"Headers: {string.Join(", ", Request.Headers.Select(h => $"{h.Key}:{h.Value}"))}");

            if (string.IsNullOrEmpty(requestBody))
            {
                Console.WriteLine("Empty request body");
                return Ok(new { RspCode = "01", Message = "Empty request" });
            }

            // Parse với MomoIPNResponse model
            var notifyData = JsonConvert.DeserializeObject<MomoIPNResponse>(requestBody);
            Console.WriteLine($"Parsed IPN data: {JsonConvert.SerializeObject(notifyData)}");

            // TODO: Verify signature ở đây

            if (notifyData?.ResultCode == 0)
            {
                Console.WriteLine($"Processing successful payment for order: {notifyData.OrderId}");

                var order = await context.Orders.FirstOrDefaultAsync(x => x.id == notifyData.OrderId);
                if (order == null)
                {
                    Console.WriteLine($"Order not found: {notifyData.OrderId}");
                    return Ok(new { RspCode = "01", Message = "Order not found" });
                }

                if (order.status == "Đã xác nhận" || order.status == "Đã thanh toán")
                {
                    Console.WriteLine($"Order already processed: {notifyData.OrderId}");
                    return Ok(new { RspCode = "00", Message = "Order already processed" });
                }

                var userId = order.user_id;
                var lstCartsByUserId = await context.CartItems.Where(x => x.user_id == userId).ToListAsync();

                using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    order.status = "Đã thanh toán";

                    if (lstCartsByUserId.Count > 0)
                    {
                        context.RemoveRange(lstCartsByUserId);
                        Console.WriteLine($"Removed {lstCartsByUserId.Count} cart items");
                    }

                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    Console.WriteLine($"Order processed successfully: {notifyData.OrderId}");
                    return Ok(new { RspCode = "00", Message = "Success" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Transaction error: {ex.Message}");
                    return Ok(new { RspCode = "99", Message = "System error" });
                }
            }
            else
            {
                Console.WriteLine($"Payment failed with result code: {notifyData?.ResultCode}");

                // Update order status to failed
                if (!string.IsNullOrEmpty(notifyData?.OrderId))
                {
                    var order = await context.Orders.FirstOrDefaultAsync(x => x.id == notifyData.OrderId);
                    if (order != null)
                    {
                        order.status = "Thanh toán thất bại";
                        await context.SaveChangesAsync();
                    }
                }

                return Ok(new { RspCode = "00", Message = "Payment failed processed" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"IPN Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return Ok(new { RspCode = "99", Message = "System error" });
        }
    }

    

}