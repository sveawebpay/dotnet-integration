using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row.Edit;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle.PaymentGateway
{
    public class EditOrderRowBuilder : Builder<EditOrderRowBuilder>
    {
        internal long Id;
        internal List<OrderRow> OrderRows;
        public EditOrderRowBuilder(IConfigurationProvider config) : base(config)
        {
            this.OrderRows = new List<OrderRow>();
        }

        public EditOrderRowBuilder SetTransactionId(long id)
        {
            Id = id;
            return this;
        }

       

        public override EditOrderRowBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }


        public EditOrderRowBuilder AddOrderRows(List<OrderRow> orderRows)
        {
            OrderRows.AddRange(orderRows);
            return this;
        }

        public AdminService.EditOrderRowRequest EditOrderRows()
        {
            
            return new AdminService.EditOrderRowRequest(this);
        }

        public override EditOrderRowBuilder SetCorrelationId(Guid? correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }
}