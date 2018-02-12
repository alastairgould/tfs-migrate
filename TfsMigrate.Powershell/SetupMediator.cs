using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Features.Variance;
using MediatR;
using TfsMigrate.Core.UseCases.ConvertTfsToGit;
using TfsMigrate.Core.UseCases.ConvertTfsToGit.Events;

namespace TfsMigrate.Powershell
{
    public static class SetupMediator
    {
        public static IMediator CreateMediator(INotificationHandler<ProgressNotification> progressNotificationHandler)
        {
            var builder = new ContainerBuilder();

            builder
              .RegisterSource(new ContravariantRegistrationSource());

            builder
              .RegisterType<Mediator>()
              .As<IMediator>()
              .InstancePerLifetimeScope();

            builder
              .Register<SingleInstanceFactory>(ctx => {
                  var c = ctx.Resolve<IComponentContext>();
                  return t => { object o; return c.TryResolve(t, out o) ? o : null; };
              })
              .InstancePerLifetimeScope();

            builder
              .Register<MultiInstanceFactory>(ctx => {
                  var c = ctx.Resolve<IComponentContext>();
                  return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
              })
              .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(ConvertTfsToGitCommand).GetTypeInfo().Assembly).AsImplementedInterfaces();

            builder.RegisterInstance(progressNotificationHandler).As<INotificationHandler<ProgressNotification>>();

            var container = builder.Build();
            return container.Resolve<IMediator>();
        }
    }
}
