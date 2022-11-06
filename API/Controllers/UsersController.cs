using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository, IMapper mapper){
            _mapper = mapper;
            _userRepository = userRepository;
        }
        
        // api/users
        
        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            //var users = _context.Users.ToListAsync();
            //return await users;
            //return Ok(await _userRepository.GetUsersAsync());
            // var users = await _userRepository.GetUsersAsync();
            // var userToResult = _mapper.Map<IEnumerable<MemberDto>>(users);
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
        }
        // רצינו שליפה אסינכרוני

        //public ActionResult<IEnumerable<AppUser>> GetUsers()
        //{
        //    return _context.Users.ToList();
        //}



        // api/users/3
        //[Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            //var user = _context.Users.FindAsync(id);
            //return await user;
            //return await _userRepository.GetUserByUsernameAsync(username);
            //var user = await _userRepository.GetUserByUsernameAsync(username);
            //return _mapper.Map<MemberDto>(user);
            return await _userRepository.GetMemberAsync(username);
        }   

        //public ActionResult<AppUser> GetUser(int id)
        //{
        //    return _context.Users.Find(id);
        //}             

        [HttpPut] 
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(username);
            
            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }
    }
}