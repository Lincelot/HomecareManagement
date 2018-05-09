using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Mobile
{
    public class Mobile_Service_ItemModel
    {
        public int uid { get; set; }

        public String name { get; set; }
        //編輯時間
        public String edit_time { get; set; }

        public int isdelete { get; set; }
    }
}