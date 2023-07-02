using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ONLINE_SCHOOL_BACKEND.Data;
using ONLINE_SCHOOL_BACKEND.Models;

namespace ONLINE_SCHOOL_BACKEND.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassMaterialsController : ControllerBase
    {
        private readonly OnlineSchoolDbContext _context;
        private readonly UserManager<OnlineSchoolUser> _userManager;

        public ClassMaterialsController(OnlineSchoolDbContext context,UserManager<OnlineSchoolUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/ClassMaterials
        [HttpGet("allStudyMaterials")]
        public async Task<ActionResult<IEnumerable<ClassMaterials>>> GetAllStudyMaterials()
        {
          if (_context.ClassStudyMaterials == null)
          {
              return NotFound("no materials found!");
          }
            return await _context.ClassStudyMaterials.Include(c => c.ForClass).Include(c => c.PostedBy).Include(c => c.Subject).ToListAsync();
        }


        // GET: api/ClassMaterials
        [HttpGet("classStudyMaterials/{className}")]
        public async Task<ActionResult<IEnumerable<ClassMaterials>>> GetClassStudyMaterials([FromRoute] string className)
        {

            var classMaterials = await _context.ClassStudyMaterials.Include(c => c.ForClass).Include(c => c.PostedBy).Include(c => c.Subject).Where(c => c.ForClass.ClassName == className).ToListAsync();


            Console.WriteLine(classMaterials);
            if (classMaterials == null || classMaterials.Count == 0)
            {
                return NotFound("no materials found!");
            }
            return Ok(new
            {
                classMaterials
            });
        }

        // GET: api/ClassMaterials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClassMaterials>> GetClassMaterials(int id)
        {
          if (_context.ClassStudyMaterials == null)
          {
              return NotFound();
          }
            var classMaterials =  _context.ClassStudyMaterials.Where(c => c.Id == id).Include(c => c.ForClass).Include(c => c.PostedBy).Include(c => c.Subject).FirstOrDefault();

            if (classMaterials == null)
            {
                return NotFound();
            }

            return Ok( new { classMaterials });
        }

        // PUT: api/ClassMaterials/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClassMaterials(int id, ClassMaterialsEditBody body)
        {

           
            var materialToEdit = _context.ClassStudyMaterials.FirstOrDefault(m => m.Id == id);
            if (materialToEdit == null)
            {
                return NotFound("material not found!");
            }
            materialToEdit.MaterialTitle = body.MaterialTitle;
            materialToEdit.MaterialContentUrl = body.MaterialContentUrl;
            materialToEdit.MaterialContentType = body.MaterialContentType;
            materialToEdit.Subject = await _context.subjectsAvailable.Where(s => s.SubjectName == body.Subject).FirstOrDefaultAsync();
            materialToEdit.PostedOn = DateTime.Now;
            materialToEdit.ForClass = await  _context.classesAvailable.Where(s => s.ClassName == body.ForClass).FirstOrDefaultAsync();
            materialToEdit.Description = body.Description;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                updatedMaterial = materialToEdit
            });
        }

        // POST: api/ClassMaterials
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostClassMaterials(ClassStudyMaterialsBody body)
        {
            ClassMaterials classMaterials = new();
           classMaterials.Description = body.Description;
            classMaterials.ForClass = _context.classesAvailable.Where(c => c.ClassName == body.ForClass).FirstOrDefault();
            classMaterials.MaterialTitle = body.MaterialTitle;
            var postedUser = await _userManager.FindByIdAsync(body.PostedBy);
            classMaterials.PostedBy = postedUser;
            classMaterials.Subject = _context.subjectsAvailable.Where(s => s.SubjectName == body.Subject).FirstOrDefault();
            classMaterials.MaterialContentType = body.MaterialContentType;
            classMaterials.MaterialContentUrl = body.MaterialContentUrl;
            classMaterials.PostedOn = body.PostedOn;

            Console.WriteLine(classMaterials);
            
        
            _context.ClassStudyMaterials.Add(classMaterials);
            await _context.SaveChangesAsync();

            return Ok(new { postedMaterial = classMaterials});
        }

        // DELETE: api/ClassMaterials/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassMaterials(int id)
        {
            if (_context.ClassStudyMaterials == null)
            {
                return NotFound();
            }
            var classMaterials = await _context.ClassStudyMaterials.FindAsync(id);
            if (classMaterials == null)
            {
                return NotFound();
            }

            _context.ClassStudyMaterials.Remove(classMaterials);
            await _context.SaveChangesAsync();

            return Ok(new { 
            message = "deleted successfully!"});
        }

        private bool ClassMaterialsExists(int id)
        {
            return (_context.ClassStudyMaterials?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
