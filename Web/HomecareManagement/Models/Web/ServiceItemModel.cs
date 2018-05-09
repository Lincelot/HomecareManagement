using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Web
{
    public class ServiceItemModel
    {
        public int service_uid { get; set; }

        public String service_name { get; set; }

        public String service_edit_time { get; set; }

        public int service_isdelete { get; set; }

        public int fk_service_item_uid { get; set; }

        public int service_item_uid { get; set; }

        public String service_item_name { get; set; }

        public String service_item_edit_time { get; set; }

        public int service_item_isdelete { get; set; }

    }
}