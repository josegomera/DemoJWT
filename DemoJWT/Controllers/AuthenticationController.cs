using AutoMapper;
using DemoJWT.DTO;
using DemoJWT.Entities;
using DemoJWT.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DemoJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticateService _authService;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IMapper _mapper;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<Rol> _roleManager;


        public AuthenticationController(IAuthenticateService authService,
                                        UserManager<Usuario> userManager,
                                        SignInManager<Usuario> signInManager,
                                        RoleManager<Rol> roleManager,
                                        IMapper mapper)
        {
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO model)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);

                    if (user == null) { BadRequest("User does not exist."); }

                    var Token = await _authService.IsAuthenticated(user);

                    HttpContext.Session.SetString("Token", Token);

                    HttpContext.Response.Headers.Add("Authorization", Token);

                    return StatusCode(200, "Login Successfully");

                }
                return StatusCode(400, "Error, Check your email and password");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Route("Logout")]
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Token");
            return Ok("Succesfully LogOut");
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null) { return StatusCode(400, "This user already exist"); }

                user = _mapper.Map<Usuario>(model);

                var result = await _userManager.CreateAsync(user, model.Password);

                return StatusCode(201, "User Created");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Route("CreateRol")]
        [HttpPost]
        public async Task<IActionResult> CreateRol([FromBody] RolesRegisterDTO model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(model.Id.ToString());

                if (role == null) { return StatusCode(404, "Rol Does not exist"); }

                role = new Rol { Name = model.Name };

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return StatusCode(201, "Rol Created");
                }
                return StatusCode(400, "This rol cannot be created!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Route("AddRolToUser")]
        [HttpPost]
        public async Task<IActionResult> AddRolToUser([FromBody] RolManagementDTO model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(model.RolId.ToString());

                if (role == null) { return StatusCode(404, "Rol Does not exist"); }

                var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user == null) { return StatusCode(404, "User does not exist."); }

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    return StatusCode(400, String.Format("User: {0} is already in this role", user));
                }

                var result = await _userManager.AddToRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {
                    return StatusCode(400, String.Format("User: {0} could not be added to role", user));
                }

                return StatusCode(204, "User Enrolled");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }

        [Route("RemoveRolFromUser")]
        [HttpPost]
        public async Task<IActionResult> RemoveRolFromUser([FromBody] RolManagementDTO model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(model.RolId.ToString());

                if (role == null) { return StatusCode(404, "Rol Does not exist"); }

                var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user == null) { return StatusCode(404, "User does not exist."); }

                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    return StatusCode(400, String.Format("User: {0} does not belong to this role", user));
                }

                var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {
                    return StatusCode(400, String.Format("User: {0} could not be removed from this role", user));
                }

                return StatusCode(204, "User Removed");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null) { return StatusCode(404, "User not found"); }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                HttpContext.Response.Headers.Add("PasswordResetToken", token);

                return StatusCode(204, "Token Generated");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Route("Resetpassword")]
        [HttpPost]
        public async Task<IActionResult> Resetpassword([FromBody]  ResetPasswordDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null) { return StatusCode(404, "User not found"); }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Email);

                if (!result.Succeeded)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return StatusCode(400, ModelState);
                }
                return StatusCode(204, "Password Changed");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
