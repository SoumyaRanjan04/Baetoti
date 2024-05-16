using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace Baetoti.API.Filters
{
    [AttributeUsage(AttributeTargets.All)]
    public class ValidateModelStateFilterAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorDictionary = new Dictionary<string, string>();

                foreach (var modelStateKey in context.ModelState.Keys)
                {
                    var modelStateVal = context.ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        //var key = Regex.Replace(modelStateKey, @"\[(.*?)\]", string.Empty);
                        var errorMessage = error.ErrorMessage;
                        if (!errorDictionary.ContainsKey(modelStateKey))
                        {
                            errorDictionary.Add(modelStateKey, errorMessage);
                        }
                    }
                }
                context.Result = new BadRequestObjectResult(new SharedResponse(false, 400, "Validation fail.", errorDictionary));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

    }
}
