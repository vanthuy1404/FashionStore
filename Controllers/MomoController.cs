using Microsoft.AspNetCore.Mvc;

namespace FashionStore.Controllers;

public class MomoController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}