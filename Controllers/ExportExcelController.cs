using Microsoft.AspNetCore.Mvc;

namespace FashionStore.Controllers;

public class ExportExcelController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}