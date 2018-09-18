using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public new IActionResult LocalRedirect(string localUrl)
        {
            if (Url.IsLocalUrl(localUrl))
                return Redirect(localUrl);

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
