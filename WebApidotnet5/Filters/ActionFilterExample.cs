using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApidotnet5.Filters
{
    public class ActionFilterExample : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
