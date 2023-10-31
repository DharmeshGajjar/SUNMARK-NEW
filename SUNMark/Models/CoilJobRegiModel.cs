using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class CoilJobRegiModel
    {
        public string FrDt { get; set; }
        public string ToDt { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public string Grade { get; set; }
        public List<SelectListItem> PrTypeList { get; set; }
        public string PrType { get; set; }
    }
}