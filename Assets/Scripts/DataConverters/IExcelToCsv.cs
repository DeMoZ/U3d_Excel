using System.IO;

namespace DataConverters
{
    public interface IExcelToCsv
    {
        string Convert(MemoryStream stream);
    }
}