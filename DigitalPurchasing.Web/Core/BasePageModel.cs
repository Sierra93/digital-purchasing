using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalPurchasing.Web.Core
{
    public class BasePageModel : PageModel
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
