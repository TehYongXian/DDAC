﻿using Amazon;
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
        public BikeListController(mvcflowershoplab1Context context)
        {
            dbname = context;
        }

        //view table record function
        public async Task<IActionResult> Index()
        {
            List<Bike> FlowerLists = await dbname.BikeTable.ToListAsync();
            ViewBag.BucketName = bucketname;
            return View(FlowerLists);
        }

        public IActionResult AddNewBike()
        {
            return View();
        }


        //Function 3: process the add new record action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewBike(Bike bike, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Upload the image to Amazon S3 and store the image key
                if (imageFile != null)
                {
                    await UploadImageToS3(imageFile, bike);
                }

                // Save the flower record to the database
                dbname.BikeTable.Add(bike);
                await dbname.SaveChangesAsync();

                return RedirectToAction("Index", "BikeList");
            }
            return View(bike);
        }



        //funtion 4 delete the item from SQL
        public async Task<IActionResult> deletePage (int ? did)
        {
            if(did == null)
            {
                return NotFound();
            }
            Bike flower = await dbname.BikeTable.FindAsync(did);

            if(flower == null)
            {
                return NotFound();
            }
            dbname.BikeTable.Remove(flower);
            await dbname.SaveChangesAsync();
            return RedirectToAction("Index", "BikeList");
        }

        //function 5 edit the selected result
        public async Task<IActionResult> editPage(int? did)
        {
            if (did == null)
            {
                return NotFound(); 
            }
            Bike flower = await dbname.BikeTable.FindAsync(did);
            if(flower == null)
            {
                return NotFound();
            }
            return View(flower);
        }

        //function 6 update information to SQL 
        public async Task<IActionResult> updatePage(Bike flower)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dbname.BikeTable.Update(flower);
                    await dbname.SaveChangesAsync();
                    return RedirectToAction("Index", "BikeList");
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return View("editPage", flower);
        }
        public async Task UploadImageToS3(IFormFile imageFile, Bike flower)
        {
            try
            {
                List<string> keys = getKeys();
                AmazonS3Client agent = new AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

                if (imageFile.Length <= 0)
                {
                    // Handle the case where the file is empty
                    ModelState.AddModelError(string.Empty, "File is empty, please try again.");
                    return;
                }
                else if (imageFile.Length > 2097152)
                {
                    // Handle the case where the file is too large
                    ModelState.AddModelError(string.Empty, "The file is over the 2MB size limit. Unable to upload.");
                    return;
                }
                else if (imageFile.ContentType.ToLower() != "image/png" && imageFile.ContentType.ToLower() != "image/jpeg")
                {
                    // Handle the case where the file type is not accepted
                    ModelState.AddModelError(string.Empty, "File type is not accepted. Please upload a PNG or JPEG image.");
                    return;
                }

                // Generate a unique key for the image
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
                    
                    flower.ImageKey = key;
                }
                else
                {
                    // Handle the case where the image upload fails
                    ModelState.AddModelError(string.Empty, "Unable to upload the image.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the S3 upload process
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }
        }

    }
}
