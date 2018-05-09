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
    public class SupervisorController : Controller
    {
        //
        // GET: /Supervisor/
        SupervisorService db = new SupervisorService();

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Title = "";
            return View();
        }

        [Authorize]
        public ActionResult Member()
        {
            return View();
        }

        [Authorize]
        public ActionResult Schedule()
        {
            return View();
        }

        [Authorize]
        public ActionResult Report()
        {
            return View();
        }

        [Authorize]
        public ActionResult Map()
        {
            return View();
        }

        #region Report

        /// <summary>初始化-取得督導資料</summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult getReportInitData()
        {
            List<InfoModel> SupervisorList = db.selectSupervisorName();
            return Json(SupervisorList);
        }

        /// <summary>取得指定督導編號旗下照服員清單</summary>
        /// <param name="SupervisorID">督導編號</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult getGridRptWorkRecord(int SupervisorID)
        {
            List<GridRptWorkRecordModel> data = db.selectGridRptWorkRecordForSupervisorID(SupervisorID);
            return Json(data);
        }


        /// <summary>單筆-照顧服務員工作時數紀錄表</summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Report_WorkRecord(int AttendantID, int EmployerID, int Year, int Month, String EmployerName)
        {
            List<string> lsStrWhere = new List<string>();
            //一頁要顯示多少班次(預設：10)
            int pageLimit = 10;
            // Prepare data in report
            DataTable dt = db.selectWorkRecordPeopleData(AttendantID, EmployerID, Year, Month);

            #region 填入分頁記號&姓名&核定時數&性別

            //計算筆數用
            int RowNum = 0;
            int sex = 0;
            double time = 0;
            List<int> intWokID = new List<int>();
            if (dt != null)
            {
                sex = int.Parse(dt.Rows[0]["sex"].ToString());
                time = int.Parse(dt.Rows[0]["time"].ToString());
                int count = 0;
                dt.Columns.Add("RowNum");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int j = int.Parse(dt.Rows[i]["uid"].ToString());
                    intWokID.Add(j);
                    if (RowNum != j)
                    {
                        count++;
                    }
                    if (count == pageLimit && i != dt.Rows.Count - 1)
                    {
                        count = 0;
                        RowNum++;
                    }
                    dt.Rows[i]["RowNum"] = RowNum;
                }
                RowNum = 0;
                String str = "";
                for (int i = 0; i < intWokID.Count; i++)
                {
                    str += "a.uid = " + intWokID[i];
                    if (RowNum != pageLimit - 1)
                    {
                        RowNum++;
                        if (i != intWokID.Count - 1)
                        {
                            str += " OR ";
                        }
                        else
                        {
                            lsStrWhere.Add(str);
                        }
                    }
                    else
                    {
                        RowNum = 0;
                        lsStrWhere.Add(str);
                        str = "";
                    }
                }
            }

            #endregion

            #region 計算時數

            List<int> lsMin1 = new List<int>();
            List<int> lsMin2 = new List<int>();
            List<int> lsDay = new List<int>();
            List<int> lsCountMin1 = new List<int>();
            List<int> lsCountMin2 = new List<int>();
            List<int> lsTotalTime = new List<int>();
            int tmp_min1 = 0;
            int tmp_min2 = 0;
            int tmp_day = 0;
            int tmp_day_count = 0;
            int tmp_count_min1 = 0;
            int tmp_count_min2 = 0;
            int tmp_total_time = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //存放至變數方便使用
                int min1 = int.Parse(dt.Rows[i]["min1"].ToString());
                int min2 = int.Parse(dt.Rows[i]["min2"].ToString());
                int day = int.Parse(dt.Rows[i]["day"].ToString());
                int count_min1 = int.Parse(dt.Rows[i]["count_min1"].ToString());
                int count_min2 = int.Parse(dt.Rows[i]["count_min2"].ToString());
                int total_time = int.Parse(dt.Rows[i]["total_time"].ToString());

                //計算服務天數
                if (tmp_day != day)
                {
                    tmp_day = day;
                    tmp_day_count++;
                }

                //計算服務類型時數
                tmp_min1 += min1;
                tmp_min2 += min2;
                tmp_count_min1 += count_min1;
                tmp_count_min2 += count_min2;
                tmp_total_time += total_time;
                if ((i + 1) % 10 == 0 || i == dt.Rows.Count - 1)
                {
                    lsMin1.Add(tmp_min1);
                    lsMin2.Add(tmp_min2);
                    lsDay.Add(tmp_day_count);
                    lsCountMin1.Add(tmp_count_min1);
                    lsCountMin2.Add(tmp_count_min2);
                    lsTotalTime.Add(tmp_total_time);
                    tmp_min1 = 0;
                    tmp_min2 = 0;
                    tmp_day = 0;
                    tmp_day_count = 0;
                    tmp_count_min1 = 0;
                    tmp_count_min2 = 0;
                    tmp_total_time = 0;
                }


            }

            #endregion

            // Set report info
            ReportWrapper rw = new ReportWrapper();
            rw.ReportPath = Server.MapPath("~/Report/Rdlc/rptWorkRecord.rdlc");
            rw.ReportDataSources.Add(new ReportDataSource("DataSet", dt));
            rw.ReportParameters.Add(new ReportParameter("rptParameterTitle", Year - 1911 + " 年 " + Month + " 月　照顧服務員工作時數紀錄表"));
            rw.ReportParameters.Add(new ReportParameter("rptParameterName", EmployerName));
            rw.ReportParameters.Add(new ReportParameter("rptParameterTime", Math.Round(time / 60, 2).ToString()));
            if (sex == 0) { rw.ReportParameters.Add(new ReportParameter("rptParameterSex", "□ 男 ■ 女")); }
            else if (sex == 1) { rw.ReportParameters.Add(new ReportParameter("rptParameterSex", "■ 男 □ 女")); }
            rw.ReportParameters.Add(new ReportParameter("rptParameterPageRowLimit", pageLimit.ToString()));
            rw.ReportParameters.Add(new ReportParameter("strWhere", string.Join(",", lsStrWhere.ToArray())));
            rw.ReportParameters.Add(new ReportParameter("rptParameterMin1", string.Join(",", lsMin1.ToArray())));
            rw.ReportParameters.Add(new ReportParameter("rptParameterMin2", string.Join(",", lsMin2.ToArray())));
            rw.ReportParameters.Add(new ReportParameter("rptParameterDay", string.Join(",", lsDay.ToArray())));
            rw.ReportParameters.Add(new ReportParameter("rptParameterCountMin1", string.Join(",", lsCountMin1.ToArray())));
            rw.ReportParameters.Add(new ReportParameter("rptParameterCountMin2", string.Join(",", lsCountMin2.ToArray())));
            rw.ReportParameters.Add(new ReportParameter("rptParameterCountTotalTime", string.Join(",", lsTotalTime.ToArray())));
            rw.SubreportProcessingEventHandlers.Add(new SubreportProcessingEventHandler(SubreportProcessingEventHandler));
            // Pass report info via session
            Session["ReportWrapper"] = rw;
            // Go report viewer page
            return Redirect("~/Report/ReportViewer.aspx");
        }

        /// <summary>多筆-照顧服務員工作時數紀錄表</summary>
        /// <param name="AttendantID"></param>
        /// <param name="EmployerID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="EmployerName"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult Report_WorkRecord(List<int> AttendantID, List<int> EmployerID, List<int> Year, List<int> Month, List<String> EmployerName, List<String> AttendantName, String lsEmail, String Type)
        {
            string result = "";
            var b = false;
            var lsMailAddress = new List<MailAddress>();
            var lsMail = new List<String>(lsEmail.Split(','));
            try
            {
                foreach (var item in lsMail)
                {
                    lsMailAddress.Add(new MailAddress(item));
                }
                b = true;
            }
            catch (Exception)
            {
                result = "100";
            }
            try
            {
                // Prepare data in report
                if (AttendantID != null && b)
                {
                    List<ReportWrapper> lsRW = new List<ReportWrapper>();
                    var lsDataTable = db.selectWorkRecordPeopleData(AttendantID, EmployerID, Year, Month);
                    for (int a = 0; a < lsDataTable.Count; a++)
                    {
                        List<string> lsStrWhere = new List<string>();
                        //一頁要顯示多少班次(預設：10)
                        int pageLimit = 10;
                        // Prepare data in report
                        DataTable dt = lsDataTable[a];

                        #region 填入分頁記號&姓名&核定時數&性別

                        //計算筆數用
                        int RowNum = 0;
                        int sex = 0;
                        double time = 0;
                        List<int> intWokID = new List<int>();
                        if (dt != null)
                        {
                            sex = int.Parse(dt.Rows[0]["sex"].ToString());
                            time = int.Parse(dt.Rows[0]["time"].ToString());
                            int count = 0;
                            dt.Columns.Add("RowNum");
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                int j = int.Parse(dt.Rows[i]["uid"].ToString());
                                intWokID.Add(j);
                                if (RowNum != j)
                                {
                                    count++;
                                }
                                if (count == pageLimit && i != dt.Rows.Count - 1)
                                {
                                    count = 0;
                                    RowNum++;
                                }
                                dt.Rows[i]["RowNum"] = RowNum;
                            }
                            RowNum = 0;
                            String str = "";
                            for (int i = 0; i < intWokID.Count; i++)
                            {
                                str += "a.uid = " + intWokID[i];
                                if (RowNum != pageLimit - 1)
                                {
                                    RowNum++;
                                    if (i != intWokID.Count - 1)
                                    {
                                        str += " OR ";
                                    }
                                    else
                                    {
                                        lsStrWhere.Add(str);
                                    }
                                }
                                else
                                {
                                    RowNum = 0;
                                    lsStrWhere.Add(str);
                                    str = "";
                                }
                            }
                        }

                        #endregion

                        #region 計算時數

                        List<int> lsMin1 = new List<int>();
                        List<int> lsMin2 = new List<int>();
                        List<int> lsDay = new List<int>();
                        List<int> lsCountMin1 = new List<int>();
                        List<int> lsCountMin2 = new List<int>();
                        List<int> lsTotalTime = new List<int>();
                        int tmp_min1 = 0;
                        int tmp_min2 = 0;
                        int tmp_day = 0;
                        int tmp_day_count = 0;
                        int tmp_count_min1 = 0;
                        int tmp_count_min2 = 0;
                        int tmp_total_time = 0;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //存放至變數方便使用
                            int min1 = int.Parse(dt.Rows[i]["min1"].ToString());
                            int min2 = int.Parse(dt.Rows[i]["min2"].ToString());
                            int day = int.Parse(dt.Rows[i]["day"].ToString());
                            int count_min1 = int.Parse(dt.Rows[i]["count_min1"].ToString());
                            int count_min2 = int.Parse(dt.Rows[i]["count_min2"].ToString());
                            int total_time = int.Parse(dt.Rows[i]["total_time"].ToString());

                            //計算服務天數
                            if (tmp_day != day)
                            {
                                tmp_day = day;
                                tmp_day_count++;
                            }

                            //計算服務類型時數
                            tmp_min1 += min1;
                            tmp_min2 += min2;
                            tmp_count_min1 += count_min1;
                            tmp_count_min2 += count_min2;
                            tmp_total_time += total_time;
                            if ((i + 1) % 10 == 0 || i == dt.Rows.Count - 1)
                            {
                                lsMin1.Add(tmp_min1);
                                lsMin2.Add(tmp_min2);
                                lsDay.Add(tmp_day_count);
                                lsCountMin1.Add(tmp_count_min1);
                                lsCountMin2.Add(tmp_count_min2);
                                lsTotalTime.Add(tmp_total_time);
                                tmp_min1 = 0;
                                tmp_min2 = 0;
                                tmp_day = 0;
                                tmp_day_count = 0;
                                tmp_count_min1 = 0;
                                tmp_count_min2 = 0;
                                tmp_total_time = 0;
                            }


                        }

                        #endregion

                        #region Set report info

                        ReportWrapper rw = new ReportWrapper();
                        rw.ReportPath = Server.MapPath("~/Report/Rdlc/rptWorkRecord.rdlc");
                        rw.ReportDataSources.Add(new ReportDataSource("DataSet", dt));
                        rw.ReportParameters.Add(new ReportParameter("rptParameterTitle", Year[a] - 1911 + " 年 " + Month[a] + " 月　照顧服務員工作時數紀錄表"));
                        rw.ReportParameters.Add(new ReportParameter("rptParameterName", EmployerName[a]));
                        rw.ReportParameters.Add(new ReportParameter("rptParameterTime", Math.Round(time / 60, 2).ToString()));
                        if (sex == 0) { rw.ReportParameters.Add(new ReportParameter("rptParameterSex", "□ 男 ■ 女")); }
                        else if (sex == 1) { rw.ReportParameters.Add(new ReportParameter("rptParameterSex", "■ 男 □ 女")); }
                        rw.ReportParameters.Add(new ReportParameter("rptParameterPageRowLimit", pageLimit.ToString()));
                        rw.ReportParameters.Add(new ReportParameter("strWhere", string.Join(",", lsStrWhere.ToArray())));
                        rw.ReportParameters.Add(new ReportParameter("rptParameterMin1", string.Join(",", lsMin1.ToArray())));
                        rw.ReportParameters.Add(new ReportParameter("rptParameterMin2", string.Join(",", lsMin2.ToArray())));
                        rw.ReportParameters.Add(new ReportParameter("rptParameterDay", string.Join(",", lsDay.ToArray())));
                        rw.ReportParameters.Add(new ReportParameter("rptParameterCountMin1", string.Join(",", lsCountMin1.ToArray())));
                        rw.ReportParameters.Add(new ReportParameter("rptParameterCountMin2", string.Join(",", lsCountMin2.ToArray())));
                        rw.ReportParameters.Add(new ReportParameter("rptParameterCountTotalTime", string.Join(",", lsTotalTime.ToArray())));
                        rw.SubreportProcessingEventHandlers.Add(new SubreportProcessingEventHandler(SubreportProcessingEventHandler));
                        if (Month[a] < 10)
                        {
                            rw.FileName = "(" + (Year[a] - 1911) + "0" + Month[a] + ")" + EmployerName[a] + "_" + AttendantName[a];
                        }
                        else
                        {
                            rw.FileName = "(" + (Year[a] - 1911) + Month[a] + ")" + EmployerName[a] + "_" + AttendantName[a];
                        }
                        rw.FileType = Type;
                        lsRW.Add(rw);

                        #endregion

                    }

                    #region 產生報表&send email

                    var lsFilePath = new List<String>();
                    for (int i = 0; i < lsRW.Count; i++)
                    {
                        var rw = lsRW[i];
                        if (rw != null)
                        {
                            #region Report

                            var RptViewer = new Microsoft.Reporting.WebForms.ReportViewer();
                            // Rdlc location
                            RptViewer.LocalReport.ReportPath = rw.ReportPath;

                            // Set report data source
                            RptViewer.LocalReport.DataSources.Clear();
                            foreach (var reportDataSource in rw.ReportDataSources)
                            { RptViewer.LocalReport.DataSources.Add(reportDataSource); }

                            //SubreportProcessingEventHandler
                            foreach (var SubreportProcessingEventHandler in rw.SubreportProcessingEventHandlers)
                            { RptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler); }

                            // Set report parameters
                            RptViewer.LocalReport.SetParameters(rw.ReportParameters);

                            RptViewer.LocalReport.Refresh();

                            Warning[] warnings;
                            string[] streamids;
                            string mimeType;
                            string encoding;
                            string extension;

                            byte[] bytes = null;

                            String filePath = Path.Combine(Path.GetTempPath(), rw.FileName);

                            switch (rw.FileType)
                            {
                                case "Excel":
                                    {
                                        bytes = RptViewer.LocalReport.Render(
                                                               "Excel", null, out mimeType, out encoding, out extension,
                                                                out streamids, out warnings);
                                        filePath += ".xls";
                                        break;
                                    }
                                case "PDF":
                                    {
                                        bytes = RptViewer.LocalReport.Render(
                                                               "PDF", null, out mimeType, out encoding, out extension,
                                                                out streamids, out warnings);
                                        filePath += ".pdf";
                                        break;
                                    }
                                case "Word":
                                    {
                                        bytes = RptViewer.LocalReport.Render(
                                                               "Word", null, out mimeType, out encoding, out extension,
                                                                out streamids, out warnings);
                                        filePath += ".doc";
                                        break;
                                    }
                            }


                            using (var fs = new FileStream(filePath, FileMode.Create))
                            {
                                fs.Write(bytes, 0, bytes.Length);
                                fs.Close();
                            }
                            lsFilePath.Add(filePath);
                            RptViewer.Dispose();

                            #endregion
                        }
                    }

                    #region Send Mail

                    using (ZipFile zip = new ZipFile(System.Text.Encoding.UTF8))
                    {
                        String file = Path.GetTempPath() + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".zip";
                        String filePassword = "";
                        Random rnd=new Random();
                        //產生4個數字的亂數
                        for (int ctr = 0; ctr <4; ctr++)
                        {
                            filePassword += rnd.Next(0, 10).ToString();
                        }
                        for (int i = 0; i < lsFilePath.Count; i++)
                        {
                            zip.Password = filePassword.ToString();
                            zip.AddFile(lsFilePath[i], "");
                        }
                        
                        zip.Save(file);

                        SMTPService.MailSender ms = new SMTPService.MailSender();
                        String title = "照顧服務員工作時數紀錄表";
                        String content = "發送者:administrator";
                        var lsAttachment = new List<Attachment>();
                        lsAttachment.Add(new Attachment(file));
                        ms.Send(title, content, lsMailAddress, lsAttachment);
                        result = filePassword;
                        filePassword = "";
                    }

                    #endregion

                    #endregion

                }
            }
            catch (Exception)
            {
                result = "-1";
                throw;
            }
            return Json(result);
        }


        /// <summary>單筆-個案服務紀錄表</summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Report_CaseServiceRecord(int AttendantID, int EmployerID, int Year, int Month, String EmployerName, String AttendantName)
        {
            var dt = db.getCaseServiceRecord(AttendantID, EmployerID, Year, Month);

            #region 計算子報表需要執行幾次

            dt.Columns.Add("pageNum");
            int pageCount = 0;
            int schedule_uid = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int j = int.Parse(dt.Rows[i]["schedule_uid"].ToString());
                if (schedule_uid != j)
                {
                    schedule_uid = j;
                    pageCount++;
                }
                if (pageCount % 2 == 0)
                {
                    dt.Rows[i]["pageNum"] = pageCount - 1;
                }
                else
                {
                    dt.Rows[i]["pageNum"] = pageCount;
                }
            }

            #endregion

            // Set report info
            ReportWrapper rw = new ReportWrapper();
            rw.ReportPath = Server.MapPath("~/Report/Rdlc/rptCaseServiceRecord.rdlc");
            rw.ReportDataSources.Add(new ReportDataSource("DataSet", dt));
            rw.ReportParameters.Add(new ReportParameter("rptParameterAttendantName", AttendantName));
            rw.ReportParameters.Add(new ReportParameter("rptParameterEmployerName", EmployerName));
            rw.SubreportProcessingEventHandlers.Add(
                new SubreportProcessingEventHandler(SubreportProcessingEventHandler));
            rw.FileName = "(測試用)CaseServiceRecord_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss");
            // Pass report info via session
            Session["ReportWrapper"] = rw;
            // Go report viewer page
            return Redirect("~/Report/ReportViewer.aspx");
        }

        /// <summary>多筆-個案服務紀錄表</summary>
        /// <param name="AttendantID"></param>
        /// <param name="EmployerID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="EmployerName"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult Report_CaseServiceRecord(List<int> AttendantID, List<int> EmployerID, List<int> Year, List<int> Month, List<String> EmployerName, List<String> AttendantName, String lsEmail, String Type)
        {
            string result = "";
            var b = false;
            var lsMailAddress = new List<MailAddress>();
            var lsMail = new List<String>(lsEmail.Split(','));
            try
            {
                foreach (var item in lsMail)
                {
                    lsMailAddress.Add(new MailAddress(item));
                }
                b = true;
            }
            catch (Exception)
            {
                result = "100";
            }
            try
            {
                // Prepare data in report
                if (AttendantID != null && b)
                {
                    List<ReportWrapper> lsRW = new List<ReportWrapper>();
                    var lsDataTable = db.getCaseServiceRecord(AttendantID, EmployerID, Year, Month);
                    for (int a = 0; a < lsDataTable.Count; a++)
                    {
                        var dt = lsDataTable[a];

                        #region 計算子報表需要執行幾次

                        dt.Columns.Add("pageNum");
                        int pageCount = 0;
                        int schedule_uid = 0;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            int j = int.Parse(dt.Rows[i]["schedule_uid"].ToString());
                            if (schedule_uid != j)
                            {
                                schedule_uid = j;
                                pageCount++;
                            }
                            if (pageCount % 2 == 0)
                            {
                                dt.Rows[i]["pageNum"] = pageCount - 1;
                            }
                            else
                            {
                                dt.Rows[i]["pageNum"] = pageCount;
                            }
                        }

                        #endregion

                        #region Set report info

                        ReportWrapper rw = new ReportWrapper();
                        rw.ReportPath = Server.MapPath("~/Report/Rdlc/rptCaseServiceRecord.rdlc");
                        rw.ReportDataSources.Add(new ReportDataSource("DataSet", dt));
                        rw.ReportParameters.Add(new ReportParameter("rptParameterAttendantName", AttendantName[a]));
                        rw.ReportParameters.Add(new ReportParameter("rptParameterEmployerName", EmployerName[a]));
                        rw.SubreportProcessingEventHandlers.Add(new SubreportProcessingEventHandler(SubreportProcessingEventHandler));
                        rw.FileType = Type;
                        if (Month[a] < 10)
                        {
                            rw.FileName = "(" + (Year[a] - 1911) + "0" + Month[a] + ")" + EmployerName[a] + "_" + AttendantName[a];
                        }
                        else
                        {
                            rw.FileName = "(" + (Year[a] - 1911) + Month[a] + ")" + EmployerName[a] + "_" + AttendantName[a];
                        }
                        lsRW.Add(rw);

                        #endregion

                    }

                    #region 產生報表&send email

                    var lsFilePath = new List<String>();
                    for (int i = 0; i < lsRW.Count; i++)
                    {
                        var rw = lsRW[i];
                        if (rw != null)
                        {
                            #region Report

                            var RptViewer = new Microsoft.Reporting.WebForms.ReportViewer();
                            // Rdlc location
                            RptViewer.LocalReport.ReportPath = rw.ReportPath;

                            // Set report data source
                            RptViewer.LocalReport.DataSources.Clear();
                            foreach (var reportDataSource in rw.ReportDataSources)
                            { RptViewer.LocalReport.DataSources.Add(reportDataSource); }

                            //SubreportProcessingEventHandler
                            foreach (var SubreportProcessingEventHandler in rw.SubreportProcessingEventHandlers)
                            { RptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler); }

                            // Set report parameters
                            RptViewer.LocalReport.SetParameters(rw.ReportParameters);

                            RptViewer.LocalReport.Refresh();

                            Warning[] warnings;
                            string[] streamids;
                            string mimeType;
                            string encoding;
                            string extension;

                            byte[] bytes = null;

                            String filePath = Path.Combine(Path.GetTempPath(), rw.FileName);

                            switch (rw.FileType)
                            {
                                case "Excel":
                                    {
                                        bytes = RptViewer.LocalReport.Render(
                                                               "Excel", null, out mimeType, out encoding, out extension,
                                                                out streamids, out warnings);
                                        filePath += ".xls";
                                        break;
                                    }
                                case "PDF":
                                    {
                                        bytes = RptViewer.LocalReport.Render(
                                                               "PDF", null, out mimeType, out encoding, out extension,
                                                                out streamids, out warnings);
                                        filePath += ".pdf";
                                        break;
                                    }
                                case "Word":
                                    {
                                        bytes = RptViewer.LocalReport.Render(
                                                               "Word", null, out mimeType, out encoding, out extension,
                                                                out streamids, out warnings);
                                        filePath += ".doc";
                                        break;
                                    }
                            }


                            using (var fs = new FileStream(filePath, FileMode.Create))
                            {
                                fs.Write(bytes, 0, bytes.Length);
                                fs.Close();
                            }
                            lsFilePath.Add(filePath);
                            RptViewer.Dispose();

                            #endregion
                        }
                    }

                    #region Send Mail

                    using (ZipFile zip = new ZipFile(System.Text.Encoding.UTF8))
                    {
                        String file = Path.GetTempPath() + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".zip";
                        String filePassword = "";
                        Random rnd = new Random();
                        //產生4個數字的亂數
                        for (int ctr = 0; ctr < 4; ctr++)
                        {
                            filePassword += rnd.Next(0, 10).ToString();
                        }
                        for (int i = 0; i < lsFilePath.Count; i++)
                        {
                            zip.Password = filePassword.ToString();
                            zip.AddFile(lsFilePath[i], "");
                        }
                        zip.Save(file);

                        SMTPService.MailSender ms = new SMTPService.MailSender();
                        String title = "個案服務紀錄表";
                        String content = "發送者:administrator";
                        var lsAttachment = new List<Attachment>();
                        lsAttachment.Add(new Attachment(file));
                        ms.Send(title, content, lsMailAddress, lsAttachment);
                        result = filePassword;
                        filePassword = "";
                    }

                    #endregion

                    #endregion

                }
            }
            catch (Exception)
            {
                result = "-1";
                throw;
            }
            return Json(result);
        }


        /// <summary>填入未選擇資料</summary>
        /// <param name="str"></param>
        /// <param name="answerName"></param>
        /// <returns></returns>
        private String FillNoSelectedItem(String[] str, String answerName)
        {
            String s = null;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].Replace("□", "") == answerName)
                {
                    str[i] = str[i].Replace("□", "■");
                }
                s += str[i] + " ";
            }
            return s;
        }

        /// <summary>子報表</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubreportProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            switch (e.ReportPath)
            {
                #region subRptWorkRecord

                case "subRptWorkRecord":
                    {
                        int pageNum = int.Parse(e.Parameters["RowNum"].Values[0].ToString());
                        int pageRowLimit = int.Parse(e.Parameters["rptParameterPageRowLimit"].Values[0].ToString());
                        List<string> strWhere = new List<string>(e.Parameters["strWhere"].Values[0].ToString().Split(','));
                        List<string> min1 = new List<string>(e.Parameters["rptParameterMin1"].Values[0].ToString().Split(','));
                        List<string> min2 = new List<string>(e.Parameters["rptParameterMin2"].Values[0].ToString().Split(','));
                        List<string> day = new List<string>(e.Parameters["rptParameterDay"].Values[0].ToString().Split(','));
                        List<string> count_min1 = new List<string>(e.Parameters["rptParameterCountMin1"].Values[0].ToString().Split(','));
                        List<string> count_min2 = new List<string>(e.Parameters["rptParameterCountMin2"].Values[0].ToString().Split(','));
                        List<string> total_time = new List<string>(e.Parameters["rptParameterCountTotalTime"].Values[0].ToString().Split(','));
                        DataTable dt = null;
                        // Prepare data in report
                        if (pageNum < strWhere.Count) { dt = db.selectWorkRecordDataTable(strWhere[pageNum]); }

                        if (dt != null)
                        {
                            dt.Columns.Add("min1");
                            dt.Columns.Add("min2");
                            dt.Columns.Add("day");
                            dt.Columns.Add("count_min1");
                            dt.Columns.Add("count_min2");
                            dt.Columns.Add("total_time");
                            foreach (DataRow item in dt.Rows)
                            {
                                item["min1"] = min1[pageNum];
                                item["min2"] = min2[pageNum];
                                item["day"] = day[pageNum];
                                item["count_min1"] = count_min1[pageNum];
                                item["count_min2"] = count_min2[pageNum];
                                item["total_time"] = total_time[pageNum];
                            }

                            #region 補足空白欄

                            if (pageNum == strWhere.Count - 1)
                            {
                                String str = (strWhere[pageNum].Replace("a.uid = ", "")).Replace(" OR ", ",");
                                List<String> lsStr = new List<string>(str.Split(','));
                                if (lsStr.Count < pageRowLimit)
                                {
                                    //排序，避免造成無法正常新增空白蘭
                                    lsStr.Sort();
                                    for (int i = lsStr.Count; i < 10; i++)
                                    {
                                        int tmp = int.Parse(lsStr[lsStr.Count - 1]) + 1;
                                        dt.Rows.Add(tmp,
                                            dt.Rows[0]["ServiceID"],
                                            dt.Rows[0]["Service_ItemID"],
                                            "",
                                            "",
                                            "",
                                            "9999-12-12 11:11:11",
                                            "9999-12-12 11:11:11",
                                            "9999-12-12 11:11:11",
                                            "9999-12-12 11:11:11",
                                            "0");
                                        lsStr.Add(tmp.ToString());
                                    }
                                }
                            }

                            #endregion
                        }

                        e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet", dt));
                        break;
                    }

                #endregion

                #region subRptCaseServiceRecord

                case "subRptCaseServiceRecord":
                    {
                        int uid = int.Parse(e.Parameters["schedule_uid"].Values[0].ToString());
                        var dt = db.getCaseServiceRecord(uid);

                        #region 填入未選擇項目

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            String itemName = dt.Rows[i]["itemName"].ToString();
                            String answerName = dt.Rows[i]["answerName"].ToString();
                            switch (itemName)
                            {
                                case "意識狀況":
                                    {
                                        String[] str = { "□意識清楚", "□意識模糊", "□意識不清" };
                                        dt.Rows[i]["answerName"] = FillNoSelectedItem(str, answerName);
                                        break;
                                    }
                                case "食慾狀況":
                                    {
                                        String[] str = { "□穩定", "□食慾不振" };
                                        dt.Rows[i]["answerName"] = FillNoSelectedItem(str, answerName);
                                        break;
                                    }
                                case "心理狀況":
                                    {
                                        String[] str = { "□喜樂", "□悶悶不樂", "□憂傷" };
                                        dt.Rows[i]["answerName"] = FillNoSelectedItem(str, answerName);
                                        break;
                                    }
                                case "皮膚狀況":
                                    {
                                        String[] str = { "□正常", "□乾燥", "□傷口：部位 _____________________" };
                                        dt.Rows[i]["answerName"] = FillNoSelectedItem(str, answerName);
                                        break;
                                    }
                                case "排便狀況":
                                    {
                                        String[] str = { "□有形", "□無形", "□軟", "□硬", "□水便", "□無" };
                                        dt.Rows[i]["answerName"] = FillNoSelectedItem(str, answerName);
                                        break;
                                    }
                                case "用藥狀況":
                                    {
                                        String[] str = { "□穩定", "□須提醒", "□無按時服藥" };
                                        dt.Rows[i]["answerName"] = FillNoSelectedItem(str, answerName);
                                        break;
                                    }
                                case "處遇":
                                    {
                                        String[] str = { "□衛教/叮嚀個案或案家屬", "□協助求救110或119" };
                                        dt.Rows[i]["answerName"] = FillNoSelectedItem(str, answerName);
                                        break;
                                    }
                                case "備註":
                                    {
                                        if (dt.Rows[i]["summary"].ToString() != "")
                                        {
                                            dt.Rows[i]["answerName"] = dt.Rows[i]["summary"].ToString().Replace("\n"," ");
                                        }
                                        else
                                        {
                                            dt.Rows[i]["answerName"] = "________________________";
                                        }

                                        break;
                                    }
                            }
                            dt.Rows[i]["itemName"] += "：";
                        }

                        #endregion

                        e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet", dt));
                        break;
                    }

                #endregion
            }
        }



        #endregion

        #region Schedule

        /// <summary>取得督導列表&服務項目</summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult getScheduleInitialData()
        {
            List<InfoModel> SupervisorList = db.selectSupervisorName();
            List<ServiceItemModel> serviceItem = db.selectServiceItme();
            var data = new Dictionary<string, object>();
            data.Add("SupervisorList", SupervisorList);
            data.Add("serviceItem", serviceItem);
            return Json(data);
        }

        /// <summary>取得督導旗下照服員清單&案主姓名&照服員的Schedule</summary>
        /// <param name="account_uid">督導編號</param>
        /// <returns></returns>
        [Authorize]
        public JsonResult getSupervisorData(int account_uid)
        {
            List<InfoModel> AttendantList = db.selectAttendantList(account_uid);
            List<InfoModel> EmployerList = db.selectEmployerList(account_uid);
            List<ScheduleModel> Schedule = db.selectSchedule(account_uid);
            var data = new Dictionary<string, object>();
            data.Add("AttendantList", AttendantList);
            data.Add("EmployerList", EmployerList);
            data.Add("Schedule", Schedule);
            return Json(data);
        }

        /// <summary>檢查工作時段是否重疊</summary>
        /// <param name="account_uid">照服員編號</param>
        /// <returns></returns>
        [Authorize]
        public JsonResult cheackWorkDateTime(int AttendantID, DateTime start, DateTime end, Boolean editMode, int taskID)
        {
            int i = db.selectRepeatDate(AttendantID, start, end, editMode, taskID);
            double j = db.selectWorkTime_Day(AttendantID, start);
            var data = new Dictionary<String, object>();
            data.Add("RepeatDate", i);
            data.Add("WorkTime_Day", (j / 60).ToString("F1"));
            return Json(data);
        }

        /// <summary>取得照服員當月已排定時數&薪資&到職日</summary>
        /// <param name="AttendantID">照服員編號</param>
        /// <returns></returns>
        [Authorize]
        public JsonResult getAttendantWorkInfo(int AttendantID)
        {
            var data = db.selectAttendanWorkInfo(AttendantID);
            double d = double.Parse(data["worktime"].ToString()) / 60;
            data["worktime"] = d.ToString("F1");
            return Json(data);
        }

        /// <summary>取得案主當月已排定時數&核定時數&自費時數</summary>
        /// <param name="EmployerID">案主編號</param>
        /// <returns></returns>
        [Authorize]
        public JsonResult getEmployerTime(int EmployerID)
        {
            var list = db.selectEmployerTime(EmployerID);
            var data = new Dictionary<String, String>();
            if (list != null)
            {
                data.Add("minutes", (list[0] / 60).ToString("F1"));
                data.Add("minutes1", (list[1] / 60).ToString("F1"));
                data.Add("minutes2", (list[2] / 60).ToString("F1"));
                data.Add("total", ((list[1] + list[2]) / 60).ToString("F1"));
            }
            else
            {
                data = null;
            }
            return Json(data);
        }

        /// <summary>建立班表</summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult setScheduleData(ScheduleModel schedule, Boolean Repeat)
        {
            int result = db.insertScheduleData(schedule);
            if (Repeat)
            {
                Boolean b = false;
                while (!b)
                {
                    DateTime start = DateTime.Parse(schedule.start);
                    DateTime end = DateTime.Parse(schedule.end);
                    if (start.AddDays(7).ToString("yyyy/MM") == start.ToString("yyyy/MM"))
                    {
                        schedule.start = start.AddDays(7).ToString("yyyy/MM/dd HH:mm:ss");
                        schedule.end = end.AddDays(7).ToString("yyyy/MM/dd HH:mm:ss");
                        if (db.selectRepeatDate(schedule.AttendantID, DateTime.Parse(schedule.start), DateTime.Parse(schedule.end), false, 0) == 0)
                        {
                            result += db.insertScheduleData(schedule);
                        }
                    }
                    else
                    {
                        b = true;
                    }
                }
            }
            return Json(result);
        }

        /// <summary>修改班表</summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public JsonResult setNewScheduleData(ScheduleModel schedule)
        {
            int i = db.updateScheduleData(schedule);
            return Json(i);
        }

        /// <summary>刪除班表</summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult delectSchedule(int taskId)
        {
            int i = db.delectScheduleData(taskId);
            return Json(i);
        }


        #endregion

        #region Member

        /// <summary>取得督導、照服員、案主資料</summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult getMemberInitData()
        {
            var data = new Dictionary<String, Object>();
            data.Add("memberlist", db.selectAllMemberInfo());
            data.Add("InfoItem", db.selectInfoItem());
            return Json(data);
        }

        /// <summary>新增使用者-督導&管理員</summary>
        /// <param name="info">資料</param>
        /// <param name="password">密碼</param>
        /// <param name="level">階級</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult setNewMember_Other(InfoModel info, String password, int level)
        {
            password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            int i = db.insertMember_Other(info, password, level);
            return Json(i);
        }

        /// <summary>新增使用者-照服員</summary>
        /// <param name="info">資料</param>
        /// <param name="password">密碼</param>
        /// <param name="level">階級</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult setNewMember_Attendant(Info_AttendantModel info, String password, int level)
        {
            password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            int i = db.insertMember_Attendant(info, password, level);
            return Json(i);
        }


        /// <summary>新增使用者-案主</summary>
        /// <param name="info">資料</param>
        /// <param name="password">密碼</param>
        /// <param name="level">階級</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult setNewMember_Employer(Info_EmployerModel info, String password, int level)
        {
            password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            int i = db.insertMember_Employer(info, password, level);
            return Json(i);
        }

        /// <summary>新增使用者-督導</summary>
        /// <param name="info">資料</param>
        /// <param name="password">密碼</param>
        /// <param name="level">階級</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult setNewMember_Supervisor(Info_SupervisorModel info, String password, int level)
        {
            password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            int i = db.insertMember_Supervisor(info, password, level);
            return Json(i);
        }


        /// <summary>檢查使用者名稱是否重複</summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult checkRepeat(String username)
        {
            int i = db.selectUsername(username);
            return Json(i);
        }

        /// <summary>更新管理員&督導資料</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult setMember_Other(InfoModel info, String password, int level)
        {
            if (password != "")
            {
                password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            }
            int i = db.updateMember_Other(info, password, level);
            return Json(i);
        }

        /// <summary>更新照服員資料</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult setMember_Attendant(Info_AttendantModel info, String password, int level)
        {
            if (password != "")
            {
                password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            }
            int i = db.updateMember_Attendant(info, password, level);
            return Json(i);
        }

        /// <summary>更新案主資料</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult setMember_Employer(Info_EmployerModel info, String password, int level)
        {
            if (password != "")
            {
                password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            }
            int i = db.updateMember_Employer(info, password, level);
            return Json(i);
        }

        /// <summary>更新督導資料</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult setMember_Supervisor(Info_SupervisorModel info, String password, int level)
        {
            if (password != "")
            {
                password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            }
            int i = db.updateMember_Supervisor(info, password, level);
            return Json(i);
        }

        /// <summary>刪除成員</summary>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult setMemberlevel(int account_uid)
        {
            int i = db.delectMember(account_uid);
            return Json(i);
        }

        #endregion

        #region Map

        /// <summary>取得督導資料</summary>
        /// <returns></returns>
        public JsonResult getAllSupervisorData()
        {
            var data = db.selectSupervisorData();
            var list = from item in data.AsEnumerable()
                       select new InfoModel
                       {
                           account_uid = item.Field<int>("account_uid"),
                           showName = item.Field<String>("displayname") + "（" + item.Field<String>("phone1") + "）"
                       };
            return Json(list);
        }

        /// <summary>取得案主服務狀態</summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public JsonResult getEmployerDataForSupervisorID(int uid)
        {
            var data = db.selectEmployerDataForSupervisorID(uid);
            var list = from item in data.AsEnumerable()
                       select new InfoModel
                       {
                           account_uid = item.Field<int>("account_uid"),
                           displayname = item.Field<String>("displayname"),
                           phone1 = item.Field<String>("phone1"),
                           address = item.Field<String>("address"),
                           online = item.Field<long>("online")
                       };
            return Json(list);
        }

        #endregion

    }
}
