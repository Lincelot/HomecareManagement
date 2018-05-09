using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Mobile
{
    public class Mobile_Work_Case_Record
    {
        public int uid { get; set; }
        
        public int schedule_uid { get; set; }
        
        public int case_record_answer_uid { get; set; }

        public String summary { get; set; }

    }
}