using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace DataConverters
{
    public class ExcelToCsvWithJavascript : IExcelToCsv
    {
#if (!UNITY_EDITOR && UNITY_WEBGL)
#pragma warning disable CA2101
        [DllImport("__Internal")]
        private static extern string ExcelHexToCSV(string hex);

        //  [DllImport("__Internal")]
        //  private static extern void ExcelPathToCSV(string fileName);
#pragma warning restore CA2101
#endif


        public string Convert(MemoryStream stream)
        {
            string hex = ByteArrayToString(stream.ToArray());
            Debug.Log($"C# byte to hex: {hex}");

            var rezult = string.Empty;
#if (!UNITY_EDITOR && UNITY_WEBGL)
#pragma warning disable CA2101
            Debug.Log("C# ExcelHexToCSV =================================================");
            rezult = ExcelHexToCSV(hex);
            Debug.Log($"C# rezult received : \n {rezult}");

            //  Debug.LogWarning("Read with javascript from disk ===================================");
            //  string fileName = "TestFile.xlsx";
            //  string filePath = string.Concat("StreamingAssets/", fileName);
            //  ExcelPathToCSV(filePath);

#pragma warning restore CA2101
#endif

            return rezult;
        }

        private string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}