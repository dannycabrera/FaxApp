using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FaxApp
{
    public class Util
    {
        public async Task<string> DownloadFile(string url)
        {
            using (var client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var contentStream = await response.Content.ReadAsStreamAsync();

                        var filePath = Path.Combine(@"C:\Temp\FaxApp\Inbound\", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf");

                        using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await contentStream.CopyToAsync(fs);
                        }

                        return filePath;
                    }
                }
            }

            return null;
        }
    }
}
