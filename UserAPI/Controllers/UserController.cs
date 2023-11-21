using Microsoft.AspNetCore.Mvc;
using UserAPI.Models;
using UserAPI.Services;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
        {
            return BadRequest("User information is incomplete.");
        }

        var createdUser = await _userService.CreateUserAsync(user);
        _logger.LogInformation("User created with email: {Email}", user.Email);
        return Ok(createdUser);
    }

    [HttpGet("Find")]
    public async Task<IActionResult> Find([FromQuery] string? email, [FromQuery] string? phone)
    {
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
        {
            return BadRequest("At least one of email or phone number is required.");
        }

        var user = await _userService.FindUserByEmailOrPhoneAsync(email, phone);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(user);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        // Maybe we someday want to be able to actually verify that the password is correct and handle some kind of login process...
        bool isValidUser = await _userService.VerifyUserPasswordAsync(email, password);

        if (!isValidUser)
        {
            return Unauthorized("Invalid credentials.");
        }

        // Continue with the login process (tokens, etc...). Out of scope for this.

        return Ok();
    }
}
