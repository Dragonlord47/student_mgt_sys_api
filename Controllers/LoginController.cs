using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using BiometricAPI.Models;

namespace BiometricAPI.Controllers
{
    public class LoginController : ApiController
    {
        BiometricSystemEntities1 db = new BiometricSystemEntities1();

        //api/Login
        //[HttpPost]
        public object Login(LoginVM data)
        {
            var emp = (from e in db.Users
                      where e.Username.Equals(data.LoginName)
                      && e.Password.Equals(data.Password)
                      select e).SingleOrDefault();

            if(emp != null)
            {
                FormsAuthentication.SetAuthCookie(emp.UserID.ToString(), false);
                return emp;
            }
            else
            {
                return NotFound();
            }
           
        }

        //api/login
        [HttpGet]
        public void Logout()
        {
            FormsAuthentication.SignOut();
        }

        [Route("api/API_status")]
        [HttpGet]
        public int API_Status()
        {          
            return 1;
        }
    }
}
 