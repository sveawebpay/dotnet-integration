using System;
using System.Xml;
using Webpay.Integration.CSharp.Response.Hosted;

namespace Webpay.Integration.CSharp.Hosted.Admin.Actions
{
    public abstract class SpecificHostedAdminResponseBase
    {
        public int StatusCode { get; private set; }
        public bool Accepted { get; private set; }
        public string ErrorMessage { get; private set; }

        protected SpecificHostedAdminResponseBase(XmlDocument response)
        {
            StatusCode = TextInt(response, "/response/statuscode").GetValueOrDefault(101);
            Accepted = StatusCode == 0;
            var errorMessage = SveaResponse.StatusCodeToMessage(StatusCode);
            ErrorMessage = errorMessage.Item2;
        }

        protected static int? AttributeInt(XmlDocument response, string element, string attribute)
        {
            try
            {
                return Convert.ToInt32(response.SelectSingleNode(element).Attributes[attribute].Value);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        protected static int? TextInt(XmlDocument response, string element)
        {
            try
            {
                return Convert.ToInt32(response.SelectSingleNode(element).InnerText);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        protected static string TextString(XmlDocument response, string element)
        {
            try
            {
                return response.SelectSingleNode(element).InnerText;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}