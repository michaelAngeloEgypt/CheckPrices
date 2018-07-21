using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPrices
{
    class ReportEntry
    {
        [Description("Index")]
        public int idx { get; set; }
        public string HGMPartCode { get; set; }
        public string TruparPartCode { get; set; }
        public decimal HGMPrice { get; set; }
        public decimal TruparPrice { get; set; }
        public decimal Difference { get; set; }
        public decimal DifferencePCT { get; set; }
        //
        [Description("Part Code")]
        public string PartCode { get { if (!string.IsNullOrEmpty(TruparPartCode)) return TruparPartCode; else return HGMPartCode; } }
        [Description("HGM Price")]
        public string HGMPriceStr { get { if (HGMPrice == 0 || HGMPrice == -1) return "no match"; else return HGMPrice.ToString(); } }
        [Description("Trupar Price")]
        public string TruparPriceStr { get { if (TruparPrice == 0 || TruparPrice == -1) return "no match"; else return TruparPrice.ToString(); } }
        [Description("Diff.")]
        public string DifferenceStr { get { if (TruparPrice == 0 || TruparPrice == -1) return string.Empty; else return Difference.ToString(); } }
        [Description("Diff. %")]
        public string DifferencePCTStr { get { if (TruparPrice == 0 || TruparPrice == -1) return string.Empty; else return DifferencePCT.ToString(); } }
    }
}
