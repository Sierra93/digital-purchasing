using System;

namespace DigitalPurchasing.Models
{
    public abstract class BaseModel<T>
    {
        public T Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public abstract class BaseModel : BaseModel<Guid>
    {
        protected BaseModel() => Id = Guid.NewGuid();
    }
}
