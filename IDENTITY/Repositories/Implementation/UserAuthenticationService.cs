using IDENTITY.Models;

using IDENTITY.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IDENTITY.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> signInManager; //identityuser wala class
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole > roleManager;
        public UserAuthenticationService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await this.userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                status.status = 0;
                status.Message = "invalid username";
                return status;
            }
            //we will match our password
            if (!await this.userManager.CheckPasswordAsync(user, model.Password))
            {
                status.status = 0;
                status.Message = "invalid password";
                return status;
            }
            var signInResult = await this.signInManager.PasswordSignInAsync(user, model.Password, false, true);
            if (signInResult.Succeeded) 
            {
                 var userRoles = await this.userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                       new Claim(ClaimTypes.Name , user.UserName)
                };
                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Name, user.UserName));
                }

                status.status = 1;
                status.Message = "Loged in succesfully";
                return status;
            }
            else if (signInResult.IsLockedOut)
            {
                status.status = 0;
                status.Message = "user locked out";
                return status;
            }
            else
            {
                status.status = 0;
                status.Message = "error in logging in ";
                return status;
            }
        }

        public async Task LogoutAsync()
        {
            await this.signInManager.SignOutAsync();
        }

        public async Task<Status> RegistrationAsync(RegistrationModel model)
        {
           var status  = new Status();
            var userExists = await this.userManager.FindByNameAsync(model.Username);

            if (userExists != null)
            {
                status.status = 0;
                status.Message = "user already exists";
                return status;
            }
            ApplicationUser user = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name,
                Email = model.Email,
                UserName = model.Username,
                EmailConfirmed = true
            };
            var result = await this.userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.status = 0;
                status.Message = "user creation failed";
                return status;
            }
            //role management
            if(!await this.roleManager.RoleExistsAsync(model.Role))
                await this.roleManager.CreateAsync(new IdentityRole(model.Role));

            if(await this.roleManager.RoleExistsAsync(model.Role))
            {
                await this.userManager.AddToRoleAsync(user, model.Role);
            }
            status.status = 1;
            status.Message = "user has registered succesfully";
            return status;




        }
    }
}
