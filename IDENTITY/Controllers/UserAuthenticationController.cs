using IDENTITY.Models;

using IDENTITY.Repositories.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace IDENTITY.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _service;
        public UserAuthenticationController(IUserAuthenticationService _service)
        {
            this._service = _service;
        }
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> Registration(RegistrationModel model)
        {
            if(!ModelState.IsValid)
                return View(model);
            model.Role = "user";
             var result = await _service.RegistrationAsync(model);
            TempData["msg"] = result.Message;
            return RedirectToAction(nameof(Registration));
            
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Login (LoginModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _service.LoginAsync(model);
            if (result.status == 1) 
            {
                return RedirectToAction("Display","Dashboard");
            
            }
            else
            {
                TempData["msg"] = result.Message;
                return RedirectToAction(nameof(Login));
            }


        }

        [Authorize]
        public async Task<IActionResult> Logout()
        { 
            await _service.LogoutAsync();

            return RedirectToAction(nameof(Login));
        }

        //public async Task<IActionResult> reg()
        //{
        //    var model = new RegistrationModel
        //    {
        //        Username = "admin1",
        //        Name = "Pukar",
        //        Email = "puk@gmail.com",
        //        Password = "Admin@12345#"
        //    };
        //    model.Role = "admin";
        //    var result = await _service.RegistrationAsync(model);

        //    return Ok(result);
        //}


    }

}
