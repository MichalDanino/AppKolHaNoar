using System.Net.Mail;
using System.Net;

namespace EmailReport;

public class Email
{
    public void SendMail()
    {
        try
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("kolhanoarreport@gmail.com");
            mail.To.Add("m.527632947@gmail.com");
            mail.Subject = "נושא המייל";
            mail.Body = "זהו תוכן המייל";

            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential("kolhanoarreport@gmail.com", "");
            smtpServer.EnableSsl = true;

            smtpServer.Send(mail);
            Console.WriteLine("המייל נשלח בהצלחה");
        }
        catch (Exception ex)
        {
            Console.WriteLine("שגיאה בשליחת המייל: " + ex.Message);
        }
    }

}
