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
    static GenericException _exception;
    static eERROR _error;
    public Exceptions()
    {
        _exception = new GenericException();
        _error= eERROR.success;
    }

    public static async Task<eERROR> checkUploadFile(string statusResponse)
    {
        AppConfig.listExceptions.Clear();   
        List<string> uploadRespone = statusResponse.Split(',').Where(a => a.Contains("responseStatus") || a.Contains
                       ("success")).ToList();

        if (!uploadRespone[0].Contains("OK"))
        {
            _error = eERROR.NetworkError;
            _exception.exceptionTitle = _error.ToString();
            _exception.exceptionMessage = "שגיאה בהתחברות לשרת. בדוק חיבור רשת ";
            AppConfig.listExceptions.Add(_exception);

        }
        if (uploadRespone[1].Contains("false"))
        {
            _exception.exceptionMessage = "הבקשה נשלחה אך נכשלה. נסה שוב או בדוק את הקובץ";
            _exception.exceptionTitle = _error.ToString();
            AppConfig.listExceptions.Add(_exception);
            _error = eERROR.acsseccError;

        }
        return _error;
    }

}
