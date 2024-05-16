using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Baetoti.API.Helpers
{
    public class ModelStateHelper
    {
        public static List<string> GetValidationErrorMessages(ActionContext context)
        {
            var errors = new List<string>();
            foreach (var modelStateKey in context.ModelState.Keys)
            {
                var modelStateVal = context.ModelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    var propertyName = "";
                    if (modelStateKey.Contains('.'))
                        propertyName = Regex.Replace(modelStateKey, @"\[(.*?)\]", string.Empty);
                    else
                        propertyName = modelStateKey;
                    var errorMessage = "";
                    if (error.ErrorMessage.Contains('.'))
                        errorMessage = error.ErrorMessage.Substring(0, error.ErrorMessage.IndexOf('.'));
                    else
                        errorMessage = error.ErrorMessage;
                    errors.Add($"{propertyName}: {errorMessage}");
                }
            }
            return errors;

        }

    }
}
