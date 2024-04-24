using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class ProcessTransferModel
    {
        public int PrcVou { get; set; }
        public int PrcCmpCdn { get; set; }
        public string PrcVNo { get; set; }
        public string Date { get; set; }
        public int PrcCurPrcVou { get; set; }
        public int PrcPrdVou { get; set; }
        public int PrcGrdMscVou { get; set; }
        public int PrcFinMscVou { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public List<SelectListItem> UserList { get; set; }
        public string PrcNB { get; set; }
        public string PrcSCH { get; set; }
        public decimal PrcOD { get; set; }
        public decimal PrcThick { get; set; }
        public decimal PrcLangth { get; set; }
        public decimal PrcQty { get; set; }
        public decimal PrcPCS { get; set; }
        public int PrcNtxPrcVou { get; set; }
        public int PrcUsr { get; set; }
        public string PrcUsrDt { get; set; }
    }
}
