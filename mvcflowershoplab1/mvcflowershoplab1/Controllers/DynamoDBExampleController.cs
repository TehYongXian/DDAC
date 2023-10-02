using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2;
using Amazon;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using mvcflowershoplab1.Models;

namespace mvcflowershoplab1.Controllers
{
    public class DynamoDBExampleController : Controller
    {
        private const string TableName = "CustomerPurchaseTable";

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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddItem()
        {
            return Index();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> addingitem(CustomerPurchase information)
        {
            information.CustomerTransactionID=Guid.NewGuid().ToString();
            List<String> keys = getKeys();
            AmazonDynamoDBClient agent = new AmazonDynamoDBClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            Dictionary<string, AttributeValue> itemlist = new Dictionary<string, AttributeValue>();
            itemlist["CustomerFullName"] = new AttributeValue { S = information.CustomerFullName };
            itemlist["TransactionID"] = new AttributeValue { S = information.CustomerTransactionID };
            itemlist["Purchase_Amount"] = new AttributeValue { N = information.PurchaseAmount.ToString() };
            itemlist["Payment_Status"] = new AttributeValue { BOOL = information.paymentstatus };
            if (!string.IsNullOrEmpty(information.paymentDate.ToString()) && information.paymentDate.ToString()!="0000-00-00")
            {
                itemlist["Payment_Date"] = new AttributeValue { S = information.paymentDate.ToString() };
            }
            PutItemRequest request = new PutItemRequest
            {
                TableName = TableName,
                Item = itemlist
            };

            await agent.PutItemAsync(request);
            return RedirectToAction("ViewItem","DynamoDBExample");
        }

        public async Task <IActionResult> ViewItem()
        {
            List<String> keys = getKeys();
            AmazonDynamoDBClient agent = new AmazonDynamoDBClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            List<CustomerPurchase> informationlist = new List<CustomerPurchase>();
            ScanRequest result = new ScanRequest
            {
                TableName = TableName
            };
            ScanResponse response = await agent.ScanAsync(result);

            if(response.Count == 0)
            {
                return BadRequest("error");
            }
            foreach(var item in response.Items)
            {
                CustomerPurchase information = new CustomerPurchase();
                information.CustomerFullName = item["CustomerFullName"].S;
                information.CustomerTransactionID = item["TransactionID"].S;
                information.paymentstatus = item["Payment_Status"].BOOL;
                information.PurchaseAmount = decimal.Parse(item["Purchase_Amount"].N);
                if (item.ContainsKey("Payment_Date"))
                {
                    information.paymentDate = DateTime.Parse(item["Payment_Date"].S);
                }
                else
                {
                    information.paymentDate = DateTime.Parse("1/1/0001 12:00:00 AM");
                }
                informationlist.Add(information);
            }
            return View(informationlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(string partitionkey, string sortkey)
        {
            List<String> keys = getKeys();
            AmazonDynamoDBClient agent = new AmazonDynamoDBClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            DeleteItemRequest request = new DeleteItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    {"CustomerFullName", new AttributeValue{S = partitionkey} },
                    {"TransactionID", new AttributeValue{S = sortkey} }
                },

            };
            await agent.DeleteItemAsync(request);
            return RedirectToAction("ViewItem", "DynamoDBExample");
        }
    }
}
