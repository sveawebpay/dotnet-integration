using System;
using System.Collections.Generic;
using Webpay.Integration.CSharp.Order;
using Webpay.Integration.CSharp.Order.Row;

namespace Webpay.Integration.CSharp.Hosted.Helper
{
    public class HostedRowFormatter<T>
    {
        private long _totalAmount;
        private long _totalVat;
        private readonly List<HostedOrderRowBuilder> _newRows;

        public HostedRowFormatter()
        {
            _totalAmount = 0L;
            _totalVat = 0L;
            _newRows = new List<HostedOrderRowBuilder>();
        }

        public List<HostedOrderRowBuilder> FormatRows(OrderBuilder<T> order)
        {
            FormatOrderRows(order);
            FormatShippingFeeRows(order);
            FormatFixedDiscountRows(order);
            FormatRelativeDiscountRows(order);

            return _newRows;
        }

        private void FormatOrderRows(OrderBuilder<T> orderBuilder) 
        {
            foreach(OrderRowBuilder row in orderBuilder.GetOrderRows()) {
                var tempRow = new HostedOrderRowBuilder();

                decimal vatFactor = row.GetVatPercent() != null ? (row.GetVatPercent().GetValueOrDefault() * new decimal(0.01)) + 1 : 0;
            
                if (row.GetName() != null) {
                    tempRow.SetName(row.GetName());
                }
            
                if (row.GetDescription() != null) {
                    tempRow.SetDescription(row.GetDescription());
                }

                decimal amountExVat = row.GetAmountExVat().GetValueOrDefault();
                tempRow.SetAmount(Convert.ToInt64((amountExVat * 100) * vatFactor));

                if (row.GetAmountExVat() != null && row.GetVatPercent() != null) {
                    tempRow.SetAmount(Convert.ToInt64((row.GetAmountExVat() *100) * vatFactor));
                    tempRow.SetVat(Convert.ToInt64(tempRow.GetAmount() - (row.GetAmountExVat() * 100)));
                }
                else if(row.GetAmountIncVat() != null && row.GetVatPercent() != null) {
                    tempRow.SetAmount(Convert.ToInt64((row.GetAmountIncVat() * 100)));
                    tempRow.SetVat(Convert.ToInt64(tempRow.GetAmount() - (tempRow.GetAmount() / vatFactor)));
                }
                else {
                    tempRow.SetAmount(Convert.ToInt64(row.GetAmountIncVat() * 100));
                    tempRow.SetVat(Convert.ToInt64((row.GetAmountIncVat() - row.GetAmountExVat()) * 100));
                }
                
                        
                if (tempRow.GetQuantity() >= 0) {
                    tempRow.SetQuantity(row.GetQuantity());
                }
            
                if (row.GetUnit() != null) {
                    tempRow.SetUnit(row.GetUnit());
                }
            
                if (null != row.GetArticleNumber()) {
                    tempRow.SetSku(row.GetArticleNumber());
                }
            
                _newRows.Add(tempRow);
                _totalAmount += Convert.ToInt64(tempRow.GetAmount() * row.GetQuantity());
                _totalVat += tempRow.GetVat() * row.GetQuantity();
            }
        }

        private void FormatShippingFeeRows(OrderBuilder<T> orderBuilder) 
        {
            if (orderBuilder.GetShippingFeeRows() == null) {
                return;
            }
        
            foreach(ShippingFeeBuilder row in orderBuilder.GetShippingFeeRows()) {
                var tempRow = new HostedOrderRowBuilder();
                decimal plusVatCounter = row.GetVatPercent() != null ? (row.GetVatPercent().GetValueOrDefault() * new decimal(0.01)) + 1 : 0;   
            
                if (row.GetName() != null) 
                {
                    tempRow.SetName(row.GetName());
                }            
                if (row.GetDescription() != null) 
                {
                    tempRow.SetDescription(row.GetDescription());
                }
                   
                if (row.GetAmountExVat() != null && row.GetVatPercent() != null) {
                    tempRow.SetAmount(Convert.ToInt64((row.GetAmountExVat().GetValueOrDefault() * 100) * plusVatCounter));
                    tempRow.SetVat(Convert.ToInt64(tempRow.GetAmount() - (row.GetAmountExVat().GetValueOrDefault() * 100)));
                } else if (row.GetAmountIncVat() != null && row.GetVatPercent() != null ) {
                    tempRow.SetAmount(Convert.ToInt64(row.GetAmountIncVat().GetValueOrDefault() * 100));
                    tempRow.SetVat(Convert.ToInt64(tempRow.GetAmount() - (tempRow.GetAmount() / plusVatCounter)));
                } else {
                    decimal amountIncVat = row.GetAmountIncVat().GetValueOrDefault();
                    tempRow.SetAmount(Convert.ToInt64(amountIncVat * 100));
                    decimal amountExVat = row.GetAmountExVat().GetValueOrDefault(); 
                    tempRow.SetVat(Convert.ToInt64(amountIncVat - amountExVat));
                }
     
                tempRow.SetQuantity(1);            
                if (row.GetUnit() != null) {
                    tempRow.SetUnit(row.GetUnit());
                }            
                if (row.GetShippingId() != null) {
                    tempRow.SetSku(row.GetShippingId());
                }            
                _newRows.Add(tempRow);
            }
        }

        private void FormatFixedDiscountRows(OrderBuilder<T> orderBuilder) {
            if (orderBuilder.GetFixedDiscountRows() == null) {
                return;
            }
        
            foreach(FixedDiscountBuilder row in orderBuilder.GetFixedDiscountRows()) 
            {
                var tempRow = new HostedOrderRowBuilder();
            
                if (row.GetName() != null) 
                {
                    tempRow.SetName(row.GetName());
                }
            
                if (row.GetDescription() != null) 
                {
                    tempRow.SetDescription(row.GetDescription());
                }
                              
                tempRow.SetAmount(Convert.ToInt64(-(row.GetAmount() * 100)));
            
                _totalAmount -= Convert.ToInt64(row.GetAmount());

                double discountFactor = tempRow.GetAmount() * 1.0 / _totalAmount;
            
                if (_totalVat > 0) 
                {
                    tempRow.SetVat(Convert.ToInt64(_totalVat * discountFactor));
                }
            
                tempRow.SetQuantity(1);
            
                if (row.GetUnit() != null) 
                {
                    tempRow.SetUnit(row.GetUnit());
                }
            
                if (row.GetDiscountId() != null) 
                {
                    tempRow.SetSku(row.GetDiscountId());
                }
            
                _newRows.Add(tempRow);
            }
        }

        private void FormatRelativeDiscountRows(OrderBuilder<T> orderBuilder) 
        {
            if (orderBuilder.GetRelativeDiscountRows() == null) 
            {
                return;
            }
        
            foreach(RelativeDiscountBuilder row in orderBuilder.GetRelativeDiscountRows()) 
            {
                var tempRow = new HostedOrderRowBuilder();
                double discountFactor = row.GetDiscountPercent() * 0.01;
            
                if (row.GetName() != null) 
                {
                    tempRow.SetName(row.GetName());
                }
            
                if (row.GetDescription() != null) 
                {
                    tempRow.SetDescription(row.GetDescription());
                }
            
                if (row.GetDiscountId() != null) 
                {
                    tempRow.SetSku(row.GetDiscountId());
                }
            
                tempRow.SetQuantity(1);
            
                if (row.GetUnit() != null) 
                {
                    tempRow.SetUnit(row.GetUnit());
                }
            
                tempRow.SetAmount(Convert.ToInt64(-(discountFactor * _totalAmount)));
                _totalAmount -= tempRow.GetAmount();
            
                 if(_totalVat > 0) 
                 {
                     tempRow.SetVat(Convert.ToInt64(-(_totalVat * discountFactor)));
                 }
            
                _newRows.Add(tempRow);
            }
        }
    
        public long FormatTotalAmount(IEnumerable<HostedOrderRowBuilder> rows) 
        {
            long amount = 0L;        
            foreach(HostedOrderRowBuilder row in rows) 
            {
                amount += row.GetAmount() * row.GetQuantity();
            }        
            return amount;
        }
    
        public long FormatTotalVat(IEnumerable<HostedOrderRowBuilder> rows) 
        {
            long vat = 0L;        
            foreach(HostedOrderRowBuilder row in rows) 
            {
                vat += row.GetVat() * row.GetQuantity();
            }        
            return vat;
        }
    }
}
