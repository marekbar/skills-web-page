using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SkillsAndTasksWebPageWcfConsumer
{
    // Uwaga: aby uzyskać instrukcje włączania trybu klasycznego usług IIS6 lub IIS7, 
    // odwiedź stronę http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Nazwa trasy
                "{controller}/{action}/{id}", // Adres URL z parametrami
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Ustawienia domyślne parametrów
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Użyj domyślnie narzędzia LocalDB na potrzeby narzędzia Entity Framework
            Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}