using System.Collections.Generic;

namespace DocumentService.Word
{
    public class TableData
    {
        public int TablePos { get; set; }
        public List<Dictionary<string, string>> Data { get; set; }
    }
}
