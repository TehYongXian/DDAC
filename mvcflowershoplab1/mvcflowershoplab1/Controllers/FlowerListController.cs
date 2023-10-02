using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcflowershoplab1.Data;
using mvcflowershoplab1.Models;

namespace mvcflowershoplab1.Controllers
{
    public class FlowerListController : Controller
    {
        //Function 1: How to connect db in single controller
        private readonly mvcflowershoplab1Context dbname;
        public FlowerListController(mvcflowershoplab1Context context)
        {
            dbname = context;
        }

        //view table record function
        public async Task<IActionResult> Index()
        {
            List<Flower> FlowerLists = await dbname.FlowerTable.ToListAsync();
            return View(FlowerLists);
        }

        public IActionResult AddNewFlower()
        {
            return View();
        }


        //Function 3: process the add new record action
        [HttpPost]
        [ValidateAntiForgeryToken]//avoid cross-site attack
        public async Task<IActionResult> AddNewFlower(Flower flower)
        {
            if(ModelState.IsValid) // if the form is valid
            {
                dbname.FlowerTable.Add(flower); //create the item
                await dbname.SaveChangesAsync(); //save item in the db
                return RedirectToAction("Index","FlowerList");
            }
            return View(flower); //if not valid, return back to the previous page with writing content
        }

        //funtion 4 delete the item from SQL
        public async Task<IActionResult> deletePage (int ? did)
        {
            if(did == null)
            {
                return NotFound();
            }
            Flower flower = await dbname.FlowerTable.FindAsync(did);

            if(flower == null)
            {
                return NotFound();
            }
            dbname.FlowerTable.Remove(flower);
            await dbname.SaveChangesAsync();
            return RedirectToAction("Index", "FlowerList");
        }

        //function 5 edit the selected result
        public async Task<IActionResult> editPage(int? did)
        {
            if (did == null)
            {
                return NotFound(); 
            }
            Flower flower = await dbname.FlowerTable.FindAsync(did);
            if(flower == null)
            {
                return NotFound();
            }
            return View(flower);
        }

        //function 6 update information to SQL 
        public async Task<IActionResult> updatePage(Flower flower)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dbname.FlowerTable.Update(flower);
                    await dbname.SaveChangesAsync();
                    return RedirectToAction("Index", "FlowerList");
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return View("editPage", flower);
        }
    }
}
