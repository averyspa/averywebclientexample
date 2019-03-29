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
using System.Text;
using System.Web;

namespace AveryWebClientExample
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // Create a request using a URL that can receive a post. 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(System.Environment.GetEnvironmentVariable("https://<baseurl>/api/v3/items?organization_id=<id>"));
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            request.Headers.Add("Content-Type:x-www-form-urlencoded;charset=UTF-8");
            request.Headers.Add("Authorization:Zoho-authtoken <token>");
            //Encode
            string stringData = "JSONString=\"" + HttpUtility.UrlEncode(requestBody) + "\"";
            var data = Encoding.UTF8.GetBytes(stringData);
            request.ContentLength = data.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                // Write the data to the request stream.
                dataStream.Write(data, 0, data.Length);
            }
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Display the status.
            //Console.WriteLine(response.StatusDescription);
            // Get the stream containing content returned by the server.
            string responseFromServer = "";
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    // Read the content.
                    responseFromServer = reader.ReadToEnd();
                }
            }
            return new OkObjectResult(responseFromServer);

        }
    }
}
