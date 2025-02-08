using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using Trackr.Domain.Models;

namespace Trackr.Api.Extensions
{
    public static class RetrieveErrorsExtension
    {
        public static ValidationProblemDetails RetrieveErrors<T>(this Result<T> result)
        {
            ModelStateDictionary state = new ModelStateDictionary();
            foreach (var error in result.Errors) state.AddModelError(error.Code, error.Description);
            var problemDetails = new ValidationProblemDetails(state);
            return problemDetails;
        }
    }
}
