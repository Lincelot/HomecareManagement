using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Mobile
{
    public class Mobile_Work_RecordModel
    {
        public int uid { get; set; }

        public int schedule_uid { get; set; }

        public DateTime start { get; set; }

        public DateTime end { get; set; }

        public int equipment_uid_1 { get; set; }

        public int equipment_uid_2 { get; set; }
    }
}