using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONLINE_SCHOOL_BACKEND.Data;
using ONLINE_SCHOOL_BACKEND.Migrations;
using ONLINE_SCHOOL_BACKEND.Models;

namespace ONLINE_SCHOOL_BACKEND.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTablesController : ControllerBase
    {
        private readonly OnlineSchoolDbContext _context;
        private readonly UserManager<OnlineSchoolUser> _userManager;

        public TimeTablesController(OnlineSchoolDbContext context,UserManager<OnlineSchoolUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/TimeTables
        [HttpGet]
        public async Task<ActionResult> GetTimeTable()
        {
          if (_context.TimeTable == null)
          {
              return NotFound();
          }
            try
            {
                var results = await _context.TimeTable.Include(t => t.Class).Include(t => t.HandlingStaff).Include(t => t.HandlingStaff.Teacher).Include(t => t.HandlingStaff.Subject).Include(t => t.Hour).Select(t => new {
                    day = t.Day,
                   
                    Hour = t.Hour.Session,
                    ForClass = t.Class.ClassName,
                    HandledBy = t.HandlingStaff.Teacher.UserName,
                    Subject = t.HandlingStaff.Subject.SubjectName,
                    MeetingURL = t.MeetLink
                    
                }).ToListAsync();
                return Ok(new
                {
                    timeTable = results
                });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




      


        [HttpGet("get_sessions")]
        public async Task<IActionResult> GetUnAllotedSessions(String day,String ClassName)
        {
            try
            {
                var timetableSetHours = _context.TimeTable.Include(T => T.Class).Include(T => T.Hour).Where(T => T.Class.ClassName == ClassName && T.Day == day).Select(T => T.Hour.Session).ToList();

                var sessionsToAssignStaff = _context.ClassHoursTable.Where(c => !timetableSetHours.Contains(c.Session)).ToList();

                return Ok(new
                {
                    className = ClassName,
                    sessionsNotAlloted = sessionsToAssignStaff
                });
            }catch(Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }


        [HttpGet("get_available_teachers")]
        public async Task<IActionResult> GetAvailableTeachers(String session,String day)
        {
            try
            {
                var busyTeachers = _context.TimeTable.Include(T => T.Hour).Include(T => T.HandlingStaff).Include(T => T.HandlingStaff.Teacher).Where(T => T.Hour.Session == session && T.Day == day).Select(T => T.HandlingStaff.Teacher.Email).ToList();
                var availableTeachers = _context.TeacherSubjects.Include(T => T.Teacher).Include(T => T.Subject).Where(T => !busyTeachers.Contains(T.Teacher.Email)).Select(T => new
                {
                    teacher = T.Teacher.Email,
                    subject = T.Subject.SubjectName
                }).ToList();
                return Ok(new
                {
                    busyTeachers = busyTeachers,
                    availableTeachers = availableTeachers
                });
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        // GET: api/TimeTables/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TimeTable>> GetTimeTable(int id)
        {
          if (_context.TimeTable == null)
          {
              return NotFound();
          }
            var timeTable = await _context.TimeTable.FindAsync(id);

            if (timeTable == null)
            {
                return NotFound();
            }

            return timeTable;
        }

        // PUT: api/TimeTables/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeTable(int id, TimeTable timeTable)
        {
            if (id != timeTable.Id)
            {
                return BadRequest();
            }

            _context.Entry(timeTable).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeTableExists(id))
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

        // POST: api/TimeTables
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostTimeTable(TimeTableInputModel timeTableInput)
        {
          if (_context.TimeTable == null)
          {
              return Problem("Entity set 'OnlineSchoolDbContext.TimeTable'  is null.");
          }

            try
            {
                TimeTable timeTable = new();
                timeTable.Day = timeTableInput.Day;
                timeTable.Hour = await _context.ClassHoursTable.FirstOrDefaultAsync(classHour => classHour.Session == timeTableInput.SessionName);
                timeTable.Class = await _context.classesAvailable.FirstOrDefaultAsync(c => c.ClassName == timeTableInput.ClassName);
                var handlingTeacher = await _userManager.FindByEmailAsync(timeTableInput.TeacherMail);
                timeTable.HandlingStaff = await _context.TeacherSubjects.Include(u => u.Subject).FirstOrDefaultAsync(teacher => teacher.Teacher == handlingTeacher);

                timeTable.MeetLink = timeTableInput.MeetLink;

                _context.TimeTable.Add(timeTable);
                await _context.SaveChangesAsync();

                return Ok(new { insertedRow = timeTable });
            }catch(Exception ex) {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/TimeTables/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeTable(int id)
        {
            if (_context.TimeTable == null)
            {
                return NotFound();
            }
            var timeTable = await _context.TimeTable.FindAsync(id);
            if (timeTable == null)
            {
                return NotFound();
            }

            _context.TimeTable.Remove(timeTable);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TimeTableExists(int id)
        {
            return (_context.TimeTable?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
