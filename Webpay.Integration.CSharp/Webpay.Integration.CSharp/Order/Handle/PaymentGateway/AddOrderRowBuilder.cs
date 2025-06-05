using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row.Add;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle.PaymentGateway
{
    public class AddOrderRowBuilder : Builder<AddOrderRowBuilder>
    {
        internal long Id;
        internal List<OrderRow> OrderRows;
        public AddOrderRowBuilder(IConfigurationProvider config) : base(config)
        {
            this.OrderRows = new List<OrderRow>();
        }

        public AddOrderRowBuilder SetTransactionId(long id)
        {
            Id = id;
            return this;
        }

       

        public override AddOrderRowBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }


        public AddOrderRowBuilder AddOrderRows(List<OrderRow> orderRows)
        {
            OrderRows.AddRange(orderRows);
            return this;
        }


        public override AddOrderRowBuilder SetCorrelationId(Guid? correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }
}