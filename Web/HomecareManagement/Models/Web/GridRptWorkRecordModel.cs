using System;

namespace HomecareManagement.Models.Web
{
    public class GridRptWorkRecordModel
    {
        public int AttendantID { get; set; }
        public int EmployerID { get; set; }
        public String EmployerName { get; set; }
        public String Displayname { get; set; }
        public String phone { get; set; }
        public int Count { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public String Year_Month { get; set; }
        public int Worktime { get; set; }
    }
}