using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace POS3K
{
    public class TotalSumChangedEventArgs : EventArgs
    {
        public readonly double LastTotalSum, NewTotalSum;
        public TotalSumChangedEventArgs(double lastTotalSum, double newTotalSum)
        {
            LastTotalSum = lastTotalSum;
            NewTotalSum = newTotalSum;
        }
    }

    public class CouponSumChangedEventArgs : EventArgs
    {
        public readonly double LastCouponSum, NewCouponSum;
        public CouponSumChangedEventArgs(double lastCouponSum, double newCouponSum)
        {
            LastCouponSum = lastCouponSum;
            NewCouponSum = newCouponSum;
        }
    }

    public class DiscountSumChangedEventArgs : EventArgs
    {
        public readonly double LastDiscountSum, NewDiscountSum;
        public DiscountSumChangedEventArgs(double lastDiscountSum, double newDiscountSum)
        {
            LastDiscountSum = lastDiscountSum;
            NewDiscountSum = newDiscountSum;
        }
    }

    public class TopaySumChangedEventArgs : EventArgs
    {
        public readonly double LastTopaySum, NewTopaySum;
        public TopaySumChangedEventArgs(double lastTopaySum, double newTopaySum)
        {
            LastTopaySum = lastTopaySum;
            NewTopaySum = newTopaySum;
        }
    }

    public class CashSumChangedEventArgs : EventArgs
    {
        public readonly double LastCashSum, NewCashSum;
        public CashSumChangedEventArgs(double lastCashSum, double newCashSum)
        {
            LastCashSum = lastCashSum;
            NewCashSum = newCashSum;
        }
    }

    public class CardSumChangedEventArgs : EventArgs
    {
        public readonly double LastCardSum, NewCardSum;
        public CardSumChangedEventArgs(double lastCardSum, double newCardSum)
        {
            LastCardSum = lastCardSum;
            NewCardSum = newCardSum;
        }
    }

    public class ChangeSumChangedEventArgs : EventArgs
    {
        public readonly double LastChangeSum, NewChangeSum;
        public ChangeSumChangedEventArgs(double lastChangeSum, double newChangeSum)
        {
            LastChangeSum = lastChangeSum;
            NewChangeSum = newChangeSum;
        }
    }

    public class LoyaltySumChangedEventArgs : EventArgs
    {
        public readonly double LastLoyaltySum, NewLoyaltySum;
        public LoyaltySumChangedEventArgs(double lastLoyaltySum, double newLoyaltySum)
        {
            LastLoyaltySum = lastLoyaltySum;
            NewLoyaltySum = newLoyaltySum;
        }
    }

    public class ActiveCheck : INotifyPropertyChanged
    {
        public List<ActivePosition> specification = new List<ActivePosition>();
        public List<Coupon> coupons = new List<Coupon>();
        public Loyalty loyalty = new Loyalty();

        public bool saleMode, returnMode, cancelMode, paymentMode, couponMode, voucherMode, autonoMode, annulationMode, finished, paymentModeCash, paymentModeCard, preFinished, totalMode;
        public string date, time, number, type;
        public double VAT7, VAT10, VAT15, VAT20;

        public event EventHandler<TotalSumChangedEventArgs> TotalSumChanged;
        protected virtual void OnTotalSumChanged(TotalSumChangedEventArgs e)
        {
            if (TotalSumChanged != null)
                TotalSumChanged(this, e);
        }
        private double totalSum;
        public double TotalSum
        {
            get { return totalSum; }
            set
            {
                if (TotalSum != value)
                {
                    totalSum = value;
                    NotifyPropertyChanged("TotalSum");
                    OnTotalSumChanged(new TotalSumChangedEventArgs(totalSum, value));
                }
            }
        }

        public event EventHandler<CouponSumChangedEventArgs> CouponSumChanged;
        protected virtual void OnCouponSumChanged(CouponSumChangedEventArgs e)
        {
            if (CouponSumChanged != null)
                CouponSumChanged(this, e);
        }
        private double couponSum;
        public double CouponSum
        {
            get { return couponSum; }
            set
            {
                if (CouponSum != value)
                {
                    couponSum = value;
                    NotifyPropertyChanged("CouponSum");
                    OnCouponSumChanged(new CouponSumChangedEventArgs(couponSum, value));
                }
            }
        }

        public event EventHandler<DiscountSumChangedEventArgs> DiscountSumChanged;
        protected virtual void OnDiscountSumChanged(DiscountSumChangedEventArgs e)
        {
            if (DiscountSumChanged != null)
                DiscountSumChanged(this, e);
        }
        private double discountSum;
        public double DiscountSum
        {
            get { return discountSum; }
            set
            {
                if (DiscountSum != value)
                {
                    discountSum = value;
                    NotifyPropertyChanged("DiscountSum");
                    OnDiscountSumChanged(new DiscountSumChangedEventArgs(discountSum, value));
                }
            }
        }

        public event EventHandler<TopaySumChangedEventArgs> TopaySumChanged;
        protected virtual void OnTopaySumChanged(TopaySumChangedEventArgs e)
        {
            if (TopaySumChanged != null)
                TopaySumChanged(this, e);
        }
        private double topaySum;
        public double TopaySum
        {
            get { return topaySum; }
            set
            {
                if (TopaySum != value)
                {
                    topaySum = value;
                    NotifyPropertyChanged("TopaySum");
                    OnTopaySumChanged(new TopaySumChangedEventArgs(topaySum, value));
                }
            }
        }

        public event EventHandler<CashSumChangedEventArgs> CashSumChanged;
        protected virtual void OnCashSumChanged(CashSumChangedEventArgs e)
        {
            if (CashSumChanged != null)
                CashSumChanged(this, e);
        }
        private double cashSum;
        public double CashSum
        {
            get { return cashSum; }
            set
            {
                if (CashSum != value)
                {
                    cashSum = value;
                    NotifyPropertyChanged("CashSum");
                    OnCashSumChanged(new CashSumChangedEventArgs(cashSum, value));
                }
            }
        }

        public event EventHandler<CardSumChangedEventArgs> CardSumChanged;
        protected virtual void OnCardSumChanged(CardSumChangedEventArgs e)
        {
            if (CardSumChanged != null)
                CardSumChanged(this, e);
        }
        private double cardSum;
        public double CardSum
        {
            get { return cardSum; }
            set
            {
                if (CardSum != value)
                {
                    cardSum = value;
                    NotifyPropertyChanged("CardSum");
                    OnCardSumChanged(new CardSumChangedEventArgs(cardSum, value));
                }
            }
        }

        public event EventHandler<ChangeSumChangedEventArgs> ChangeSumChanged;
        protected virtual void OnChangeSumChanged(ChangeSumChangedEventArgs e)
        {
            if (ChangeSumChanged != null)
                ChangeSumChanged(this, e);
        }
        private double changeSum;
        public double ChangeSum
        {
            get { return changeSum; }
            set
            {
                if (ChangeSum != value)
                {
                    changeSum = value;
                    NotifyPropertyChanged("ChangeSum");
                    OnChangeSumChanged(new ChangeSumChangedEventArgs(changeSum, value));
                }
            }
        }

        public event EventHandler<LoyaltySumChangedEventArgs> LoyaltySumChanged;
        protected virtual void OnLoyaltySumChanged(LoyaltySumChangedEventArgs e)
        {
            if (LoyaltySumChanged != null)
                LoyaltySumChanged(this, e);
        }
        private double loyaltySum;
        public double LoyaltySum
        {
            get { return loyaltySum; }
            set
            {
                if (LoyaltySum != value)
                {
                    loyaltySum = value;
                    NotifyPropertyChanged("LoyaltySum");
                    OnLoyaltySumChanged(new LoyaltySumChangedEventArgs(loyaltySum, value));
                }
            }
        }

        //private bool returnMode;
        //public bool ReturnMode
        //{
        //    get { return returnMode; }
        //    set
        //    {
        //        if (returnMode != value)
        //        {
        //            ReturnMode = value;
        //            NotifyPropertyChanged("ReturnMode");
        //        }
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName = "")
        {
            //PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Clear()
        {
            finished = true;
            saleMode = false;
            returnMode = false;
            cancelMode = false;
            paymentMode = false;
            couponMode = false;
            voucherMode = false;
            paymentModeCash = false;
            paymentModeCard = false;
            preFinished = false;
            //  Закомментировал, т.к. это мы делаем по нажатию A либо при отсутствии связи
            //autonoMode = false;
            //  Закомментировал, т.к. это мы делаем в Window2 по нажатию Enter
            //annulationMode = false;

            TotalSum = 0;
            DiscountSum = 0;
            CouponSum = 0;
            TopaySum = 0;
            CashSum = 0;
            CardSum = 0;
            ChangeSum = 0;
            LoyaltySum = 0;

            loyalty.Code = null;

            VAT7 = 0;
            VAT10 = 0;
            VAT15 = 0;
            VAT20 = 0;

            specification.Clear();
            coupons.Clear();
        }

        public void AddToSpecification(ActivePosition activePosition)
        {
            ActivePosition newActivePosition = new ActivePosition {
                Index = specification.Count + 1,
                Barcode = activePosition.Barcode,
                Description = activePosition.Description,
                DescriptionLat = activePosition.DescriptionLat,
                Price = activePosition.Price,
                Quantity = activePosition.Quantity,
                Sum = activePosition.Sum,
                Discount = activePosition.Discount,
                VATRate = activePosition.VATRate,
                VATSum = activePosition.VATSum
            };
            specification.Add(newActivePosition);
        }

        public void RecalculateTotals()
        {
            this.TotalSum = 0;
            this.DiscountSum = 0;
            this.CouponSum = 0;
            this.TopaySum = 0;
            this.CashSum = 0;
            this.CardSum = 0;
            this.ChangeSum = 0;

            foreach (ActivePosition activePosition in this.specification)
            {
                this.TotalSum += activePosition.Sum;
                this.DiscountSum += activePosition.Discount;
            }
            this.TotalSum = Math.Round(this.TotalSum, 2);
            this.DiscountSum = Math.Round(this.DiscountSum, 2);

            this.TopaySum = Math.Round(this.TotalSum - this.DiscountSum, 2);
        }

        //  првоерка купона и добавление в купоны текущего чека
        public bool VerifyCoupon(string stringCoupon, out string resultString)
        {
            Coupon coupon = new Coupon
            {
                SerialField = stringCoupon
            };
            resultString = "OK";
            if (CouponsDBF.GetRecordBySerial(ref coupon))
            {
                //if (coupons.IndexOf(coupon) == -1)
                if (coupons.Find(coupons => coupons.SerialField == coupon.SerialField) == null)
                {
                    if (AddCouponToSpecification(coupon))
                    {
                        coupons.Add(coupon);
                        return true;
                    }
                    else
                    {
                        resultString = "НЕТ УСЛОВИЙ ДЛЯ КУПОНА";
                        return false;
                    }
                }
                else
                {
                    resultString = "КУПОН УЖЕ ПРИМЕНЕН";
                    return false;
                }
            }
            else
                resultString = "КУПОН НЕ НАЙДЕН";
            return false;
        }

        public bool VerifyLoyalty(string stringLoyaltyCode, out string resultString)
        {
            loyalty.Code = stringLoyaltyCode;
            resultString = "OK";
            if (LoyaltyDBF.GetRecordByCode(ref loyalty))
            {
                if (loyalty.Banned)
                {
                    resultString = "КАРТА ЛОЯЛЬНОСТИ ЗАБЛОКИРОВАНА";
                    return false;
                }
                else if (!loyalty.Activated)
                {
                    resultString = "КАРТА ЛОЯЛЬНОСТИ НЕ АКТИВИРОВАНА";
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
                resultString = "КАРТА ЛОЯЛЬНОСТИ НЕ НАЙДЕНА";
            return false;
        }

        public bool AddCouponToSpecification(Coupon coupon)
        {
            bool result = false;
            foreach (ActivePosition activePosition in this.specification)
            {
                if (activePosition.Barcode == coupon.CodeField)
                {
                    if (coupon.DiscountField > activePosition.PercentDiscount)
                    {
                        activePosition.PercentDiscount = coupon.DiscountField;
                        activePosition.Sum = activePosition.Quantity * activePosition.Price;
                        activePosition.Discount = Math.Round(activePosition.Quantity * activePosition.Price * coupon.DiscountField / 100, 2);

                        result = true;
                    }
                }
            }

            return result;
        }

        public void AddCheckToDBF()
        {
            //DateTime dateTime = DateTime.Now;

            //date = dateTime.Date.ToString();
            //date = date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2);

            //time = dateTime.TimeOfDay.ToString();
            //time = time.Substring(0, 2) + time.Substring(3, 2) + time.Substring(6, 2);

            //string latestDn = ChequesDBF.GetLatestDn(date);
            //string latestD = latestDn.Substring(0, 8);
            //string latestN = latestDn.Substring(8);

            //if (date == latestD)
            //{
            //    number = (Convert.ToInt16(latestN) + 1).ToString();
            //    //MessageBox.Show("даты совпадают");
            //}
            //else if (Convert.ToInt64(date) > Convert.ToInt64(latestD))
            //{
            //    number = "1";
            //    //MessageBox.Show("текущая дата больше");
            //}
            //else if (Convert.ToInt64(date) > Convert.ToInt64(latestD))
            //{
            //    MessageBox.Show("текущая дата меньше! Ошибка");
            //    return;
            //}

            if (!ChequesDBF.AddCheck(this))
            {
                MessageBox.Show("Ошибка записи чека..");
                Environment.Exit(1);
            }
        }

        public void PrepareCheck()
        {
            DateTime dateTime = DateTime.Now;

            date = dateTime.Date.ToString();
            date = date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2);

            time = dateTime.TimeOfDay.ToString();
            time = time.Substring(0, 2) + time.Substring(3, 2) + time.Substring(6, 2);

            string latestDn = ChequesDBF.GetLatestDn(date);
            if (latestDn == null)
                latestDn = "000000000000";
            string latestD = latestDn.Substring(0, 8);
            string latestN = latestDn.Substring(8);

            if (date == latestD)
            {
                number = (Convert.ToInt16(latestN) + 1).ToString();
                //MessageBox.Show("даты совпадают");
            }
            else if (Convert.ToInt64(date) > Convert.ToInt64(latestD))
            {
                number = "1";
                //MessageBox.Show("текущая дата больше");
            }
            else if (Convert.ToInt64(date) > Convert.ToInt64(latestD))
            {
                MessageBox.Show("текущая дата меньше! Ошибка");
                return;
            }
        }

        public bool QualifyCancel(ActivePosition activePosition)
        {
            int qty = 0;

            foreach (ActivePosition position in this.specification)
            {
                if (position.Barcode == activePosition.Barcode)
                    qty += position.Quantity;
            }

            if (qty >= activePosition.Quantity)
                return true;
            else
                return false;
        }
    }
}
