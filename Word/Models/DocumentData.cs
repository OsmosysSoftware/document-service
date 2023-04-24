using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentService.Word.Models
{
    public class DocumentData
    {
        public List<ContentData> Placeholders { get; set; }
        public List<TableData> TablesData { get; set; }
    }
}
