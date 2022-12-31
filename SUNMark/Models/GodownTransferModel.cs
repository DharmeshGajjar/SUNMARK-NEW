using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class GodownTransferModel
    {
        public int isPrint { get; set; }
        public string Vou { get; set; }

        public string Vno { get; set; }

        public string CmpVou { get; set; }

        public string ChallanNo { get; set; }

        public string Date { get; set; }

        public string FromGodown { get; set; }

        public string ToGodown { get; set; }

        public string ProductType { get; set; }

        public string ProductTypeString { get; set; }

        public string Remarks { get; set; }

        public string Location { get; set; }

        public string VehicleNo { get; set; }

        public string ModeOfTransport { get; set; }

        public string Purpose { get; set; }

        public string[] CoilNo { get; set; }

        public string[] Grade { get; set; }

        public string[] Thick { get; set; }

        public string[] OD { get; set; }

        public string[] NB { get; set; }

        public string[] SCH { get; set; }

        public string[] FeetPer { get; set; }

        public string[] PCS { get; set; }

        public string[] ProcessDone { get; set; }

        public string[] Width { get; set; }

        public string[] Qty { get; set; }

        public string[] GridRemarks { get; set; }

        public string[] Product { get; set; }
    }

    public class GdnTrnPrintDetails
    {
        public string Id { get; set; }
        public string Html { get; set; }

    }
}