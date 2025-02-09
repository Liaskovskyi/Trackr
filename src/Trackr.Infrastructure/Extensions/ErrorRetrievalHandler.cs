using Azure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;
using Trackr.Infrastructure.DTO;

namespace Trackr.Infrastructure.Extensions
{
    internal static class ErrorRetrievalHandler
    {
        public async static Task<Result<T>> HandleError<T>(this HttpResponseMessage response)
        {
            string? responseError = await response.Content.ReadAsStringAsync();
            try
            {
                SpotifyErrorResponse? obj = JsonConvert.DeserializeObject<SpotifyErrorResponse>(responseError);
                return Result<T>.Failure(obj?.Error.Status.ToString() ?? "RequestFailed", obj?.Error.Message ?? "Spotify request error.");
            }
            catch (JsonException) { return Result<T>.Failure("RequestFailed", responseError); }
        }
    }
}
