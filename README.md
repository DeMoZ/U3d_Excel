# U3d_Excel
Read data from excel file

This started to solve the issue to read Excel file in Unity3d WebGL build and get the contained data as string formatted as csv.

Common Excel libraries (Microsoft.interop.excel, NPOI) that works fine for editor runtime/PC build/android build don’t work with Unity3d web build.


WebGL build can work with javascript functions and use javascript libraries, so here we go.

What is required:
1 JS library for excel (SheetJS for now).
2 Template for WebGL build.
3 javascript function that receive file and returns csv as string.
4 C# classes that read file from StreamingAssets, send into JS function and receive back convertion result.

1) JS library
       Get the xlsx.full.min.js file from 
       https://github.com/SheetJS/sheetjs
       put library into :
       \Assets\WebGLTemplates\ExcelTemplate\ExcelLibs\

2) Template for WebGL build.
       Create WebGL buld. In Build folder you will get index.html file. 
Open the html file with edior program (etc. notepad) and add script into the file:
       <script src="ExcelLibs/xlsx.full.min.js"></script>
       Save and close.
       Copy that html file into :
       Assets/WebGLTemplates/ExcelTemplate/
	Goto Unity ProjectSetting/Player/Resolution and Presentation.
	Select the template that you added (by the name of folder).

3) javascript function
	Create .jslib file and copy-paste
mergeInto(LibraryManager.library, {

    ExcelHexToCSV: function (hexStr) {
        console.log("javascript: ExcelHexToCSV");
        console.log("javascript received: " + Pointer_stringify(hexStr));

        // convert part
        var str = Pointer_stringify(hexStr);
        var a = [];
        for (var i = 0, len = str.length; i < len; i += 2) {
            a.push(parseInt(str.substr(i, 2), 16));
        }
        var data = new Uint8Array(a);
        console.log("javascript hex_to_byte:" + data);

        // excel part
        var workbook = XLSX.read(data, {type: "array"});
        var sheetname = workbook.SheetNames[0];
        console.log("javascript sheetname:  " + sheetname);
        var sheetdata = XLSX.utils.sheet_to_csv(workbook.Sheets[sheetname]);
        console.log("javascript sheetdata: = " + sheetdata);

        var rezult = sheetdata;

        var returnStr = rezult;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
}


       What the script does:
       Receives string witch is byte array converted to hex.
       Converts it back into array.
       Passes the array into SheetJS library.
       Returns csv as sting.
       
4) C# classes
	Now we need to :
       Download excel file as stream
		Get byte array from stream
		Convert to hex string
		Send to JS function and receive the reply

For reading file you can use any method you want. I red file from StreamingAssets with UnityWebRequest.
This way I receive MemoryStream and get byte[] from it. 
private string ByteArrayToString(byte[] ba)
{
    StringBuilder hex = new StringBuilder(ba.Length * 2);
    foreach (byte b in ba)
        hex.AppendFormat("{0:x2}", b);
    return hex.ToString();
}

The test class. Works on WebGL Build Only. See the console log in browser (F12). Use Build and Run. Otherwise you will get browser message with “blocked reading files from disk”.
This error can be fixed by letting your browser to work with files (this is unsafe)
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace DataConverters
{
    public class ExcelToCsvWithJavascript
    {
#if (!UNITY_EDITOR && UNITY_WEBGL)
#pragma warning disable CA2101
        [DllImport("__Internal")]
        private static extern string ExcelHexToCSV(string hex);
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

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace DataLoaders
{
    public class MemoryStreamLoader : IMemoryStreamLoader
    {
        public async void Load(string fileName, Action<MemoryStream> onComplete, Action<string> onFailed)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            Debug.Log(filePath);

            UnityWebRequest uwr = UnityWebRequest.Get(filePath);
            await uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
                onFailed?.Invoke(uwr.error);
            else
            {
                byte[] results = uwr.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                {
                    onComplete?.Invoke(stream);
                }
            }
        }
    }
}
	
