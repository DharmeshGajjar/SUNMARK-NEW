using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class GodownMasterModel
    {
        public int GdnVou { get; set; }
        public string GdnName { get; set; }
        public string GdnAdd { get; set; }
        public string GdnPhone { get; set; }
        public int GdnAccVou { get; set; }
        public List<SelectListItem> AccountList { get; set; }
        public string AccNm { get; set; }
        public int GdnActYN { get; set; }
        public List<SelectListItem> ActiveList { get; set; }
        public string ActiveYN { get; set; }
    }
}
