using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
	public class JobInwardModel
	{
        public int isPrint { get; set; }
        public int InwVou { get; set; }
        public int InwTypeVou { get; set; }
        public List<SelectListItem> TypeList { get; set; }
        public int InwCmpVou { get; set; }
        public long InwVouNo { get; set; }
        public List<SelectListItem> CompanyList { get; set; }
        public string CmpNm { get; set; }
        public int InwVNo { get; set; }
        public string InwDt { get; set; }
        public string InwRefNo { get; set; }
        public int InwPOMVou { get; set; }
        public List<SelectListItem> PurchaseOrderList { get; set; }
        public string OrmVNo { get; set; }
        public string InwBillNo { get; set; }
        public string InwBillDt { get; set; }
        public int InwAccVou { get; set; }
        public List<CustomDropDown> AccountList { get; set; }
        public string AccName { get; set; }
        public string CtyName { get; set; }
        public string InwVehNo { get; set; }
        public decimal InwFrightRt { get; set; }
        public int InwTrnVou { get; set; }
        public List<CustomDropDown> TransportList { get; set; }
        public string InwTransNm { get; set; }
        public string InwDrvNm { get; set; }
        public string InwDrvMob { get; set; }
        public string InwRem { get; set; }
        public decimal InwBillQty { get; set; }
        public int InwPtyVou { get; set; }
        public List<SelectListItem> ProductTypeList { get; set; }
        public string InwPrdTyp { get; set; }
        public int InwPrdVou { get; set; }
        public List<SelectListItem> MainProductList { get; set; }
        public string InwPrdNm { get; set; }
        public string InwLRNo { get; set; }
        public string InwWPNo { get; set; }
        public string InwWPWeight { get; set; }
        public int IsCheckSup { get; set; } = 0;
        public int InwCoilTypeVou { get; set; }
        public List<SelectListItem> CoilTypeList { get; set; }
        public string CoilType { get; set; }
        public int InwFrGdnVou { get; set; }
        public List<SelectListItem> FrGodownList { get; set; }
        public string IssCoilNo { get; set; }
        public decimal Width { get; set; }
        public decimal Qty { get; set; }
        public decimal Thick { get; set; }
        public List<JobInwardGridModel> JobInwardList { get; set; }
        public JobInwardGridModel JobInward { get; set; }

        public List<SelectListItem> GodownCoilList { get; set; }
        public List<SelectListItem> CoilPrefixList { get; set; }
        public List<SelectListItem> NBList { get; set; }
        public List<SelectListItem> SCHList { get; set; }
        public List<SelectListItem> IntCoilTypeList { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public string Data { get; set; }
    }
    public class JobInwardGridModel
    {
        public long IntVou { get; set; }
        public long IntInwVou { get; set; }
        public string IntGrade { get; set; }        
        public int IntGdnVou { get; set; }        
        public string IntSupCoilNo { get; set; }
        public string IntHeatNo { get; set; }
        public int IntPrefix { get; set; }
        public string IntCoilNo { get; set; }
        public decimal IntThick { get; set; }
        public string IntNB { get; set; }
        public string IntSCH { get; set; }
        public decimal IntWidth { get; set; }
        public decimal IntQty { get; set; }
        public decimal IntOD { get; set; }
        public string IntRem { get; set; }
        public int IntCoilTypeVou { get; set; }
        public List<SelectListItem> IntCoilTypeList { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public List<SelectListItem> GodownCoilList { get; set; }
        public List<SelectListItem> CoilPrefixList { get; set; }
        public List<SelectListItem> NBList { get; set; }
        public List<SelectListItem> SCHList { get; set; }
    }

    public class JobInwardPrintDetails
    {
        public string Id { get; set; }
        public string Html { get; set; }

    }

}
