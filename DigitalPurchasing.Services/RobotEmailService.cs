using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalPurchasing.Core.Interfaces;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;

namespace DigitalPurchasing.Services
{
    public class RobotEmailService : IRobotEmailService
    {
        private readonly IList<IEmailProcessor> _emailProcessors;

        public RobotEmailService(IList<IEmailProcessor> emailProcessors) => _emailProcessors = emailProcessors;

        public void CheckAppEmail()
        {
            using (var client = new ImapClient(new NullProtocolLogger()))
            {
                client.Connect("imap.yandex.ru", 993, true);
                client.Authenticate("app@digitalpurchasing.com", "Ar7jXeiedFPNYmUNqUZbphPW");

                client.Inbox.Open(FolderAccess.ReadOnly);

                var query = SearchQuery.DeliveredAfter(DateTime.Now.AddHours(-12));

                foreach (var uniqueId in client.Inbox.Search(query))
                {
                    var message = client.Inbox.GetMessage(uniqueId);
                    Console.WriteLine("[match] {0}: {1} {2}", uniqueId, message.Subject, message.Date);

                    foreach (var processor in _emailProcessors)
                    {
                        processor.Process();
                    }
                }
            }
        }
    }


}
