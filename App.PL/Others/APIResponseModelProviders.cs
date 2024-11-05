using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Net.Mime;

namespace App.PL.Others
{
    /// <summary>
    /// Swagger 相關全域 controller action attributes
    /// </summary>
    public class APIResponseModelProviders : IApplicationModelProvider
    {
        public int Order => 3;

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            foreach (ControllerModel controller in context.Result.Controllers)
            {
                foreach (ActionModel action in controller.Actions)
                {
                    // 如果 action 包含 [Authorize] 屬性，添加 401 response 判斷
                    var hasAuthAttribute = action.Attributes.Any(e => e.GetType() == typeof(AuthorizeAttribute));
                    if (hasAuthAttribute)
                    {
                        action.Filters.Add(new ProducesResponseTypeAttribute(typeof(void), StatusCodes.Status401Unauthorized));
                    }

                    // 預設所有 response content-type 皆為 application/json
                    action.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
                    // 預設所有 action 皆可能造成 internal server error
                    action.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status500InternalServerError));
                }
            }
        }
    }
}
