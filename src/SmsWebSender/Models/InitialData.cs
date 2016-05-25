using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace SmsWebSender.Models
{
    public class InitialData
    {
        private const string DefaultUserName = "Fanney";
        private const string DefaultUserEmail = "fanney@likamiogsal.is";

        public static async Task InitializeUser(IServiceProvider serviceProvider, string defaultUserPassword)
        {
            var userManager = (UserManager<ApplicationUser>)serviceProvider.GetService(typeof(UserManager<ApplicationUser>));

            var user = await userManager.FindByEmailAsync(DefaultUserEmail);
            if (user != null)
            {
                return;
            }

            user = new ApplicationUser
            {
                UserName = DefaultUserName,
                Email = DefaultUserEmail
            };

            var result = await userManager.CreateAsync(user);
            var bla = await userManager.AddPasswordAsync(user, defaultUserPassword);
        }
    }
}
