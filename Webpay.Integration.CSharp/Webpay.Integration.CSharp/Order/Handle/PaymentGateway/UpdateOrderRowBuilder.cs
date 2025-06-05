using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row.Update;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle.PaymentGateway
{
    public class UpdateOrderRowBuilder : Builder<UpdateOrderRowBuilder>
    {
        internal long Id;
        internal List<OrderRow> OrderRows;
        public UpdateOrderRowBuilder(IConfigurationProvider config) : base(config)
        {
            this.OrderRows = new List<OrderRow>();
        }

        public UpdateOrderRowBuilder SetTransactionId(long id)
        {
            Id = id;
            return this;
        }

       

        public override UpdateOrderRowBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }


        public UpdateOrderRowBuilder AddOrderRows(List<OrderRow> orderRows)
        {
            OrderRows.AddRange(orderRows);
            return this;
        }

        public AdminService.UpdateOrderRowRequest EditOrderRows()
        {
            
            return new AdminService.UpdateOrderRowRequest(this);
        }

        public override UpdateOrderRowBuilder SetCorrelationId(Guid? correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }
}