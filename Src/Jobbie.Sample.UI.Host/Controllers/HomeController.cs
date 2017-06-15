using Microsoft.AspNetCore.Mvc;

namespace Jobbie.Sample.UI.Host.Controllers
{
    public sealed class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Error() => View();
    }
}
