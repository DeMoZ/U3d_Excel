using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace DataConverters
{
    public class ExcelToCsvWithNpoi : IExcelToCsv
    {
        public string Convert(MemoryStream stream)
        {
            Debug.Log($"stream to array: {stream.ToArray()}");

            IWorkbook book;
            book = new XSSFWorkbook(stream); // xlsx

            if (book == null)
                book = new HSSFWorkbook(stream); // xls

            Debug.Log($"stream unpacked");
            Debug.Log($"sheets total {book.NumberOfSheets}");
            StringBuilder stringBuilder = new StringBuilder();
            
            for (int sh = 0; sh < book.NumberOfSheets; sh++)
            {
                ISheet sheet = book.GetSheetAt(sh);
                Debug.Log($"sheet name: {sheet.SheetName}");
                stringBuilder.Append(sheet.SheetName);
                stringBuilder.Append("\n");

                for (int rw = 0; rw < sheet.LastRowNum; rw++)
                {
                    IRow row = sheet.GetRow(rw);
                    if (row == null) continue;

                    for (int cl = 0; cl < row.Cells.Count; cl++)
                    {
                        ICell cell = row.GetCell(cl);
                        string value = GetCellValueAsString(cell);
                        stringBuilder.Append(value);
                        stringBuilder.Append(",");
                    }

                    stringBuilder.Append("\n");
                }
            }

            var result = stringBuilder.ToString();
            Debug.Log("End convert");

            return result;
        }

        private string GetCellValueAsString(ICell cell)
        {
            string stringCellValue = string.Empty;

            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.String:
                        stringCellValue = cell.StringCellValue;
                        break;
                    case CellType.Numeric:
                        stringCellValue = cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                        break;
                    case CellType.Formula:
                        stringCellValue = "FORMULA";
                        break;
                    case CellType.Boolean:
                        stringCellValue = cell.BooleanCellValue.ToString();
                        break;
                    case CellType.Blank:
                        stringCellValue = String.Empty;
                        break;
                    default:
                        Debug.LogWarning("Unknown cell type");
                        break;
                }
            }
            //Debug.LogWarning($"cell[{cell.Row},{ce}]: {value}");

            return stringCellValue;
        }
    }
}