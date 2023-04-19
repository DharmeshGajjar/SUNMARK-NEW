using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class StraightingMasterModel
    {
        public int StrVou { get; set; }
        public int StrCmpVou { get; set; }
        public string Vno { get; set; }
        public string Date { get; set; }
        public string Shift { get; set; }
        public string MachineNo { get; set; }
        public string SupEmpVou { get; set; }
        public string ManEmpVou { get; set; }
        public string IssuePrdVou { get; set; }
        public string Finish { get; set; }
        public string FinishVou { get; set; }
        public string NextProc { get; set; }
        public string NextPrcVou { get; set; }
        public string HFQty { get; set; }
        public string NitricQty { get; set; }
        public string LimeQty { get; set; }
        public string Remarks { get; set; }
        public StrGridModel Straighting { get; set; }
    }
    public class StrGridModel
    {
        public long StrAVou { get; set; }
        public long StrAStrVou { get; set; }
        public string[] Grade { get; set; }
        public int[] GradeVou { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public decimal[] StrThick { get; set; }
        public string StrThickStr { get; set; }
        public decimal[] StrOD { get; set; }
        public string StrODStr { get; set; }
        public decimal[] StrLength { get; set; }
        public string StrLengthStr { get; set; }
        public decimal[] StrNoOfPipe { get; set; }
        public string StrNoOfPipeStr { get; set; }
        public decimal[] StrWeight { get; set; }
        public string StrWeightStr { get; set; }
        public int[] RecPrdVou { get; set; }
        public List<SelectListItem> RecProductList { get; set; }
        public string[] RecProduct { get; set; }
        public string[] StrInTime { get; set; }
        public string StrInTimeStr { get; set; }
        public string[] StrOutTime { get; set; }
        public string StrOutTimeStr { get; set; }
        public string[] StrCoilNo { get; set; }
        public string StrCoilNoStr { get; set; }
        public string[] StrType { get; set; }
        public string StrTypeStr { get; set; }
        public int[] StatusVou { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public string[] StrStatus { get; set; }

    }
}
