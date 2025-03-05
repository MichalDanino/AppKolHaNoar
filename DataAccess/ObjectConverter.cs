using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess;
public static class ObjectConverter
{
    public static List<Object> listObject = new List<Object>();

    public static T ConvertToClass<T>(object obj) where T : class
    {
        if (obj is T)
        {
            return obj as T;
        }
        return null;
    }

    
}
