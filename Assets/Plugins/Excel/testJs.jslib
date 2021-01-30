mergeInto(LibraryManager.library, {
    HowToReturnString: function (str) {
        var returnStr = Pointer_stringify(str);
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },

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

    ExcelPathToCSV: function (url) {
        console.log("javascript read from " + url);

        // reader
        var req = new XMLHttpRequest;
        req.open("GET", url, true);
        req.responseType = "arraybuffer";
        req.onload = (function (e) {
            var data = new Uint8Array(req.response);

            console.log("javascript bytes : " + data);

            //=> -- check part!!---------------------------
            // convert to hex
            var hexStr = "";
            for (var i = 0; i < data.length; i++) {
                var hex = (data[i] & 255).toString(16);
                hex = hex.length === 1 ? "0" + hex : hex;
                hexStr += hex
            }
            console.log("javascript data to hex:  " + hexStr);

            // convert back
            var str = hexStr;
            var a = [];
            for (var i = 0, len = str.length; i < len; i += 2) {
                a.push(parseInt(str.substr(i, 2), 16));
            }
            data = new Uint8Array(a);
            console.log("javascript hex_to_byte:" + data);            
            //<=------------------------------------------
            // excel part
            var workbook = XLSX.read(data, {type: "array"});
            var sheetname = workbook.SheetNames[0];
            console.log("javascript sheetname:  " + sheetname);
            var sheetdata = XLSX.utils.sheet_to_csv(workbook.Sheets[sheetname]);
            console.log("javascript sheetdata: = " + sheetdata);

            var rezult = sheetdata;
        });
        req.onerror = (function (e) {
            alert("Error " + e.target.status + " occurred while receiving the document.")
        });
        req.send()
    },

    ByteToString: function (data) {
        if (!data) {
            return '';
        }

        var hexStr = "";
        for (var i = 0; i < data.length; i++) {
            var hex = (data[i] & 255).toString(16);
            hex = hex.length === 1 ? "0" + hex : hex;
            hexStr += hex
        }
        return hexStr;
    },

    StringTyByte: function (str) {
        if (!str) {
            return new Uint8Array();
        }

        var a = [];
        for (var i = 0, len = str.length; i < len; i += 2) {
            a.push(parseInt(str.substr(i, 2), 16));
        }

        return new Uint8Array(a);
    },
});