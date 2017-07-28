using DataAccess.Manager;
using DataModel.DataContext;
using DataModel.Repository;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Api.Service.User;
using AutoMapper;
using Unity.WebApi;

namespace Api
{
    public static class UnityConfig
    {
        private static UnityContainer container = new UnityContainer();

        public static UnityContainer GetConfiguredContainer()
        {
            return container;
        }

        public static void RegisterComponents()
        {
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<SocialTrainingContext>();

            container.RegisterType<UserService>();
            container.RegisterType<UserManager>();
            container.RegisterType<UserRepository>();

            var config = new AutoMapperConfig().Configure();
            config.AssertConfigurationIsValid();
            
            container.RegisterInstance<MapperConfiguration>(config);
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}