using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FaxApp
{
    public class CallbackController : ApiController
    {
        [HttpPost]
        [Route("Status")]
        public async Task<HttpResponseMessage> Status()
        {
            try
            {
                Console.WriteLine("Status...");

                string queryString = await Request.Content.ReadAsStringAsync();
                var data = HttpUtility.ParseQueryString(queryString);

                Console.WriteLine($"Status... {queryString}");

                // Respond with empty 200/OK to Twilio
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Status.Exception - {e.Message}: {e.StackTrace}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetFile(string fileName)
        {
            Console.WriteLine($"GetFile... {fileName}");

            var filePath = Path.Combine(@"C:\Temp\FaxApp\Outbound", fileName);

            if (File.Exists(filePath))
            {
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(filePath, FileMode.Open);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(filePath);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentLength = stream.Length;
                return result;
            }
            else
            {
                Console.WriteLine($"GetFile.BadRequest - fileName: {fileName}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        [Route("Sent")]
        public HttpResponseMessage Sent()
        {
            try
            {
                Console.WriteLine("Fax is being sent...");

                string xmlString = "<Response>" +
                                        "<Receive action =\"/faxapp/callback/received\"/>" +
                                    "</Response>";

                // Respond with empty 200/OK to Twilio
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(xmlString, Encoding.UTF8, "text/xml")
                };

                return response;
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Sent.Exception - {e.Message}: {e.StackTrace}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("Received")]
        public async Task<HttpResponseMessage> Received()
        {
            try
            {
                Console.WriteLine("Fax received...");

                // log the URL of the PDF received in the fax
                string queryString = await Request.Content.ReadAsStringAsync();
                Console.WriteLine($"Received [{queryString}]", null, false);

                var data = HttpUtility.ParseQueryString(queryString);
                var mediaUrl = data.GetValues("MediaUrl").FirstOrDefault();
                var FaxSid = data.GetValues("FaxSid").FirstOrDefault();
                var from = data.GetValues("From").FirstOrDefault();
                var FaxStatus = data.GetValues("FaxStatus").FirstOrDefault();
                var NumPages = data.GetValues("NumPages").FirstOrDefault();

                Console.WriteLine($"Received Fax from: {from}, Pages: {NumPages}, status: {FaxStatus}");

                var filePath = await new Util().DownloadFile(mediaUrl);

                if (filePath != null)
                {
                    Console.WriteLine($"Fax file: {filePath}");
                }
                
                // Respond with empty 200/OK to Twilio
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Received.Exception - {e.Message}: {e.StackTrace}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
