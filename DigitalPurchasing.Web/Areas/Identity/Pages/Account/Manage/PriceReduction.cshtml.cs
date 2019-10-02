using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalPurchasing.Web.Areas.Identity.Pages.Account.Manage
{
    public class PriceReductionModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required, Display(Name = "Deadline предоставления КП после 1-го запроса в часах")]
            [Range(0.5, int.MaxValue)]
            public double QuotationRequestResponseHours { get; set; }

            [Required, Display(Name = "Deadline предоставления КП в ответ на запрос на понижение в часах")]
            [Range(0.5, int.MaxValue)]
            public double PriceReductionResponseHours { get; set; }

            [Required, Display(Name = "Deadline по сроку закрытия конкурса в часах с момент отправки 1-го запроса КП")]
            [Range(0.5, int.MaxValue)]
            public double AutoCloseCLHours { get; set; }

            [Required, Display(Name = "Целевая скидка к минимальной цене, %")]
            [Range(1, 99)]
            public double DiscountPercentage { get; set; }

            [Required, Display(Name = "Количество раундов для отправки запросов в автоматическом режиме")]
            [Range(0, int.MaxValue)]
            public int RoundsCount { get; set; }

            [Required, Display(Name = "Отправлять запрос на понижение цены")]
            public SendPriceReductionTo SendPriceReductionTo { get; set; }
        }

        public PriceReductionModel(UserManager<User> userManager)
            => _userManager = userManager;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Input = new InputModel
            {
                DiscountPercentage = user.PRDiscountPercentage,
                QuotationRequestResponseHours = user.QuotationRequestResponseHours,
                AutoCloseCLHours = user.AutoCloseCLHours,
                PriceReductionResponseHours = user.PriceReductionResponseHours,
                SendPriceReductionTo = user.SendPriceReductionTo,
                RoundsCount = user.RoundsCount
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

            user.QuotationRequestResponseHours = Input.QuotationRequestResponseHours;
            user.AutoCloseCLHours = Input.AutoCloseCLHours;
            user.PriceReductionResponseHours = Input.PriceReductionResponseHours;
            user.PRDiscountPercentage = Input.DiscountPercentage;
            user.SendPriceReductionTo = Input.SendPriceReductionTo;
            user.RoundsCount = Input.RoundsCount;

            await _userManager.UpdateAsync(user);

            StatusMessage = "Изменения сохранены";
            return RedirectToPage();
        }
    }
}
