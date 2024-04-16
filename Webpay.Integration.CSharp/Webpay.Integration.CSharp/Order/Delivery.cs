using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webpay.Integration.CSharp.Order.Handle;
using Webpay.Integration.CSharp.Order.Row.credit;

namespace Webpay.Integration.CSharp.Order
{
    public class Delivery
    {
        public int Id {  get; set; }
        public List<NewCreditOrderRowBuilder> NewOrderRows { get; set; }
        public List<CreditOrderRowBuilder> OrderRows { get; set; }
        public Delivery() {
            NewOrderRows = new List<NewCreditOrderRowBuilder>();
            OrderRows = new List<CreditOrderRowBuilder>();
        }
        public Delivery AddOrderRows(List<NewCreditOrderRowBuilder> newOrderRows, List<CreditOrderRowBuilder> orderRows)
        {
            NewOrderRows.AddRange(newOrderRows);
            OrderRows.AddRange(orderRows);
            return this;
        }
    }
}
