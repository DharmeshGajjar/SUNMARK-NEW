using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class OutwardModel
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


        public List<OutwardGridModel> OutwardList { get; set; }
        public OutwardGridModel Outward { get; set; }

    }
    public class OutwardGridModel
    {
        public long OutAVou { get; set; }
        public long OutAOutVou { get; set; }

        public int[] OutAGrdVou { get; set; }
        public string[] OutAGrdCoil { get; set; }
        public string OutAGrdCoilStr { get; set; }
        public List<SelectListItem> GradeList { get; set; }
        public string OutAGrdVouStr { get; set; }
        public string OutAGrade { get; set; }


        public int[] OutAGrdVouPipe { get; set; }
        public List<SelectListItem> GradeListPipe { get; set; }
        public string OutAGrdVouPipeStr { get; set; }
        public string[] OutAGrdPipe { get; set; }
        public string OutAGrdPipeStr { get; set; }

        public int[] OutAFinshVouPipe { get; set; }
        public List<SelectListItem> FinishListPipe { get; set; }
        public string OutAFinshVouPipeStr { get; set; }
        public string[] OutAFinshPipe { get; set; }
        public string OutAFinishPipeStr { get; set; }


        public int[] OutAProceVouPipe { get; set; }
        public List<SelectListItem> ProceListPipe { get; set; }
        public string OutAProceVouPipeStr { get; set; }
        public string[] OutAProcePipe { get; set; }
        public string OutAProcePipeStr { get; set; }



        public int[] OutASpeVou { get; set; }
        public string[] OutASpe { get; set; }
        public List<SelectListItem> SpacificationList { get; set; }
        public string OutASpeVouStr { get; set; }
        public string[] SpecifiName { get; set; }
        public string SpecifiNameStr { get; set; }

        public int[] OutAPrdVou { get; set; }
        public string OutAPrdVouStr { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public string PrdNm { get; set; }

        public int[] OutAGdnCoilVou { get; set; }
        public string[] OutAGdnCoil { get; set; }
        public string OutAGdnCoilVouStr { get; set; }
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

        public int[] OutAGdnPipeVou { get; set; }
        public string[] OutAGdnPipe { get; set; }
        public string OutAGdnPipeVouStr { get; set; }
        public List<SelectListItem> GodownPipeList { get; set; }
        public string[] GodownPipe { get; set; }

        public int[] OutAGdnOtherVou { get; set; }
        public string[] OutAGdnOther { get; set; }
        public string OutAGdnOtherVouStr { get; set; }
        public List<SelectListItem> GodownOtherList { get; set; }
        public string[] GodownOther { get; set; }

        public int[] OutALocCoilVou { get; set; }
        public string[] OutALocCoil { get; set; }
        public string OutALocCoilVouStr { get; set; }
        public List<SelectListItem> LocationCoilList { get; set; }
        public string[] LocationCoil { get; set; }

        public int[] OutALocPipeVou { get; set; }
        public string[] OutALocPipe { get; set; }
        public string OutALocPipeVouStr { get; set; }
        public List<SelectListItem> LocationPipeList { get; set; }
        public string[] LocationPipe { get; set; }

        public int[] OutALocOtherVou { get; set; }
        public string[] OutALocOther { get; set; }
        public string OutALocOtherVouStr { get; set; }
        public List<SelectListItem> LocationOtherList { get; set; }
        public string[] LocationOther { get; set; }

        public int[] OutACoilPrefixVou { get; set; }
        public string[] OutACoilPrefix { get; set; }

        public string OutACoilPrefixVouStr { get; set; }
        public List<SelectListItem> CoilPrefixList { get; set; }
        public string[] CoilPrefix { get; set; }
        public string[] CoilNo { get; set; }

        public decimal[] OutAThickCoil { get; set; }
        public string OutAThickCoilStr { get; set; }
        public decimal[] OutAODCoil { get; set; }
        public string OutAODCoilStr { get; set; }
        public decimal[] OutANBCoil { get; set; }
        public string OutANBCoilStr { get; set; }

        public decimal[] OutASCHCoil { get; set; }
        public string OutASCHCoilStr { get; set; }

        public decimal[] OutALenghtPipe { get; set; }
        public string OutALengthPipeStr { get; set; }

        public decimal[] OutAWeightPipe { get; set; }
        public string OutAWeightPipeStr { get; set; }


        public decimal[] OutAThickPipe { get; set; }
        public string OutAThickPipeStr { get; set; }

        public decimal[] OutAWidth { get; set; }
        public string OutAWidthStr { get; set; }

        public decimal[] OutAQtyCoil { get; set; }
        public string OutAQtyCoilStr { get; set; }

        public decimal[] OutAQtyPipe { get; set; }
        public string OutAQtyPipeStr { get; set; }

        public decimal[] OutAQtyOther { get; set; }
        public string OutAQtyOtherStr { get; set; }

        public decimal[] OutAOD { get; set; }
        public string OutAODStr { get; set; }
        public decimal[] OutAPcs { get; set; }
        public string OutAPcsStr { get; set; }
        public string[] OutARemksCoil { get; set; }
        public string OutARemksCoilStr { get; set; }
        public string[] OutARemksPipe { get; set; }
        public string OutARemksPipeStr { get; set; }
        public string[] OutARemksOther { get; set; }
        public string OutARemksOtherStr { get; set; }

        public string[] OutASufix { get; set; }
        public string OutASufixStr { get; set; }
        public int[] OutACoilTypeVou { get; set; }
        public string[] OutACoilType { get; set; }

        public string OutACoilTypeVouStr { get; set; }
        public List<SelectListItem> OutACoilTypeList { get; set; }
        public string[] ProfilePictureCoil { get; set; }
        public string ProfilePictureCoilstr { get; set; }
        public IFormFile pdfUpload1 { get; set; }
        public string[] ProfilePicturePipe { get; set; }
        public string ProfilePicturePipestr { get; set; }
        public IFormFile pdfUpload2 { get; set; }
        public string[] ProfilePictureOther { get; set; }
        public string ProfilePictureOtherstr { get; set; }
        public IFormFile pdfUpload3 { get; set; }

    }

    public class OutwardPrintDetails
    {
        public string Id { get; set; }
        public string Html { get; set; }

    }
}
