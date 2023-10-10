using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class JobWorkEntryModel
    {

        public int isPrint { get; set; }
        public int JobId { get; set; } = 0;
        
        public string JobType { get; set; }
        public string JobTypeN { get; set; }

        public string VNo { get; set; }

        public string Date { get; set; }

        public string JobShift { get; set; }
        public string JobShiftN { get; set; }
        public string JobMacNo { get; set; }
        public string JobMacNoN { get; set; }
        public string JobOprMscVou { get; set; }
        public string JobOprMscVouN { get; set; }
        public string RecDate { get; set; }

        public string IssueCoilNo { get; set; }

        public string Grade { get; set; }
        
        public string Finish { get; set; }
        
        public string Thick { get; set; }
        
        public string CompanyName { get; set; }
        
        public string CompanyCode { get; set; }
        
        public string ProductName { get; set; }
        
        public string JobWorker { get; set; }

        public string GodownVou { get; set; }

        public string IssueCoilNoId { get; set; }

        public string Qty { get; set; }

        public string Width { get; set; }

        public string Remarks { get; set; }

        public string ActualQty { get; set; }

        public string ActualWidth { get; set; }

        public string CompleteDate { get; set; }

        public string JobNo { get; set; }
        public int LotTypeVou { get; set; }

        public List<JobWorkEntrySplit> JobWorkEntrySplitsList { get; set; }

        public List<JobWorkEntrySlit> JobWorkEntrySlitsList { get; set; }
        public string FrFinDt { get; set; }
        public string ToFinDt { get; set; }
    }

    public class JobWorkEntrySplit
    {
        public string FormulaWt { get; set; }

        public string VendorWt { get; set; } = "0";

        public string CoilNo { get; set; }

        public string Remarks { get; set; }

        public string RecCoilNo { get; set; }

        public string Godown { get; set; }
    }

    public class JobWorkEntrySlit
    {
        public string Width { get; set; }

        public string NoOfCoil { get; set; }

        public string FormulaWt { get; set; }

        public string ActualWt { get; set; } = "0";
            
        public string CoilNo { get; set; }

        public string Remarks { get; set; }

        public string Type { get; set; }

        public string RecCoilNo { get; set; }

        public string NB { get; set; }

        public string SCH{ get; set; }

        public string Godown { get; set; }

        public string OD { get; set; }

        public string DelChk { get; set; }


    }
    public class JobWorkPrintDetails
    {
        public string Id { get; set; }
        public string Html { get; set; }

    }
}
