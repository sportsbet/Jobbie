using Jobbie.Sample.Scheduler.Contracts.Api;
using Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning;
using WebApi.Hal;
using Hal = WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Hypermedia
{
    internal sealed class HypermediaConfiguration : IHypermediaConfiguration
    {
        private readonly ILatestApiVersion _version;
        private readonly IHypermediaAppender<ApiError> _errorAppender;
        private readonly IHypermediaAppender<ApiVersion> _versionAppender;
        private readonly IHypermediaAppender<PagedList<ApiVersion>> _versionPagedAppender;
        private readonly IHypermediaAppender<Job> _jobAppender;
        private readonly IHypermediaAppender<PagedList<Job>> _jobPagedAppender;
        private readonly IHypermediaAppender<JobByDescriptionPagedList> _jobByDescriptionPagedAppender;
        private readonly IHypermediaAppender<Schedule> _scheduleAppender;
        private readonly IHypermediaAppender<PagedList<Schedule>> _schedulePagedAppender;
        private readonly IHypermediaAppender<ScheduleByJobPagedList> _scheduleByJobPagedAppender;
        private readonly IHypermediaAppender<ScheduleByDescriptionPagedList> _scheduleByDescriptionPagedAppender;
        private readonly IHypermediaBuilder _builder;
        private readonly CuriesLink _curie;

        public IHypermediaResolver Resolver => _builder.Build();

        public HypermediaConfiguration(
            ILatestApiVersion version,
            IHypermediaAppender<ApiError> errorAppender,
            IHypermediaAppender<ApiVersion> versionAppender,
            IHypermediaAppender<PagedList<ApiVersion>> versionPagedAppender,
            IHypermediaAppender<Job> jobAppender,
            IHypermediaAppender<PagedList<Job>> jobPagedAppender,
            IHypermediaAppender<JobByDescriptionPagedList> jobByDescriptionPagedAppender,
            IHypermediaAppender<Schedule> scheduleAppender,
            IHypermediaAppender<PagedList<Schedule>> schedulePagedAppender,
            IHypermediaAppender<ScheduleByJobPagedList> scheduleByJobPagedAppender,
            IHypermediaAppender<ScheduleByDescriptionPagedList> scheduleByDescriptionPagedAppender)
        {
            _version = version;
            _errorAppender = errorAppender;
            _versionAppender = versionAppender;
            _versionPagedAppender = versionPagedAppender;
            _jobAppender = jobAppender;
            _jobPagedAppender = jobPagedAppender;
            _jobByDescriptionPagedAppender = jobByDescriptionPagedAppender;
            _scheduleAppender = scheduleAppender;
            _schedulePagedAppender = schedulePagedAppender;
            _scheduleByJobPagedAppender = scheduleByJobPagedAppender;
            _scheduleByDescriptionPagedAppender = scheduleByDescriptionPagedAppender;

            _curie = new CuriesLink(Curies.Jobbie, $"https://jobbie-api.com/v{_version}/docs/{{rel}}");

            _builder = Hal.Hypermedia.CreateBuilder();

            RegisterApiError();
            RegisterVersion();
            RegisterJob();
            RegisterSchedule();
        }

        private void RegisterApiError()
        {
            _builder
                .Register(
                    _curie.CreateLink<ApiError>(
                        Relationships.Error,
                        $"~/v{_version}/error"),
                    _errorAppender);
        }

        private void RegisterVersion()
        {
            _builder
                .Register(
                    _curie.CreateLink<ApiVersion>(
                        Relationships.ApiVersion,
                        $"~/v{_version}/apiversion/{{versionNumber}}"),
                    _versionAppender);

            _builder
                .Register(
                    _curie.CreateLink<PagedList<ApiVersion>>(
                        Relationships.ApiVersion_Query,
                        $"~/v{_version}/apiversion?pageIndex={{pageIndex}}&pageSize={{pageSize}}&sortDirection={{sortDirection}}&sortProperty={{sortProperty}}"),
                    _versionPagedAppender);
        }

        private void RegisterJob()
        {
            _builder
                .Register(
                    _curie.CreateLink<Job>(
                        Relationships.Job,
                        $"~/v{_version}/job/{{jobId}}"),
                    _jobAppender);

            _builder
                .Register(
                    _curie.CreateLink<PagedList<Job>>(
                        Relationships.Job_Query,
                        $"~/v{_version}/job?pageIndex={{pageIndex}}&pageSize={{pageSize}}&sortDirection={{sortDirection}}&sortProperty={{sortProperty}}"),
                    _jobPagedAppender);

            _builder
                .Register(
                    _curie.CreateLink<JobByDescriptionPagedList>(
                        Relationships.Job_QueryBy_Description,
                        $"~/v{_version}/job?description={{description}}&pageIndex={{pageIndex}}&pageSize={{pageSize}}&sortDirection={{sortDirection}}&sortProperty={{sortProperty}}"),
                    _jobByDescriptionPagedAppender);
        }

        private void RegisterSchedule()
        {
            _builder
                .Register(
                    _curie.CreateLink<Schedule>(
                        Relationships.Schedule,
                        $"~/v{_version}/schedule/{{scheduleId}}"),
                    _scheduleAppender);

            _builder
                .Register(
                    _curie.CreateLink<PagedList<Schedule>>(
                        Relationships.Schedule_Query,
                        $"~/v{_version}/schedule?pageIndex={{pageIndex}}&pageSize={{pageSize}}&sortDirection={{sortDirection}}&sortProperty={{sortProperty}}"),
                    _schedulePagedAppender);

            _builder
                .Register(
                    _curie.CreateLink<ScheduleByJobPagedList>(
                        Relationships.Schedule_QueryBy_Job,
                        $"~/v{_version}/schedule?jobId={{jobId}}&pageIndex={{pageIndex}}&pageSize={{pageSize}}&sortDirection={{sortDirection}}&sortProperty={{sortProperty}}"),
                    _scheduleByJobPagedAppender);

            _builder
                .Register(
                    _curie.CreateLink<ScheduleByDescriptionPagedList>(
                        Relationships.Schedule_QueryBy_Description,
                        $"~/v{_version}/schedule?description={{description}}&pageIndex={{pageIndex}}&pageSize={{pageSize}}&sortDirection={{sortDirection}}&sortProperty={{sortProperty}}"),
                    _scheduleByDescriptionPagedAppender);
        }
    }
}