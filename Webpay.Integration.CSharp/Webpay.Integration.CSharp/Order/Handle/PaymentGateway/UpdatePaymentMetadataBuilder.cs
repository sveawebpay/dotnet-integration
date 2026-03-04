using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.AdminService.PaymentGateway;
using Webpay.Integration.CSharp.Config;
using Webpay.Integration.CSharp.Order.Row;
using Webpay.Integration.CSharp.Order.Row.Update;
using Webpay.Integration.CSharp.Util.Constant;

namespace Webpay.Integration.CSharp.Order.Handle.PaymentGateway
{
    public class UpdatePaymentMetadataBuilder : Builder<UpdatePaymentMetadataBuilder>
    {
        internal long TransactionId;
        internal Dictionary<string, string> Metadata = new Dictionary<string, string>(); 
        public UpdatePaymentMetadataBuilder(IConfigurationProvider config) : base(config)
        {
            this.Metadata = new Dictionary<string, string>();
        }

        public UpdatePaymentMetadataBuilder SetTransactionId(long id)
        {
            TransactionId = id;
            return this;
        }
        public UpdatePaymentMetadataBuilder AddAddMetdata(Dictionary<string, string> items)
        {
            Metadata = items;
            return this;
        }
        public UpdatePaymentMetadataBuilder AddAddMetdataItem(string key, string value)
        {
            Metadata.Add(key,value);
            return this;
        }

        public override UpdatePaymentMetadataBuilder SetCountryCode(CountryCode countryCode)
        {
            throw new NotImplementedException();
        }

        public override UpdatePaymentMetadataBuilder SetCorrelationId(Guid? correlationId)
        {
            throw new NotImplementedException();
        }
    }
}