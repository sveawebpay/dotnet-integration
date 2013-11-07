using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Webservice.Getpaymentplanparams
{
    public class PaymentPlanPricePerMonth
    {
        public List<Dictionary<string, long>> Calculate(decimal amount, GetPaymentPlanParamsEuResponse paymentPlanParams)
        {
            var pricesPerMonth = new List<Dictionary<string, long>>();

            if (null == paymentPlanParams)
            {
                return pricesPerMonth;
            }

            foreach (var campaignCode in paymentPlanParams.CampaignCodes)
            {
                decimal fromAmount = campaignCode.FromAmount;
                decimal toAmount = campaignCode.ToAmount;
                decimal monthlyAnnuityFactor = campaignCode.MonthlyAnnuityFactor;
                decimal notificationFee = campaignCode.NotificationFee;

                if (!(fromAmount <= amount) || !(amount <= toAmount))
                {
                    continue;
                }

                var priceMap = new Dictionary<String, long>();
                var pricePerMonth = (long)Math.Round(amount * monthlyAnnuityFactor + notificationFee);
                priceMap.Add("campaignCode", campaignCode.CampaignCode);
                priceMap.Add("pricePerMonth", pricePerMonth);
                pricesPerMonth.Add(priceMap);
            }

            return pricesPerMonth;
        }
    }
}