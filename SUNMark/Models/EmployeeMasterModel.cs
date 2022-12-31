using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class EmployeeMasterModel
    {
        public int EmpVou { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public int EmpShtId { get; set; }
        public List<SelectListItem> EmpShiftList { get; set; }
        public string EmpShift { get; set; }
        public string EmpMobile { get; set; }
        public int EmpDsgMscId { get; set; }
        public List<SelectListItem> DesignationList { get; set; }
        public string EmpDsgNm { get; set; }
        public int EmpDepMscId { get; set; }
        public List<SelectListItem> DepartmentMscList { get; set; }
        public string EmpDepNm { get; set; }
        public int EmpActId { get; set; }
        public List<SelectListItem> EmpActiveList { get; set; }
        public string ActiveYN { get; set; }
        public string EmpEmail { get; set; }
        public string ProfilePicture { get; set; }
        public IFormFile profilePhoto { get; set; }
        public string UserID { get; set; }

    }
}
