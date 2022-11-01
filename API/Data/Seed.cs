using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using API.Entities;
using System.Security.Cryptography;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
         {  // קורה קובץ נתונים ומכניס לבסיס הנתונים
            if( await context.Users.AnyAsync()) return; // אם יש כבר נתונים ואז לא עושה

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$W0rd"));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);
            }

            await context.SaveChangesAsync();
        }
    }
}