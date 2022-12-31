using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class CompanyMasterModel
    {
        public int CmpVou { get; set; }

        public string CmpCode { get; set; }

        public string CmpName { get; set; }

        public string CmpAdd { get; set; }

        public int ClientId { get; set; }

        public string StartDate { get; set; }

        public   List<SelectListItem> GetClientlist { get; set; }
    }
}
