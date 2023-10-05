using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcflowershoplab1.Data;
using mvcflowershoplab1.Models;
using System;

namespace mvcflowershoplab1.Controllers
{
    public class mainPageController : Controller
    {
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
        public mainPageController(mvcflowershoplab1Context context)
        {
            dbname = context;
        }

        //view table record function
        public async Task<IActionResult> Index()
        {
            List<Bike> FlowerLists = await dbname.FlowerTable.ToListAsync();
            ViewBag.BucketName = bucketname;
            return View(FlowerLists);
        }
    }
}
