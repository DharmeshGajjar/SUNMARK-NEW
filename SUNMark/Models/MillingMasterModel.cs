using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class MillingMasterModel
    {
        public string Vno { get; set; }
        public int Vou { get; set; }
        public string CompanyVou { get; set; }

        public string SizeFromTo { get; set; }
        public string Date { get; set; }
        public string ShiftVou { get; set; }
        public string MillNo { get; set; }
        public string Operator1 { get; set; }
        public string Operator2 { get; set; }
        public string Supervisor { get; set; }
        public string ProductCode { get; set; }
        public string IssueCoilNo { get; set; }
        public string IssueCoilNoVou { get; set; }
        public string OD { get; set; }
        public string NB { get; set; }
        public string NBVou { get; set; }
        public string SCH { get; set; }
        public string SCHVou { get; set; }
        public string Grade { get; set; }
        public string Thick { get; set; }
        public string Width { get; set; }
        public string PrcVou { get; set; }
        public string FeetPer { get; set; }
        public string PCS { get; set; }
        public string Weight { get; set; }
        public string PCSWeight { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string RecPrdVou { get; set; }
        public string ScrapLength { get; set; }
        public string ScrapWeight { get; set; }
        public string ScrapPipeProductVou { get; set; }
        public string RLPrdVou { get; set; }
        public string RLPCS { get; set; }
        public string RLWeight { get; set; }
        public string ProcessVou { get; set; }
        public string NoOfTourch { get; set; }
        public string NoOfTourch2 { get; set; }
        public decimal WeldingSpeed { get; set; }
        public string AMP { get; set; }
        public string Voltage { get; set; }
        public string AMP2 { get; set; }
        public string Voltage2 { get; set; }
        public string Remarks { get; set; }
        public string Read1Thick { get; set; }
        public string Read1OD { get; set; }
        public string Read2OD { get; set; }
        public string Read2Thick { get; set; }
        public string FinishDate { get; set; }
        public string Reason { get; set; }
        public string RemainingWeight { get; set; }
        public string StopFromTime1 { get; set; }
        public string StopToTime1 { get; set; }
        public string StopReason1 { get; set; }
        public string StopFromTime2 { get; set; }
        public string StopToTime2 { get; set; }
        public string StopReason2 { get; set; }
        public string MilMaxOD { get; set; }
        public string MilMinOD { get; set; }
        public string BearingLoss { get; set; }


        //Filter Option Field
        public string CoilNo { get; set; }
        public decimal FltOD { get; set; }
        public decimal FltFeetPer { get; set; }
        public string FltNB { get; set; }
        public List<SelectListItem> NBList { get; set; }
        public string FltSCH { get; set; }
        public List<SelectListItem> SCHList { get; set; }
        public long FltGrade { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public string FrDt { get; set; }
        public string ToDt { get; set; }
        public string FltVno { get; set; }


    }
}
