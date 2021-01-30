using System;
using System.Collections;
using System.IO;

namespace DataLoaders
{
    public interface IMemoryStreamLoader
    {
        void Load(string fileName, Action<MemoryStream> onCompleted, Action<string> onFailed);
    }
}