using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class OpeningStokModel
    {
        public int OblVou { get; set; }
        public int OblNVno { get; set; }
        public string OblDt { get; set; }
        public string OblRefNo { get; set; }

        public int OblCmpVou { get; set; }
        public List<SelectListItem> OblCmpVouList { get; set; }

        public int OblGdnVou { get; set; }
        public List<SelectListItem> OblGdnVouList { get; set; }

        public int OblLocVou { get; set; }
        public List<SelectListItem> OblLocVouList { get; set; }

        public int OblAccVou { get; set; }
        public List<SelectListItem> OblAccVouList { get; set; }


        public int LotGrade { get; set; }
        public List<SelectListItem> LotGradeList { get; set; }
        public string Grade { get; set; }

        public int LotFinish { get; set; }
        public List<SelectListItem> LotFinishList { get; set; }
        public string Finish { get; set; }

        public int CoilTypeVou { get; set; }
        public List<SelectListItem> CoilTypeList { get; set; }
        public string CoilType { get; set; }


        public int OblPrdVou { get; set; }
        public List<SelectListItem> OblPrdVouList { get; set; }


        public string OblRem { get; set; }


        #region " LOT Table Fields"
        public string LotSupCoilNo { get; set; }
        public int LotFinMscVou { get; set; }

        public decimal LotQty { get; set; }
        public decimal LotWidth { get; set; }
        public decimal LotThick { get; set; }

        public decimal LotInwQty { get; set; }
        public decimal LotInwWidth { get; set; }
        public decimal LotInwThick { get; set; }

        public string LotHeatNo { get; set; }

        public int LotTypVou { get; set; }
        public string LotType {get; set;}
        public string LotSuffix { get; set; }
        public int CoilNo { get; set; }
        public List<SelectListItem> LotTypVouList { get; set; }
        #endregion

        public int LotPrcTypVou { get; set; }
        public decimal LotOD { get; set; }
        public decimal FeetPer { get; set; }
        public int PCS { get; set; } 

        public string NB { get; set; }
        public string SCH { get; set; }

        public string FltVno { get; set; }
    }
}
