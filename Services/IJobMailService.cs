namespace ONLINE_SCHOOL_BACKEND.Services
{
    public interface IJobMailService
    {
        void FireAndForgetJob();

        void ReccuringJob();

        void DelayedJob();

        void ContinuationJob();
    }
}
