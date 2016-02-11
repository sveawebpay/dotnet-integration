using System.Runtime.Serialization;

namespace Webpay.Integration.CSharp.AdminWS
{


    public partial class BasicResponse
    {
        public bool Accepted { get; set; }

        [OnDeserialized]
        public void SetAccepted(StreamingContext context)
        {
            this.Accepted = (this.ResultCode == 0);
        }

    }
}