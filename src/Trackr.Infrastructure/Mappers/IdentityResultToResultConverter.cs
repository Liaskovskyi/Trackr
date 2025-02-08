using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using Trackr.Domain.Models;

public class IdentityResultToResultConverter<T> : ITypeConverter<IdentityResult, Result<T>>
{
    public Result<T> Convert(IdentityResult source, Result<T> destination, ResolutionContext context)
    {
        if (source.Succeeded)
        {
            return Result<T>.Success(default!); 
        }

        var errors = source.Errors
            .Select(e => new ResultError(e.Code, e.Description))
            .ToList();

        return Result<T>.Failure(errors);
    }
}
