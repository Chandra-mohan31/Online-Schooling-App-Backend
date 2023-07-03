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
    public class AssignmentModelsController : ControllerBase
    {
        private readonly OnlineSchoolDbContext _context;

        public AssignmentModelsController(OnlineSchoolDbContext context)
        {
            _context = context;
        }

        // GET: api/AssignmentModels
        [HttpGet]
        public async Task<IActionResult> GetAssignments()
        {
          if (_context.Assignments == null)
          {
              return NotFound("no assignments posted!");
          }
            var assignments  =  await _context.Assignments.Include(a => a.ForClass).Include(a => a.PostedBy).Include(a => a.Subject).Select(a => new
            {
                a.Id,
                a.ForClass.ClassName,
                a.PostedBy.Email,
                a.Subject.SubjectName,
                a.Title,
                a.Description,
                a.DueDateTime
            }).ToListAsync();

            return Ok(new { 
            assignments
            });

        }

        // GET: api/AssignmentModels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssignmentModel(int id)
        {
          if (_context.Assignments == null)
          {
              return NotFound();
          }
            var assignment = await _context.Assignments.Include(a => a.ForClass).Include(a => a.PostedBy).Include(a => a.Subject).Where(a => a.Id == id).Select(a => new
            {
                a.Id,
                a.ForClass.ClassName,
                a.PostedBy.Email,
                a.Subject.SubjectName,
                a.Title,
                a.Description,
                a.DueDateTime
            }).FirstOrDefaultAsync();

            if (assignment == null)
            {
                return NotFound("no assignment found for the given id!");
            }

            return Ok(new {
            assignment
            });
        }



        [HttpGet("/postedAssignments/{teacherId}")]
        public async Task<IActionResult> GetPostedAssignments([FromRoute]string teacherId)
        {
            if (_context.Assignments == null)
            {
                return NotFound();
            }
            var assignmentsPosted = _context.Assignments.Include(a => a.ForClass).Include(a => a.PostedBy).Include(a => a.Subject).Where(a => a.PostedBy.Id == teacherId).Select(a => new
            {
                a.Id,
                a.ForClass.ClassName,
                a.PostedBy.Email,
                a.Subject.SubjectName,
                a.Title,
                a.Description,
                a.DueDateTime
            }).ToList();

            if (assignmentsPosted == null)
            {
                return NotFound("No posted Assignments!");
            }

            return Ok(new
            {
                assignmentsPosted
            });
        }


        [HttpGet("/classAssignments/{className}")]
        public async Task<IActionResult> GetClassAssignments([FromRoute]string className)
        {
            if (_context.Assignments == null)
            {
                return NotFound("No given assignments!");
            }
            var classAssignments = _context.Assignments.Include(a => a.ForClass).Include(a => a.PostedBy).Include(a => a.Subject).Where(a => a.ForClass.ClassName == className).Select(a => new
            {
                a.Id,
                a.ForClass.ClassName,
                a.PostedBy.Email,
                a.Subject.SubjectName,
                a.Title,
                a.Description,
                a.DueDateTime
            }).ToList();

            if (classAssignments == null)
            {
                return NotFound("no given assignments!");
            }

            return Ok(new
            {
                classAssignments
            });
        }




        // PUT: api/AssignmentModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignmentModel(int id, PostAssignmentBody assignment)
        {
            AssignmentModel assignmentToEdit = _context.Assignments.Where(a => a.Id == id).FirstOrDefault();
            Console.WriteLine(assignmentToEdit.Title);

            assignmentToEdit.Title = assignment.Title;
            assignmentToEdit.Subject = _context.subjectsAvailable.Where(s => s.SubjectName == assignment.SubjectName).FirstOrDefault();
            assignmentToEdit.ForClass = _context.classesAvailable.Where(c => c.ClassName == assignment.ForClass).FirstOrDefault();
            assignmentToEdit.Description = assignment.Description;
            assignmentToEdit.DueDateTime = assignment.DueDateTime;

            Console.WriteLine(assignmentToEdit.ForClass.ClassName);
            Console.WriteLine(assignment.ForClass);



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok("failed to update!");
            }

            return Ok(new
            {
                assignmentToEdit
            });
        }

        // POST: api/AssignmentModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostAssignmentModel(PostAssignmentBody assignmentBody)
        {
          

            AssignmentModel assignmentModel = new();
            assignmentModel.PostedBy = await _context.Users.Where(u => u.Email == assignmentBody.PostedByEmail).FirstOrDefaultAsync();
            assignmentModel.Subject = _context.subjectsAvailable.Where(s => s.SubjectName == assignmentBody.SubjectName).FirstOrDefault();
            assignmentModel.ForClass = _context.classesAvailable.Where(c => c.ClassName == assignmentBody.ForClass).FirstOrDefault();
            assignmentModel.Title = assignmentBody.Title;
            assignmentModel.Description = assignmentBody.Description;
            assignmentModel.DueDateTime = assignmentBody.DueDateTime;
            _context.Assignments.Add(assignmentModel);
            await _context.SaveChangesAsync();

            return Ok(new { assignmentModel,message="posted successfully!" });
        }

        // DELETE: api/AssignmentModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignmentModel(int id)
        {
            if (_context.Assignments == null)
            {
                return NotFound();
            }
            var assignmentModel = await _context.Assignments.FindAsync(id);
            if (assignmentModel == null)
            {
                return NotFound("assignment with given id is not found!");
            }

            _context.Assignments.Remove(assignmentModel);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "deleted successfully!"
            });
        }

        private bool AssignmentModelExists(int id)
        {
            return (_context.Assignments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
