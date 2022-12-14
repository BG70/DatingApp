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
using API.Extensions;
using API.Helpers;

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
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            //var users = _context.Users.ToListAsync();
            //return await users;
            //return Ok(await _userRepository.GetUsersAsync());
            // var users = await _userRepository.GetUsersAsync();
            // var userToResult = _mapper.Map<IEnumerable<MemberDto>>(users);
           
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            userParams.CurrentUsername = user.UserName;

            if(string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            var users = await _userRepository.GetMembersAsync(userParams);
            
            Response.AddPaginationHeader(users.CurrentPage, userParams.PageSize,
                users.TotalCount, users.TotalPages);

            return Ok(users);
        }
        // רצינו שליפה אסינכרוני

        //public ActionResult<IEnumerable<AppUser>> GetUsers()
        //{
        //    return _context.Users.ToList();
        //}



        // api/users/3
        //[Authorize]
        [HttpGet("{username}", Name = "GetUser")]
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
            //var username = User.GetUsername(); //User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            
            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }
    }
}