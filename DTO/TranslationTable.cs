using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO;
public class TranslationTable
{
    public static Dictionary <string, string> PropertyNamesInHebrew = new Dictionary<string, string>
        {
            { "callRouting_ID", "מזהה" },
            { "CallRouting_NameChannel", "שם הערוץ" },
            { "CallRouting_Category", "סוג הערוץ" },
            { "CallRouting_Subcategory", "תת-קטגוריה" },
            { "callRouting_Navigat", "מס' השלוחה בימות המשיח" },

            { "Campaign_ID", "מזהה" },
            { "Campaign_Number", "מס' קמפיין" },
            { "Campaign_Channel", "מס' השלוחה (אם רוצים שירוץ בעת עדכון שלוחה מוסיימת,לכתוב כאן את השלוחה) ג" },

            { "ChannelExtension_ID", "מזהה" },
            { "ChannelExtension_ChannelID", "מזהה שלוחה" },
            { "ChannelExtension_Name", "שם הערוץ" },
            { "ChannelExtension_Long", "מס' השלוחה לסרטונים ארוכים" },
            { "ChannelExtension_Short", "מספר השלוחה לסרטונים קצרים(פחות מ10 דק')" },
            { "ChannelExtension_Campaign", "מס' קמפיין להפעלה(בעת עדכו ןהשלוחה)" },
            { "ChannelExtension_RunningTime", "שעה ויום לעדכון" }
        };


    static Dictionary<string, string> campaingPropertyNamesInHebrew = new Dictionary<string, string>
        {
            
   
        };

    static Dictionary<string, string> ChannelExtensionPropertyNamesInHebrew = new Dictionary<string, string>
        {
        };
}
