using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BiometricAPI.Models;

namespace BiometricAPI.Controllers
{
    public class VisitorsController : ApiController
    {
        private BiometricSystemEntities3 db = new BiometricSystemEntities3();

        // GET: api/Visitors
        public IQueryable<Visitor> GetVisitors()
        {
            return db.Visitors;
        }

        // GET: api/Visitors/5
        [ResponseType(typeof(Visitor))]
        public async Task<IHttpActionResult> GetVisitor(int id)
        {
            Visitor visitor = await db.Visitors.FindAsync(id);
            if (visitor == null)
            {
                return NotFound();
            }

            return Ok(visitor);
        }

        [Route("api/Current_Visitors")]
        [HttpGet]
        public IQueryable<Visitor> Get_Current_Students()
        {
            var date = DateTime.Now.ToString("dd/MM/yyyy");
            IQueryable<Visitor> result = from visitor in db.Visitors
                                         where visitor.DATE_OF_VISIT.Contains(date)
                                         select visitor;

            return result;

        }


        [Route("api/Visitor/Search")]
        [HttpPost]
        public IQueryable<Visitor> GetStudents(UserSearchFormat data)
        {
            IQueryable<Visitor> result = null;

            if (data.columnNumber == 1)
            {
                result = from visitor in db.Visitors
                         where visitor.FIRSTNAME.StartsWith(data.columnValue)
                         select visitor;

            }
            else if (data.columnNumber == 2)
            {
                result = from visitor in db.Visitors
                         where visitor.LASTNAME.StartsWith(data.columnValue)
                         select visitor;

            }
            else if (data.columnNumber == 3)
            {
                result = from visitor in db.Visitors
                         where visitor.WHOM_TO_SEE.StartsWith(data.columnValue)
                         select visitor;

            }
            else if (data.columnNumber == 4)
            {
                result = from visitor in db.Visitors
                         where visitor.DATE_OF_VISIT.StartsWith(data.columnValue)
                         select visitor;

            }
            else if (data.columnNumber == 5)
            {
                result = from visitor in db.Visitors
                         where visitor.TIME_IN.StartsWith(data.columnValue)
                         select visitor;

            }
            else if (data.columnNumber == 6)
            {
                result = from visitor in db.Visitors
                         where visitor.TIME_OUT.StartsWith(data.columnValue)
                         select visitor;

            }
            return result;

        }


        [Route("api/Visitor_Current/Search")]
        [HttpPost]
        public IQueryable<Visitor> GetStudents_Current(UserSearchFormat data)
        {
            IQueryable<Visitor> result = null;
            var date = DateTime.Now.ToString("dd/MM/yyyy");
            if (data.columnNumber == 1)
            {
                result = from visitor in db.Visitors
                         where visitor.FIRSTNAME.StartsWith(data.columnValue) && visitor.DATE_OF_VISIT.Contains(date)
                         select visitor;

            }
            else if (data.columnNumber == 2)
            {
                result = from visitor in db.Visitors
                         where visitor.LASTNAME.StartsWith(data.columnValue) && visitor.DATE_OF_VISIT.Contains(date)
                         select visitor;

            }
            else if (data.columnNumber == 3)
            {
                result = from visitor in db.Visitors
                         where visitor.WHOM_TO_SEE.StartsWith(data.columnValue) && visitor.DATE_OF_VISIT.Contains(date)
                         select visitor;

            }
            else if (data.columnNumber == 4)
            {
                result = from visitor in db.Visitors
                         where visitor.DATE_OF_VISIT.Contains(date)
                         select visitor;

            }
            else if (data.columnNumber == 5)
            {
                result = from visitor in db.Visitors
                         where visitor.TIME_IN.StartsWith(data.columnValue) && visitor.DATE_OF_VISIT.Contains(date)
                         select visitor;

            }
            else if (data.columnNumber == 6)
            {
                result = from visitor in db.Visitors
                         where visitor.TIME_OUT.StartsWith(data.columnValue) && visitor.DATE_OF_VISIT.Contains(date)
                         select visitor;

            }
            return result;

        }


        [Route("api/Update_Visitor")]
        [HttpPost]
        public async Task<IHttpActionResult> Update_Visitor(Visitor data)
        {
            string time = DateTime.Now.ToString("hh:mm");
            Visitor person = await db.Visitors.FindAsync(data.SN);

            
            person.TIME_OUT = time;
            await db.SaveChangesAsync();
            return Ok("Data updated");
        }


        // PUT: api/Visitors/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutVisitor(int id, Visitor visitor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != visitor.SN)
            {
                return BadRequest();
            }

            db.Entry(visitor).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VisitorExists(id))
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

        // POST: api/Visitors
        [ResponseType(typeof(Visitor))]
        public async Task<IHttpActionResult> PostVisitor(Visitor visitor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Visitors.Add(visitor);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = visitor.SN }, visitor);
        }

        // DELETE: api/Visitors/5
        [ResponseType(typeof(Visitor))]
        public async Task<IHttpActionResult> DeleteVisitor(int id)
        {
            Visitor visitor = await db.Visitors.FindAsync(id);
            if (visitor == null)
            {
                return NotFound();
            }

            db.Visitors.Remove(visitor);
            await db.SaveChangesAsync();

            return Ok(visitor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VisitorExists(int id)
        {
            return db.Visitors.Count(e => e.SN == id) > 0;
        }
    }
}