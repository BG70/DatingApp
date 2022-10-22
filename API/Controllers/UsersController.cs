using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
   // [ApiController]
   // [Route("api/[controller]")]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        public UsersController(DataContext context){
            _context = context;
        }
        
        // api/users
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = _context.Users.ToListAsync();
            return await users;
        }
        // רצינו שליפה אסינכרוני

        //public ActionResult<IEnumerable<AppUser>> GetUsers()
        //{
        //    return _context.Users.ToList();
        //}



        // api/users/3
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = _context.Users.FindAsync(id);
            return await user;
        }   

        //public ActionResult<AppUser> GetUser(int id)
        //{
        //    return _context.Users.Find(id);
        //}              
    }
}