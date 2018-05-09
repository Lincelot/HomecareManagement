using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Models.Web
{
    public class Info_EmployerModel
    {
        public String username { get; set; }

        public int account_uid { get; set; }

        public String displayname { get; set; }

        public String birthday { get; set; }

        public int sex { get; set; }

        public String address { get; set; }

        public String phone1 { get; set; }

        public String phone2 { get; set; }

        //以下為案主才有的資料

        public int supervisorID { get; set; }

        public String supervisorName { get; set; }

        //失能程度
        public int info_employer_item1_uid { get; set; }

        //經濟程度
        public int info_employer_item2_uid { get; set; }

        //案主身份
        public List<int> info_employer_item3_uid { get; set; }

        public String info_employer_item3_uid_str { get; set; }

        //補助身份
        public int info_employer_sub_uid { get; set; }

        //緊急聯絡人
        public String emg1_displayname { get; set; }

        public String emg1_phone1 { get; set; }

        public String emg1_phone2 { get; set; }

        public String emg2_displayname { get; set; }

        public String emg2_phone1 { get; set; }

        public String emg2_phone2 { get; set; }

        //核定時數(分)
        public int minutes1 { get; set; }

        //自費時數(分)
        public int minutes2 { get; set; }

        public String summary { get; set; }

    }
}