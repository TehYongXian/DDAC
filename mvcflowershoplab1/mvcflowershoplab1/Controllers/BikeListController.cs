using Amazon;
using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcflowershoplab1.Data;
using mvcflowershoplab1.Models;

namespace mvcflowershoplab1.Controllers
{
    public class BikeListController : Controller
    {
        private readonly mvcbicyclerentalContext dbname;
        private const string bucketname = "bicyclerental";
        private List<string> getKeys()
        {
            List<string> keys = new List<string>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfiguration conf = builder.Build();

            keys.Add(conf["Keys:Key1"]);
            keys.Add(conf["Keys:Key2"]);
            keys.Add(conf["Keys:Key3"]);

            return keys;
        }
        public BikeListController(mvcbicyclerentalContext context)
        {
            dbname = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Bike> BikeLists = await dbname.BikeTable.ToListAsync();
            ViewBag.BucketName = bucketname;
            return View(BikeLists);
        }

        public IActionResult AddNewBike()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewBike(Bike bicycle, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    await UploadImageToS3(imageFile, bicycle);
                }

 
                dbname.BikeTable.Add(bicycle);
                await dbname.SaveChangesAsync();
                return RedirectToAction("Index", "BikeList");
            }
            return View(bicycle);
        }

        public async Task<IActionResult> deletePage (int ? did)
        {
            if(did == null)
            {
                return NotFound();
            }
            Bike bicycle = await dbname.BikeTable.FindAsync(did);

            if(bicycle == null)
            {
                return NotFound();
            }
            dbname.BikeTable.Remove(bicycle);
            await dbname.SaveChangesAsync();
            return RedirectToAction("Index", "BikeList");
        }

        public async Task<IActionResult> editPage(int? did)
        {
            if (did == null)
            {
                return NotFound(); 
            }
            Bike bicycle = await dbname.BikeTable.FindAsync(did);
            if(bicycle == null)
            {
                return NotFound();
            }
            return View(bicycle);
        }

        public async Task<IActionResult> updatePage(Bike bicycle)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dbname.BikeTable.Update(bicycle);
                    await dbname.SaveChangesAsync();
                    return RedirectToAction("Index", "BikeList");
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return View("editPage", bicycle);
        }
        public async Task UploadImageToS3(IFormFile imageFile, Bike bicycle)
        {
            try
            {
                List<string> keys = getKeys();
                AmazonS3Client agent = new AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

                if (imageFile.Length <= 0)
                {
                    ModelState.AddModelError(string.Empty, "File is empty, please try again.");
                    return;
                }
                else if (imageFile.Length > 2097152)
                {
                    ModelState.AddModelError(string.Empty, "The file is over the 2MB size limit. Unable to upload.");
                    return;
                }
                else if (imageFile.ContentType.ToLower() != "image/png" && imageFile.ContentType.ToLower() != "image/jpeg")
                {
                    ModelState.AddModelError(string.Empty, "File type is not accepted. Please upload a PNG or JPEG image.");
                    return;
                }

                var key = "images/" + Guid.NewGuid() + "_" + imageFile.FileName;

                using var stream = imageFile.OpenReadStream();

                var request = new PutObjectRequest
                {
                    BucketName = bucketname,
                    Key = key,
                    InputStream = stream,
                    ContentType = imageFile.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                var response = await agent.PutObjectAsync(request);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {

                    bicycle.ImageKey = key;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to upload the image.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }
        }

    }
}
