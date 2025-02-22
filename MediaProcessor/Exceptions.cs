using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using static DTO.Enums;
using static Google.Apis.Requests.BatchRequest;


namespace MediaProcessor;
public  class Exceptions
{
    static GenericMessage _exception;
    static eERROR _error;
    public Exceptions()
    {
        _exception = new GenericMessage();
        _error= eERROR.SUCCESS;
    }

    public static async Task<eERROR> checkUploadFile(string statusResponse)
    {
        AppConfig.listExceptions.Clear();   
        List<string> uploadRespone = statusResponse.Split(',').Where(a => a.Contains("responseStatus") || a.Contains
                       ("success")).ToList();

        if (!uploadRespone[0].Contains("OK"))
        {
            _error = eERROR.NETWORKERROR;
            _exception.MessageTitle = _error.ToString();
            _exception.MessageContent = "שגיאה בהתחברות לשרת. בדוק חיבור רשת ";
            AppConfig.listExceptions.Add(_exception);

        }
        if (uploadRespone[1].Contains("false"))
        {
            _exception.MessageContent = "הבקשה נשלחה אך נכשלה. נסה שוב או בדוק את הקובץ";
            _exception.MessageTitle = _error.ToString();
            AppConfig.listExceptions.Add(_exception);
            _error = eERROR.ACCESERROR;

        }
        return _error;
    }

}
