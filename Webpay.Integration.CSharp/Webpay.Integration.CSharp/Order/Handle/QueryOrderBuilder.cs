using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class QueryOrderBuilder
    {
        private long _orderId;
        public QueryOrderBuilder SetOrderId( long orderId )
        {
            _orderId = orderId;
            return this;
        }

        //public void QueryOrderBuilder SetCountryCode()
        //{
        //}
    }
}
