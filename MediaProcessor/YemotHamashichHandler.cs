using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Linq;


namespace MediaProcessor
{
    public class YemotHamashichHandler
    {
        private static readonly string baseUrl = "https://www.call2all.co.il//ym/api/";
        private const int chunkSize = 50000000;
        private const string remotePath = "ivr2:";
        private static string token = "";
        public static string ApiDID = "";
        public static string password = "";
        string folderPath = @"C:\Program Files\KolHaNoar\Downloads";
        private static HttpClient httpClient = new HttpClient();

        /// <summary>
        /// The function is designed to define all the necessary components for uploading a file to the system,
        /// including creating an HTTP POST request with content in multipart/form-data format, 
        /// adding required metadata and fields, handling the file as a data stream, 
        /// and sending the request to the server while processing the received response.
        /// </summary>
        public async void UplaodFiles()
        {
            using (httpClient = new HttpClient())
            {
                SetApi();
                token = await HandleRequest();
                string[] videoFiles = Directory.GetFiles(Main.MainDirectoryPath + @"\Downloads");

                foreach (var videoFile in videoFiles)
                {
                    int chunkSize = 50000000; // 1MB
                    await foreach (var chunk in GetChunks(videoFile, chunkSize))
                    {

                        var formData = CreateFormData(chunk, videoFile);
                        string response = await Uploadfile(baseUrl + "/ym/api/UploadFile", formData);
                    }
                }
            }
        }

        /// <summary>
        /// Get token
        /// </summary>
        /// <returns>Token code</returns>
        public async Task<string> HandleRequest()
        {

            var response = await httpClient.GetStringAsync(baseUrl + $"Login?username={ApiDID}&password={password}");
            var jsonResponse = JObject.Parse(response);

            if (jsonResponse["responseStatus"].ToString() == "OK")
            {
                return jsonResponse["token"].ToString();
            }
            else
            {
                return null;
            }

        }
        static async IAsyncEnumerable<byte[]> GetChunks(string filePath, int chunkSize)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[chunkSize];
                int bytesRead;

                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, chunkSize)) > 0)
                {
                    if (bytesRead == chunkSize)
                    {
                        yield return buffer;
                    }
                    else
                    {
                        yield return buffer[..bytesRead];
                        yield break;
                    }
                }
            }
        }

        /// <summary>
        /// Create FormData to upload file by HTTP Post request
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="contentName"></param>
        /// <param name="partialData"></param>
        /// <returns>FormData object</returns>
        static MultipartFormDataContent CreateFormData(byte[] bytes, string contentName, (string uuid, long fileSize, int partCount, int part)? partialData = null)
        {
            var data = new Dictionary<string, object>
        {
            { "token", token },
            { "path", remotePath+"content/" },
            { "convertAudio", 1 },
            { "autoNumbering", true }
        };

            if (partialData.HasValue)
            {
                data["uploader"] = "yemot-admin";
                data["qquuid"] = partialData.Value.uuid;
                data["qqfilename"] = contentName;
                data["qqtotalfilesize"] = partialData.Value.fileSize;
                data["qqtotalparts"] = partialData.Value.partCount;

                if (partialData.Value.part < partialData.Value.partCount)
                {
                    data["qqpartbyteoffset"] = chunkSize * partialData.Value.part;
                    data["qqpartindex"] = partialData.Value.part;
                    data["qqchunksize"] = bytes.Length;
                }
            }

            var formData = new MultipartFormDataContent();
            foreach (var kvp in data)
            {
                formData.Add(new StringContent(kvp.Value.ToString()), kvp.Key);
            }

            if (bytes != null)
            {
                formData.Add(new ByteArrayContent(bytes) { Headers = { ContentType = new MediaTypeHeaderValue("application/octet-stream") } },
                              partialData.HasValue ? "qqfile" : "file", contentName);
            }

            return formData;
        }
        static async Task<string> Uploadfile(string url, HttpContent payload)
        {

            var response = await httpClient.PostAsync(url, payload);
            return await response.Content.ReadAsStringAsync();

        }

        public async Task<string> RunCampaign(string campain)
        {
            token = await HandleRequest();
            var response = await httpClient.GetAsync(baseUrl + $"RunCampaign/?token={token}&templateId={campain}");
            return await response.Content.ReadAsStringAsync();

        }

        private void SetApi()
        {
            string resourceName = Main.MainDirectoryPath + @".env";
            // Assembly assembly = Assembly.Load("DTO");
            Env.Load(resourceName);
            password = Env.GetString("PASSWORD");
            ApiDID = Env.GetString("USERNAME");


        }
    }
}
