
using System.Collections.Generic;

namespace SerialPortExample
{
    public class BatchTotal
    {
        public List<string> FiftyCents { get; set; } //we get quantity from the machine and we need to convert it into something readable for humans
        public decimal FiftyCentsTotal { get; set; }
        public int FiftyCentsQuantity { get; set; }
        public List<string> TwentyCents { get; set; }
        public decimal TwentyCentsTotal { get; set; }
        public int TwentyCentsQuantity { get; set; }
        public List<string> TwoPounds { get; set; }
        public decimal TwoPoundsTotal { get; set; }
        public int TwoPoundsQuantity { get; set; }
        public List<string> TwoCents { get; set; }
        public decimal TwoCentsTotal { get; set; }

        public int TwoCentsQuantity { get; set; }
        public List<string> TenCents { get; set; }
        public decimal TenCentsTotal { get; set; }
        public int TenCentsQuantity { get; set; }
        public List<string> OnePounds  { get; set; }
        public decimal OnePoundsTotal { get; set; }
        public int OnePoundsQuantity { get; set; }
        public List<string> OneCents { get; set; }
        public decimal OneCentsTotal { get; set; }
        public int OneCentsQuantity { get; set; }
        public List<string> FiveCents { get; set; }
        public decimal FiveCentsTotal { get; set; }
        public int FiveCentsQuantity { get; set; }
        public List<string> QuantityTotal { get; set; }  //quantity total needs to always equal sum of all denom quantities
        public int Quantity { get; set; }
        public decimal SumTotal { get; set; }  //sum total  = fiftycentstotaL + TWENTYCENTS TOTAL + twopoundstotal etc.....  how much money was inserted to the machine

    }
}
