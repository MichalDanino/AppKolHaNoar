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
            { "Campaign_Name", "שם הקמפיין" },
            { "Campaign_RunningTime", "שעה לעדכון" },
            { "Campaign_RunningDay", "יום לעדכון" },

            { "ChannelExtension_ID", "מזהה" },
            { "ChannelExtension_ChannelID", "מזהה שלוחה" },
            { "ChannelExtension_Name", "שם הערוץ" },
            { "ChannelExtension_Long", "מס' השלוחה לסרטונים ארוכים" },
            { "ChannelExtension_Short", "מספר השלוחה לסרטונים קצרים(פחות מ10 דק')" },
            { "ChannelExtension_Campaign", "מס' קמפיין להפעלה(בעת עדכו ןהשלוחה)" },
            { "ChannelExtension_RunningTime", "שעה לעדכון" },
            { "ChannelExtension_RunningDay", "יום לעדכון" },
            { "ChannelExtension_RunningLastDay", "תאריך הרצה אחרונה" },


            { "ForbiddenWords_Word", "מילים לסינון" }
        };

    public static Dictionary<string, string> passwords = new Dictionary<string, string> {
                    {"שינוי סיסמה למערכת ימות המשיח", "PASSWORD"},
                    {"שינוי שם משתמש לימות המשיח","USERNAME"},
                    { "שינוי מפתח יוטיוב","YOUTUBE_API_KEY"},
                    { "שינוי שם המפתח יוטיוב","APPLICATION_NAME"} ,
                    { "שינוי סיסמת מנהל לאפליקציה","MANAGERPASSWORD"} };

    public static Dictionary<string, int> keyValuePairs = new Dictionary<string, int>()
        {
            {"ראשון",1},
            {"שני",2},
            {"שלישי",3},
            {"רביעי",4},
            {"חמישי",5},
            {"שישי",6},
            {"מוצאי שבת",7},
        };

}
