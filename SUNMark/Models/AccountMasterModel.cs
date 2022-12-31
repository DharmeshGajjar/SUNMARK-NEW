using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class AccountMasterModel
    {
        public int AccVou { get; set; }
        public string AccCd { get; set; }
        public string AccName { get; set; }
        public int AccType { get; set; }
        public List<SelectListItem> TypeList { get; set; }
        public string TypeNm { get; set; }
        public string AccAdd { get; set; }
        public int AccCtyVou { get; set; }
        public List<CustomDropDown> CityList { get; set; }
        public string AccCity { get; set; }
        public string AccState { get; set; }
        public string AccCountry { get; set; }
        public string AccGST { get; set; }
        public string AccPAN { get; set; }
        public string AccPhone { get; set; }
        public string AccMob { get; set; }
        public string AccEmail { get; set; }
        public string AccOth1 { get; set; }
        public string AccOth2 { get; set; }
        public string AccOth3 { get; set; }
        public string AccOth4 { get; set; }
        public string AccOth5 { get; set; }
        public string AccConPer1 { get; set; }
        public string AccConPerMob1 { get; set; }
        public string AccConPerEmail1 { get; set; }
        public string AccConPer2 { get; set; }
        public string AccConPerMob2 { get; set; }
        public string AccConPerEmail2 { get; set; }
        public string AccConPer3 { get; set; }
        public string AccConPerMob3 { get; set; }
        public string AccConPerEmail3 { get; set; }
        public string AccConPer4 { get; set; }
        public string AccConPerMob4 { get; set; }
        public string AccConPerEmail4 { get; set; }

    }
}
