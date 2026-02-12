using Microsoft.AspNetCore.Mvc.Filters;

namespace Neuraltech.SharedKernel.Infraestructure.Filters
{
    /// <summary>
    /// Stores the controller type in HttpContext.Items so that exception handlers
    /// can resolve the correct IStringLocalizer after UseExceptionHandler() clears the endpoint metadata.
    /// </summary>
    public class ControllerTypeFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["ControllerType"] = context.Controller.GetType();
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
