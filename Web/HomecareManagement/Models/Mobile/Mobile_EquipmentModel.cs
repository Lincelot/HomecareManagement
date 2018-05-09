using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Mobile
{
    public class Mobile_EquipmentModel
    {
        public int uid { get; set; }

        public int account_uid { get; set; }

        public String macaddress { get; set; }

        //類型(Beacon=0&Mobile=1)
        public int type { get; set; }
        //編輯時間
        public String edit_time { get; set; }
    }
}