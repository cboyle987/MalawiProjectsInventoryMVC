using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MalawiProjectsInventoryMVC.Controllers;

[AllowAnonymous]
public class CallbackController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }
}