using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace Baetoti.Shared.Response.Shared
{
    public class GenericResponses
    {
        public static string ExceptionResponse()
        {
            return JsonConvert.SerializeObject(new
            {
                isSuccess = false,
                statusCode = HttpStatusCode.InternalServerError,
                message = "Oops! something went wrong"
            });
        }

        public static SharedResponse InvalidModelStateResponse(List<string> errors)
        {
            return new SharedResponse(false, 400, "Validation fail", errors);
        }

    }

}
