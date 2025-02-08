using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models
{
    public class ResultError
    {
        public string Code { get; }
        public string Description { get; }

        public ResultError(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }

}
