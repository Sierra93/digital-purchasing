using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;

namespace DigitalPurchasing.Services
{
    public class ReceivedEmailService : IReceivedEmailService
    {
        private readonly ApplicationDbContext _db;

        public ReceivedEmailService(ApplicationDbContext db)
            => _db = db;

        public bool IsProcessed(uint uid)
        {
            var email = _db.ReceivedEmails.FirstOrDefault(q => q.UniqueId == uid);
            return email != null && email.IsProcessed;
        }

        public void MarkProcessed(uint uid, bool isProcessed)
        {
            var email = _db.ReceivedEmails.FirstOrDefault(q => q.UniqueId == uid);
            if (email == null)
            {
                _db.ReceivedEmails.Add(new ReceivedEmail { UniqueId = uid, IsProcessed = isProcessed });
                _db.SaveChanges();
            }
            else
            {
                if (email.IsProcessed) return;
                email.IsProcessed = true;
                _db.SaveChanges();
            }
        }
    }
}
