using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Core
{
    public class OrderAttribute : Attribute
    {
        public int Order { get; }

        public OrderAttribute(int order) => Order = order;
    }
}
