using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using DotNetEnv;
using DTO;
using Newtonsoft.Json.Linq;
using static DTO.Enums;
using static MediaProcessor.AppStaticParameter;

namespace MediaProcessor.API;
public class YemotHamashichAPI
{

    private static readonly string baseUrl = "https://www.call2all.co.il//ym/api/";
    private const int chunkSize = 50000000;
    private const string remotePath = "ivr2:";

    private static string token = "";
    private static HttpClient httpClient;
    static GenericMessage exceptions;
    static DateTime timestamp;
    public eStatus status;


    static YemotHamashichAPI()
    {
        httpClient = new HttpClient();
        exceptions = new GenericMessage();
    }
    /// <summary>
    /// The function is designed to define all the necessary components for uploading a file to the system,
    /// including creating an HTTP POST request with content in multipart/form-data format, 
    /// adding required metadata and fields, handling the file as a data stream, 
    /// and sending the request to the server while processing the received response.
    /// </summary>
    public async Task<eStatus>  UplaodFiles()
    {

        using (httpClient = new HttpClient())
        {
            try
            {
                status = eStatus.SUCCESS;
                //get token
                token = await HandleRequest();
                //patch to upload files
                string[] videoFiles = Directory.GetFiles(AppConfig.rootURL + @"Downloads");
                List<string> uploadRespone = new List<string>();
                VideoDetails videoDetails = new VideoDetails();
                foreach (var videoFile in videoFiles)
                {
                    if (videoFile.Contains(".mp3"))
                    {
                        int chunkSize = 50000000; // 1MB
                        await foreach (var chunk in GetChunks(videoFile, chunkSize))
                        {

                            var formData = CreateFormData(chunk, videoFile);
                            string response = await Uploadfile(formData);
                            status = await Exceptions.checkUploadFile(response, videoDetails);
                            if (status != eStatus.SUCCESS)
                            {
                                break;
                            }
                        }
                        Directory.Delete(videoFile, true);

                    }
                    // if there is problem with the internet connection.stop all uploading
                    if (status == eStatus.NETWORKERROR)
                    {
                        break;
                    }

                }
            }
            catch (Exception ex) { }
            return status;
        }
    }

    /// <summary>
    /// Get token
    /// </summary>
    /// <returns>Token code</returns>
    public async Task<string> HandleRequest()
    {
        
        var response =  httpClient.GetStringAsync(baseUrl + $"Login?username={AppConfig.UserNameYH}&password={AppConfig.passwordYH}").Result;
        timestamp = DateTime.Now;
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

    /// <summary>
    /// upload file to YemotHamshich
    /// </summary>
    /// <param name="payload">FromDate with all parmeter to upload file</param>
    /// <returns>The server status of upload file</returns>
    static async Task<string> Uploadfile(HttpContent payload)
    {
        string requestURL = baseUrl + "/ym/api/UploadFile";
        var response = await httpClient.PostAsync(requestURL, payload);
        return await response.Content.ReadAsStringAsync();

    }

    public async Task<string> RunCampaign(string campain)
    {
        if (IsTimestampExpired())
        {
            token = await HandleRequest();
        }
        timestamp = DateTime.Now;       

        var response = await httpClient.GetAsync(baseUrl + $"RunCampaign/?token={token}&templateId={campain}");
        return await response.Content.ReadAsStringAsync();

    }

    public async Task<List<Campaign>> GetCampaing()
    {
        List<Campaign> campaigns = new List<Campaign>();
        if (IsTimestampExpired())
           token = await HandleRequest();

        if(token != null) { 
         var response =  httpClient.GetStringAsync(baseUrl + $"GetTemplates/?token={token}").Result;
            requsetLegal(response);
            if (globalStatus != eStatus.SUCCESS)
                return campaigns;

            //Extracting data from JSON

            JObject jsonResponse = JObject.Parse(response);
        
        campaigns = jsonResponse["templates"]
           .Select(t => new Campaign
           {
               Campaign_Name = (string)t["description"],
               Campaign_Number = t["templateId"].ToString()
           })
           .ToList();
            //Enter to the DB
        MultiSourceDataService dB = new MultiSourceDataService();
        await dB.UpdateCampainTable(campaigns);
        return campaigns;
        }
        return campaigns;
    }

    private bool IsTimestampExpired()
    {
        return timestamp.AddMinutes(30) <= DateTime.Now;
    }
private void requsetLegal(string response)
    {     
        globalStatus = eStatus.SUCCESS;

        string[] temp = response.Split(',');
        foreach (string s in temp)
        {
            if(s.Contains("responseStatus"))
            {
                if (!s.Contains("OK"))
                    globalStatus = eStatus.ACCESERROR;
                break;
                   
            }
        }
    }
}

