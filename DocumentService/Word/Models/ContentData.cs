namespace DocumentService.Word.Models
{
    public class ContentData
    {
        public string Placeholder { get; set; }
        public string Content { get; set; }
        public ContentType ContentType { get; set; }
        public ParentBody ParentBody { get; set; }
    }
}
