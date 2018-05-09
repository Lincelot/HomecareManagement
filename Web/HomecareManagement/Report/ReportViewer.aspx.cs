using HomecareManagement.Service;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Web.Security;

namespace HomecareManagement.Report
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GenerateReport();
                DisableUnwantedExportFormat(RptViewer, "PDF");
            }
        }

        private void GenerateReport()
        {
            var ReportWrapperSessionKey = "ReportWrapper";

            var rw = (ReportWrapper)Session[ReportWrapperSessionKey];
            if (rw != null)
            {
                // Rdlc location
                RptViewer.LocalReport.ReportPath = rw.ReportPath;

                // Set report data source
                RptViewer.LocalReport.DataSources.Clear();
                foreach (var reportDataSource in rw.ReportDataSources)
                { RptViewer.LocalReport.DataSources.Add(reportDataSource); }

                //SubreportProcessingEventHandler
                foreach (var SubreportProcessingEventHandler in rw.SubreportProcessingEventHandlers)
                {
                    RptViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
                }

                // Set report parameters
                RptViewer.LocalReport.SetParameters(rw.ReportParameters);

                // Refresh report
                RptViewer.LocalReport.Refresh();
            }


            // Remove session
            Session[ReportWrapperSessionKey] = null;
            Session.Clear();
        }

        /// <summary>隱藏指定輸出格式</summary>
        /// <param name="ReportViewerID"></param>
        /// <param name="strFormatName"></param>
        public void DisableUnwantedExportFormat(Microsoft.Reporting.WebForms.ReportViewer ReportViewerID, string strFormatName)
        {
            FieldInfo info;

            foreach (RenderingExtension extension in ReportViewerID.LocalReport.ListRenderingExtensions())
            {
                if (extension.Name == strFormatName)
                {
                    info = extension.GetType().GetField("m_isVisible", BindingFlags.Instance | BindingFlags.NonPublic);
                    info.SetValue(extension, false);
                }
            }
        }
    }
}