using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class MachineMasterModel
    {
        public int MacVou { get; set; }
        public int MacTypeID { get; set; }
        public List<SelectListItem> MachineTypeList { get; set; }
        public String MachineType { get; set; }
        public string MacCd { get; set; }
        public string MacName { get; set; }
        public int MacGdnVou { get; set; }
        public List<SelectListItem> GodownList { get; set; }
        public String GdnName { get; set; }
        public int MacLocVou { get; set; }
        public List<SelectListItem> LocationList { get; set; }
        public String LocName { get; set; }
        public int MacCapHr { get; set; }
        public String MacDesc { get; set; }



        public int GdnVou { get; set; }
        public List<SelectListItem> GodownNameList { get; set; }
        public int MacS1Opr1EmpVou { get; set; }
        public List<CustomDropDown> EmployeeListS1Opr1 { get; set; }
        public int MacS1Opr2EmpVou { get; set; }
        public List<CustomDropDown> EmployeeListS1Opr2 { get; set; }
        public int MacS1SuperEmpVou { get; set; }
        public List<CustomDropDown> EmployeeSuperList1 { get; set; }
        public int MacS2Opr1EmpVou { get; set; }
        public List<CustomDropDown> EmployeeListS2Opr1 { get; set; }
        public int MacS2Opr2EmpVou { get; set; }
        public List<CustomDropDown> EmployeeListS2Opr2 { get; set; }
        public int MacS2SuperEmpVou { get; set; }
        public List<CustomDropDown> EmployeeSuperList2 { get; set; }
        public decimal MacSizeRngFr { get; set; }
        public decimal MacSizeRngTo { get; set; }

    }
}
