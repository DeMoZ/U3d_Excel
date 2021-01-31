using System.IO;
using DataConverters;
using DataLoaders;
using UnityEngine;

public class LoadExcelAndConvertToCsv : UnityEngine.MonoBehaviour
{
    private const string FilePath = "TestSpreadsheet.xlsx";

    [SerializeField] private ConvertLibrary _convertLibrary = ConvertLibrary.Npoi;

    private enum ConvertLibrary
    {
        SheetJs,
        Npoi
    }

    private void Start()
    {
        IMemoryStreamLoader loader = new MemoryStreamLoader();
        loader.Load(FilePath, OnLoaded, OnLoadFailed);
    }

    private void OnLoaded(MemoryStream stream)
    {
        IExcelToCsv excelToCsv = SwitchLibrary(_convertLibrary);
        string csv = excelToCsv.Convert(stream);

        Debug.Log($"C# rezult received : \n {csv}");
    }

    private IExcelToCsv SwitchLibrary(ConvertLibrary convertLibrary)
    {
#if UNITY_WEBGL || !UNITY_EDITOR
        return new ExcelToCsvWithJavascript();
#else
        if (convertLibrary == ConvertLibrary.Npoi) 
            return new ExcelToCsvWithNpoi();
        //else return new ExcelToCsvWithJavascript();
#endif
    }

    private void OnLoadFailed(string message) =>
        Debug.LogWarning($"not Loaded:\n{message}");
}