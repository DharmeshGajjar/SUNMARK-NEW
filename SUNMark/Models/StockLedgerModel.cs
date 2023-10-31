using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class StockLedgerModel
    {
        public int CompanyVou { get; set; }
        public List<SelectListItem> CompanyList { get; set; }
        public string Company { get; set; }

        public int TrnTypeVou { get; set; }
        public List<SelectListItem> TrnTypeList { get; set; }
        public string TrnType { get; set; }
        public string TrnTypeName { get; set; }

        public int RecIssVou { get; set; }
        public List<SelectListItem> RecIssList { get; set; }
        public string RecIss { get; set; }

        public int PrdVou { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public string Product { get; set; }

        public string VNo { get; set; }
        public string Thick { get; set; }
        public string Width { get; set; }
        public string Qty { get; set; }
        public string OD { get; set; }
        public List<SelectListItem> NBList { get; set; }
        public string NB { get; set; }
        public string Sch { get; set; }
        public string FrDt { get; set; }
        public string ToDt { get; set; }

        public int StockYNVou { get; set; }
        public List<SelectListItem> StockYNList { get; set; }
        public string StockYN { get; set; }
        public string Dt { get; set; }
        public int ShiftVou { get; set; }
        public List<SelectListItem> ShiftList { get; set; }
        public string Shift { get; set; }
        public int MachineVou { get; set; }
        public List<SelectListItem> MachineList { get; set; }
        public int GradeVou { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public int FinishVou { get; set; }
        public List<SelectListItem> FinishList { get; set; }
        public string Machine { get; set; }
    }
    public class MillingPrintDetails
    {
        public string Id { get; set; }
        public string Html { get; set; }

    }

}
