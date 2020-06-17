using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Azure.LogAnalytics.Model;

namespace Azure.LogAnalytics
{
    public static class FunctionApp
    {
        // Update customerId to your Log Analytics workspace ID

        static readonly string customerId = System.Environment.GetEnvironmentVariable("LogAnalyticWorkSpaceId", EnvironmentVariableTarget.Process);
        
        // For sharedKey, use either the primary or the secondary Connected Sources client authentication key   
        static readonly string sharedKey = System.Environment.GetEnvironmentVariable("WorkSpaceSharedKey", EnvironmentVariableTarget.Process);

        // LogName is name of the event type that is being submitted to Azure Monitor
        static readonly string LogName = System.Environment.GetEnvironmentVariable("GenericLogName", EnvironmentVariableTarget.Process);

        static string TimeStampField = "";

        [FunctionName("Post-Logs")]
        public static async Task<IActionResult> PostLogs(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("PostLogs: HTTP trigger function  processed a request.");
            try
            {   
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var jsonObject = JsonConvert.DeserializeObject<LogItem[]>(requestBody);
                //TODO :)
                var json = JsonConvert.SerializeObject(jsonObject);

                // Create a hash for the API signature
                var datestring = DateTime.UtcNow.ToString("r");
                var jsonBytes = Encoding.UTF8.GetBytes(json);
                string stringToHash = "POST\n" + jsonBytes.Length + "\napplication/json\n" + "x-ms-date:" + datestring + "\n/api/logs";
                string hashedString = BuildSignature(stringToHash, sharedKey);
                string signature = "SharedKey " + customerId + ":" + hashedString;

                PostData(signature, datestring, json);

               
            }
            catch
            {
                //TODO
            }
            finally
            {
                //TODO
            }
            //TODO Message
            return new OkObjectResult("OK");

        }
        // Build the API signature
        public static string BuildSignature(string message, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = Convert.FromBase64String(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hash = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Send a request to the POST API endpoint
        public static void PostData(string signature, string date, string json)
        {
            try
            {
                string url = "https://" + customerId + ".ods.opinsights.azure.com/api/logs?api-version=2016-04-01";

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Log-Type", LogName);
                client.DefaultRequestHeaders.Add("Authorization", signature);
                client.DefaultRequestHeaders.Add("x-ms-date", date);
                client.DefaultRequestHeaders.Add("time-generated-field", TimeStampField);

                System.Net.Http.HttpContent httpContent = new StringContent(json, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                Task<System.Net.Http.HttpResponseMessage> response = client.PostAsync(new Uri(url), httpContent);

                System.Net.Http.HttpContent responseContent = response.Result.Content;
                string result = responseContent.ReadAsStringAsync().Result;
                Console.WriteLine("Return Result: " + result);
            }
            catch (Exception excep)
            {
                Console.WriteLine("API Post Exception: " + excep.Message);
            }
        }
    }
}
