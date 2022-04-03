using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using Perfolizer.Horology;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.BencharkTest.Exporters
{
    public class EcsContextCsvExcelExporter : ExporterBase
    {
        public static IExporter Default { get; set; } = new EcsContextCsvExcelExporter();
        protected override string FileExtension => "csv";
        protected override string FileCaption => "context-report";

        public EcsContextCsvExcelExporter()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public override void ExportToLog(Summary summary, ILogger logger)
        {
            var filePath = summary.ResultsDirectoryPath;
            var fileName = $"{summary.Title}-context-report.xlsx";
            var fullName = Path.Combine(filePath, fileName);
            using (var package = new ExcelPackage(new FileInfo(fullName)))
            {
                var worksheet = package.Workbook.Worksheets.Add("Context");
                var strBuilder = new StringBuilder();
                var dataList = GetDatas(summary);
                var allContexts = ((EcsContextType[])Enum.GetValues(typeof(EcsContextType)))
                    .Select(x => x.ToString())
                    .ToList();
                var allComArrs = ((EntityComponentArrangement[])Enum.GetValues(typeof(EntityComponentArrangement)))
                    .Select(x => x.ToString())
                    .ToList();

                var row = 1;
                var contextStartCol = 4;
                var contextEndCol = contextStartCol + allContexts.Count;

                // Header
                worksheet.Cells[row, 1].Value = "Type";
                worksheet.Cells[row, 2].Value = "Method";
                worksheet.Cells[row, 3].Value = "ComponentArrangement";
                for (int i = 0; i < allContexts.Count; i++)
                    worksheet.Cells[1, i + 4].Value = allContexts[i];
                row++;
                strBuilder.AppendLine($"Type,Method,ComponentArrangement,{ string.Join(",", allContexts)}");

                // Content
                var allTotals = new decimal[allContexts.Count];
                var methodTotals = new decimal[allContexts.Count];
                var methodCsvMeans = "";

                var types = dataList
                    .Select(x => x.Type)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
                foreach (var type in types)
                {
                    var methods = dataList
                        .Where(x => x.Type == type)
                        .Select(x => x.Method)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();
                    foreach (var method in methods)
                    {
                        if (dataList.Any(x => x.Type == type && x.Method == method && allComArrs.Contains(x.ComponentArrangement)))
                        {
                            for (int i = 0; i < allComArrs.Count; i++)
                            {
                                var comArr = allComArrs[i];
                                var contexts = dataList
                                    .Where(x => x.Type == type && x.Method == method && x.ComponentArrangement == comArr)
                                    .Select(x => x.EcsContext)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToList();

                                if (contexts.Count > 0)
                                {
                                    for (int j = 0; j < contexts.Count; j++)
                                    {
                                        var data = dataList
                                            .Where(x => x.Type == type && x.Method == method &&
                                                x.ComponentArrangement == comArr && x.EcsContext == allContexts[j])
                                            .FirstOrDefault();

                                        if (data != null)
                                        {
                                            allTotals[j] += data.Mean;
                                            methodTotals[j] += data.Mean;

                                            worksheet.Cells[row, 1].Value = type;
                                            worksheet.Cells[row, 2].Value = method;
                                            worksheet.Cells[row, 3].Value = comArr;
                                            worksheet.Cells[row, j + contextStartCol].Value = data.Mean;
                                            methodCsvMeans += $",{data.Mean}";
                                        }
                                    }
                                    ExcelCreateThreeColorScaleRule(worksheet, new ExcelAddress(row, contextStartCol, row, contextStartCol + contextEndCol));
                                    strBuilder.AppendLine($"{type},{method},{comArr}{methodCsvMeans}");
                                    row++;
                                    methodCsvMeans = "";
                                }
                            }
                            ExcelWriteTotals(worksheet, "MethodTotals", methodTotals, row++, contextStartCol, contextEndCol);
                            Array.Clear(methodTotals, 0, methodTotals.Length);
                        }
                        else
                        {
                            var contexts = dataList
                                       .Where(x => x.Type == type && x.Method == method)
                                       .Select(x => x.EcsContext)
                                       .Distinct()
                                       .OrderBy(x => x)
                                       .ToList();

                            if (contexts.Count > 0)
                            {
                                for (int j = 0; j < contexts.Count; j++)
                                {
                                    var data = dataList
                                        .Where(x => x.Type == type && x.Method == method && x.EcsContext == allContexts[j])
                                        .FirstOrDefault();

                                    if (data != null)
                                    {
                                        allTotals[j] += data.Mean;

                                        worksheet.Cells[row, 1].Value = type;
                                        worksheet.Cells[row, 2].Value = method;
                                        worksheet.Cells[row, 3].Value = "?";
                                        worksheet.Cells[row, j + contextStartCol].Value = data.Mean;
                                        methodCsvMeans += $",{data.Mean}";
                                    }
                                }
                                ExcelCreateThreeColorScaleRule(worksheet, new ExcelAddress(row, contextStartCol, row, contextStartCol + contextEndCol));
                                strBuilder.AppendLine($"{type},{method},?{methodCsvMeans}");
                                row++;
                                methodCsvMeans = "";
                            }
                        }
                    }
                }
                ExcelWriteTotals(worksheet, "AllTotals", allTotals, row, contextStartCol, contextEndCol);

                for (int i = 0; i < contextStartCol + allContexts.Count; i++)
                    worksheet.Cells[1, i + 1].AutoFitColumns();

                package.Save();
                logger.Write(strBuilder.ToString());
            }
        }

        private void ExcelWriteTotals(ExcelWorksheet worksheet, string name, decimal[] totals, int row, int contextStartCol, int contextEndCol)
        {
            worksheet.Cells[row, 1].Value = name;
            for (int i = 0; i < contextEndCol - contextStartCol; i++)
                worksheet.Cells[row, i + contextStartCol].Value = totals[i];
            ExcelCreateThreeColorScaleRule(worksheet, new ExcelAddress(row, contextStartCol, row, contextStartCol + contextEndCol));
        }

        private void ExcelCreateThreeColorScaleRule(ExcelWorksheet worksheet, ExcelAddress address)
        {
            var rule = worksheet.ConditionalFormatting.AddThreeColorScale(address);

            rule.LowValue.Color = Color.FromArgb(99, 190, 123);
            rule.HighValue.Color = Color.FromArgb(248, 105, 107);
            rule.MiddleValue.Color = Color.FromArgb(255, 255, 132);
            rule.MiddleValue.Type = eExcelConditionalFormattingValueObjectType.Percent;
        }

        private List<CustomCsvData> GetDatas(Summary summary)
        {
            var typeCol = summary.Table.Columns.FirstOrDefault(x => x.Header == "Type");
            var methodCol = summary.Table.Columns.FirstOrDefault(x => x.Header == "Method");
            var comArrCol = summary.Table.Columns.FirstOrDefault(x => x.Header == "ComponentArrangement");
            var contextCol = summary.Table.Columns.FirstOrDefault(x => x.Header == "ContextType");
            var meanCol = summary.Table.Columns.FirstOrDefault(x => x.Header == "Mean");

            var bestTimeUnit = TimeUnit.GetBestTimeUnit(summary.Reports.Select(x => x.ResultStatistics.Mean).ToArray());
            var timedMeans = summary.Reports
                .Select(x => TimeUnit.Convert(x.ResultStatistics.Mean, TimeUnit.Nanosecond, bestTimeUnit))
                .ToArray();
            var data = new List<CustomCsvData>();
            for (int i = 0; i < summary.Table.FullContent.Count(); i++)
            {
                var line = summary.Table.FullContent[i];
                data.Add(new CustomCsvData
                {
                    Type = typeCol != null
                        ? line[typeCol.Index]
                        : "?",
                    Method = methodCol != null
                        ? line[methodCol.Index]
                        : "?",
                    ComponentArrangement = comArrCol != null
                        ? line[comArrCol.Index]
                        : "?",
                    EcsContext = contextCol != null
                        ? line[contextCol.Index] == "Nativ(...)inous [26]"
                            ? "Native_ArcheType_Continous"
                            : line[contextCol.Index]
                        : "?",
                    Mean = meanCol != null
                        ? Decimal.Round((decimal)timedMeans[i], 3, MidpointRounding.AwayFromZero)
                        : 0,
                });
            }

            return data;
        }

        private struct CustomKey : IEquatable<CustomKey>, IComparable<CustomKey>
        {
            public string Type { get; set; }
            public string Method { get; set; }
            public string ComponentArrangement { get; set; }

            public static bool operator !=(CustomKey lhs, CustomKey rhs) => !(lhs == rhs);

            public static bool operator ==(CustomKey lhs, CustomKey rhs) =>
                lhs.Type == rhs.Type &&
                lhs.Method == rhs.Method &&
                lhs.ComponentArrangement == rhs.ComponentArrangement;

            public int CompareTo(CustomKey other)
            {
                var compare = Type.CompareTo(other.Type);
                if (compare == 0)
                    compare = Method.CompareTo(Method);
                if (compare == 0)
                    compare = ComponentArrangement.CompareTo(ComponentArrangement);
                return compare;
            }

            public bool Equals(CustomKey other) => this == other;

            public override bool Equals(object other) => other is CustomKey obj && this == obj;

            public override int GetHashCode()
            {
                int hashCode = -612338121;
                hashCode = hashCode * -1521134295 + Type.GetHashCode();
                hashCode = hashCode * -1521134295 + Method.GetHashCode();
                hashCode = hashCode * -1521134295 + ComponentArrangement.GetHashCode();
                return hashCode;
            }
        }

        private class CustomCsvData
        {
            public string Type { get; set; }
            public string Method { get; set; }
            public string ComponentArrangement { get; set; }
            public string EcsContext { get; set; }
            public decimal Mean { get; set; }
        }
    }
}
