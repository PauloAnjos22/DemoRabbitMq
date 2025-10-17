using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Configuration
{
    public class MailSettings
    {
        public string Host { get; set; }      // smtp.gmail.com
        public int Port { get; set; }         // 587
        public string User { get; set; }      // your-email@gmail.com
        public string Pass { get; set; }      // your-password
        public string From { get; set; }      // noreply@yourapp.com
        public string FromName { get; set; }  // "Your App Name"
    }
}
