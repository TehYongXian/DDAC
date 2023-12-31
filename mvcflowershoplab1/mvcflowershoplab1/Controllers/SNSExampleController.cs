﻿using Microsoft.AspNetCore.Mvc;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using Amazon.S3;

namespace mvcflowershoplab1.Controllers
{
    public class SNSExampleController : Controller
    {
        private const string topicArn = "arn:aws:sns:us-east-1:971861707236:SNSSample";

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

        //create the subscription page for the customer (using index function)
        public IActionResult Index()
        {
            return View();
        }

        //function 2: process the subscription 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> processSubscribe(string emailadd)
        {
            List<string> keys = getKeys();
            AmazonSimpleNotificationServiceClient client = new AmazonSimpleNotificationServiceClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            try
            {
                SubscribeRequest request = new SubscribeRequest
                {
                    TopicArn = topicArn,
                    Protocol = "email",
                    Endpoint = emailadd

                };
                SubscribeResponse response = await client.SubscribeAsync(request);
                ViewBag.confirmationID = response.ResponseMetadata.RequestId;
                return View();
            }
            catch(AmazonSimpleNotificationServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //function 3: create a broadcast dashboard for admin
        public IActionResult adminbroadcastmsg()
        {
            return View();
        }

        //function 4: send broadcast message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> broadcastmsg(string subject, string MsgBody)
        {
            List<string> keys = getKeys();
            AmazonSimpleNotificationServiceClient client = new AmazonSimpleNotificationServiceClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            try
            {
                PublishRequest request = new PublishRequest
                {
                    TopicArn = topicArn,
                    Subject = subject,
                    Message = MsgBody
                };
                await client.PublishAsync(request);
                return View();
            }
            catch(AmazonSimpleNotificationServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
