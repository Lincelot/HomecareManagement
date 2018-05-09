using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Web
{
    public class GridLicenseModel
    {
        public int LicenseUID { get; set; }

        public String name { get; set; }

        public String summary { get; set; }

        public String edit_time { get; set; }
    }
}