using Hangfire;
using Microsoft.AspNetCore.Mvc;
using ONLINE_SCHOOL_BACKEND.Services;

namespace ONLINE_SCHOOL_BACKEND.Controllers
{

    
    public class BackgroundTaskController : Controller
    {

        private readonly IJobMailService _jobMailService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        public BackgroundTaskController(IJobMailService jobMailService, IBackgroundJobClient backgroundJobClient,IRecurringJobManager recurringJobManager)
        {
            _jobMailService = jobMailService;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }
        [HttpGet("/FireAndForgetJob")]
        public IActionResult CreateFireAndForget()
        {
            _backgroundJobClient.Enqueue(() => _jobMailService.FireAndForgetJob());
            return Ok();
        }

        [HttpGet("/RecurringJob")]
        public IActionResult CreateRecurringJob()
        {
            _recurringJobManager.AddOrUpdate<TimeTablesController>("Timetable for the Day", x => x.MailTimeTable(), Cron.Daily,TimeZoneInfo.Local);
            return Ok();
        }
    }
}
