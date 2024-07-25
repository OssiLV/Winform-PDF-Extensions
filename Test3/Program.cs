using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

class Program
{
    static void Main(string[] args)
    {
        string pdfFilePath = @"test2.pdf"; // Path to your source PDF
        string outputPath = @"output"; // Output directory for split PDFs
        int interval = 10; // Number of pages per split
        int pageNameSuffix = 0;

        // Initialize a new PdfReader instance with the contents of the source PDF file
        PdfReader reader = new PdfReader(pdfFilePath);
        FileInfo file = new FileInfo(pdfFilePath);
        string pdfFileName = file.Name.Substring(0, file.Name.LastIndexOf(".")) + "-";

        Program obj = new Program();

        for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber += interval)
        {
            pageNameSuffix++;
            string newPdfFileName = string.Format(pdfFileName + " {0}", pageNameSuffix);
            obj.SplitAndSaveInterval(pdfFilePath, outputPath, pageNumber, interval, newPdfFileName);
        }
    }

    private void SplitAndSaveInterval(string pdfFilePath, string outputPath, int startPage, int interval, string pdfFileName)
    {
        using (PdfReader reader = new PdfReader(pdfFilePath))
        {
            Document document = new Document();
            PdfCopy copy = new PdfCopy(document, new FileStream(outputPath + "\\" + pdfFileName + ".pdf", FileMode.Create));
            document.Open();

            for (int pageNumber = startPage; pageNumber < (startPage + interval); pageNumber++)
            {
                if (reader.NumberOfPages >= pageNumber)
                {
                    copy.AddPage(copy.GetImportedPage(reader, pageNumber));
                }
                else
                {
                    break;
                }
            }

            document.Close();
        }
    }
}
