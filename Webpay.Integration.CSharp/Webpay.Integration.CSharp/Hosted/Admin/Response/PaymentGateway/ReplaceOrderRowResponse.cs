using System.Collections.Generic;
using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response.PaymentGateway
{
    public class ReplaceOrderRowResponse : SpecificHostedAdminResponseBase
    {
        public List<int> OrderRows { get; private set; }
        public ReplaceOrderRowResponse(XmlDocument response) : base(response)
        {
            OrderRows = new List<int>();

            XmlNodeList idNodes = response.SelectNodes("/response/orderrows/orderrow/rowid");
            if (idNodes != null && idNodes.Count > 0)
            {
                foreach (XmlNode node in idNodes)
                    OrderRows.Add(int.Parse(node.InnerText));
            }

        }
    }


}