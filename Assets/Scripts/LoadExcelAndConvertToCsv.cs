using System.IO;
using DataConverters;
using DataLoaders;
using UnityEngine;

public class LoadExcelAndConvertToCsv : UnityEngine.MonoBehaviour
{
    private const string FilePath = "TestSpreadsheet.xlsx";
        
    private void Start()
    {
        IMemoryStreamLoader loader = new MemoryStreamLoader();
        loader.Load(FilePath, OnLoaded, OnLoadFailed);
    }

    private void OnLoaded(MemoryStream stream)
    {
        ExcelToCsvWithJavascript excelToCsv = new ExcelToCsvWithJavascript();
        string csv = excelToCsv.Convert(stream);
            
        Debug.Log($"C# rezult received : \n {csv}");
    }

    private void OnLoadFailed(string message) =>
        Debug.LogWarning($"not Loaded:\n{message}");
}