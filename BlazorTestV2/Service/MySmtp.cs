using System.Net;
using System.Net.Mail;

namespace BlazorTestV2.Service
{
    public class MySmtp
    {
        private SmtpClient smtp;
        public MySmtp() 
        {
            IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
            var setting = config.GetRequiredSection("SmtpServer");

            if (setting.Exists())
            {
                NetworkCredential credential = new NetworkCredential(
                    setting.GetSection("UserName").Value, 
                    setting.GetSection("Password").Value);
                smtp = new SmtpClient
                {
                    Host = setting.GetSection("Host").Value,
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = credential,
                    Timeout = 100000 //100 sec
                };
            }
            else
            {
                throw new Exception("No smtp setting. Please add SmtpServer related variables in appsetting.");
            }
        }

        public void Send(string MailTo, string MailCC, string MailSubject, string MailBody)
        {
            try
            {
                IConfiguration config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build();
                var setting = config.GetRequiredSection("SmtpServer");
                MailMessage message = new MailMessage(setting.GetSection("From").Value, MailTo, MailSubject, MailBody);
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception("Smtp send fail.");
            }
        }

    }
}
