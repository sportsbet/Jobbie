using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using Autofac;
using Autofac.Extras.Quartz;
using Autofac.Integration.WebApi;
using Jobbie.Executor.Commands;
using Jobbie.Infrastructure.Autofac;
using Jobbie.Scheduler.Autofac;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.IoC
{
    internal static class IoCBootstrapper
    {
        private static readonly Assembly _thisAssembly = Assembly.GetExecutingAssembly();
        private static readonly Assembly _jobAssembly = Assembly.GetAssembly(typeof(JobExecutor));
        public static IContainer Container;

        public static IContainer Init()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(_thisAssembly);

            builder.RegisterBootstrappers(_thisAssembly);

            builder.RegisterAssemblyModules(_thisAssembly);

            builder.RegisterReferenceModules();

            builder.RegisterQuartzModules();

            Container = builder.Build();

            Container.InitBootstrappers();

            return Container;
        }

        private static void RegisterReferenceModules(this ContainerBuilder container)
        {
            container
                .RegisterModule<JobbieSchedulerAutofacModule>()
                .RegisterModule<JobbieInfrastructureAutofacModule>();
        }

        private static void RegisterQuartzModules(this ContainerBuilder container)
        {
            var quartz =
                new QuartzAutofacFactoryModule
                {
                    ConfigurationProvider = c =>
                    {
                        var connectionString = ConfigurationManager.ConnectionStrings["Jobbie"].ConnectionString;
                        return new NameValueCollection
                        {
                            {"quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz"},
                            {"quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz"},
                            {"quartz.jobStore.tablePrefix", "QRTZ_"},
                            {"quartz.jobStore.dataSource", "quartzDb"},
                            {"quartz.dataSource.quartzDb.connectionString", connectionString},
                            {"quartz.dataSource.quartzDb.provider", "SqlServer-20"},
                            {"quartz.scheduler.instanceName", "Jobbie"},
                            {"quartz.scheduler.instanceId", "AUTO"},
                            {"quartz.jobStore.clustered", "true"}
                        };
                    }
                };
            container.RegisterModule(quartz);
            container.RegisterModule(new QuartzAutofacJobsModule(_jobAssembly));
        }
    }
}