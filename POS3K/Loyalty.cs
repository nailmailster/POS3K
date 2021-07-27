using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POS3K
{
    public class Loyalty
    {
        public string Code { get; set; }
        public bool Banned { get; set; }
        public bool Activated { get; set; }
        public double Sums { get; set; }
        public int Points { get; set; }
        public double Percent { get; set; }
        public string FIO { get; set; }
        public string FIOLat { get; set; }
        public double ChSums { get; set; }
        public double ChPerc { get; set; }
        public double SumSub { get; set; }
        public double SumAdd { get; set; }
        public double SumNew { get; set; }
    }
}
