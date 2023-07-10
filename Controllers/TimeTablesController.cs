using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
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
        private readonly IConfiguration _configuration;


        public TimeTablesController(OnlineSchoolDbContext context,UserManager<OnlineSchoolUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
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
                    id = t.Id,
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

        [NonAction]
        private void SendMail(String emailSubject, String emailMessage, String toEmail)
        {
            toEmail = "testname1234554321@gmail.com";//for testing purposes
            try
            {

                string fromPassword = _configuration["fromEmailPass"];
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("testname1234554321@gmail.com"));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = emailSubject;

                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = emailMessage,

                };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                smtp.Authenticate("testname1234554321@gmail.com", fromPassword);

                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //[NonAction]
        [HttpGet("mailrecurring")]
        public async Task<IActionResult> MailTimeTable()
        {

            String emailSubject = "Your Schedule for the Day";
            var users = _context.Users.ToList();
            
            StringBuilder emailMessage = new StringBuilder();
            

            string dayOfWeek = DateTime.Today.ToString("dddd");
            string capitalizedDay = char.ToUpper(dayOfWeek[0]) + dayOfWeek.Substring(1);
            Console.WriteLine(capitalizedDay);


            var results = await _context.TimeTable.Include(t => t.Class).Include(t => t.HandlingStaff).Include(t => t.HandlingStaff.Teacher).Include(t => t.HandlingStaff.Subject).Include(t => t.Hour).Where(t => t.Day == capitalizedDay).Select(t => new {
                id = t.Id,
                day = t.Day,

                Hour = t.Hour.Timing,
                ForClass = t.Class.ClassName,
                HandledBy = t.HandlingStaff.Teacher.Id,
                handledByUserName = t.HandlingStaff.Teacher.UserName,
                Subject = t.HandlingStaff.Subject.SubjectName,
                MeetingURL = t.MeetLink

            }).ToListAsync();

            foreach (var user in users)
            {
                
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles[0];

                if(userRole == "Teacher")
                {
                    var res = results.Where(r => r.HandledBy == user.Id).ToList();
                    //Console.WriteLine(user.Id + " : Teacher");
                    Console.WriteLine(res.Count);
                    if (res.Count > 0)
                    {
                        emailMessage.AppendLine("<table>");
                        emailMessage.AppendLine("<tr><th>Hour</th><th>Class</th><th>Handled By</th><th>Subject</th><th>Meeting CODE</th></tr>");
                        foreach (var item in res)
                        {
                            emailMessage.AppendLine("<tr>");
                            emailMessage.AppendLine($"<td>{item.Hour}</td>");
                            emailMessage.AppendLine($"<td>{item.ForClass}</td>");
                            emailMessage.AppendLine($"<td>{"you"}</td>");
                            emailMessage.AppendLine($"<td>{item.Subject}</td>");
                            emailMessage.AppendLine($"<td>{item.MeetingURL}</td>");
                            emailMessage.AppendLine("</tr>");
                        }
                        
                    }
                    emailMessage.AppendLine("</table>");
                    Console.WriteLine(emailMessage);
                    //send the table as mail and set the emailMessage back to empty
                    SendMail(emailSubject, emailMessage.ToString(), user.Email);
                    emailMessage.Clear();


                }
                else if(userRole == "Student")
                {
                    var belongingClass = _context.StudentClasses.Include(s => s.Class).Include(s => s.Student).Where(s => s.Student.Id == user.Id).FirstOrDefault();
                    if(belongingClass != null)
                    {
                        Console.WriteLine(belongingClass.Class.ClassName);
                        var res = results.Where(r => r.ForClass == belongingClass.Class.ClassName).ToList();
                        Console.WriteLine(res.Count);
                        if (res.Count > 0)
                        {
                            emailMessage.AppendLine("<table>");
                            emailMessage.AppendLine("<tr><th>Hour</th><th>Class</th><th>Handled By</th><th>Subject</th><th>Meeting CODE</th></tr>");
                            foreach (var item in res)
                            {
                                emailMessage.AppendLine("<tr>");
                                emailMessage.AppendLine($"<td>{item.Hour}</td>");
                                emailMessage.AppendLine($"<td>{item.ForClass}</td>");
                                emailMessage.AppendLine($"<td>{item.handledByUserName}</td>");
                                emailMessage.AppendLine($"<td>{item.Subject}</td>");
                                emailMessage.AppendLine($"<td>{item.MeetingURL}</td>");
                                emailMessage.AppendLine("</tr>");
                            }
                        }
                        emailMessage.AppendLine("</table>");
                        Console.WriteLine(emailMessage);

                        //send the table as mail and set the emailMessage back to empty
                        SendMail(emailSubject, emailMessage.ToString(), user.Email);

                        emailMessage.Clear();

                    }


                }
            }
           

            return Ok();
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


        [HttpGet("getSessionDetails")]
        public async Task<IActionResult> GetSessionDetails(String day,String className,String session)
        {
            try
            {
                var sessionDetails = _context.TimeTable.Include(T => T.HandlingStaff).Include(T => T.HandlingStaff.Teacher).Include(T => T.HandlingStaff.Subject).Where(T => T.Day == day && T.Class.ClassName == className && T.Hour.Session == session).Select(T => new
                {
                    teacher = T.HandlingStaff.Teacher.Email,
                    subject = T.HandlingStaff.Subject.SubjectName,
                    meetLink = T.MeetLink
                }).SingleOrDefault();
                if(sessionDetails == null)
                {
                    return Ok(new
                    {
                        sessionDetails = "null"
                    });
                }
                return Ok(new
                {
                    sessionDetails = sessionDetails
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }


            return Ok();
        }



        [HttpGet("getAllClasses")]

        public async Task<IActionResult> GetAllClasses()
        {
            try
            {
                var classes = _context.classesAvailable.Select(c => c.ClassName).ToList();

                return Ok(new
                {
                    classes = classes
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("getAllSessions")]

        public async Task<IActionResult> GetAllSessions()
        {
            try
            {
                var sessions = _context.ClassHoursTable.ToList();

                return Ok(new
                {
                    sessions = sessions
                });
            }
            catch (Exception ex)
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

            return Ok("Deleted successfully!");
        }


        [HttpDelete("deleteAll")]
        public async Task<IActionResult> ClearTimeTable()
        {
            if(_context.TimeTable == null)
            {
                return Ok(new { message = "Time table is empty already!" });
            } 
            await _context.TimeTable.ExecuteDeleteAsync();
            _context.SaveChanges();

            return Ok(new { message = "Time table cleared!"});

        }

 

       
        
        private bool TimeTableExists(int id)
        {
            return (_context.TimeTable?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
