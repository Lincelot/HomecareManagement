using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Web
{
    public class InfoModel
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

        public long online { get; set; }
    }
}