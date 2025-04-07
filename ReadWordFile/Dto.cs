using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadWordFile
{
    public class Dto
    {
    }

    public class CargoManifest
    {
        public string? VOYAGE { get; set; }
        public string? BL_NUMBER { get; set; }
        public string? PORT_OF_LOAD { get; set; }
        public string? PORT_OF_DISCHARGE { get; set; }
        public string? PLACE_OF_DELIVERY { get; set; }
        public string? SHIPPER { get; set; }
        public string? CONSIGNEE { get; set; }
        public string? NOTIFY_PARTY { get; set; }
        public string? DESCRIPTION_OF_GOODS { get; set; }
        public string? NUMBER_OF_PKGS { get; set; }
        public string? TYPE_OF_PKG { get; set; }
        public string? GROSS_WEIGHT { get; set; }
        public string? VOLUME { get; set; }
        public string? MARKS_AND_NUMBERS { get; set; }
        public string? CONTAINER_NUM { get; set; }
        public string? SEAL_NUMBER { get; set; }
    }
}
