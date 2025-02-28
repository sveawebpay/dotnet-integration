using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Webpay.Integration.CSharp.Hosted.Admin.Response
{
    public class LowerAmountConfirmResponse: LowerAmountResponse
    {
        public LowerAmountConfirmResponse(XmlDocument response) : base(response)
        {
        }
    }
}
