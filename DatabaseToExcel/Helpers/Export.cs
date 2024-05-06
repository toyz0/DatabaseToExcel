using OfficeOpenXml;
using System.Data;

namespace Helpers
{
    public class Export
    {
        public async Task<byte[]> ExcelAsync(DataTable dt, bool isAutoFitColumns = false, string password = null)
        {
            //Config EPPlus license
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Sheet1");
                sheet.Cells["A1"].LoadFromDataTable(dt, true);

                if (isAutoFitColumns)
                    SetAutoFitColumns(sheet);

                return await package.GetAsByteArrayAsync(password);
            }
        }

        private void SetAutoFitColumns(ExcelWorksheet workSheet)
        {
            var table = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
            table.AutoFitColumns();
        }
    }
}
