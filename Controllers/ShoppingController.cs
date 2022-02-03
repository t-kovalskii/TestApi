using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TestApi.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using TestApi.Models;

namespace TestApi.Controllers
{
    [Route("api/list")]
    [ApiController]
    [TypeFilter(typeof(AuthFilter))]
    public class ShoppingController : ControllerBase
    {
        private readonly ShoppingListContext _context;

        public ShoppingController(ShoppingListContext context)
            => _context = context;

        /// <summary>Returns current user's shopping list</summary>
        /// <returns>Current user's shopping list</returns>
        /// <response code="200">Returns shopping list json object</response>
        /// <response code="500">Failed to get user's shopping list</response>
        /// <response code="401">User is not authorized or login credentials are invalid</response>
        [HttpGet]
        public IActionResult GetList()
        {
            var shoppingList = HttpContext.Items["shoppingList"] as ShoppingList;
            if (shoppingList == null) 
                return StatusCode(500, "Failed to get shopping list");

            var listItems = _context.Items
                .Where(item => item.ShoppingListId == shoppingList.Id);

            return Ok(new 
            {
                title = shoppingList.Title, 
                items = listItems.Select(item => new 
                { 
                    id = item.Id,
                    name = item.Name, value = item.Value 
                })
            });
        }

        /// <summary>Adds item to the shopping list</summary>
        /// <returns>Created item</returns>
        /// <response code="201">Returns created item</response>
        /// <response code="401">User is not authorized or login credentials are invalid</response>
        /// <response code="500">Failed to get user's shopping list</response>
        [HttpPost]
        public IActionResult PostItem(ItemRequest item)
        {
            var shoppingList = HttpContext.Items["shoppingList"] as ShoppingList;
            Console.WriteLine($"Shopping list id: {shoppingList?.Id}");
            if (shoppingList == null) 
                return StatusCode(500, "Failed to get shopping list");

            var newItem = new Item() 
            {
                Name = item.Name, Value = item.Value,
                ShoppingListId = shoppingList.Id
            };
            _context.Items.Add(newItem);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetItem), new { item_id = newItem.Id }, newItem);
        }

        /// <summary>Returns item with the given id in the shopping list</summary>
        /// <returns>Item json object</returns>
        /// <response code="200">Returns item</response>
        /// <response code="401">User is not authorized or login credentials are invalid</response>
        /// <response code="204">No item found with given id</response>
        /// <response code="500">Failed to get user's shopping list</response>
        [HttpGet("{item_id:int}")]
        public IActionResult GetItem(int item_id)
        {
            var shoppingList = HttpContext.Items["shoppingList"] as ShoppingList;
            if (shoppingList == null) 
                return StatusCode(500, "Failed to get shopping list");

            var item = _context.Items
                .Where(item => item.Id == item_id).FirstOrDefault();

            if (item == null) return NoContent();

            if (item.ShoppingListId != shoppingList.Id)
                return Unauthorized(new { message = "Not authorized to view this item" });

            return Ok(new 
            {
                id = item.Id, name = item.Name,
                value = item.Value
            });
        }

        /// <summary>Modifies item in the user's shopping list by its id</summary>
        /// <returns>Modified item</returns>
        /// <response code="201">Returns modified item</response>
        /// <response code="204">No item found with given id</response>
        /// <response code="401">User is not authorized or login credentials are invalid</response>
        /// <response code="500">Failed to get user's shopping list</response>
        [HttpPut("{item_id:int}")]
        public IActionResult ModifyItem(int item_id, ItemRequest itm)
        {
            var shoppingList = HttpContext.Items["shoppingList"] as ShoppingList;
            if (shoppingList == null) 
                return StatusCode(500, "Failed to get shopping list");

            var item = _context.Items
                .Where(item => item.Id == item_id).FirstOrDefault();

            if (item == null) return NoContent();

            if (item.ShoppingListId != shoppingList.Id)
                return Unauthorized(new { message = "You are not authorized to edit this item" });

            item.Name = itm.Name;
            item.Value = itm.Value;

            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetItem), new { item_id = item.Id }, item);
        }

        /// <summary>Deletes item with the given id from the shopping list</summary>
        /// <returns>Empty body</returns>
        /// <response code="200">Item deleted</response>
        /// <response code="204">No item found with given id</response>
        /// <response code="401">User is not authorized or login credentials are invalid</response>
        /// <response code="500">Failed to get user's shopping list</response>
        [HttpDelete("{item_id:int}")]
        public IActionResult DeleteItem(int item_id)
        {
            var shoppingList = HttpContext.Items["shoppingList"] as ShoppingList;
            if (shoppingList == null) 
                return StatusCode(500, "Failed to get shopping list");

            var item = _context.Items.Where(item => item.Id == item_id).FirstOrDefault();
            if (item == null) return NoContent();

            if (item.ShoppingListId != shoppingList.Id)
                return Unauthorized(new { message = "Not authorized to delete this item" });

            _context.Items.Remove(item);
            _context.SaveChanges();

            return Ok(new { message = "Item deleted" });
        }
    }
}
