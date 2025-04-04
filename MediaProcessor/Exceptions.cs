using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using static DTO.Enums;
using static Google.Apis.Requests.BatchRequest;


namespace MediaProcessor;
public class Exceptions
{
    static GenericMessage _exception;
    static eStatus _error;
    public Exceptions()
    {
        _exception = new GenericMessage();
        _error = eStatus.SUCCESS;
    }

    public static async Task<eStatus> checkUploadFile(string statusResponse, VideoDetails videoDatails)
    {

        _error = eStatus.SUCCESS;

        List<string> uploadRespone = statusResponse.Split(',').Where(a => a.Contains("responseStatus") || a.Contains
                       ("success")).ToList();

        if (!uploadRespone[0].Contains("OK"))
        {
            _error = eStatus.NETWORKERROR;
            _exception.MessageTitle = _error.ToString();
            _exception.MessageContent = "שגיאה בהתחברות לשרת. בדוק חיבור רשת ";

            AppConfig.listExceptions.Add(_exception);

        }
        if (uploadRespone[1].Contains("false"))
        {
            _exception.MessageContent = "הבקשה נשלחה אך נכשלה. נסה שוב או בדוק את הקובץ";
            _exception.MessageTitle = _error.ToString();
            _exception.subMessage = $"ווידאו {videoDatails.VideoDetails_Title} לא עלה למערכת (מהה סרטון: {videoDatails.VideoDetails_videoPath}  ";
            AppConfig.listExceptions.Add(_exception);
            _error = eStatus.ACCESERROR;

        }
        return _error;
    }

    public static async Task<eStatus> checkExtensionMappingValidity(string ChannelID)
    {

        _error = eStatus.SUCCESS;

            _error = eStatus.FAILED;
            _exception.MessageTitle = _error.ToString();
            _exception.MessageContent = $"ערוץ {ChannelID}  לא מכיל נתיב להעלאת הקבצים שלו, אנא עדכנו את המערכת. ";

            AppConfig.listExceptions.Add(_exception);

        
        return _error;

    }
}
