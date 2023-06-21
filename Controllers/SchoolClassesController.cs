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
    public class SchoolClassesController : ControllerBase
    {
        private readonly OnlineSchoolDbContext _context;

        public SchoolClassesController(OnlineSchoolDbContext context)
        {
            _context = context;
        }

        // GET: api/SchoolClasses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SchoolClasses>>> GetclassesAvailable()
        {
          if (_context.classesAvailable == null)
          {
              return NotFound();
          }
            return await _context.classesAvailable.ToListAsync();
        }

        [HttpGet("{className}")]
        public async Task<IActionResult> GetSchoolClass([FromRoute] string className)
        {
            if (_context.classesAvailable == null)
            {
                return NotFound();
            }
            var schoolClass = await _context.classesAvailable.FirstOrDefaultAsync(c => c.ClassName == className);
            if (schoolClass == null)
            {
                return NotFound();
            }


            return Ok(schoolClass);
        }




        // POST: api/SchoolClasses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SchoolClasses>> PostSchoolClasses(SchoolClasses schoolClasses)
        {
          if (_context.classesAvailable == null)
          {
              return Problem("Entity set 'OnlineSchoolDbContext.classesAvailable'  is null.");
          }
            _context.classesAvailable.Add(schoolClasses);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SchoolClassesExists(schoolClasses.ClassName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(schoolClasses);
        }

        // DELETE: api/SchoolClasses/5
        [HttpDelete("{className}")]
        public async Task<IActionResult> DeleteSchoolClasses([FromRoute] string className)
        {
            if (_context.classesAvailable == null)
            {
                return NotFound();
            }
            var schoolClass = await _context.classesAvailable.FirstOrDefaultAsync(c => c.ClassName == className);
            if (schoolClass == null)
            {
                return NotFound();
            }

            _context.classesAvailable.Remove(schoolClass);
            await _context.SaveChangesAsync();

            return Ok("Deleted Successfully!");
        }


        private bool SchoolClassesExists(string className)
        {
            return (_context.classesAvailable?.Any(e => e.ClassName == className)).GetValueOrDefault();
        }
    }
}
