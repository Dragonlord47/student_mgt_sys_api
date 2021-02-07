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

namespace WebApplication3.Controllers
{
    public class StudentsController : ApiController
    {

        private BiometricSystemEntities2 db = new BiometricSystemEntities2();

        // GET: api/Students
        public IQueryable<Student> GetStudents()
        {
            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            foreach (Student student in db.Students)
            {
                if (student.pictureURL != null)
                    student.pictureURL = baseUrl + "/StudentImages" + student.pictureURL;
                else
                    student.pictureURL = baseUrl + "/StudentImages/user_picture.png";
            }
            return db.Students;
        }

        [Route("api/Student/Image")]
        [ResponseType(typeof(FileUpload))]
        public IHttpActionResult uploadFiles()
        {

            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                //Get the uploaded image from the files collection
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
                if (httpPostedFile != null)
                {
                    FileUpload imgupload = new FileUpload();
                    int length = httpPostedFile.ContentLength;
                    imgupload.imagedata = new byte[length];
                    httpPostedFile.InputStream.Read(imgupload.imagedata, 0, length);
                    imgupload.imagename = System.IO.Path.GetFileName(httpPostedFile.FileName);
                    var filename = System.IO.Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                    var student_update = (from student in db.Students where student.StudentID.Equals(filename) select student).SingleOrDefault();
                    student_update.pictureURL = "/" + httpPostedFile.FileName;
                    db.SaveChanges();
                    var fileSavePath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/StudentImages"), httpPostedFile.FileName);
                    httpPostedFile.SaveAs(fileSavePath);
                    return Ok("Image Uploaded");
                }

            }
                return Ok("Image is not Uploaded");
        }



        [Route("api/Current_Student")]
        [HttpGet]
        public IQueryable<Student> Get_Current_Students()
        {
            IQueryable<Student> result = from student in db.Students
                                         where !student.status.Contains("graduated")  && !student.status.Contains("expelled")
                                         select student;


            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            foreach (Student student in db.Students)
            {
                if (student.pictureURL != null)
                    student.pictureURL = baseUrl + "/StudentImages" + student.pictureURL;
                else
                    student.pictureURL = baseUrl + "/StudentImages/user_picture.png";
            }

            return result;

        }




        [Route("api/Male_Student")]
        [HttpGet]
        public IQueryable<Student> Get_Male_Students()
        {
            IQueryable<Student> result = from student in db.Students
                                         where !student.Gender.Contains("Fe") && !student.status.Contains("graduated") && !student.status.Contains("expelled")
                                         select student;


            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            foreach (Student student in db.Students)
            {
                if (student.pictureURL != null)
                    student.pictureURL = baseUrl + "/StudentImages" + student.pictureURL;
                else
                    student.pictureURL = baseUrl + "/StudentImages/user_picture.png";
            }

            return result;

        }

        [Route("api/Female_Student")]
        [HttpGet]
        public IQueryable<Student> Get_Female_Students()
        {
            IQueryable<Student> result = from student in db.Students
                                         where student.Gender.Contains("Female")  && !student.status.Contains("graduated") && !student.status.Contains("expelled")
                                         select student;


            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            foreach (Student student in db.Students)
            {
                if (student.pictureURL != null)
                    student.pictureURL = baseUrl + "/StudentImages" + student.pictureURL;
                else
                    student.pictureURL = baseUrl + "/StudentImages/user_picture.png";
            }

            return result;

        }


        [Route("api/Student/Matric")]
        [HttpPost]
        public  Student GetStudentByMatric(UserSearchFormat data)
        {
            Student result = null;
            result = (from student in db.Students
                      where student.MatricNumber.Contains(data.columnValue)
                      select student).FirstOrDefault();

            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

            if (result.pictureURL != null)
                result.pictureURL = baseUrl + "/StudentImages" + result.pictureURL;
            else
                result.pictureURL = baseUrl + "/StudentImages/user_picture.png";

            return result;

        }


        [Route("api/Archived_Student")]
        [HttpGet]
        public IQueryable<Student> Get_Archived_Students()
        {
            IQueryable<Student> result = from student in db.Students
                                         where student.status.Contains("graduated") || student.status.Contains("expelled")
                                         select student;


            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            foreach (Student student in db.Students)
            {
                if (student.pictureURL != null)
                    student.pictureURL = baseUrl + "/StudentImages" + student.pictureURL;
                else
                    student.pictureURL = baseUrl + "/StudentImages/user_picture.png";
            }

            return result;

        }

        [Route("api/Update_Student")]
        [HttpPost]
        public async Task <IHttpActionResult> Update_Students(Student data)
        {
            var matric_number = data.MatricNumber.Trim();

            var person = (from student in db.Students
                          where student.MatricNumber.Contains(matric_number)
                          select student).FirstOrDefault();
           
                        person.Department = data.Department;
                        person.StudentLevel = data.StudentLevel;
                        person.Hostel = data.Hostel;
                        person.Room_number = data.Room_number;
                
            await db.SaveChangesAsync();
            return Ok("Data updated");
        }

        [Route("api/Update_Level")]
        [HttpPost]
        public async Task<IHttpActionResult> Update_Level()
        {
            var result = from student in db.Students
                         select student;

            foreach(Student student in result)
            {
                var level = student.StudentLevel;
                int updated_level = int.Parse(level.Trim()) + 100;
                if(updated_level <= 400)
                {
                    student.StudentLevel = updated_level.ToString();
                }
                else if(updated_level == 600 || updated_level == 500)
                {
                    student.status = "graduated";
                }
            }

            await db.SaveChangesAsync();
            return Ok("Data updated");
        }

        [Route("api/Update_CheckStudent")]
        [HttpPost]
        public async Task<IHttpActionResult> Update_Check_Students(Student data)
        {
            var person = (from student in db.Students
                         where student.MatricNumber.Contains(data.MatricNumber)
                         select student).FirstOrDefault();

           
            person.LaptopModel = data.LaptopModel;
            person.LaptopSerialNumber = data.LaptopSerialNumber;
            person.LaptopType = data.LaptopType;
            person.PhoneIMEI = data.PhoneIMEI.Trim();
            person.PhoneModel = data.PhoneModel;
            person.PhoneType = data.PhoneType;
            person.PhoneNumber = data.PhoneNumber.Trim();
            person.StudentLevel = data.StudentLevel;
            person.Hostel = data.Hostel;
            person.Room_number = data.Room_number;
            person.status = data.status;

            await db.SaveChangesAsync();
            return Ok("Data updated");
        }

        [Route("api/Student/Search")]
        [HttpPost]
        public IQueryable<Student> GetStudents(UserSearchFormat data)
        {
            IQueryable<Student> result = null;
                
            if (data.columnNumber == 1)
            {
                result = from student in db.Students
                             where student.StudentID.StartsWith(data.columnValue)
                             select student;
              
            }
            else if (data.columnNumber == 2)
            {
                result = from student in db.Students
                             where student.MatricNumber.StartsWith(data.columnValue)
                             select student;
               
            }
            else if (data.columnNumber == 3)
            {
                result = from student in db.Students
                             where student.Surname.StartsWith(data.columnValue)
                             select student;
                
            }
            else if (data.columnNumber == 4)
            {
                result = from student in db.Students
                             where student.Firstname.StartsWith(data.columnValue)
                             select student;
                
            }
            else if (data.columnNumber == 5)
            {
                result = from student in db.Students
                             where student.Department.StartsWith(data.columnValue)
                             select student;
               
            }
            else if (data.columnNumber == 6)
            {
                result = from student in db.Students
                             where student.StudentLevel.StartsWith(data.columnValue)
                             select student;
              
            }
            else if (data.columnNumber == 7)
            {
                result = from student in db.Students
                             where student.PhoneNumber.StartsWith(data.columnValue)
                             select student;
               
            }
            else if (data.columnNumber == 8)
            {
                result = from student in db.Students
                             where student.PhoneType.StartsWith(data.columnValue)
                             select student;
                
            }
            else if (data.columnNumber == 9)
            {
                result = from student in db.Students
                             where student.PhoneModel.StartsWith(data.columnValue)
                             select student;
             
            }
            else if (data.columnNumber == 10)
            {
                result = from student in db.Students
                             where student.PhoneIMEI.StartsWith(data.columnValue)
                             select student;
               
            }
            else if (data.columnNumber == 11)
            {
                result = from student in db.Students
                             where student.LaptopType.StartsWith(data.columnValue)
                             select student;
               
            }
            else if (data.columnNumber == 12)
            {
                result = from student in db.Students
                             where student.LaptopModel.StartsWith(data.columnValue)
                             select student;
               
            }
            else if (data.columnNumber == 13)
            {
                result = from student in db.Students
                             where student.LaptopSerialNumber.StartsWith(data.columnValue)
                             select student;  
            }

            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            foreach (Student student in db.Students)
            {
                if (student.pictureURL != null)
                    student.pictureURL = baseUrl + "/StudentImages" + student.pictureURL;
                else
                    student.pictureURL = baseUrl + "/StudentImages/user_picture.png";
            }

            return result;

        }



        [Route("api/StudentMale/Search")]
        [HttpPost]
        public IQueryable<Student> GetStudents_Male(UserSearchFormat data)
        {
            IQueryable<Student> result = null;

            if (data.columnNumber == 1)
            {
                result = from student in db.Students
                         where student.StudentID.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 2)
            {
                result = from student in db.Students
                         where student.MatricNumber.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 3)
            {
                result = from student in db.Students
                         where student.Surname.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 4)
            {
                result = from student in db.Students
                         where student.Firstname.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 5)
            {
                result = from student in db.Students
                         where student.Department.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 6)
            {
                result = from student in db.Students
                         where student.StudentLevel.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 7)
            {
                result = from student in db.Students
                         where student.PhoneNumber.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 8)
            {
                result = from student in db.Students
                         where student.PhoneType.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 9)
            {
                result = from student in db.Students
                         where student.PhoneModel.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 10)
            {
                result = from student in db.Students
                         where student.PhoneIMEI.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 11)
            {
                result = from student in db.Students
                         where student.LaptopType.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 12)
            {
                result = from student in db.Students
                         where student.LaptopModel.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;

            }
            else if (data.columnNumber == 13)
            {
                result = from student in db.Students
                         where student.LaptopSerialNumber.StartsWith(data.columnValue) && !student.Gender.Contains("Fe")
                         select student;
            }

            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            foreach (Student student in db.Students)
            {
                if (student.pictureURL != null)
                    student.pictureURL = baseUrl + "/StudentImages" + student.pictureURL;
                else
                    student.pictureURL = baseUrl + "/StudentImages/user_picture.png";
            }

            return result;

        }


        [Route("api/StudentFemale/Search")]
        [HttpPost]
        public IQueryable<Student> GetStudents_Female(UserSearchFormat data)
        {
            IQueryable<Student> result = null;

            if (data.columnNumber == 1)
            {
                result = from student in db.Students
                         where student.StudentID.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 2)
            {
                result = from student in db.Students
                         where student.MatricNumber.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 3)
            {
                result = from student in db.Students
                         where student.Surname.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 4)
            {
                result = from student in db.Students
                         where student.Firstname.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 5)
            {
                result = from student in db.Students
                         where student.Department.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 6)
            {
                result = from student in db.Students
                         where student.StudentLevel.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 7)
            {
                result = from student in db.Students
                         where student.PhoneNumber.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 8)
            {
                result = from student in db.Students
                         where student.PhoneType.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 9)
            {
                result = from student in db.Students
                         where student.PhoneModel.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 10)
            {
                result = from student in db.Students
                         where student.PhoneIMEI.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 11)
            {
                result = from student in db.Students
                         where student.LaptopType.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 12)
            {
                result = from student in db.Students
                         where student.LaptopModel.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;

            }
            else if (data.columnNumber == 13)
            {
                result = from student in db.Students
                         where student.LaptopSerialNumber.StartsWith(data.columnValue) && student.Gender.Contains("Female")
                         select student;
            }

            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            foreach (Student student in db.Students)
            {
                if (student.pictureURL != null)
                    student.pictureURL = baseUrl + "/StudentImages" + student.pictureURL;
                else
                    student.pictureURL = baseUrl + "/StudentImages/user_picture.png";
            }

            return result;

        }


        // GET: api/Students/5
        [ResponseType(typeof(Student))]
        public IHttpActionResult GetStudent(int id)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // PUT: api/Students/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutStudent(int id, Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != student.SN)
            {
                return BadRequest();
            }

            db.Entry(student).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/Students
        [ResponseType(typeof(Student))]
        public IHttpActionResult PostStudent(Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Students.Add(student);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = student.SN }, student);
        }

        // DELETE: api/Students/5
        [ResponseType(typeof(Student))]
        public IHttpActionResult DeleteStudent(int id)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            db.Students.Remove(student);
            db.SaveChanges();

            return Ok(student);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StudentExists(int id)
        {
            return db.Students.Count(e => e.SN == id) > 0;
        }
    }
}