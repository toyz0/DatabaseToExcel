using OfficeOpenXml;
using System.Data;

namespace Helpers
{
    public class Export
    {
        static Export()
        {
            //Config EPPlus license
            //EPPlus 5+ is free for non-commercial use only; commercial use needs a paid license.
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        //Writes dt to filePath as a single sheet; overwrites the file if it already exists.
        public async Task ExcelAsync(DataTable dt, string filePath, bool isAutoFitColumns = false, string? password = null)
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Sheet1");
                //true: write the column names as the first row.
                sheet.Cells["A1"].LoadFromDataTable(dt, true);

                //EPPlus writes DateTime as a plain serial number unless the column has a date format.
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Columns[i].DataType != typeof(DateTime))
                        continue;

                    //DATE columns arrive as DateTime with midnight times; hide the 00:00:00 for them.
                    bool dateOnly = dt.Rows.Cast<DataRow>().All(r => r.IsNull(i) || ((DateTime)r[i]).TimeOfDay == TimeSpan.Zero);
                    sheet.Column(i + 1).Style.Numberformat.Format = dateOnly ? "yyyy-mm-dd" : "yyyy-mm-dd hh:mm:ss";
                }

                //Dimension is null when the sheet is empty.
                if (isAutoFitColumns && sheet.Dimension != null)
                    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

                //Save straight to disk so the whole workbook is not held in memory again as a byte[].
                await package.SaveAsAsync(new FileInfo(filePath), password);
            }
        }
    }
}
