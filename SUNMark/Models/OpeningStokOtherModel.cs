using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class OpeningStokOtherModel
    {
        public int OblVou { get; set; }
        public int OblNVno { get; set; }
        public string OblDt { get; set; }

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

        public int LotFinish { get; set; }
        public List<SelectListItem> LotFinishList { get; set; }

        public int OblPrdVou { get; set; }
        public List<SelectListItem> OblPrdVouList { get; set; }

        public int LotPrcTypVou { get; set; }
        public List<SelectListItem> LotPrcTypVouList { get; set; }
        
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

        public int CoilNo { get; set; }
        public List<SelectListItem> LotTypVouList { get; set; }
        #endregion
    }
}
