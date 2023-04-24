using System.Collections.Generic;

namespace DocumentService.Word.Models
{
    public class TableData
    {
        public int TablePos { get; set; }
        public List<Dictionary<string, string>> Data { get; set; }
    }
}
