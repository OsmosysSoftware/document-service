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

# Targeted frameworks
1. .NET Framework 4.5.2
2. .NET Standard 2.0 - Can be installed as a dependency in applications running on .NET Framework 4.8 and modern .NET (Core, v5, v6 and later).

# Citations
- [NPOI](https://github.com/nissl-lab/npoi)
- [OpenXML](https://github.com/dotnet/Open-XML-SDK)
- [wkhtmltopdf](https://wkhtmltopdf.org/)

# License
The DocumentService is licensed under the [MIT](https://github.com/OsmosysSoftware/document-service/blob/main/LICENSE) license.