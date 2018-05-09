using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Web
{
    public class GridEquipmentModel
    {
        public int EquipmentUID { get; set; }

        public int account_uid { get; set; }

        public String MACAddress { get; set; }

        public int type { get; set; }

        public String typeName { get; set; }

        public String summary { get; set; }

        public String edit_time { get; set; }

        public int isdelete { get; set; }

        public String status { get; set; }

        public String displayname { get; set; }

        public String phone { get; set; }

    }
}