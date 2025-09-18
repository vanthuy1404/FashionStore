namespace FashionStore.Entities.Dtos;

// Response từ MoMo Create Payment API
public class MomoCreateResponse
{
    public string PartnerCode { get; set; }
    public string RequestId { get; set; }
    public string OrderId { get; set; }
    public long Amount { get; set; }
    public long ResponseTime { get; set; }
    public string Message { get; set; }
    public int ResultCode { get; set; }
    public string PayUrl { get; set; }      // URL để redirect user đến MoMo
    public string QrCodeUrl { get; set; }   // URL QR code để scan
    public string Signature { get; set; }
}

// Response từ MoMo IPN (webhook callback)
public class MomoIPNResponse
{
    public string PartnerCode { get; set; }
    public string RequestId { get; set; }
    public string OrderId { get; set; }
    public long Amount { get; set; }
    public string OrderInfo { get; set; }
    public string OrderType { get; set; }
    public long TransId { get; set; }
    public int ResultCode { get; set; }
    public string Message { get; set; }
    public string PayType { get; set; }
    public long ResponseTime { get; set; }
    public string ExtraData { get; set; }
    public string Signature { get; set; }
}

// Generic MoMo response có thể dùng cho cả 2 cases
public class MomoResponse
{
    public string PartnerCode { get; set; }
    public string RequestId { get; set; }
    public string OrderId { get; set; }
    public long Amount { get; set; }
    public string OrderInfo { get; set; }
    public string OrderType { get; set; }
    public long? TransId { get; set; }
    public long ResponseTime { get; set; }
    public string Message { get; set; }
    public int ResultCode { get; set; }
    public string PayUrl { get; set; }      // Chỉ có trong create response
    public string QrCodeUrl { get; set; }   // Chỉ có trong create response
    public string PayType { get; set; }     // Chỉ có trong IPN
    public string ExtraData { get; set; }
    public string Signature { get; set; }
}