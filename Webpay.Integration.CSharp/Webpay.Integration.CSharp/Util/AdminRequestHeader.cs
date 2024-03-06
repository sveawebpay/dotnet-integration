using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webpay.Integration.CSharp.Util
{
    public class AdminRequestHeader
    {
        public AdminRequestHeader(string key, object value) {
            Header = new KeyValuePair<string, object>(key, value);
        }
       public KeyValuePair<string, object> Header { get; set; }
    }
}
