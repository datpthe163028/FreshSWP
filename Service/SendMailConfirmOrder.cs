using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Project.Service
{
    public class SendMailConfirmOrder
    {
        public static async Task<string> SendGmail(string _from, string _to, string _subject, string _body, string _gmail, string _password)
        {
            using (var message = new MailMessage(_from, _to, _subject, _body))
            {
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                message.ReplyToList.Add(new MailAddress(_from));
                message.Sender = new MailAddress(_from);

                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_gmail, _password);

                    try
                    {
                        await smtpClient.SendMailAsync(message);
                        return "gui email thanh cong";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return "gui email that bai";
                    }
                }
            }
        }
    }
}
