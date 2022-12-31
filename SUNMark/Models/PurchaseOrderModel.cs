using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class PurchaseOrderModel
    {
        public long OrmVou { get; set; }
        public int OrmCmpVou { get; set; }
        public List<SelectListItem> CompanyList { get; set; }
        public string CmpNm { get; set; }
        public int OrmVchVou { get; set; }
        public List<SelectListItem> VoucherTypeList { get; set; }
        public string VchType { get; set; }
        public int OrmVNo { get; set; }
        public string OrmDt { get; set; }
        public string OrmRefNo { get; set; }
        public int OrmAccVou { get; set; }
        public int OrmSalesPersonVou { get; set; }
        public string OrdQtyIn { get; set; }
        public string OrdShipTo { get; set; }
        public List<CustomDropDown> AccountList { get; set; }
        public List<CustomDropDown> SalesPersonList { get; set; }
        public string AccName { get; set; }
        public string SalesName { get; set; }
        public string CtyName { get; set; }
        public string OrmDueDt { get; set; }
        public int OrmPtyVou { get; set; }
        public List<SelectListItem> ProductTypeList { get; set; }
        public string PtyName { get; set; }
        public string OrmRem { get; set; }
        public string OrmWONo { get; set; }
        public string OrmLastWONo { get; set; }
        public string OrmPONo { get; set; }
    
        public string PODate { get; set; }

        public string PODateString { get; set; }

        public string ShipTo { get; set; }

        public string QtyIn { get; set; }

        public PurchaseOrderGridModel PurchaseOrder { get; set; }
    }
    public class PurchaseOrderGridModel
    {
        public long OrdVou { get; set; }
        public long OrdOrmVou { get; set; }
        public int OrdSrNo { get; set; }
        public string[] OrdPrdVou { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public string OrdPrdVouStr { get; set; }
        public string PrdName { get; set; }
        public int[] OrdGrdVou { get ; set; }
        public string[] OrdGrdCoil { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public string OrdGrdVouStr { get; set; }
        public string GrdName { get; set; }

        public string Measure { get; set; }

        public string RatePerstr { get; set; }
        public string Endstr { get; set; }
        public string Pipestr { get; set; }
        public string[] RatePer { get; set; }
        public string[] End { get; set; }
        public string[] Pipe { get; set; }
        public string[] SupSrNo { get; set; }
        public string[] SupProduct { get; set; }
        public string[] ItemCode { get; set; }


        public string[] OrdGrdVouPipe { get; set; }
        public List<SelectListItem> GradeListPipe { get; set; }

        public List<SelectListItem> MeasureList { get; set; }

        public List<SelectListItem> RatePerList { get; set; }

        public List<SelectListItem> EndList { get; set; }
        public List<SelectListItem> PipeList { get; set; }
        public string OrdGrdVouPipeStr { get; set; }
        public string[] OrdGrdPipe { get; set; }

        public string[] OrdSpeVou { get; set; }
        public string[] OrdSpe { get; set; }
        public List<SelectListItem> SpacificationList { get; set; }
        public string OrdSpeVouStr { get; set; }
        public string[] SpecifiName { get; set; }

        public decimal[] OrdWidth { get; set; }
        public string OrdWidthStr { get; set; }

        public decimal[] OrdThickCoil { get; set; }
        public string OrdThickCoilStr { get; set; }

        public decimal[] OrdThickPipe { get; set; }
        public decimal[] OrdActThickPipe { get; set; }
        public string OrdActThickPipeStr { get; set; }

        public decimal[] OrdQtyCoil { get; set; }
        public string OrdQtyCoilStr { get; set; }

        public decimal[] OrdRtCoil { get; set; }
        public string OrdRtCoilStr { get; set; }

        public decimal[] OrdQtyOther { get; set; }

        public decimal[] OrdPenQty { get; set; }
        public string OrdPenQtyStr { get; set; }

        public string[] OrdOD { get; set; }
        public string[] OrdSCH { get; set; }

        public string[] OrdFinish { get; set; }
        public string OrdODStr { get; set; }
        public string OrdSCHStr { get; set; }
        public string OrdFinishStr { get; set; }
        public decimal[] Length { get; set; }

        public decimal[] OrdFeetPer { get; set; }

        public decimal[] OrdFoot { get; set; }
        public string OrdFeetPerStr { get; set; }
        public decimal[] OrdPcs { get; set; }
        public string OrdPcsStr { get; set; }
        public decimal[] Quantity { get; set; }
        public string OrdFootStr { get; set; }
        public string[] OrdNB { get; set; }
        public string OrdNBStr { get; set; }
        public string[] OrdWeight { get; set; }
        public string[] OrdSch { get; set; }

        public string[] OrdRate { get; set; }
        public string OrdSchStr { get; set; }
        public decimal[] OrdPenPcs { get; set; }
        public string OrdPenPcsStr { get; set; }
        public string[] OrdRemksCoil { get; set; }
        public string OrdRemksStr { get; set; }

        public string[] OrdRemksPipe { get; set; }

        public string[] OrdRemksOther { get; set; }

    }
}
