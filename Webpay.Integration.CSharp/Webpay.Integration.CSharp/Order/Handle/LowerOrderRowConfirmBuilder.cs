using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Order.Row.LowerAmount;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle
{
    public class LowerOrderRowConfirmBuilder : Builder<LowerOrderRowConfirmBuilder>
    {
        internal long Id;
        internal List<OrderRow> OrderRows;
        internal DateTime CaptureDate;
        public LowerOrderRowConfirmBuilder(IConfigurationProvider config) : base(config)
        {
            this.OrderRows = new List<OrderRow>();
        }

        public LowerOrderRowConfirmBuilder SetTransactionId(long id)
        {
            Id = id;
            return this;
        }

        public LowerOrderRowConfirmBuilder SetCaptureDate(DateTime captureDate)
        {
            CaptureDate = captureDate;
            return this;
        }

        public override LowerOrderRowConfirmBuilder SetCountryCode(CountryCode countryCode)
        {
            _countryCode = countryCode;
            return this;
        }


        public LowerOrderRowConfirmBuilder AddOrderRows(IList<OrderRow> orderRows)
        {
            OrderRows.AddRange(orderRows);
            return this;
        }



        public AdminService.LowerOrderRowConfirmRequest LowerOrderRows()
        {
            return new AdminService.LowerOrderRowConfirmRequest(this);
        }

        public override LowerOrderRowConfirmBuilder SetCorrelationId(Guid? correlationId)
        {
            _correlationId = correlationId;
            return this;
        }
    }
}