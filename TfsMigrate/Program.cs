using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Features.Variance;
using MediatR;
using TfsMigrate.Core.UseCases.ConvertTfsToGit;

namespace TfsMigrate
{
    public class Program
    {
        static void Main(string[] args)
        {
            var mediatr = SetupMediator();

            mediatr.Send(new ConvertTfsToGitCommand(new System.Uri(""),
                "",
                @""));
        }

        private static IMediator SetupMediator()
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

            var container = builder.Build();
            return container.Resolve<IMediator>();
        }
    }
}
