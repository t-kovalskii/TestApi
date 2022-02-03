using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestApi.Data;
using TestApi.Models;
using TestApi;

namespace TestApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ShoppingListContext _context;

        public AuthController(ShoppingListContext context)
            => _context = context;

        /// <summary>Registers new shopping list</summary>
        /// <returns>Id of newly created shopping list</returns>
        /// <response code="200">Id of newly created shopping list</response>
        /// <response code="400">Incorrect request body</response>
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest registerRequest)
        {
            var list = new ShoppingList()
            {
                Title = registerRequest.Title, 
                Password = registerRequest.Password
            };

            _context.ShoppingLists.Add(list);
            _context.SaveChanges();

            var authString = GetAuthString(list.Id.ToString(), list.Password);

            Response.Cookies.Append("auth", authString);
            return Ok(new { id = list.Id });
        }

        /// <summary>Logs the user in if given list id and password are correct</summary>
        /// <returns>Sets auth cookies and sends json obejct with message</returns>
        /// <response code="200">Logged in</response>
        /// <response code="404">List with the given id not found</response>
        /// <response code="400">Incorrect request body</response>
        /// <response code="401">Incorrect password</response>
        [HttpPost("login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            var shoppingList = _context.ShoppingLists
                .Where(list => list.Id == loginRequest.Id).FirstOrDefault();

            if (shoppingList == null) 
                return NotFound(new { message = "List with this id not found" });

            if (loginRequest.Password != shoppingList.Password)
                return Unauthorized(new { message = "Incorrect password" });

            Response.Cookies.Append("auth", 
                GetAuthString(shoppingList.Id.ToString(), shoppingList.Password));
            return Ok(new { message = "Logged in" });
        }

        /// <summary>Logs out the user</summary>
        /// <returns>Object with message</returns>
        /// <response code="200">Logged out</response>
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("auth", "", new CookieOptions() 
            { 
                Expires = DateTime.Now.AddDays(-1)
            });

            return Ok(new { message = "Logged out" });
        }

        private static string GetAuthString(string id, string password)
        {
            var encodedIdAndPassword = new List<string>() { id, password }
                .Select(s => StringHelpers.ToBase64(s));
            
            return string.Join(":", encodedIdAndPassword);
        }
    }
}
