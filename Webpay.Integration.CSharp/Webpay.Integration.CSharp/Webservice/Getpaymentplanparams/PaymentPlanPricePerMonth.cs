using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Exception;
using Webpay.Integration.CSharp.WebpayWS;

namespace Webpay.Integration.CSharp.Webservice.Getpaymentplanparams
{
    public class PaymentPlanPricePerMonth
    {
        public List<Dictionary<string, long>> Calculate(decimal amount, GetPaymentPlanParamsEuResponse paymentPlanParams)
        {
            var pricesPerMonth = new List<Dictionary<string, long>>();

            if (paymentPlanParams == null)
            {
                return pricesPerMonth;
            }

            foreach (var campaignCode in paymentPlanParams.CampaignCodes)
            {
                if (!(campaignCode.FromAmount <= amount) || !(amount <= campaignCode.ToAmount))
                {
                    continue;
                }

                var priceMap = new Dictionary<String, long>();
                var numberOfPayments = Math.Max(1, campaignCode.ContractLengthInMonths - campaignCode.NumberOfPaymentFreeMonths);

                var paymentFactor = CalculatePaymentFactor(numberOfPayments, (double)campaignCode.InterestRatePercent / 100);
                double pricePerMonth;

                switch(campaignCode.PaymentPlanType)
                {
                    case PaymentPlanTypeCode.InterestAndAmortizationFree:
                        pricePerMonth = Math.Round((double)campaignCode.InitialFee + (double)amount + (double)campaignCode.NotificationFee);
                        break;

                    case PaymentPlanTypeCode.InterestFree:
                        pricePerMonth = Math.Round(((double)campaignCode.InitialFee + (double)amount + ((double)campaignCode.NotificationFee * numberOfPayments)) / numberOfPayments);
                        break;

                    case PaymentPlanTypeCode.Standard:
                        pricePerMonth = Math.Round(((double)campaignCode.InitialFee + ((double)amount * paymentFactor + (double)campaignCode.NotificationFee) * numberOfPayments) / numberOfPayments);
                        break;

                    default:
                        throw new SveaWebPayException("Invalid PaymentPlanTypeCode");
                }
                
                priceMap.Add("campaignCode", campaignCode.CampaignCode);
                priceMap.Add("pricePerMonth", (long)pricePerMonth);
                pricesPerMonth.Add(priceMap);
            }

            return pricesPerMonth;
        }

        private double CalculatePaymentFactor(int numberOfPayments, double yearlyInterestRate, int paymentFrequencyPerYear = 12)
        {
            double monthlyInterestRate = yearlyInterestRate / paymentFrequencyPerYear;
            return monthlyInterestRate / (1 - Math.Pow(1 + monthlyInterestRate, -numberOfPayments));
        }
    }
}