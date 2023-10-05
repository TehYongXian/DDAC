using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcflowershoplab1.Data;
using mvcflowershoplab1.Models;
using System.Diagnostics;

namespace mvcflowershoplab1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, mvcflowershoplab1Context context)
        {
            _logger = logger;
            dbname = context;
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //Function 1: How to connect db in single controller
        private readonly mvcflowershoplab1Context dbname;
        private const string bucketname = "bicyclerental";
        private List<string> getKeys()
        {
            List<string> keys = new List<string>();

            //connect to the appsattings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfiguration conf = builder.Build();

            keys.Add(conf["Keys:Key1"]);
            keys.Add(conf["Keys:Key2"]);
            keys.Add(conf["Keys:Key3"]);

            return keys;
        }

        //view table record function
        public async Task<IActionResult> Index()
        {
            List<Bike> BikeLists = await dbname.BikeTable.ToListAsync();
            ViewBag.BucketName = bucketname;
            return View(BikeLists);
        }
    }
}
