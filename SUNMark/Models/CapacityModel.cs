using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class CapacityModel
    {
        public long CapVou { get; set; }
        public int CapMacVou { get; set; }
        public List<SelectListItem> MachineList { get; set; }
        public string CapMacSize { get; set; }
        public CapacityGridModel Capacity { get; set; }
    }
    public class CapacityGridModel
    {
        public long CapAVou { get; set; }
        public long CapACapVou { get; set; }
        public int CapASrNo { get; set; }
        public decimal[] CapAThick { get; set; }
        public string CapAThickStr { get; set; }
        public decimal[] CapAOD { get; set; }
        public string CapAODStr { get; set; }
        public decimal[] CapACapacity { get; set; }
        public string CapACapacityStr { get; set; }

    }

}
