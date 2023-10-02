using Microsoft.AspNetCore.Mvc;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using System.Net.Mime;

namespace mvcflowershoplab1.Controllers
{
    public class S3ExampleController : Controller
    {
        //define bucket name for the code to use
        private const string bucketname = "mvcflowershoplab1";

        //collect back the keys from the appsettings.json
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

        //function 2: modify index function to become upload image page
        public IActionResult Index()
        {
            return View();
        }

        //function 3: upload multiple images to the s3 bucket
        public async Task<IActionResult> ProcessUploadImage(List<IFormFile> imagefile)
        {
            //3.1 connect to the AWS account first
            List<string> keys = getKeys();
            AmazonS3Client agent = new AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            //3.2 start validate each of the image content
            foreach(var singlefile in imagefile)
            {
                if(singlefile.Length <= 0)
                {
                    return BadRequest("File of " + singlefile.FileName + "is no content, please try again");
                }
                else if (singlefile.Length > 2097152)
                {
                    return BadRequest("it is over 2MB limit of size. Unable to upload");
                }
                else if(singlefile.ContentType.ToLower() != "image/png" && singlefile.ContentType.ToLower() != "/image.jpeg")
                {
                    return BadRequest("file of " + singlefile.FileName + " is not an accepting file type, please try again");
                }
                //3.3 start validate each of the image content
                try
                {
                    //create a request to add item to the bucket
                    PutObjectRequest request = new PutObjectRequest
                    {
                        BucketName = bucketname,
                        Key = "images/" + singlefile.FileName, //store the image in a same folder under same bucket
                        InputStream = singlefile.OpenReadStream(),
                        CannedACL = S3CannedACL.PublicRead // ensure everyone can view the object from the web browser
                    };

                    //execute the request by asking the agent to do that
                    await agent.PutObjectAsync(request);
                }
                catch (AmazonS3Exception ex)
                {
                    return BadRequest("file of " + singlefile.FileName + "unable to upload. Error: " + ex.Message);
                }
            }

            //3.4 return back to the display image gallery page to view the new images
            return RedirectToAction("Index", "S3Example");
        }

        //function4: view image from s3 bucket
        public async Task<IActionResult>DisplayImage()
        {
            List<string> keys = getKeys();
            AmazonS3Client agent = new
                AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            string token = null;
            List<S3Object> objectlist = new List<S3Object>(); 
            
            try
            {
                do
                {
                    ListObjectsRequest request = new ListObjectsRequest
                    {
                        BucketName = bucketname
                    };
                    ListObjectsResponse response = await agent.ListObjectsAsync(request);
                    token = response.NextMarker;
                    objectlist.AddRange(response.S3Objects);

                } while(token != null);
            }
            catch(AmazonS3Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return View(objectlist);
        }

        //function 5:delete single image from s3 bucket
        public async Task<IActionResult> DeletePage(string imagekey, string imagebucket)
        {
            List<string> keys = getKeys();
            AmazonS3Client agent = new
                AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = imagebucket,
                    Key = imagekey
                };
                await agent.DeleteObjectAsync(request);
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return RedirectToAction("DisplayImage", "S3Example");
        }

        //function 6: download image from s3 bucket
        public async Task<IActionResult> DownloadPage(string imagekey, string imagebucket)
        {
            List<string> keys = getKeys();
            AmazonS3Client agent = new
                AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            Stream downloadstreamitem;

            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = imagebucket,
                    Key = imagekey
                };
                GetObjectResponse response = await agent.GetObjectAsync(request);
                using (var responseStream = response.ResponseStream)
                {
                    downloadstreamitem = new MemoryStream();
                    await responseStream.CopyToAsync(downloadstreamitem);
                    downloadstreamitem.Position = 0;
                }
                string imagefile = Path.GetFileName(imagekey);

                Response.Headers.Add("Content-Disposition", new ContentDisposition //convert from temp to permanent
                {
                    FileName = imagefile,
                    Inline = false // false = prompt the user for downloading; true = browser to try to show the file inline 
                }.ToString());
            }
            catch(AmazonS3Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return File(downloadstreamitem, "image/jpeg");
        }

        //function 7: control the distributio url to tird party with time limit.
        public IActionResult ViewPage(string imagekey, string imagebucket)
        {
            List<string> keys = getKeys();
            AmazonS3Client agent = new
                AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            Stream downloadstreamitem;

            try
            {
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = imagebucket,
                    Key = imagekey,
                    Expires = DateTime.Now.AddSeconds(5) //this link expired after 5 seconds
                };
                ViewBag.imagetempurl = agent.GetPreSignedURL(request);
            }
            catch( AmazonS3Exception ex )
            {
                return BadRequest(ex.Message);
            }
            return View();
        }
    }
}
