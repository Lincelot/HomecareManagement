using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;

namespace HomecareManagement.Report
{
    public class ReportWrapper
    {
        // Constructors
        public ReportWrapper()
        {
            ReportDataSources = new List<ReportDataSource>();
            ReportParameters = new List<ReportParameter>();
            SubreportProcessingEventHandlers = new List<SubreportProcessingEventHandler>();
        }


        // Properties
        public string ReportPath { get; set; }

        public List<ReportDataSource> ReportDataSources { get; set; }

        public List<ReportParameter> ReportParameters { get; set; }

        public List<SubreportProcessingEventHandler> SubreportProcessingEventHandlers { get; set; }

        public bool IsDownloadDirectly { get; set; }

        public String FileName { get; set; }

        public String FileType { get; set; }
    }
}