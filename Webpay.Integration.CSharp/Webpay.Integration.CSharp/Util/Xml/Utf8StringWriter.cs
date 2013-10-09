using System.IO;
using System.Text;

namespace Webpay.Integration.CSharp.Util.Xml
{
    public class Utf8StringWriter : StringWriter
    {
        private readonly Encoding _encoding;

        public Utf8StringWriter(Encoding encoding)
        {
            _encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return _encoding; }
        }
    }
}
