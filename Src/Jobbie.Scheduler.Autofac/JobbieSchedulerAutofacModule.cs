using Autofac;
using Jobbie.Scheduler.Commands;
using Jobbie.Scheduler.Queries;

namespace Jobbie.Scheduler.Autofac
{
    public sealed class JobbieSchedulerAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterCommands(builder);
            RegisterQueries(builder);
        }

        private static void RegisterCommands(ContainerBuilder builder)
        {
            builder
                .RegisterType<JobCreator>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<JobDeleter>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<JobScheduler>()
                .AsImplementedInterfaces();
        }

        private static void RegisterQueries(ContainerBuilder builder)
        {
            builder
                .RegisterType<ScheduleConverter>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<JobReader>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<ScheduleReader>()
                .AsImplementedInterfaces();
        }
    }
}