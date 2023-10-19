# DocumentService
DocumentService is a library with the following functions
1. **Generate Word documents** - Read Word document files as a template and replace the placeholder with actual data.
2. **Generate PDF documents** - Read an HTML file as a template and replace placeholders with actual data. Convert the HTML file to PDF

# Features

## Word document generation
- Replace placeholders in text paragraph with values.
- Replace placeholders in tables.
- Multiple placeholders in the same table cell/line/paragraph can be replaced.
- Populate table with new data.
- Replace images with image's placeholder. The image's postion will be maintained based on position of its placeholder image. Image size will be also be maintained based on placeholder image.

## PDF document generation
- Converts an HTML document to PDF.
- Replace placeholders in the document with actual string data.

# Basic usage

## PDF generation
```csharp
List<ContentMetaData> contentList = new List<ContentMetaData>
{
    new ContentMetaData { Placeholder = "Incident UID", Content = "I-20230822-001" },
    new ContentMetaData { Placeholder = "Description", Content = "Suspicious activity reported" },
    new ContentMetaData { Placeholder = "Site", Content = "Headquarters" }
};

// Tools\\index.html - The path of the html template file in which changes are to be made.
// Tools\\OutputFile.pdf - The path of the final pdf output file.
PdfDocumentGenerator.GeneratePdfByTemplate("Tools\\index.html", contentList, "Tools\\OutputFile.pdf");
```

## Word document generation
```csharp
string templateFilePath = @"C:\Users\Admin\Desktop\Osmosys\Work\Projects\Document Service Component\Testing\Document.docx";
string outputFilePath = @"C:\Users\Admin\Desktop\Osmosys\Work\Projects\Document Service Component\Testing\Test_Output.docx";

List<TableData> tablesData = new List<TableData>()
    {
        new TableData()
            {
                TablePos = 5,
                Data = new List<Dictionary<string, string>>()
                {
                    new Dictionary<string, string>()
                    {
                        { "Item Name", "1st med" },
                        { "Dosage", "1" },
                        { "Quantity", "1" },
                        { "Precautions", "Take care" }
                    },
                    new Dictionary<string, string>()
                    {
                        { "Item Name", "2nd med" },
                        { "Dosage", "1" },
                        { "Quantity", "1" },
                    }
                }
            }
    };

    List<ContentData> contents = new List<ContentData>()
    {
        new ContentData
        {
            Placeholder = "Picture 1",
            Content = @"../testImage1.jpg",
            ContentType = ContentType.Image,
            ParentBody = ParentBody.None
        },
        new ContentData
        {
            Placeholder = "Picture 2",
            Content = @"../testImage2.jpg",
            ContentType = ContentType.Image,
            ParentBody = ParentBody.None
        },
    };

    DocumentData documentData = new DocumentData()
    {
        Placeholders = contents,
        TablesData = tablesData
    };

    WordDocumentGenerator.GenerateDocumentByTemplate(templateFilePath, documentData, outputFilePath);
}
```

# Targeted frameworks
1. .NET Framework 4.5.2
2. .NET Standard 2.0 - Can be installed as a dependency in applications running on .NET Framework 4.8 and modern .NET (Core, v5, v6 and later).

# Citations
- [NPOI](https://github.com/nissl-lab/npoi)
- [OpenXML](https://github.com/dotnet/Open-XML-SDK)
- [wkhtmltopdf](https://wkhtmltopdf.org/)

# License
The DocumentService is licensed under the [MIT](https://github.com/OsmosysSoftware/document-service/blob/main/LICENSE) license.
## 👏 Big Thanks to Our Contributors

<a href="https://github.com/OsmosysSoftware/document-service/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=OsmosysSoftware/document-service" alt="Contributors" />
</a>

We appreciate the time and effort put in by all contributors to make this project better!