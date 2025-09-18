using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using FashionStore.Entities.Dtos;
using System.Drawing;

namespace FashionStore.Controllers;

[ApiController]
[Route("api/ExportExcel")]
public class ExportExcelController : Controller
{
    [HttpPost("order/{id}")]
    public IActionResult ExportOrder([FromRoute] string id, [FromBody] OrderDTO order)
    {
        if (order == null || order.Id != id)
            return BadRequest(new { message = "Thông tin đơn hàng không hợp lệ" });

        // EPPlus 8: khai báo license non-commercial
        ExcelPackage.License.SetNonCommercialPersonal("Dang Van Thuy");

        using (var package = new ExcelPackage())
        {
            var ws = package.Workbook.Worksheets.Add("ChiTietDonHang");

            // ----- CHIỀU RỘNG CỘT CỐ ĐỊNH (A..E) -----
            ws.Column(1).Width = 36; // A: "Sản phẩm" (merge dọc)
            ws.Column(2).Width = 36; // B: Tên sản phẩm/Màu/Size
            ws.Column(3).Width = 6;  // C: SL
            ws.Column(4).Width = 14; // D: Đơn giá
            ws.Column(5).Width = 16; // E: Thành tiền (và cột tổng)

            // ===== Header cửa hàng (A1:E3) =====
            ws.Cells["A1:E1"].Merge = true;
            ws.Cells["A1"].Value = "Fasion Store";
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A1"].Style.Font.Size = 14;
            ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells["A2:E2"].Merge = true;
            ws.Cells["A2"].Value = "Địa chỉ: số 128 đường Trần Hưng Đạo, thành phố Nam Định";
            ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells["A3:E3"].Merge = true;
            ws.Cells["A3"].Value = "Điện thoại: 0927354888";
            ws.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // ===== Empty row (row 4) =====
            ws.Cells["A4:E4"].Merge = true;

            // ===== Tiêu đề chính (dòng 5) =====
            ws.Cells["A5:E5"].Merge = true;
            ws.Cells["A5"].Value = "CHI TIẾT ĐƠN HÀNG";
            ws.Cells["A5"].Style.Font.Bold = true;
            ws.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // ===== Thông tin đơn hàng (A6..A10 left labels, B..E values) =====
            ws.Cells["A6"].Value = "Mã đơn hàng";
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells["B6:E6"].Merge = true;
            ws.Cells["B6"].Value = order.Id;

            ws.Cells["A7"].Value = "Ngày đặt";
            ws.Cells["A7"].Style.Font.Bold = true;
            ws.Cells["B7:E7"].Merge = true;
            ws.Cells["B7"].Value = order.Date.ToString("dd-MM-yyyy HH:mm");

            ws.Cells["A8"].Value = "Trạng thái";
            ws.Cells["A8"].Style.Font.Bold = true;
            ws.Cells["B8:E8"].Merge = true;
            ws.Cells["B8"].Value = order.Status;

            ws.Cells["A9"].Value = "Địa chỉ";
            ws.Cells["A9"].Style.Font.Bold = true;
            ws.Cells["B9:E9"].Merge = true;
            ws.Cells["B9"].Value = order.Address;

            ws.Cells["A10"].Value = "SĐT";
            ws.Cells["A10"].Style.Font.Bold = true;
            ws.Cells["B10:E10"].Merge = true;
            ws.Cells["B10"].Value = order.Phone;

            // ===== Bảng sản phẩm (row 11 onwards) =====
            int headerRow = 11;
            int itemsCount = order.Items?.Count ?? 0;

            // Header của bảng (ở cột A..E)
            ws.Cells[headerRow, 1].Value = "Sản phẩm";
            ws.Cells[headerRow, 2].Value = "Sản phẩm/Màu/Size";
            ws.Cells[headerRow, 3].Value = "SL";
            ws.Cells[headerRow, 4].Value = "Đơn giá";
            ws.Cells[headerRow, 5].Value = "Thành tiền";
            using (var hdr = ws.Cells[headerRow, 1, headerRow, 5])
            {
                hdr.Style.Font.Bold = true;
                hdr.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            }

            // Fill products rows (bắt đầu từ headerRow+1)
            int r = headerRow + 1;
            if (itemsCount == 0)
            {
                // Nếu không có sản phẩm, để trống
                ws.Cells[r, 2].Value = "-";
                ws.Cells[r, 3].Value = 0;
                ws.Cells[r, 4].Value = 0;
                ws.Cells[r, 5].Value = 0;
                r++;
            }
            else
            {
                foreach (var item in order.Items)
                {
                    ws.Cells[r, 1].Value = ""; // Empty cell for "Sản phẩm" column
                    ws.Cells[r, 2].Value = $"{item.Product.Name}/{item.Color}/{item.Size}";
                    ws.Cells[r, 2].Style.WrapText = true;
                    ws.Cells[r, 3].Value = item.Quantity;
                    ws.Cells[r, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws.Cells[r, 4].Value = item.Product.Price;
                    ws.Cells[r, 4].Style.Numberformat.Format = "#,##0";
                    ws.Cells[r, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    ws.Cells[r, 5].Value = item.Product.Price * item.Quantity;
                    ws.Cells[r, 5].Style.Numberformat.Format = "#,##0";
                    ws.Cells[r, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    r++;
                }
            }

            // ===== Các dòng tổng kết (đặt vào cột D (4) & E (5)) =====
            ws.Cells[r, 4].Value = "Mã giảm giá";
            ws.Cells[r, 5].Value = order.MaCoupon ?? "Không áp dụng";
            ws.Cells[r, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            r++;

            ws.Cells[r, 4].Value = "Phần trăm giảm";
            ws.Cells[r, 5].Value = order.PhanTram.HasValue ? $"{order.PhanTram.Value * 100}%" : "0%";
            ws.Cells[r, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            r++;

            ws.Cells[r, 4].Value = "Phí vận chuyển";
            ws.Cells[r, 5].Value = order.ShippingFee ?? 0;
            ws.Cells[r, 5].Style.Numberformat.Format = "#,##0";
            ws.Cells[r, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            r++;

            ws.Cells[r, 4].Value = "Tổng";
            ws.Cells[r, 4].Style.Font.Bold = true;
            ws.Cells[r, 5].Value = order.Total;
            ws.Cells[r, 5].Style.Numberformat.Format = "#,##0";
            ws.Cells[r, 5].Style.Font.Bold = true;
            ws.Cells[r, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            int endRow = r;

            // ===== BORDER từ dòng 5 đến dòng chứa "Tổng" (cột A..E) =====
            using (var borderedRange = ws.Cells[5, 1, endRow, 5])
            {
                borderedRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                borderedRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                borderedRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                borderedRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            // ===== Empty row before thank you message =====
            r++;
            ws.Cells[r, 1, r, 5].Merge = true;

            // ===== Lời cảm ơn (merge tới E) =====
            int thankRow = r + 1;
            ws.Cells[thankRow, 1, thankRow, 5].Merge = true;
            ws.Cells[thankRow, 1].Value = "Chúc quý khách mua sắm vui vẻ tại Fstore. Hẹn gặp lại!";
            ws.Cells[thankRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[thankRow, 1].Style.Font.Italic = true;

            // AutoFit (giữ width đã set cho 1..5, nhưng auto fit nhẹ cho các còn lại)
            ws.Cells[1, 1, thankRow, 5].AutoFitColumns();

            var fileBytes = package.GetAsByteArray();
            return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"DonHang_{order.Id}.xlsx");
        }
    }
}