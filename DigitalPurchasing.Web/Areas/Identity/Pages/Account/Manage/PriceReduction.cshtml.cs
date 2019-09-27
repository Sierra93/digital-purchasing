using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalPurchasing.Web.Areas.Identity.Pages.Account.Manage
{
    public class PriceReductionModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required, Display(Name = "Целевая скидка к минимальной цене, %")]
            public decimal DiscountPercentage { get; set; }
        }

        public PriceReductionModel(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Input = new InputModel
            {
                DiscountPercentage = user.PRDiscountPercentage
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            user.PRDiscountPercentage = Input.DiscountPercentage;
            await _userManager.UpdateAsync(user);

            StatusMessage = "Изменения сохранены";
            return RedirectToPage();
        }
    }
}
