


using Dapper;
using Microsoft.AspNetCore.Mvc;
using NewPointWebApi.Data;

[Route("/api")]
[ApiController]

public class AccountController(ISqlDataAccess db, ILogger<AccountController> logger) : ControllerBase
{
    private readonly ISqlDataAccess _db = db;
    private readonly ILogger _logger = logger;

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            string sql = @"SELECT id_user , username , phone , user_type , password , date_added FROM User_Data";

            using var conn = _db.OpenConnection();
            await conn.OpenAsync();

            var result = conn.Query<UserData>(sql).ToList();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
        }
    }

    [HttpPost("addUser")]

    public async Task<IActionResult> CreateUser(UserDataDto user)
    {
        try
        {
            string sql = @"INSERT INTO User_Data (username , phone , user_type , password)
            VALUES (@username , @phone , @user_type , @password)
            ";

            var conn = _db.OpenConnection();
            await conn.OpenAsync();

            await _db.SaveData(sql, new
            {
                user.username,
                user.phone,
                user.user_type,
                user.password
            });

            return Ok("User added success");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong!", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });

        }
    }
    [HttpPatch("updateUser/{userId}")]

    public async Task<IActionResult> CreateUser(int userId, UserDataDto user)
    {
        try
        {
            string sql = @"UPDATE User_Data SET 
            username=@username , phone=@phone , user_type=@user_type , password=@password
            WHERE id_user=@userId";

            var conn = _db.OpenConnection();
            await conn.OpenAsync();

            await _db.SaveData(sql, new
            {
                userId,
                user.username,
                user.phone,
                user.user_type,
                user.password
            });

            return Ok("User updated success");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong!", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });

        }
    }

    [HttpGet("login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        try
        {
            string sql = @"SELECT id_user , username , password , user_type FROM User_Data WHERE username=@username AND password=@password"; ;

            var conn = _db.OpenConnection();
            await conn.OpenAsync();

            var result = await _db.LoadData<UserData, dynamic>(sql, new { username, password });
            string message = "";
            var user = result.FirstOrDefault();
            if (user != null)
            {
                if (user.password == password)
                {
                    message = "Logged in successfully";
                }
                else
                {
                    message = "Invalid username or password";
                }
            }
            else
            {
                message = "Invalid username or password";
            }
            var response = new {
                 User = user,
                 Message = message
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Something went wrong!", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });

        }
    }
}