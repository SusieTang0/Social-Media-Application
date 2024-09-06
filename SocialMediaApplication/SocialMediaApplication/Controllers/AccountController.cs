using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SocialMediaApplication.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace SocialMediaApplication.Services
{

    public class AccountController : Controller
    {
        private readonly FirebaseService _firebaseService;

        public AccountController(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, IFormFile profilePicture)
        {
            try
            {


                var authLink = await _firebaseService.RegisterUser(email, password);

                string imageUrl = null;

                if (profilePicture != null)
                {
                    using var stream = profilePicture.OpenReadStream();
                    imageUrl = await _firebaseService.UploadProfilePictureAsync(stream, profilePicture.FileName);
                }

                var userProfile = new SocialMediaApplication.Models.User
                {
                    UserId = authLink.User.LocalId,
                    Email = email,
                    Name = email,
                    Bio = "This is your bio. You can update it later.",
                    ProfilePictureUrl = imageUrl ?? "~/images/logo150.png"
                };

                await _firebaseService.SaveUserProfileAsync(authLink.User.LocalId, userProfile);

                HttpContext.Session.SetString("userId", authLink.User.LocalId);

                return RedirectToAction("Index", "UserPage");
            }
            catch (FirebaseAuthException ex)
            {

                ViewBag.ErrorMessage = ex.Message;
                return View("Register");
            }
            catch (Exception ex)
            {

                ViewBag.ErrorMessage = "This email already has an account. Please Login.";
                return View("Register");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var authLink = await _firebaseService.LoginUser(email, password);

                HttpContext.Session.SetString("userId", authLink.User.LocalId);
                var claims = new List<Claim>
               {
                    new Claim(ClaimTypes.Name, email)

                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "UserPage");
            }

            catch (Exception ex)
            {

                ViewBag.ErrorMessage = "Email does not exist or the password is incorrect";
                return View("Login");
            }



        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            string userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }


            var profile = await _firebaseService.GetUserProfileAsync(userId);
            if (profile == null)
            {

                return RedirectToAction("Error");
            }

            return View(profile); // Pass the SocialMediaApplication.Models.User object to the view
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            string userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }


            var profile = await _firebaseService.GetUserProfileAsync(userId);
            if (profile == null)
            {
                return RedirectToAction("Error");
            }

            return View(profile);
        }

        [HttpPost("Account/UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(string name, string bio, IFormFile profilePicture, string existingProfilePictureUrl)
        {
            string userId = HttpContext.Session.GetString("userId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            string imageUrl = existingProfilePictureUrl;

            if (profilePicture != null && profilePicture.Length > 0)
            {
                using var stream = profilePicture.OpenReadStream();
                imageUrl = await _firebaseService.UploadProfilePictureAsync(stream, profilePicture.FileName);
            }

            var userProfile = new Models.User
            {
                UserId = userId,
                Name = name,
                Bio = bio,
                ProfilePictureUrl = imageUrl
            };

            await _firebaseService.SaveUserProfileAsync(userId, userProfile);

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {

            HttpContext.Session.Clear();


            await HttpContext.SignOutAsync();


            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string resetEmail)
        {
            try
            {

                var userExists = await _firebaseService.CheckIfUserExists(resetEmail);

                if (!userExists)
                {
                    TempData["ResetErrorMessage"] = "This email does not exist. Please create an account.";
                    return RedirectToAction("ResetPassword");
                }


                TempData["ResetMessage"] = "A password reset link would be sent to your email (simulation).";


                return RedirectToAction("Login", new { email = resetEmail });
            }
            catch (Exception ex)
            {
                TempData["ResetErrorMessage"] = "Failed to process your request. Please try again.";
                return RedirectToAction("ResetPassword");
            }
        }
    }
}
