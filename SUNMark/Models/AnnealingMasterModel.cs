using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class AnnealingMasterModel
    {
        public int AnnVou { get; set; }
        public int AnnCmpVou { get; set; }
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
        public string LDOQty { get; set; }
        public string Remarks { get; set; }
        public AnnelGridModel Annel { get; set; }

    }
    public class AnnelGridModel
    {
        public long AnnAVou { get; set; }
        public long AnnAAnnVou { get; set; }
        public string[] Grade { get; set; }
        public int[] GradeVou { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public decimal[] AnnThick { get; set; }
        public string AnnThickStr { get; set; }
        public decimal[] AnnOD { get; set; }
        public string AnnODStr { get; set; }
        public decimal[] AnnLength { get; set; }
        public string AnnLengthStr { get; set; }
        public decimal[] AnnNoOfPipe { get; set; }
        public string AnnNoOfPipeStr { get; set; }
        public decimal[] AnnWeight { get; set; }
        public string AnnWeightStr { get; set; }
        public int[] RecPrdVou { get; set; }
        public List<SelectListItem> RecProductList { get; set; }
        public string[] RecProduct { get; set; }
        public string[] AnnInTime { get; set; }
        public string AnnInTimeStr { get; set; }
        public string[] AnnOutTime { get; set; }
        public string AnnOutTimeStr { get; set; }
        public string[] AnnCoilNo { get; set; }
        public string AnnCoilNoStr { get; set; }
        public string[] AnnType { get; set; }
        public string AnnTypeStr { get; set; }
        public decimal[] AnnRPM { get; set; }
        public string AnnRPMStr { get; set; }
        public int[] AnnTDS1 { get; set; }
        public string AnnTDS1Str { get; set; }
        public int[] AnnTDS2 { get; set; }
        public string AnnTDS2Str { get; set; }
        public int[] AnnTDS3 { get; set; }
        public string AnnTDS3Str { get; set; }
        public int[] AnnTDS4 { get; set; }
        public string AnnTDS4Str { get; set; }
        public int[] AnnNoPBatch { get; set; }
        public string AnnNoPBatchStr { get; set; }
    }
}
