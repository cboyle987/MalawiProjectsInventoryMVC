using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace MalawiProjectsInventoryMVC.Controllers;

public class AccountController(IConfiguration configuration) : Controller
{
    private string auth0domain = configuration["Auth0:Domain"] ?? throw new InvalidOperationException();

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
        {
            RedirectUri = Url.Action("Index", "Home")
        });

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }
    
}