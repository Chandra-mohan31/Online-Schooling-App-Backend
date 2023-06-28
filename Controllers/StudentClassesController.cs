using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONLINE_SCHOOL_BACKEND.Data;
using ONLINE_SCHOOL_BACKEND.Models;

namespace ONLINE_SCHOOL_BACKEND.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentClassesController : ControllerBase
    {
        private readonly OnlineSchoolDbContext _context;
        private readonly UserManager<OnlineSchoolUser> _userManager;


        public StudentClassesController(OnlineSchoolDbContext context,UserManager<OnlineSchoolUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/StudentClasses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentClasses>>> GetStudentClasses()
        {
          if (_context.StudentClasses == null)
          {
              return NotFound();
          }
            return await _context.StudentClasses.Include(student => student.Student).Include(student => student.Class).ToListAsync();
        }




        // POST: api/StudentClasses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentClasses>> PostStudentClasses(StudentClasses studentClasses)
        {
          if (_context.StudentClasses == null)
          {
              return Problem("Entity set 'OnlineSchoolDbContext.StudentClasses'  is null.");
          }
            _context.StudentClasses.Add(studentClasses);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudentClasses", new { id = studentClasses.Id }, studentClasses);
        }

        // DELETE: api/StudentClasses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentClasses(int id)
        {
            if (_context.StudentClasses == null)
            {
                return NotFound();
            }
            var studentClasses = await _context.StudentClasses.FindAsync(id);
            if (studentClasses == null)
            {
                return NotFound();
            }

            _context.StudentClasses.Remove(studentClasses);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("getStudentBelongingClass")]
        public async Task<IActionResult> getStudentBelongingClass([FromQuery] string userId)
        {
            Console.WriteLine(userId);
            var StudentClass = _context.StudentClasses.Include(s => s.Student).Include(s => s.Class).Where(s => s.Student.Id == userId).Select(s => new
            {
                className = s.Class.ClassName,


            }).SingleOrDefault();
            Console.WriteLine(StudentClass);
            if (StudentClass == null)
            {
                return BadRequest("Class Not found!");
            }
            return Ok(new
            {
                StudentClass
            });
        }
        private bool StudentClassesExists(int id)
        {
            return (_context.StudentClasses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
