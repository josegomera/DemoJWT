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
        private readonly UserManager<Usuario> _userManager;

        HttpClient cliente = new HttpClient();

        public AuthenticationController(IAuthenticateService authService,
                                        UserManager<Usuario> userManager,
                                        SignInManager<Usuario> signInManager)
        {
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                string Token;
                if (_authService.IsAuthenticated(user, out Token))
                {

                    HttpContext.Session.SetString("Token", Token);

                    cliente.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer",
                    HttpContext.Session.GetString("Token"));

                    return Ok(new { token = Token });
                }
            }
            return BadRequest();
        }

        [Route("Logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
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

                if (user != null) { BadRequest("This user already exist"); }

                user = new Usuario
                {
                    UserName = model.Username,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    Cedula = model.Cedula
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                return StatusCode(201);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resultUrl = Url.Action("Resetpassword", "Authentication",
                        new { token = token, email = user.Email }, Request.Scheme);

                    System.IO.File.WriteAllText("resetlink", resultUrl);
                }
                else
                {
                    BadRequest("No encontramos tu correo en la base de datos");
                }
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        public IActionResult Resetpassword(string token, string email)
        {
            return Ok(new ResetPasswordDTO { Token = token, Email = email });
        }

        [Route("Resetpassword")]
        [HttpPost]
        public async Task<IActionResult> Resetpassword(ResetPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Email);

                    if (!result.Succeeded)
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                        return BadRequest(ModelState);
                    }
                    return Ok();
                }
                return BadRequest(ModelState);
            }
            return BadRequest(ModelState);
        }
    }
}
