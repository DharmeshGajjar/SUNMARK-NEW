using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class InwRegiModel
    {
        public string CoilNo { get; set; }
        public string FrWidth { get; set; }
        public string ToWidth { get; set; }
        public string FrThick { get; set; }
        public string ToThick { get; set; }
        public string FrWeight { get; set; }
        public string ToWeight { get; set; }
        public string OD { get; set; }
        public List<SelectListItem> NBList { get; set; }
        public string NB { get; set; }
        public List<SelectListItem> SCHList { get; set; }
        public string SCH { get; set; }
        public string FeetPer { get; set; }
        public int FInishVou { get; set; }
        public List<SelectListItem> FinishList { get; set; }

        public int GradeVou { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public string Grade { get; set; }

        public string FrRecDt { get; set; }
        public string ToRecDt { get; set; }
        public string FrIssDt { get; set; }
        public string ToIssDt { get; set; }

        public int StockYNVou { get; set; }
        public List<SelectListItem> StockYNList { get; set; }
        public string StockYN { get; set; }

        public int GodownVou { get; set; }
        public List<SelectListItem> GodownList { get; set; }
        public string Godown { get; set; }

        public int CompanyVou { get; set; }
        public List<SelectListItem> CompanyList { get; set; }
        public string Company { get; set; }
        public string DoneProc { get; set; }
        public string NextProc { get; set; }

        public int AccountVou { get; set; }
        public List<CustomDropDown> AccountList { get; set; }
        public string Account { get; set; }
        public int CoilTypeVou { get; set; }
        public List<SelectListItem> CoilTypeList { get; set; }
        public string CoilType { get; set; }
        public string SupCoilNo { get; set; }
    }
    public class InwRegiPrintDetails
    {
        public string ToDt { get; set; }
        public string CmpVou { get; set; }
        public string Html { get; set; }

    }

    public class InwPrintDetails
    {
        public string CoilNo { get; set; }
        public string FrDt { get; set; }
        public string ToDt { get; set; }
        public string CmpVou { get; set; }
        public List<SelectListItem> CompanyList { get; set; }
        public string Company { get; set; }

        public string GrdVou { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public string Grade { get; set; }
        public string FrWidth { get; set; }
        public string ToWidth { get; set; }
        public string FrThick { get; set; }
        public string ToThick { get; set; }
        public string FrQty { get; set; }
        public string ToQty { get; set; }
        public string Html { get; set; }

    }

}
