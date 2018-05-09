using HomecareManagement.Models.Mobile;
using HomecareManagement.Models.Web;
using HomecareManagement.Report;
using HomecareManagement.Service;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;

namespace HomecareManagement.Controllers
{
    public class AttendantController : Controller
    {
        AttendantService db = new AttendantService();
        //
        // GET: /Attendant/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult getMobileData(String UUID)
        {
            Dictionary<String, object> data = new Dictionary<string, object>();
            int i = db.selectMobileMAC(UUID.ToUpper());
            if (i > 0)
            {
                data.Add("Equipment", db.selectEquipment());
                data.Add("Service", db.selectService());
                data.Add("Schedule", db.selectMobile_Schedule(i));
                data.Add("Schedule_Service", db.selectSchedule_Service(i));
                data.Add("Employer", db.selectAllEmployerInfo(i));

                var dt_CaseRecordItem = db.selectCaseRecordItem();
                var list_CaseRecordItem = from item in dt_CaseRecordItem.AsEnumerable()
                                          select new Mobile_CaseRecordModel
                                          {
                                              uid = item.Field<int>("uid"),
                                              name = item.Field<String>("name")
                                          };
                data.Add("CaseRecordItem", list_CaseRecordItem);

                var dt_CaseRecordAnswer = db.selectCaseRecordAnswer();
                var list_CaseRecordAnswer = from item in dt_CaseRecordAnswer.AsEnumerable()
                                            select new Mobile_CaseRecordModel
                                            {
                                                uid = item.Field<int>("uid"),
                                                case_serivce_record_item_uid = item.Field<int>("case_serivce_record_item_uid"),
                                                name = item.Field<String>("name")
                                            };
                data.Add("CaseRecordAnswer", list_CaseRecordAnswer);
            }
            if (data.Count > 0) { data.Add("status", "ok"); }
            else { data.Add("status", "error"); }
            return Json(data);
        }

        [HttpPost]
        public JsonResult setRecordData(String UUID, List<Mobile_Work_ServiceModel> ServiceRecordList, List<Mobile_Work_Case_Record> CaseRecordList)
        {
            var data = new Dictionary<String, Object>();
            int i = db.selectMobileMAC(UUID.ToUpper());
            if (i > 0)
            {
                data.Add("status", "ok");
                data.Add("ServiceRecordList", db.insertWork_Service(ServiceRecordList));
                data.Add("CaseRecordList", db.insertWork_Case_Record(CaseRecordList));
            }
            else
            {
                data.Add("status", "error");
            }
            return Json(data);
        }

        [HttpPost]
        public JsonResult setWorkRecord(String UUID, List<Mobile_Work_RecordModel> WorkRecordList)
        {
            var data = new Dictionary<String, Object>();
            int i = db.selectMobileMAC(UUID.ToUpper());
            if (i > 0)
            {
                int uid = db.selectEquipmentUIDFromMobile(UUID);
                data.Add("status", "ok");
                data.Add("WorkRecordList", db.insertWork_Record(WorkRecordList, uid));
            }
            else
            {
                data.Add("status", "error");
            }
            return Json(data);
        }
    }
}
