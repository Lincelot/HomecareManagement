using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Web
{
    public class Info_AttendantModel
    {
        public String username { get; set; }

        public int account_uid { get; set; }

        public String displayname { get; set; }

        public String birthday { get; set; }

        public int sex { get; set; }

        public String address { get; set; }

        public String phone1 { get; set; }

        public String phone2 { get; set; }

        //以下為照服員才有的資料

        public int supervisorID { get; set; }

        public String supervisorName { get; set; }

        public int pay { get; set; }

        public String firstday { get; set; }

        public List<int> lsLicense { get; set; }

        public String summary { get; set; }

    }
}