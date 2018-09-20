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
        [Description("LiftParts Part Code")]
        public string LiftPartsPartCode { get; set; }
        public decimal HGMPrice { get; set; }
        public decimal TruparPrice { get; set; }
        public decimal LiftPartsPrice { get; set; }
        public decimal DifferenceTrupar { get; set; }
        public decimal DifferenceTruparPCT { get; set; }
        public decimal DifferenceLiftParts { get; set; }
        public decimal DifferenceLiftPartsPCT { get; set; }
      
        //
        [Description("Part Code")]
        public string PartCode { get { if (!string.IsNullOrEmpty(TruparPartCode)) return TruparPartCode; else return HGMPartCode; } }
        [Description("HGM Price")]
        public string HGMPriceStr { get { if (HGMPrice == 0 || HGMPrice == -1) return "no match"; else return HGMPrice.ToString(); } }
        [Description("Trupar Price")]
        public string TruparPriceStr { get { if (TruparPrice == 0 || TruparPrice == -1) return "no match"; else return TruparPrice.ToString(); } }
        [Description("LiftParts Price")]
        public string LiftPartsPriceStr { get { if (LiftPartsPrice == 0 || LiftPartsPrice == -1) return "no match"; else return LiftPartsPrice.ToString(); } }

        [Description("Diff. Trupar")]
        public string DifferenceTruparStr { get { if (TruparPrice == 0 || TruparPrice == -1) return string.Empty; else return DifferenceTrupar.ToString(); } }
        [Description("Diff. Trupar %")]
        public string DifferenceTruparPCTStr { get { if (TruparPrice == 0 || TruparPrice == -1) return string.Empty; else return DifferenceTruparPCT.ToString(); } }

        [Description("Diff. LiftParts")]
        public string DifferenceLiftPartsStr { get { if (LiftPartsPrice == 0 || LiftPartsPrice == -1) return string.Empty; else return DifferenceLiftParts.ToString(); } }
        [Description("Diff. LiftParts %")]
        public string DifferenceLiftPartsPCTStr { get { if (LiftPartsPrice == 0 || LiftPartsPrice == -1) return string.Empty; else return DifferenceLiftPartsPCT.ToString(); } }
    }
}
