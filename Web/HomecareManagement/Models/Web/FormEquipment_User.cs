using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Web
{
    public class FormEquipment_User
    {

        public int account_uid { get; set; }

        public String showName { get; set; }

        public String displayname { get; set; }

        public String phone { get; set; }

        public int level { get; set; }

        public String levelName { get; set; }
    }
}