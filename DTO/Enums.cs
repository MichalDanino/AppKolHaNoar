using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DTO;
public static class Enums
{
    public enum eStatus
    {
        //לבדוק תקינות של של האינם
        SUCCESS,               //succesfully
        NETWORKERROR,           //error 500
        ACCESERROR,           //בעיות הרשאה או גישה
        FAILED,
        APIQuota
    }
    public enum eDialogType
    {
        OK,
        ASK,
        GETPARAMETER,
        MultyButton,
        ERROR,
        INPUT
    }
}
