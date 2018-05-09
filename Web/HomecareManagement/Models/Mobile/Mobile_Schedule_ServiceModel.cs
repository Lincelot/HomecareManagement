using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Mobile
{
    public class Mobile_Schedule_ServiceModel
    {
        public int uid { get; set; }
        public int schedule_uid { get; set; }
        public int service_uid { get; set; }
    }
}