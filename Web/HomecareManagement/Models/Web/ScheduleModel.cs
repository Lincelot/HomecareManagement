using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Web
{
    public class ScheduleModel
    {
        public int colorId { get; set; }
        public int taskId { get; set; }
        public int ownerId { get; set; }
        public String title { get; set; }
        public String start { get; set; }
        public String end { get; set; }
        public String Attendant { get; set; }
        public String Employer { get; set; }
        public String LastEditTime { get; set; }
        public String LastEditer { get; set; }
        public List<int> serviceItem { get; set; }
        public String summary { get; set; }
        public int AttendantID { get; set; }
        public int EmployerID { get; set; }
        public int pay { get; set; }
    }
}