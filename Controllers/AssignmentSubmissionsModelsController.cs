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
    public class AssignmentSubmissionsModelsController : ControllerBase
    {
        private readonly OnlineSchoolDbContext _context;

        public AssignmentSubmissionsModelsController(OnlineSchoolDbContext context)
        {
            _context = context;
        }

        // GET: api/AssignmentSubmissionsModels
        [HttpGet]
        public async Task<IActionResult> GetAllSubmittedAssignments()
        {

            var allAssignmentsSubmitted = _context.AssignmentSubmissions.Include(a => a.Assignment).Select(a =>
            new {
                a.Id,
                assignmentTitle = a.Assignment.Title,
                assignmentId = a.Assignment.Id,
                
                a.SubmissionDateTime,
                a.StudentUserName,
                a.Status,
                a.StudentSubmissionFileURL,
                
  
            }).ToList();
          if (allAssignmentsSubmitted == null)
          {
              return NotFound("no assignment submissions made");
          }
            return Ok(new { allAssignmentsSubmitted });
        }

        // GET: api/AssignmentSubmissionsModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssignmentSubmissionsModel>> GetAssignmentSubmissionsModel(int id)
        {
          if (_context.AssignmentSubmissions == null)
          {
              return NotFound();
          }
            var assignmentSubmissionsModel = await _context.AssignmentSubmissions.Include(a => a.Assignment).Where(a => a.Id == id).FirstOrDefaultAsync();

            if (assignmentSubmissionsModel == null)
            {
                return NotFound();
            }

            return assignmentSubmissionsModel;
        }


        [HttpGet("getAssigmentSubmissions{assignmentId}")]
        public async Task<ActionResult<AssignmentSubmissionsModel>> GetSubmissionsForAssignment([FromRoute]int assignmentId)
        {
            if (_context.AssignmentSubmissions == null)
            {
                return NotFound("no submissions made!");
            }
            var submissionsForAssignment = _context.AssignmentSubmissions.Include(a => a.Assignment).Where(a => a.Assignment.Id == assignmentId).Select(a =>
            new {
                submissionId = a.Id,

                assignmentTitle = a.Assignment.Title,
                a.SubmissionDateTime,
                submittedBy = a.StudentUserName,
                a.Status,
                a.StudentSubmissionFileURL,


            }).ToList();

            if (submissionsForAssignment == null)
            {
                return NotFound(new { message = "no submissions were made yet!" });
            }

            return Ok(new
            {
                submissionsForAssignment
            });
        }

        // PUT: api/AssignmentSubmissionsModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignmentSubmissionsModel(int id, AssignmentSubmissionsModel assignmentSubmissionsModel)
        {
            if (id != assignmentSubmissionsModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(assignmentSubmissionsModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentSubmissionsModelExists(id))
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

        // POST: api/AssignmentSubmissionsModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AssignmentSubmissionsModel>> PostAssignmentSubmissionsModel(AssignmentSubmissionBody body)
        {

            AssignmentSubmissionsModel assignmentSubmission = new AssignmentSubmissionsModel();

            assignmentSubmission.StudentSubmissionFileURL = body.StudentSubmissionFileURL;
            assignmentSubmission.SubmissionDateTime = body.SubmissionDateTime;
            assignmentSubmission.StudentUserName = body.StudentUserName;
            assignmentSubmission.Status = body.Status;
            assignmentSubmission.Assignment = _context.Assignments.Where(a => a.Id == body.AssignmentId).FirstOrDefault();

            _context.AssignmentSubmissions.Add(assignmentSubmission);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                assignmentSubmission
            });
        }

        // DELETE: api/AssignmentSubmissionsModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignmentSubmissionsModel(int id)
        {
            if (_context.AssignmentSubmissions == null)
            {
                return NotFound($"no submissions found!");
            }
            var assignmentSubmissionsModel = await _context.AssignmentSubmissions.FindAsync(id);
            if (assignmentSubmissionsModel == null)
            {
                return NotFound($"assignment submission with {id} not found");
            }

            _context.AssignmentSubmissions.Remove(assignmentSubmissionsModel);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"assignment submission of {id} deleted successfully!" });
        }

        private bool AssignmentSubmissionsModelExists(int id)
        {
            return (_context.AssignmentSubmissions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
