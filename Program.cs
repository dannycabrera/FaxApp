using Microsoft.Owin.Hosting;
using System;
using Twilio;
using Twilio.Rest.Fax.V1;

namespace FaxApp
{
    class Program
    {
        static IDisposable _app = null;

        static void Main(string[] args)
        {
            _app = WebApp.Start<Startup>(url: "http://192.168.1.100:9000/");

            while (true)
            {
                Console.WriteLine("Enter command(s): ex. SendFax");
                var input = Console.ReadLine();

                if (input.ToLower() == "sendfax")
                {
                    Console.WriteLine("Number to send fax to... ex. +19541112345");
                    var to = Console.ReadLine();

                    Console.WriteLine("File to send... ex. Fax.pdf");
                    var filename = Console.ReadLine();

                    var myRemoteAddress = "http://myExternalIPOrUrl:9000/faxapp/callback";

                    Console.WriteLine($"SendFax to: {to}...");

                    TwilioClient.Init("TwilioAccountSid", "TwilioAuthToken");

                    var fax = FaxResource.Create(
                        from: "Replace with your Twilio Fax Number",
                        to: to,
                        mediaUrl: new Uri($"{myRemoteAddress}/getfile?fileName={filename}"),
                        statusCallback: new Uri($"{myRemoteAddress}/status")
                    );

                    Console.WriteLine($"Sent Fax sid: {fax.Sid}, status: {fax.Status}");
                }
            }
        }
    }
}
