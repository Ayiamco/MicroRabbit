using System;
using Laundromat.SharedKernel.Core;

namespace Laundromat.EmailService.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var emailConfiguration=new EmailConfiguration
            {
                From = Environment.GetEnvironmentVariable("EmailConfigFrom"),
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("EmailConfigPort")),
                Password=Environment.GetEnvironmentVariable("EmailConfigPassword"),
                UserName= Environment.GetEnvironmentVariable("EmailConfigUsername"),
                SmtpServer= Environment.GetEnvironmentVariable("EmailConfigSmtpServer")
            };
            
            var emailSender = new EmailSender(emailConfiguration);
            var exchange=new EmailExchange();
            exchange.ConsumeEmail(emailSender);
            Console.ReadLine(); 
            
            
        }
    }
}
