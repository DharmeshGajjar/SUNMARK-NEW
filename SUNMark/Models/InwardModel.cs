using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class InwardModel
    {
        public int isPrint { get; set; }
        public int InwVou { get; set; }
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


        public List<InwardGridModel> InwardList { get; set; }
        public InwardGridModel Inward { get; set; }

    }
    public class InwardGridModel
    {
        public long IntVou { get; set; }
        public long IntInwVou { get; set; }

        public int[] IntGrdVou { get; set; }
        public string[] IntGrdCoil { get; set; }
        public string IntGrdCoilStr { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public string IntGrdVouStr { get; set; }
        public string IntGrade { get; set; }


        public int[] IntGrdVouPipe { get; set; }
        public List<SelectListItem> GradeListPipe { get; set; }
        public string IntGrdVouPipeStr { get; set; }
        public string[] IntGrdPipe { get; set; }
        public string IntGrdPipeStr { get; set; }

        public int[] IntFinshVouPipe { get; set; }
        public List<SelectListItem> FinishListPipe { get; set; }
        public string IntFinshVouPipeStr { get; set; }
        public string[] IntFinshPipe { get; set; }
        public string IntFinishPipeStr { get; set; }


        public int[] IntProceVouPipe { get; set; }
        public List<SelectListItem> ProceListPipe { get; set; }
        public string IntProceVouPipeStr { get; set; }
        public string[] IntProcePipe { get; set; }
        public string IntProcePipeStr { get; set; }



        public int[] IntSpeVou { get; set; }
        public string[] IntSpe { get; set; }
        public List<SelectListItem> SpacificationList { get; set; }
        public string IntSpeVouStr { get; set; }
        public string[] SpecifiName { get; set; }
        public string SpecifiNameStr { get; set; }

        public int[] IntPrdVou { get; set; }
        public string IntPrdVouStr { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public string PrdNm { get; set; }
        
        public int[] IntGdnCoilVou { get; set; }
        public string[] IntGdnCoil { get; set; }
        public string IntGdnCoilVouStr { get; set; }
        public List<SelectListItem> GodownCoilList { get; set; }
        public string[] GodownCoil { get; set; }

        public string[] SupCoilNo { get; set; }
        public string SupCoilNoStr { get; set; }

        public string[] BillNoCoil { get; set; }
        public string BillNoCoilStr { get; set; }

        public string[] BillNoPipe { get; set; }
        public string BillNoPipeStr { get; set; }

        public string[] BillNoOther { get; set; }
        public string BillNoOtherStr { get; set; }

        public string[] HeatNo { get; set; }
        public string HeatNoStr { get; set; }

        public int[] IntGdnPipeVou { get; set; }
        public string[] IntGdnPipe { get; set; }
        public string IntGdnPipeVouStr { get; set; }
        public List<SelectListItem> GodownPipeList { get; set; }
        public string[] GodownPipe { get; set; }

        public int[] IntGdnOtherVou { get; set; }
        public string[] IntGdnOther { get; set; }
        public string IntGdnOtherVouStr { get; set; }
        public List<SelectListItem> GodownOtherList { get; set; }
        public string[] GodownOther { get; set; }

        public int[] IntLocCoilVou { get; set; }
        public string[] IntLocCoil { get; set; }
        public string IntLocCoilVouStr { get; set; }
        public List<SelectListItem> LocationCoilList { get; set; }
        public string[] LocationCoil { get; set; }

        public int[] IntLocPipeVou { get; set; }
        public string[] IntLocPipe { get; set; }
        public string IntLocPipeVouStr { get; set; }
        public List<SelectListItem> LocationPipeList { get; set; }
        public string[] LocationPipe { get; set; }

        public int[] IntLocOtherVou { get; set; }
        public string[] IntLocOther { get; set; }
        public string IntLocOtherVouStr { get; set; }
        public List<SelectListItem> LocationOtherList { get; set; }
        public string[] LocationOther { get; set; }

        public int[] IntCoilPrefixVou { get; set; }
        public string[] IntCoilPrefix { get; set; }

        public string IntCoilPrefixVouStr { get; set; }
        public List<SelectListItem> CoilPrefixList { get; set; }
        public string[] CoilPrefix { get; set; }
        public string[] CoilNo { get; set; }

        public decimal[] IntThickCoil { get; set; }
        public string IntThickCoilStr { get; set; }
        public decimal[] IntODCoil { get; set; }
        public string IntODCoilStr { get; set; }
        public decimal[] IntNBCoil { get; set; }
        public string IntNBCoilStr { get; set; }

        public decimal[] IntSCHCoil { get; set; }
        public string IntSCHCoilStr { get; set; }

        public decimal[] IntLenghtPipe { get; set; }
        public string IntLengthPipeStr { get; set; }

        public decimal[] IntWeightPipe { get; set; }
        public string IntWeightPipeStr { get; set; }


        public decimal[] IntThickPipe { get; set; }
        public string IntThickPipeStr { get; set; }

        public decimal[] IntWidth { get; set; }
        public string IntWidthStr { get; set; }

        public decimal[] IntQtyCoil { get; set; }
        public string IntQtyCoilStr { get; set; }

        public decimal[] IntQtyPipe { get; set; }
        public string IntQtyPipeStr { get; set; }

        public decimal[] IntQtyOther { get; set; }
        public string IntQtyOtherStr { get; set; }

        public decimal[] IntOD { get; set; }
        public string IntODStr { get; set; }
        public decimal[] IntPcs { get; set; }
        public string IntPcsStr { get; set; }
        public string[] IntRemksCoil { get; set; }
        public string IntRemksCoilStr { get; set; }
        public string[] IntRemksPipe { get; set; }
        public string IntRemksPipeStr { get; set; }
        public string[] IntRemksOther { get; set; }
        public string IntRemksOtherStr { get; set; }

        public string[] IntSufix { get; set; }
        public string IntSufixStr { get; set; }
    }

    public class InwardPrintDetails
    {
        public string Id { get; set; }
        public string Html { get; set; }

    }
}
