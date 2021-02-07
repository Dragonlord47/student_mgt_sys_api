using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BiometricAPI.Models;

namespace BiometricAPI.Controllers
{
    public class AdminController : ApiController
    {
        private BiometricSystemEntities1 db = new BiometricSystemEntities1();

        // GET: api/Admin
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }


        [Route("api/GetAdminEmail")]
        [HttpPost]
        public IQueryable<User> Get_Admin(doubleString values)
        {

       
            var result = from user in db.Users
                         where user.Email.Contains(values.value1)
                         select user;


            return result;

        }


        [Route("api/get_status")]
        [HttpPost]
        public string Get_Status(string email)
        {
            IQueryable<User> result = from user in db.Users
                                         where user.Email.Contains(email)
                                         select user;


            var admin = result.ElementAt(0);
            var status = admin.email_code;
            
            return status;

        }


        [Route("api/passwordReset")]
        [HttpPost]
        public async Task<IHttpActionResult> ResetPassword(doubleString values)
        {
            var admin = (from user in db.Users
                                       where user.Email.Contains(values.value1)
                                       select user).SingleOrDefault();


           
           admin.Password = values.value2;

            await db.SaveChangesAsync();
            return Ok("Data updated");

        }


        [Route("api/set_status")]
        [HttpPost]
        public async Task<IHttpActionResult> Update_status(doubleString values)
        {
            var admin = (from user in db.Users
                          where user.Email.Contains(values.value1)
                          select user).SingleOrDefault();

            admin.email_code = values.value2;

            await db.SaveChangesAsync();
            return Ok("Data updated");

        }



        [Route("api/Admin/Search")]
        [HttpPost]
        public IQueryable<User> GetUsers(UserSearchFormat data)
        {
            if (data.columnNumber == 1)
            {
                var result = from user in db.Users
                             where user.Firstname.StartsWith(data.columnValue)
                             select user;
                return result;
            }
            else if (data.columnNumber == 2)
            {
                var result = from user in db.Users
                             where user.Lastname.StartsWith(data.columnValue)
                             select user;
                return result;
            }
            else if (data.columnNumber == 3)
            {
                var result = from user in db.Users
                             where user.AdminType.StartsWith(data.columnValue)
                             select user;
                return result;
            }
            else if (data.columnNumber == 4)
            {
                var result = from user in db.Users
                             where user.UserID.StartsWith(data.columnValue)
                             select user;
                return result;
            }
            return null;

        }

        //GET: api/Admin/Search


        // GET: api/Admin/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Admin/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.SN)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Admin
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                db.Users.Add(user);
                db.SaveChanges();

            }
            catch(DbEntityValidationException ex)
            {
                foreach( var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach(var validationError in entityValidationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = user.SN }, user);



        }

        // DELETE: api/Admin/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.SN == id) > 0;
        }
    }
}