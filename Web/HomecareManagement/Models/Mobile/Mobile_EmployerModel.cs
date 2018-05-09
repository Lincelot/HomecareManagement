using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Mobile
{
    public class Mobile_EmployerModel
    {
        public int uid { get; set; }
        //案主編號
        public int account_uid_1 { get; set; }
        public String birthday { get; set; }
        public int sex { get; set; }
        public String address { get; set; }
        //案主姓名
        public String employer_name { get; set; }
        public String employer_phone1 { get; set; }
        public String employer_phone2 { get; set; }
        //失能程度
        public String employer_item1 { get; set; }
        //經濟程度
        public String employer_item2 { get; set; }
        //案主身分
        public String employer_item3 { get; set; }
        //緊急聯絡人
        public String emg1_displayname { get; set; }

        public String emg1_phone1 { get; set; }

        public String emg1_phone2 { get; set; }

        public String emg2_displayname { get; set; }

        public String emg2_phone1 { get; set; }

        public String emg2_phone2 { get; set; }
        //疾病
        public String summary { get; set; }
    }
}