using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class LocationMasterModel
    {
        public int LocVou { get; set; }
        public int LocGdnVou { get; set; }
        public List<SelectListItem> GodownList { get; set; }
        public string GdnName { get; set; }
        public string LocName { get; set; }
        public string LocDesc { get; set; }
        public int LocActYN { get; set; }
        public List<SelectListItem> ActiveList { get; set; }
        public string ActiveYN { get; set; }

    }
}
