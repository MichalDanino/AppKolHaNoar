using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DTO;
public static class Enums
{
    public enum eERROR
    {
        //לבדוק תקינות של של האינם
        success,               //succesfully
        NetworkError,           //error 500
        acsseccError,           //בעיות הרשאה או גישה
    }
}
