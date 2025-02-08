using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Trackr.Domain.Models;

public class IdentityMapProfile : Profile
{
    public IdentityMapProfile()
    {
        CreateMap<IdentityError, ResultError>();

        CreateMap(typeof(IdentityResult), typeof(Result<>))
            .ConvertUsing(typeof(IdentityResultToResultConverter<>));
    }
}
