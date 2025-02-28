using Webpay.Integration.CSharp.Order.Create;
using Webpay.Integration.CSharp.Order.Validator;

namespace Webpay.Integration.CSharp.Test.Order
{
    internal class VoidValidator : OrderValidator
    {
        public int NoOfCalls;

        public int GetNoOfCalls()
        {
            return NoOfCalls;
        }

        public override string Validate(CreateOrderBuilder order)
        {
            Errors = "";
            NoOfCalls++;
            return "";
        }
    }
}