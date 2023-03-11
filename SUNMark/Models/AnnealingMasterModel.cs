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
        public decimal[] Thick { get; set; }
        public string ThickStr { get; set; }
        public decimal[] OD { get; set; }
        public string ODStr { get; set; }
        public int[] Length { get; set; }
        public string LengthStr { get; set; }
        public decimal[] NoOfPipe { get; set; }
        public string NoOfPipeStr { get; set; }
        public decimal[] Weight { get; set; }
        public string WeightStr { get; set; }
        public int[] RecPrdVou { get; set; }
        public List<SelectListItem> RecProductList { get; set; }
        public string[] RecProduct { get; set; }
        public string[] InTime { get; set; }
        public string InTimeStr { get; set; }
        public string[] OutTime { get; set; }
        public string OutTimeStr { get; set; }
        public string[] OilLevel { get; set; }
        public string OilLevelStr { get; set; }
        public string[] RPM { get; set; }
        public string RPMStr { get; set; }
    }
}
