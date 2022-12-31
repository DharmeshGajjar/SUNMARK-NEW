using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Models
{
    public class PicklingMasterModel
    {
        public int PikVou { get; set; }

        public int PikCmpVou { get; set; }

        public string Vno { get; set; }

        public string Date { get; set; }

        public string Shift { get; set; }

        public string MachineNo { get; set; }

        public string SupEmpVou { get; set; }

        public string ManEmpVou { get; set; }

        public string IssuePrdVou { get; set; }

        public string Finish { get; set; }

        public string FinishVou { get; set; }

        public string Grade { get; set; }

        public string GradeVou { get; set; }

        public string Width { get; set; }

        public string Thick { get; set; }

        public string OD { get; set; }

        public string NoOfPipe { get; set; }

        public string Weight { get; set; }

        public string RecPrdVou { get; set; }

        public string InTime { get; set; }

        public string OutTime { get; set; }

        public string HFQty { get; set; }

        public string NitricQty { get; set; }

        public string LimeQty { get; set; }

        public string RPM { get; set; }

        public string Remarks
        {
            get; set;
        }
    }
}
