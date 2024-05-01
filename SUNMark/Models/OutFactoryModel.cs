using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class OutFactoryModel
    {
        public int isPrint { get; set; }
        public int OutVou { get; set; }
        public int OutCmpVou { get; set; }
        public long OutVouNo { get; set; }
        public List<SelectListItem> CompanyList { get; set; }
        public string CmpNm { get; set; }
        public int OutVNo { get; set; }
        public string OutDt { get; set; }
        public string OutRefNo { get; set; }
        public int OutPOMVou { get; set; }
        public List<SelectListItem> PurchaseOrderList { get; set; }
        public string OrmVNo { get; set; }
        public string OutBillNo { get; set; }
        public string OutBillDt { get; set; }
        public int OutAccVou { get; set; }
        public List<CustomDropDown> AccountList { get; set; }
        public string AccName { get; set; }
        public string CtyName { get; set; }
        public string OutVehNo { get; set; }
        public decimal OutFrightRt { get; set; }
        public int OutTrnVou { get; set; }
        public List<CustomDropDown> TransportList { get; set; }
        public string OutTransNm { get; set; }
        public string OutDrvNm { get; set; }
        public string OutDrvMob { get; set; }
        public string OutRem { get; set; }
        public decimal OutBillQty { get; set; }
        public int OutPtyVou { get; set; }
        public List<SelectListItem> ProductTypeList { get; set; }
        public string OutPrdTyp { get; set; }
        public int OutPrdVou { get; set; }
        public List<SelectListItem> MainProductList { get; set; }
        public string OutPrdNm { get; set; }
        public string OutLRNo { get; set; }
        public string OutWPNo { get; set; }
        public string OutWPWeight { get; set; }
        public int IsCheckSup { get; set; } = 0;
        public int OutCoilTypeVou { get; set; }
        public List<SelectListItem> CoilTypeList { get; set; }
        public string CoilType { get; set; }

        public List<SelectListItem> GradeList { get; set; }
        public List<SelectListItem> GodownCoilList { get; set; }
        public List<SelectListItem> SpacificationList { get; set; }
        public List<SelectListItem> FinishList { get; set; }
        public List<SelectListItem> ProcessList { get; set; }

        public List<OutFactoryGridModel> OutFactoryList { get; set; }
        public OutFactoryGridModel OutFactory { get; set; }
        public string Data { get; set; }
    }
    public class OutFactoryGridModel
    {
        public long OutAVou { get; set; }
        public long OutAOutVou { get; set; }
        public long OutAGrade { get; set; }
        public long OutAGdnVou { get; set; }
        public long OutASpaVou { get; set; }
        public long OutAFinVou { get; set; }
        public long OutAPrcVou { get; set; }
        public string OutASupCoilNo { get; set; }
        public string OutAHeatNo { get; set; }
        public string OutACoilNo { get; set; }
        public decimal OutAThick { get; set; }
        public decimal OutAWidth { get; set; }
        public decimal OutAQty { get; set; }
        public decimal OutAPcs { get; set; }
        public decimal OutAOD { get; set; }
        public decimal OutALength { get; set; }
        public string OutARem { get; set; }        
        public List<SelectListItem> GradeList { get; set; }
        public List<SelectListItem> GodownCoilList { get; set; }
        public List<SelectListItem> SpacificationList { get; set; }
        public List<SelectListItem> FinishList { get; set; }
        public List<SelectListItem> ProcessList { get; set; }
    }
    public class OutFactoryPrintDetails
    {
        public string Id { get; set; }
        public string Html { get; set; }

    }

}

