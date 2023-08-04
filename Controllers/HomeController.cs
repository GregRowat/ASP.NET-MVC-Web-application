using Lab4.Data;
using Microsoft.AspNetCore.Mvc;

// Controller Class for the Home controller
namespace Lab4.Controllers
{
    public class HomeController : Controller
    {
        private readonly NewsDbContext _newsDbContext;


        public HomeController (NewsDbContext newsDbContext)
        {
            _newsDbContext = newsDbContext;
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
