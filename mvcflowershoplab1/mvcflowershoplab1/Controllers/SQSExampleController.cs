using Microsoft.AspNetCore.Mvc;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
using mvcflowershoplab1.Models;
using System.Xml.Linq;
using mvcflowershoplab1.Data;
using Microsoft.EntityFrameworkCore;

namespace mvcflowershoplab1.Controllers
{
    public class SQSExampleController : Controller
    {

        private const string queueName = "OrderQueueSample";
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
        public SQSExampleController(mvcflowershoplab1Context context)
        {
            dbname = context;
        }

        //function 1: display the reserve page with number count in queue
        public async Task<IActionResult> Index()
        {
            List<string> keys = getKeys();
            AmazonSQSClient client = new AmazonSQSClient(
                keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            //generate the queue URL based on the queuename
            var response = await client.GetQueueUrlAsync(new GetQueueUrlRequest { QueueName = queueName });
            GetQueueAttributesRequest attReq = new GetQueueAttributesRequest
            {
                QueueUrl = response.QueueUrl,
                AttributeNames = { "ApproximateNumberOfMessages" }
            };
            GetQueueAttributesResponse attresponse = await client.GetQueueAttributesAsync(attReq);
            ViewBag.count = attresponse.ApproximateNumberOfMessages;
            List<Bike> BikeLists = await dbname.FlowerTable.ToListAsync();
            ViewBag.BucketName = bucketname;
            return View(BikeLists);
        }

        //function 2: send message to queue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(string bikeID, string name)
        {
            // Assuming you have retrieved the user's name and reservation time
            string userName = name; // Replace with actual user data
            DateTime reservationTime = DateTime.UtcNow; // Replace with actual reservation time

            // Create a reservation message
            var reservation = new
            {
                ReserveID = Guid.NewGuid().ToString(),
                BikeID = bikeID,
                UserName = userName,
                ReservationTime = reservationTime
            };

            // Serialize the reservation message to JSON
            string reservationMessage = JsonConvert.SerializeObject(reservation);

            // Send the reservation message to the SQS queue
            List<string> keys = getKeys();
            AmazonSQSClient client = new AmazonSQSClient(
                keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            try
            {
                var response = await client.GetQueueUrlAsync(new GetQueueUrlRequest { QueueName = queueName });
                SendMessageRequest request = new SendMessageRequest
                {
                    QueueUrl = response.QueueUrl,
                    MessageBody = reservationMessage
                };
                await client.SendMessageAsync(request);
                ViewBag.reserveID = reservation.ReserveID;
                return View("Reserve");
            }
            catch (AmazonSQSException ex)
            {
                // Handle any errors or exceptions, e.g., log the error or display an error message to the user.
                return BadRequest(ex.Message);
            }
        }

        //function 3: Receive message from queue[Admin dashboard for controlling the order reservation
        public async Task<IActionResult> ReadMsgFromQueue()
        {
            List<string> keys = getKeys();
            AmazonSQSClient client = new AmazonSQSClient(
                keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            var response = await client.GetQueueUrlAsync(new GetQueueUrlRequest { QueueName = queueName });
            //create empty list
            List<KeyValuePair<ReserveInfo, string>> messagelist = new List<KeyValuePair<ReserveInfo, string>>();

            try
            {
                ReceiveMessageRequest request = new ReceiveMessageRequest
                {
                    QueueUrl = response.QueueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 10,
                    VisibilityTimeout = 10
                };
                ReceiveMessageResponse response1 = await client.ReceiveMessageAsync(request);
                if(response1.Messages.Count <= 0)
                {
                    ViewBag.errormessage = "Do not have any client waiting in the list";
                }
                else
                {
                    for(int i = 0;  i < response1.Messages.Count; i++)
                    {
                        ReserveInfo clientinfo = JsonConvert.DeserializeObject<ReserveInfo>(response1.Messages[i].Body);
                        string deleteid = response1.Messages[i].ReceiptHandle; //for delete message use
                        messagelist.Add(new KeyValuePair<ReserveInfo, string>(clientinfo, deleteid));
                    }
                }
                return View(messagelist);
            }
            catch(AmazonSQSException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //function 4: delete message from queue
        public async Task<IActionResult> deleteMessage(string deleteid, string word)
        {
            List<string> keys = getKeys();
            AmazonSQSClient client = new AmazonSQSClient(
                keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            var response = await client.GetQueueUrlAsync(new GetQueueUrlRequest { QueueName = queueName });

            try
            {
                if(word == "accept")
                {
                    Console.WriteLine("Pass data to the database");
                }
                else //cancel
                {
                    Console.WriteLine("Delete the message only without bringing to the database");
                }
                DeleteMessageRequest request = new DeleteMessageRequest
                {
                    QueueUrl = response.QueueUrl,
                    ReceiptHandle = deleteid
                };
                await client.DeleteMessageAsync(request);
            }
            catch( AmazonSQSException ex )
            {
                return BadRequest(ex.Message);
            }
            return RedirectToAction("ReadMsgFromQueue", "SQSExample");
        }
    }
}

