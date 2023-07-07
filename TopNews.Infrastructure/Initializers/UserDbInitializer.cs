using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.Entities.User;

namespace TopNews.Infrastructure.Initializers
{
    public static class UserDbInitializer
    {
        public static void SeedUsers(UserManager<AppUser> userManager)
        {
            if (userManager.FindByEmailAsync("admin@gmail.com").Result == null)
            {
                AppUser user = new AppUser
                {
                    FirstName = "John",
                    LastName = "Pork",
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+xx(xxx)xxx-xx-xx",
                    PhoneNumberConfirmed = true
                };

                IdentityResult result = userManager.CreateAsync(user, "Qwert-1").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }
    }
}
