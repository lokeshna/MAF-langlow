using Microsoft.AspNetCore.Mvc;

namespace MAF.LangflowBot.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
