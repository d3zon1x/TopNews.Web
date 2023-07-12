using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopNews.Core.DTOS.User
{
    public class UserLoginDTO
    {
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public bool rememberMe;

        //public bool validation(String username, String password)
        //{
        //    if (username == this._username && password == this._password)
        //        return true;          
        //    return false;
        //}
    }
}
