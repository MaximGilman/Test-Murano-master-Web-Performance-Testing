using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;

namespace Test_Murano_Denis_Bardakov
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<Test_Murano_Denis_Bardakov.Models.EmployeesContext>(null);

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
