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
    public class TeacherSubjectsController : ControllerBase
    {
        private readonly OnlineSchoolDbContext _context;

        public TeacherSubjectsController(OnlineSchoolDbContext context)
        {
            _context = context;
        }

        // GET: api/TeacherSubjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherSubjects>>> GetTeacherSubjects()
        {
          if (_context.TeacherSubjects == null)
          {
              return NotFound();
          }
            return await _context.TeacherSubjects.ToListAsync();
        }

        // GET: api/TeacherSubjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherSubjects>> GetTeacherSubjects(int id)
        {
          if (_context.TeacherSubjects == null)
          {
              return NotFound();
          }
            var teacherSubjects = await _context.TeacherSubjects.FindAsync(id);

            if (teacherSubjects == null)
            {
                return NotFound();
            }

            return teacherSubjects;
        }

        // PUT: api/TeacherSubjects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacherSubjects(int id, TeacherSubjects teacherSubjects)
        {
            if (id != teacherSubjects.Id)
            {
                return BadRequest();
            }

            _context.Entry(teacherSubjects).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherSubjectsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TeacherSubjects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TeacherSubjects>> PostTeacherSubjects(TeacherSubjects teacherSubjects)
        {
          if (_context.TeacherSubjects == null)
          {
              return Problem("Entity set 'OnlineSchoolDbContext.TeacherSubjects'  is null.");
          }
            _context.TeacherSubjects.Add(teacherSubjects);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeacherSubjects", new { id = teacherSubjects.Id }, teacherSubjects);
        }

        // DELETE: api/TeacherSubjects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacherSubjects(int id)
        {
            if (_context.TeacherSubjects == null)
            {
                return NotFound();
            }
            var teacherSubjects = await _context.TeacherSubjects.FindAsync(id);
            if (teacherSubjects == null)
            {
                return NotFound();
            }

            _context.TeacherSubjects.Remove(teacherSubjects);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeacherSubjectsExists(int id)
        {
            return (_context.TeacherSubjects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
