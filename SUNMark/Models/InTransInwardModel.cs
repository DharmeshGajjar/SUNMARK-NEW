using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class InTransInwardModel
    {
        public long InwVou { get; set; }
        public long InwCmpVou { get; set; }
        public long InwVouNo { get; set; }
        public List<SelectListItem> CompanyList { get; set; }
        public string CmpNm { get; set; }
        public int InwVNo { get; set; }
        public string InwDt { get; set; }
        public string InwETADt { get; set; }
        public string InwRefNo { get; set; }
        public long InwAccVou { get; set; }
        public List<CustomDropDown> AccountList { get; set; }
        public string AccName { get; set; }
        public string CtyName { get; set; }
        public decimal InwFrightRt { get; set; }
        public string InwRem { get; set; }
        public decimal InwBillQty { get; set; }
        public long InwPtyVou { get; set; }
        public List<SelectListItem> ProductTypeList { get; set; }
        public string InwPrdTyp { get; set; }
        public int InwPrdVou { get; set; }

        public long InwGdnVou { get; set; }
        public List<SelectListItem> HdGodwonList { get; set; }
        public string HDGodown { get; set; }
        public string IsCheckCoil { get; set; } = "0";
        public List<SelectListItem> MainProductList { get; set; }
        public string InwPrdNm { get; set; }
        public int InstExl { get; set; } = 1;
        public List<InTransInwardGridModel> InwardList { get; set; }
        public InTransInwardGridModel Inward { get; set; }
        
        public string FltVno { get; set; }

    }

    public class InTransInwardGridModel
    {
        public long IntVou { get; set; }
        public long IntInwVou { get; set; }

        public long IntCoilTypeVou { get; set; }
        public string[] IntCoilType { get; set; }
        public string IntCoilTypeStr { get; set; }
        public List<SelectListItem> CoilTypeList { get; set; }
        public string IntCoilTypeVouStr { get; set; }
        public string CoilType { get; set; }


        public long[] IntGrdVou { get; set; }
        public string[] IntGrdCoil { get; set; }
        public string IntGrdCoilStr { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public string IntGrdVouStr { get; set; }
        public string IntGrade { get; set; }


        public long[] IntGrdVouPipe { get; set; }
        public List<SelectListItem> GradeListPipe { get; set; }
        public string IntGrdVouPipeStr { get; set; }
        public string[] IntGrdPipe { get; set; }
        public string IntGrdPipeStr { get; set; }

        public long[] IntFinshVouPipe { get; set; }
        public List<SelectListItem> FinishListPipe { get; set; }
        public string IntFinshVouPipeStr { get; set; }
        public string[] IntFinshPipe { get; set; }
        public string IntFinishPipeStr { get; set; }


        public long[] IntProceVouPipe { get; set; }
        public List<SelectListItem> ProceListPipe { get; set; }
        public string IntProceVouPipeStr { get; set; }
        public string[] IntProcePipe { get; set; }
        public string IntProcePipeStr { get; set; }



        public long[] IntSpeVou { get; set; }
        public string[] IntSpe { get; set; }
        public List<SelectListItem> SpacificationList { get; set; }
        public string IntSpeVouStr { get; set; }
        public string[] SpecifiName { get; set; }
        public string SpecifiNameStr { get; set; }

        public long[] IntPrdVou { get; set; }
        public string IntPrdVouStr { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public string PrdNm { get; set; }

        public long[] IntGdnCoilVou { get; set; }
        public string[] IntGdnCoil { get; set; }
        public string IntGdnCoilStr { get; set; }
        public List<SelectListItem> GodownCoilList { get; set; }
        public string IntGdnCoilVouStr { get; set; }
        public string GodownCoil { get; set; }

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

        public long[] IntGdnPipeVou { get; set; }
        public string[] IntGdnPipe { get; set; }
        public string IntGdnPipeVouStr { get; set; }
        public List<SelectListItem> GodownPipeList { get; set; }
        public string[] GodownPipe { get; set; }

        public long[] IntGdnOtherVou { get; set; }
        public string[] IntGdnOther { get; set; }
        public string IntGdnOtherVouStr { get; set; }
        public List<SelectListItem> GodownOtherList { get; set; }
        public string[] GodownOther { get; set; }

        public long[] IntLocCoilVou { get; set; }
        public string[] IntLocCoil { get; set; }
        public string IntLocCoilVouStr { get; set; }
        public List<SelectListItem> LocationCoilList { get; set; }
        public string[] LocationCoil { get; set; }

        public long[] IntLocPipeVou { get; set; }
        public string[] IntLocPipe { get; set; }
        public string IntLocPipeVouStr { get; set; }
        public List<SelectListItem> LocationPipeList { get; set; }
        public string[] LocationPipe { get; set; }

        public long[] IntLocOtherVou { get; set; }
        public string[] IntLocOther { get; set; }
        public string IntLocOtherVouStr { get; set; }
        public List<SelectListItem> LocationOtherList { get; set; }
        public string[] LocationOther { get; set; }
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

        public string[] IntSCNo { get; set; }
        public string IntSCNoStr { get; set; }

        public decimal[] IntPOWidth { get; set; }
        public string IntPOWidthStr { get; set; }

        public decimal[] IntPOThick { get; set; }
        public string IntPOThickStr { get; set; }

    }

}
