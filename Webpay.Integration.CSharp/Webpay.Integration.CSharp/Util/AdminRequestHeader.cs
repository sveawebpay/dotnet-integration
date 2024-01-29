using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webpay.Integration.CSharp.Util
{
    public class AdminRequestHeader
    {
        public AdminRequestHeader(string key, string value) {
            Header = new KeyValuePair<string, string>(key, value);
        }
       public KeyValuePair<string, string> Header { get; set; }
    }
}
