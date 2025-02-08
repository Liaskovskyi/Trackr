//using Microsoft.AspNet.Identity;
//using Microsoft.AspNetCore.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models
{
    public class User : IdentityUser
    {
        //public int Id { get; }
        //public string UserName { get; set; }
        //public string Email { get; set; }
        //public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; }
    }
}
