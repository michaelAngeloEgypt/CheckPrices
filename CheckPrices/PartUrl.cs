using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPrices
{
    class PartUrl
    {
        public string idx { get; set; }
        public string hgmURL { get; set; }
        public string TruparURL { get; set; }

        public static List<PartUrl> refs { get; private set; }
        public static void GetRefs(string refPath)
        {
            try
            {
                refs.Clear();
                using (var sr = new StreamReader(refPath))
                {
                    var csv = new CsvHelper.CsvReader(sr);
                    var res = csv.GetRecords<PartUrl>().ToList();
                    if (res != null && res.Count() > 0)
                        res.ForEach(r => refs.Add(r));
                }
            }
            catch (Exception x)
            {
                XLogger.Error(x);
            }
        }

        static PartUrl()
        {
            refs = new List<PartUrl>();
        }
    }
}
