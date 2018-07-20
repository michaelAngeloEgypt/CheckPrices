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
        public string idx { get; set; }
        [Description("Part Code")]
        public string PartCode { get; set; }
        [Description("HGM Price")]
        public decimal HGMPrice { get; set; }
        [Description("Trupar Price")]
        public decimal TruparPrice { get; set; }
        [Description("Diff.")]
        public decimal Difference { get; set; }
        [Description("Diff. %")]
        public decimal DifferencePCT { get; set; }
    }
}
