using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class QueryOrderBuilder : Builder<QueryOrderBuilder>
    {
        private long _orderId;

        public QueryOrderBuilder SetOrderId( long orderId )
        {
            _orderId = orderId;
            return this;
        }

        public override QueryOrderBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }
    }
}
