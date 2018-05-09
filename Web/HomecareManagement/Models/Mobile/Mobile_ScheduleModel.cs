using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Mobile
{
    public class Mobile_ScheduleModel
    {
        public int uid { get; set; }
        //照服員編號
        public int account_uid_1 { get; set; }
        //案主編號
        public int account_uid_2 { get; set; }
        public String start { get; set; }
        public String end { get; set; }
        public String edit_time { get; set; }
        public String summary { get; set; }
    }
}