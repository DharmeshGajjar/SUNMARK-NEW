using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class PicklingMasterModel
    {
        
        public int PikVou { get; set; }
        public int PikCmpVou { get; set; }
        public string Vno { get; set; }
        public string Date { get; set; }
        public string Shift { get; set; }
        public string MachineNo { get; set; }
        public string SupEmpVou { get; set; }
        public string ManEmpVou { get; set; }
        public string IssuePrdVou { get; set; }
        //public string Finish { get; set; }
        //public string FinishVou { get; set; }
        //public string NextProc { get; set; }
        //public string NextPrcVou { get; set; }
        public string HFQty { get; set; }
        public string NitricQty { get; set; }
        public string LimeQty { get; set; }
        public string Remarks { get; set; }
        public PikGridModel Pikling { get; set; }
        public List<PikGridModel> LstPikling { get; set; }
        public int isPrint { get; set; }

        //Filter Option Field
        public decimal FltOD { get; set; }
        public decimal FltFeetPer { get; set; }
        public string FltNB { get; set; }
        public List<SelectListItem> FltNBList { get; set; }
        public string FltSCH { get; set; }
        public List<SelectListItem> FltSCHList { get; set; }
        public long FltGrade { get; set; }
        public List<SelectListItem> FltGradeList { get; set; }
        public string FrDt { get; set; }
        public string ToDt { get; set; }

        public string FltVno { get; set; }
    }
    public class PikGridModel
    {
        public long PikAVou { get; set; }
        public long PikAPikVou { get; set; }
        public string Grade { get; set; }
        public int GradeVou { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public string NB { get; set; }
        public int[] NbVou { get; set; }
        public List<SelectListItem> NBList { get; set; }
        public string SCH { get; set; }
        public int[] SchVou { get; set; }
        public List<SelectListItem> SCHList { get; set; }
        public decimal PikThick { get; set; }
        public string PikThickStr { get; set; }
        public decimal PikOD { get; set; }
        public string PikODStr { get; set; }
        public decimal PikLength { get; set; }
        public string PikLengthStr { get; set; }
        public decimal PikNoOfPipe { get; set; }
        public string PikNoOfPipeStr { get; set; }
        public decimal PikWeight { get; set; }
        public string PikWeightStr { get; set; }
        public int RecPrdVou { get; set; }
        public List<SelectListItem> RecProductList { get; set; }
        public string RecProduct { get; set; }
        public int StatusVou { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public string PikStatus { get; set; }
        public int FinishVou { get; set; }
        public List<SelectListItem> FinishList { get; set; }
        public string PikFinish { get; set; }
        public int NextProcVou { get; set; }
        public List<SelectListItem> NextProcList { get; set; }
        public string PikNextProc { get; set; }
        public string PikInTime { get; set; }
        public string PikInTimeStr { get; set; }
        public string PikOutTime { get; set; }
        public string PikOutTimeStr { get; set; }
        public string PikCoilNo { get; set; }
        public string PikCoilNoStr { get; set; }
        public string PikType { get; set; }
        public string PikTypeStr { get; set; }
    }
    public class PicklingPrintDetails
    {
        public string Id { get; set; }
        public string Html { get; set; }
        //public string Html { get; set; }

    }
}
