using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class PackingMasterModel
    {
        public int Vou { get; set; }

        public int CompanyVou { get; set; }

        public string Type { get; set; }
        public string SearchType { get; set; }

        public string BundleNo { get; set; }
        public string SearcBundleNo { get; set; }

        public string SrNo { get; set; }

        public string WONumber { get; set; }
        public string SearcWONumber { get; set; }

        public string PONumber { get; set; }
        public string SearcPONumber { get; set; }

        public string ProductVou { get; set; }

        public string ProductCode { get; set; }

        public string SpecificationVou { get; set; }

        public string FinishVou { get; set; }

        public string Finish { get; set; }

        public string GradeVou { get; set; }

        public string Grade { get; set; }

        public string Width { get; set; }

        public string Thick { get; set; }

        public string OD { get; set; }

        public string HeatNumber { get; set; }

        public string Qty { get; set; }
        public string NoOfCopy { get; set; }

        public string FeetInPCS { get; set; }

        public string QtyInFeet { get; set; }

        public string QtyInMeter { get; set; }

        public string QtyInKg { get; set; }

        public string PackerName { get; set; }

        public string QualityCheckerName { get; set; }

        public string SubBundleNumber { get; set; }

        public string PackingDate { get; set; }

        public string OutDate { get; set; }

        public string OutBy { get; set; }
        public string NB { get; set; }
        public string SCH { get; set; }
        public string NbSCHBind { get; set; }

        public bool IsUpdate { get; set; } = false;
        public string FrRecDt { get; set; }
        public string ToRecDt { get; set; }


    }
}
