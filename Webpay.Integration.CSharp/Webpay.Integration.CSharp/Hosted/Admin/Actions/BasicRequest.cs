using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public class BasicRequest
    {
        public readonly string CorrelationId;
        public BasicRequest(string correlationId = "")
        {
            CorrelationId = correlationId;
        }
    }
}
