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
- Replace images with image's placeholder. The image's position will be maintained based on the position of its placeholder image. Image size will also be maintained based on placeholder image.

## PDF document generation
- Converts an HTML document to PDF.
- Replace placeholders in the document with actual string data.

# How to set up the Application in a Docker-based environment (Linux)

Setting up the app in a Docker-based environment enables developers of non-Windows origins to run the backend application on their machine to test the APIs.

## Steps

1. [Install Docker](https://docs.docker.com/engine/install/) on your machine. Choose to follow the instructions based on your device OS.
2. [Install Docker Compose](https://docs.docker.com/compose/install/). A separate installation is required for Linux-based OS. If you are using Windows or macOS, installing the Docker Desktop app includes Docker Compose.
3. Clone the project `document-service`.
4. (Optional) [Install Docker Extension for VS Code](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker).
5. In root directory of project, create a new file `.env`.
6. Copy data from [example template](.env.example) into `.env`. Then set suitable credentials. Use a valid email. Ensure password has 8-20 characters with 1 uppercase, 1 lowercase, 1 number & 1 special character.
7. Set `environment` variables `ASPNETCORE_ENVIRONMENT` and `BUILD_CONFIGURATION` as per requirement in [docker-compose.yaml](./docker-compose.yaml). Ensure correct formatting:

#### Development (Default Configuration)
```yaml
      - ASPNETCORE_ENVIRONMENT=Development
      - BUILD_CONFIGURATION=Debug
```

#### Testing/Staging
```yaml
      - ASPNETCORE_ENVIRONMENT=Development
      - BUILD_CONFIGURATION=Release
```

#### Production
```yaml
      - ASPNETCORE_ENVIRONMENT=Production
      - BUILD_CONFIGURATION=Release
```

8. Ensure Dockeris running.
9. Then, in root directory of project, execute the following command to build container for `document-service`:

```shell
docker-compose -f docker-compose.yaml up
```

10. The project will run on `http://localhost:5000`. Please check [Troubleshooting](#troubleshooting) if the build failed.
11. You can access the **Swagger UI** at `http://localhost:5000/swagger/index.html` in **Development** Environment.
12. Test the API via **Postman**. The app can be accessed using `http://localhost:5000/<API>`.

## Troubleshooting

A known issue while building the container is the following:

```shell
E: failed to solve: process "/bin/sh -c <sample Dockerfile step>" did not complete successfully: exit code: 100
```

This is a network related issue where it is failing to fetch files from an external source. It can be verified in the **Docker logs**:

```shell
E: Failed to fetch http://sample/link/for.file Unable to connect to sample.download.location:80: [IP: ...]
```

**Solution:** Prune the failed build and rebuild the application using the following commands:

```shell
# prune all unused containers, networks, images, build cache
docker system prune -a

# rebuild the container
docker-compose -f docker-compose.yaml up
```

**NOTE:** Please go through the [official documentation on prune command](https://docs.docker.com/config/pruning/) before using it.

# How to set up the library (Windows)

## Steps for installing wkhtmltopdf
- Go to website: https://wkhtmltopdf.org/downloads.html
- Select the version of wkhtmltopdf installer that you need to download based on your system requirements.
- Finish Installation.

## Including wkhtmltopdf executable file to build package
- Go to the location to the bin files of your project where the DocumentService DLL is located.
- Create a folder called Tools and place the wkhtmltopdf.exe file there. wkhtmltopdf.exe can be found in the Program Files in C directory after it is installed.

Note: We use a Temp folder to temporarily hold the modified HTML file before converting it to a PDF file. After the conversion is done, the temporary file is removed. The code is already provided with the location of the temp file, so no modification is required in the code, and the temp folder will be used automatically.

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