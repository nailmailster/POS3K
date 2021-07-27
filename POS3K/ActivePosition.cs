using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace POS3K
{
    public enum ActivePositionModes
    {
        codeReady = 0,
        codeInput,
        codeDone
    };

    public enum ActivePositionTypes
    {
        barcodeType,
        quantityType,
        loyaltyType,
        couponType,
        voucherType,
        cashType,
        cardType
    };

    public class RegimeChangedEventArgs : EventArgs
    {
        public readonly ActivePositionModes LastRegime, NewRegime;
        public RegimeChangedEventArgs(ActivePositionModes lastRegime, ActivePositionModes newRegime)
        {
            LastRegime = lastRegime;
            NewRegime = newRegime;
        }
    }

    public class ActivePosition : INotifyPropertyChanged
    {
        public readonly ActivePositionTypes barcodeInput = ActivePositionTypes.barcodeType;
        public readonly ActivePositionTypes couponInput = ActivePositionTypes.couponType;
        public readonly ActivePositionTypes voucherInput = ActivePositionTypes.voucherType;

        public event EventHandler<RegimeChangedEventArgs> RegimeChanged;
        protected virtual void OnRegimeChanged(RegimeChangedEventArgs e)
        {
            if (RegimeChanged != null)
                RegimeChanged(this, e);
        }
        private ActivePositionModes regime;
        public ActivePositionModes Regime
        {
            get { return regime; }
            set
            {
                if (regime == value)
                    return;
                OnRegimeChanged(new RegimeChangedEventArgs(regime, value));
                regime = value;
            }
        }

        private ActivePositionTypes inputType;
        public ActivePositionTypes InputType
        {
            get { return inputType; }
            set
            {
                if (inputType == value)
                    return;
                inputType = value;
            }
        }

        private int index;
        public int Index { get; set; }

        private string barcode;
        public string Barcode { get; set; }

        private string description;
        public string Description { get; set; }

        private string descriptionLat;
        public string DescriptionLat { get; set; }

        private string info = "";
        public string Info
        {
            get { return info; }
            set
            {
                if (info != value)
                {
                    info = value;
                    NotifyPropertyChanged("Info");
                }
            }
        }

        private int quantity;
        public int Quantity { get; set; }

        private double price;
        public double Price { get; set; }

        private double sum;
        public double Sum { get; set; }

        private double percentDiscount;
        public double PercentDiscount { get; set; }

        private double discount;
        public double Discount { get; set; }

        public bool isWeight;
        public bool IsWeight { get; set; }

        public int vATRate;
        public int VATRate { get; set; }

        public double vATSum;
        public double VATSum { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (propertyName == "Info")
            {
                //PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Clear()
        {
            Regime = ActivePositionModes.codeReady;
            InputType = ActivePositionTypes.barcodeType;
            Barcode = "";
            //Info = "";
            Quantity = 0;
            Price = 0;
            Sum = 0;
            PercentDiscount = 0;
            Discount = 0;
            IsWeight = false;
            VATRate = 0;
            VATSum = 0;
        }

        public void Calculate(ActiveCheck activeCheck)
        {
            Sum = Price * Quantity;
            activeCheck.TotalSum += Sum;
            activeCheck.TotalSum = Math.Round(activeCheck.TotalSum, 2);
            if (VATRate > 0)
            {
                VATSum = Math.Round(Sum / (100 + VATRate) * VATRate, 2);
                if (VATRate == 7)
                    activeCheck.VAT7 += Math.Round(activeCheck.VAT7 + VATSum, 2);
                else if (VATRate == 10)
                    activeCheck.VAT10 += Math.Round(activeCheck.VAT10 + VATSum, 2);
                else if (VATRate == 15)
                    activeCheck.VAT15 = Math.Round(activeCheck.VAT15 + VATSum, 2);
                else if (VATRate == 20)
                    activeCheck.VAT20 += Math.Round(activeCheck.VAT20 + VATSum, 2);
            }

            //activeCheck.DiscountSum += Discount;
            //activeCheck.TopaySum = activeCheck.TotalSum - activeCheck.DiscountSum;
        }
    }
}
