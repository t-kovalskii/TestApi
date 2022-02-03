using System.Net;
using TestApi.Data;
using TestApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestApi;

public class AuthFilter : IActionFilter
{
    private readonly ShoppingListContext _dbcontext;

    public AuthFilter(ShoppingListContext context)
        => _dbcontext = context;

    public void OnActionExecuting(ActionExecutingContext actionContext)
    {
        var authString = actionContext.HttpContext.Request.Cookies["auth"];

        if (authString == null) 
        {
            SendUnauthenticated(actionContext);
            return;
        }

        var idAndPassword = authString!.Split(":")
            .Select(s => StringHelpers.FromBase64(s)).ToArray();
        var (id, password) = (idAndPassword.FirstOrDefault(), idAndPassword.LastOrDefault());

        if (id == null || password == null) 
        {
            SendUnauthenticated(actionContext);
            return;
        }

        var shoppingList = GetUserShoppingList(id, password);
        if (shoppingList != null)
            actionContext.HttpContext.Items["shoppingList"] = shoppingList;
        else 
            SendUnauthenticated(actionContext);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    private void SendUnauthenticated(ActionExecutingContext context)
    {
        context.Result = new ObjectResult(context.ModelState)
        { 
            Value = new { message = "Not logged in" }, 
            StatusCode = (int) HttpStatusCode.Unauthorized 
        };
    }

    private ShoppingList? GetUserShoppingList(string id, string password)
    {
        if (!int.TryParse(id, out int listId)) return null;

        var list = _dbcontext.ShoppingLists
            .Where(l => l.Id == listId).FirstOrDefault();

        if (list == null) return null;

        return list.Password == password ? list : null;
    }
}
