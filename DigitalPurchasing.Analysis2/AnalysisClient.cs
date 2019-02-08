using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Analysis2
{
    public abstract class AnalysisClient<T> where T : class, ICopyable<T>
    {
        public Guid Id { get; set; }
        public List<T> Items { get; set; } = new List<T>();
        public DateTime? Date { get; set; }
    }
}
