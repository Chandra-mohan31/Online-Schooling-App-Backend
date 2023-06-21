using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONLINE_SCHOOL_BACKEND.Data;
using ONLINE_SCHOOL_BACKEND.Models;

namespace ONLINE_SCHOOL_BACKEND.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolSubjectsController : ControllerBase
    {
        private readonly OnlineSchoolDbContext _context;

        public SchoolSubjectsController(OnlineSchoolDbContext context)
        {
            _context = context;
        }

        // GET: api/SchoolSubjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SchoolSubjects>>> GetsubjectsAvailable()
        {
          if (_context.subjectsAvailable == null)
          {
              return NotFound();
          }
            return await _context.subjectsAvailable.ToListAsync();
        }



        [HttpGet("{subName}")]
        public async Task<IActionResult> GetSchoolClass([FromRoute] string subName)
        {
            if (_context.subjectsAvailable == null)
            {
                return NotFound();
            }
            var schoolSubject = await _context.subjectsAvailable.FirstOrDefaultAsync(s => s.SubjectName == subName);
            if (schoolSubject == null)
            {
                return NotFound();
            }


            return Ok(schoolSubject);
        }


        // POST: api/SchoolSubjects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SchoolSubjects>> PostSchoolSubjects(SchoolSubjects schoolSubject)
        {
          if (_context.subjectsAvailable == null)
          {
              return Problem("Entity set 'OnlineSchoolDbContext.subjectsAvailable'  is null.");
          }
            _context.subjectsAvailable.Add(schoolSubject);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SchoolSubjectsExists(schoolSubject.SubjectName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(schoolSubject);
        }

        // DELETE: api/SchoolSubjects/5
        [HttpDelete("{subName}")]
        public async Task<IActionResult> DeleteSchoolSubjects([FromRoute]string subName)
        {
            if (_context.subjectsAvailable == null)
            {
                return NotFound();
            }
            var schoolSubject = await _context.subjectsAvailable.FindAsync(subName);
            if (schoolSubject == null)
            {
                return NotFound();
            }

            _context.subjectsAvailable.Remove(schoolSubject);
            await _context.SaveChangesAsync();

            return Ok("deleted successfully!");
        }

        private bool SchoolSubjectsExists(string subName)
        {
            return (_context.subjectsAvailable?.Any(e => e.SubjectName == subName)).GetValueOrDefault();
        }
    }
}
