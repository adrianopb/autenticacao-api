using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutenticacaoAPI.Services;
using AutenticacaoAPI.Entities;

namespace AutenticacaoAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]User userParam)
        {
            var user = _userService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Credenciais inválidas." });

            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}