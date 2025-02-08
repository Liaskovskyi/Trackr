using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public List<ResultError> Errors { get; }

        private Result(bool success, T? value, List<ResultError> errors)
        {
            IsSuccess = success;
            Value = value;
            Errors = errors;
        }

        public static Result<T> Success(T value) => new(true, value, new List<ResultError>());

        public static Result<T> Failure(List<ResultError> errors) => new(false, default, errors);
        public static Result<T> Failure(string code, string description) =>
            new(false, default, new List<ResultError> { new ResultError(code, description) });

    }

}
