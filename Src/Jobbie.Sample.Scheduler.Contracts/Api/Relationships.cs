namespace Jobbie.Sample.Scheduler.Contracts.Api
{
    public static class Relationships
    {
        public const string Error = "error";
        public const string ApiVersion = "apiversion";
        public const string ApiVersion_Query = "apiversion-query";
        public const string Job = "job";
        public const string Job_Query = "job-query";
        public const string Job_QueryBy_Description = "job-queryby-description";
        public const string Job_Create = "job-create";
        public const string Job_Delete = "job-delete";
        public const string Schedule = "schedule";
        public const string Schedule_Query = "schedule-query";
        public const string Schedule_QueryBy_Job = "schedule-queryby-job";
        public const string Schedule_QueryBy_Description = "schedule-queryby-description";
        public const string Schedule_Create = "schedule-create";
        public const string Schedule_Delete = "schedule-delete";
    }
}