// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using mvcflowershoplab1.Areas.Identity.Data;

namespace mvcflowershoplab1.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<mvcbicyclerentalUser> _userManager;
        private readonly SignInManager<mvcbicyclerentalUser> _signInManager;

        public IndexModel(
            UserManager<mvcbicyclerentalUser> userManager,
            SignInManager<mvcbicyclerentalUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            //add
            [Required(ErrorMessage = "You must enter the Name first")]
            [Display(Name = "Customer Name")]
            [StringLength(100, ErrorMessage = "Between 10-100 chars", MinimumLength = 10)]
            public string CName { get; set; }
            [Required]
            [Display(Name = "Customer Age")]
            [Range(18, 100, ErrorMessage = "Only 18-100 years old person are allowed to register")]
            public int CAge { get; set; }
            [Required]
            [Display(Name = "Customer DOB")]
            [DataType(DataType.Date)]
            public DateTime CDOB { get; set; }
            [Required]
            [Display(Name ="Customer Address")]
            public string CAddress { get; set; }
        }

        private async Task LoadAsync(mvcbicyclerentalUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                //add
                CName = user.CustomerFullName,
                CAge = user.CustomerAge,
                CDOB = user.CustomerDOB,
                CAddress = user.CustomerAddress
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            
            //add
            if(Input.CName != user.CustomerFullName)
            {
                user.CustomerFullName = Input.CName;
            }
            if (Input.CAge != user.CustomerAge)
            {
                user.CustomerAge = Input.CAge;
            }
            if (Input.CDOB != user.CustomerDOB)
            {
                user.CustomerDOB = Input.CDOB;
            }
            if (Input.CAddress != user.CustomerAddress)
            {
                user.CustomerAddress = Input.CAddress;
            }

            //add
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
