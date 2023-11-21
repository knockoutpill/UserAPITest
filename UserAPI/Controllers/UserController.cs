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

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <remarks>
    /// This endpoint is responsible for creating a new user with the provided details. 
    /// The user's email and password are required fields. 
    /// If successful, it returns the details of the newly created user.
    /// </remarks>
    /// <param name="user">The user object containing email, phone number, and password.</param>
    /// <response code="200">Returns the newly created user</response>
    /// <response code="400">If the user is null, or the required fields are not provided, or if the model state is not valid</response>
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


    /// <summary>
    /// Finds a user by their email and/or phone number.
    /// </summary>
    /// <remarks>
    /// This endpoint searches for a user based on the provided email or phone number. 
    /// At least one of these fields must be provided for the search to be conducted. 
    /// If a matching user is found, their details are returned.
    /// </remarks>
    /// <param name="email">The email address of the user to be found. This field is optional.</param>
    /// <param name="phone">The phone number of the user to be found. This field is optional.</param>
    /// <response code="200">Returns the found user's details</response>
    /// <response code="400">If both email and phone number are not provided or are empty</response>
    /// <response code="404">If no user is found that matches the provided email or phone number</response>
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


    /// <summary>
    /// Attempts to log in a user with the provided email and password.
    /// </summary>
    /// <remarks>
    /// This endpoint is responsible for the user authentication process. 
    /// It currently verifies if the provided email and password match an existing user. 
    /// In the future, it might be extended to handle token generation and other aspects of the login process.
    /// </remarks>
    /// <param name="email">The email address of the user attempting to log in.</param>
    /// <param name="password">The password of the user attempting to log in.</param>
    /// <response code="200">If the login attempt is successful (future implementations may return a token)</response>
    /// <response code="401">If the login attempt is unsuccessful due to invalid credentials</response>
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
