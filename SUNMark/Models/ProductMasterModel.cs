using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class ProductMasterModel
    {
        public int PrdVou { get; set; }
        public int PrdTypVou { get; set; }
        public List<SelectListItem> TypeList { get; set; }
        public String PrdType { get; set; }
        public string PrdNm { get; set; }
        public string PrdCd { get; set; }
        public int PrdMscUntVou { get; set; }
        public List<SelectListItem> UnitList { get; set; }
        public string PrdUnit { get; set; }
        public string PrdDesc { get; set; }
    }
}
