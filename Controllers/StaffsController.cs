using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BiometricAPI.Models;

namespace BiometricAPI.Controllers
{
    public class StaffsController : ApiController
    {
        private BiometricSystemEntities5 db = new BiometricSystemEntities5();

        // GET: api/Staffs
        public IQueryable<Staff> GetStaffs()
        {
            return db.Staffs;
        }

        // GET: api/Staffs/5
        [ResponseType(typeof(Staff))]
        public async Task<IHttpActionResult> GetStaff(int id)
        {
            Staff staff = await db.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

        // PUT: api/Staffs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutStaff(int id, Staff staff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != staff.SN)
            {
                return BadRequest();
            }

            db.Entry(staff).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(id))
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

        // POST: api/Staffs
        [ResponseType(typeof(Staff))]
        public async Task<IHttpActionResult> PostStaff(Staff staff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Staffs.Add(staff);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = staff.SN }, staff);
        }

        // DELETE: api/Staffs/5
        [ResponseType(typeof(Staff))]
        public async Task<IHttpActionResult> DeleteStaff(int id)
        {
            Staff staff = await db.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            db.Staffs.Remove(staff);
            await db.SaveChangesAsync();

            return Ok(staff);
        }


        [Route("api/All_Staff")]
        [HttpGet]
        public IQueryable<Staff> Get_Current_Students()
        {
            IQueryable<Staff> result = from staff in db.Staffs
                                         select staff;                                        


            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            foreach (Staff staff in db.Staffs)
            {
                if (staff.pictureURL != null)
                    staff.pictureURL = baseUrl + "/StaffImages" + staff.pictureURL;
                else
                    staff.pictureURL = baseUrl + "/StaffImages/user_picture.png";
            }

            return result;

        }


        [Route("api/Staff/Search")]
        [HttpPost]
        public IQueryable<Staff> GetStaff(UserSearchFormat data)
        {
            IQueryable<Staff> result = null;

            
            if (data.columnNumber == 1)
            {
                result = from staff in db.Staffs
                         where staff.Surname.StartsWith(data.columnValue)
                         select staff;

            }
            else if (data.columnNumber == 2)
            {
                result = from staff in db.Staffs
                         where staff.Firstname.StartsWith(data.columnValue)
                         select staff;

            }
            else if (data.columnNumber == 3)
            {
                result = from staff in db.Staffs
                         where staff.Staff__ID.StartsWith(data.columnValue)
                         select staff;

            }
            else if (data.columnNumber == 4)
            {
                result = from staff in db.Staffs
                         where staff.Staff_Type.StartsWith(data.columnValue)
                         select staff;

            }

            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            foreach (Staff staff in db.Staffs)
            {
                if (staff.pictureURL != null)
                    staff.pictureURL = baseUrl + "/StaffImages" + staff.pictureURL;
                else
                    staff.pictureURL = baseUrl + "/StaffImages/user_picture.png";
            }



            return result;

        }


        [Route("api/Staff/Id")]
        [HttpPost]
        public Staff GetStaffByID(UserSearchFormat data)
        {
            Staff result = null;
            result = (from staff in db.Staffs
                      where staff.Staff__ID.Contains(data.columnValue)
                      select staff).FirstOrDefault();

            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

            if (result.pictureURL != null)
                result.pictureURL = baseUrl + "/StaffImages" + result.pictureURL;
            else
                result.pictureURL = baseUrl + "/StaffImages/user_picture.png";

            return result;

        }



        [Route("api/Staff/Image")]
        [ResponseType(typeof(FileUpload))]
        public IHttpActionResult uploadFiles()
        {

            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                //Get the uploaded image from the files collection
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedStaffImage"];
                if (httpPostedFile != null)
                {
                    FileUpload imgupload = new FileUpload();
                    int length = httpPostedFile.ContentLength;
                    imgupload.imagedata = new byte[length];
                    httpPostedFile.InputStream.Read(imgupload.imagedata, 0, length);
                    imgupload.imagename = System.IO.Path.GetFileName(httpPostedFile.FileName);
                    var filename = System.IO.Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                    var staff_update = (from staff in db.Staffs where staff.Staff__ID.Equals(filename) select staff).SingleOrDefault();
                    staff_update.pictureURL = "/" + httpPostedFile.FileName;
                    db.SaveChanges();
                    var fileSavePath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/StaffImages"), httpPostedFile.FileName);
                    httpPostedFile.SaveAs(fileSavePath);
                    return Ok("Image Uploaded");
                }

            }
            return Ok("Image is not Uploaded");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StaffExists(int id)
        {
            return db.Staffs.Count(e => e.SN == id) > 0;
        }
    }
}