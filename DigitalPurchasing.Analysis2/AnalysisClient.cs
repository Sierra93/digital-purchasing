using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Analysis2
{
    public class AnalysisClient<T> where T : class
    {
        public Guid Id { get; set; }
        public List<T> Items { get; set; } = new List<T>();
        public DateTime? Date { get; set; } 
    }
}
