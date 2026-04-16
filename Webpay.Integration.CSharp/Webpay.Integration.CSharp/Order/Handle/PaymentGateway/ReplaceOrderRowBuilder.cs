using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row.Replace;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle.PaymentGateway
{
    public class ReplaceOrderRowBuilder : Builder<ReplaceOrderRowBuilder>
    {
        internal long Id;
        internal List<OrderRow> OrderRows;
        public ReplaceOrderRowBuilder(IConfigurationProvider config) : base(config)
        {
            this.OrderRows = new List<OrderRow>();
        }

        public ReplaceOrderRowBuilder SetTransactionId(long id)
        {
            Id = id;
            return this;
        }

       

        public override ReplaceOrderRowBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }


        public ReplaceOrderRowBuilder AddOrderRows(List<OrderRow> orderRows)
        {
            OrderRows.AddRange(orderRows);
            return this;
        }


        public override ReplaceOrderRowBuilder SetCorrelationId(Guid? correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }
}