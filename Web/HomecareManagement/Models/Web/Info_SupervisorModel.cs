using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Web
{
    public class Info_SupervisorModel
    {
        public String username { get; set; }

        public int account_uid { get; set; }

        public String displayname { get; set; }

        public String birthday { get; set; }

        public int sex { get; set; }

        public String address { get; set; }

        public String phone1 { get; set; }

        public String phone2 { get; set; }

        public String showName { get; set; }

        //督導限定

        public List<int> lsLicense { get; set; }

        public String firstday { get; set; }

        public List<int> lsTrain { get; set; }

        public String proBG { get; set; }

        public int eduBG { get; set; }
    }
}