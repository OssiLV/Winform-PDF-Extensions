using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfSharp.Pdf.Content.Objects;

namespace Test4
{
    class Program
    {

        static string RemoveSpecialChar(string value)
        {
            Regex reg = new Regex("[*'\",_&#^@?/:<>|]");
            return  reg.Replace(value, string.Empty);
        }

        static int ExtractPageNumber(string value)
        {
            
            try
            {
               ;

                var spaceIndex = value.IndexOf(' ');
                if (spaceIndex > 0)
                {
                    return int.Parse(value.Substring(0, spaceIndex));
                } else
                {
                    return int.Parse(Regex.Match(value.ToString(), "[0-9]+").Value);
        
                }

            } catch {
                Console.WriteLine("Error ExtractPageNumber");
                return -1;  
            }

        }

        static void Main(string[] args)
        {

            SplitAndSaveIntervalRun();

            //Execute();
            Console.ReadLine();
        }

       static void Execute()
        {
            string outputPath = "output\\file1";
            PdfReader reader = new iTextSharp.text.pdf.PdfReader("file1.pdf");

            IList<Dictionary<string, object>> book_mark = SimpleBookmark.GetBookmark(reader);

            Dictionary<string, int> book_mark_page = new Dictionary<string, int>();

            foreach (Dictionary<string, object> bk in book_mark)
            {
                if (!bk.ContainsKey("Title"))
                {
                    return;
                }

                if (bk.ContainsKey("Page"))
                {
                    book_mark_page.Add(bk["Title"].ToString(), ExtractPageNumber(bk["Page"].ToString()));
                }
                else if (bk.ContainsKey("Named"))
                {
                    book_mark_page.Add(bk["Title"].ToString(), ExtractPageNumber(bk["Named"].ToString()));
                }
            }

            var list = book_mark_page.ToList();
            int y = 1;
            for (int i = 0; i <= list.Count - 1; i++)
            {

                //string extractTitle = "(" + y++.ToString() + ")" + " - " + list[i].Key;
                //string titleResult = RemoveSpecialChar(extractTitle);
                string titleResult = "Bookmark - " + y++.ToString();
                int fromPage = list[i].Value;
                int toPage = 0;
                if (i + 1 == list.Count)
                {
                    toPage = reader.NumberOfPages;
                }
                else
                {
                    toPage = list[i + 1].Value - 1;
                }

                Console.WriteLine(titleResult + "\t" + "From: " + fromPage + "\t" + "To: " + toPage.ToString());
                ExtractPages(reader, $"{outputPath}\\{titleResult}.pdf", fromPage, toPage);
            }
            reader.Close();
            Console.ReadLine();
        }

        /*
        public static void SplitPDFByBookmark(string inputFilePath, string outputFilePath1, string outputFilePath2, int pageNumber)
        {
            PdfReader reader = new PdfReader(inputFilePath);
            int totalPages = reader.NumberOfPages;

            // Create two separate documents
            Document document1 = new Document(reader.GetPageSizeWithRotation(1));
            Document document2 = new Document(reader.GetPageSizeWithRotation(pageNumber));

            PdfWriter writer1 = PdfWriter.GetInstance(document1, new FileStream(outputFilePath1, FileMode.Create));
            PdfWriter writer2 = PdfWriter.GetInstance(document2, new FileStream(outputFilePath2, FileMode.Create));

            document1.Open();
            document2.Open();

            PdfContentByte cb1 = writer1.DirectContent;
            PdfContentByte cb2 = writer2.DirectContent;

            for (int i = 1; i <= totalPages; i++)
            {
                if (i < pageNumber)
                {
                    document1.SetPageSize(reader.GetPageSizeWithRotation(i));
                    document1.NewPage();
                    PdfImportedPage page = writer1.GetImportedPage(reader, i);
                    cb1.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                }
                else
                {
                    document2.SetPageSize(reader.GetPageSizeWithRotation(i));
                    document2.NewPage();
                    PdfImportedPage page = writer2.GetImportedPage(reader, i);
                    cb2.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                }
            }

            document1.Close();
            document2.Close();
        }
        */
        public static void ExtractPages(PdfReader reader, string outputPdfPath,
            int startPage, int endPage)
        {
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            try
            {
                // Intialize a new PdfReader instance with the contents of the source Pdf file:
                

                // For simplicity, I am assuming all the pages share the same size
                // and rotation as the first page:
                sourceDocument = new Document(reader.GetPageSizeWithRotation(startPage));

                // Initialize an instance of the PdfCopyClass with the source 
                // document and an output file stream:
                pdfCopyProvider = new PdfCopy(sourceDocument,
                    new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));

                sourceDocument.Open();

                // Walk the specified range and add the page copies to the output file:
                for (int i = startPage; i <= endPage; i++)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                    pdfCopyProvider.AddPage(importedPage);
                }
                sourceDocument.Close();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        
        public static void SplitAndSaveIntervalRun()
        {
            string pdfFilePath = @"test2.pdf"; // Path to your input PDF
            string outputPath = @"output"; // Output directory for split PDFs
            int interval = 10; // Number of pages per split
            int pageNameSuffix = 0;

            // Initialize a new PdfReader instance with the contents of the source PDF file:
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
        public void SplitAndSaveInterval(string pdfFilePath, string outputPath, int startPage, int interval, string pdfFileName)
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
        
        public static int GetTotalNumberBookmark(PdfReader pdfReader)
        {
            return iTextSharp.text.pdf.SimpleBookmark.GetBookmark(pdfReader).Count;
        }

        /*
        void WalkOutlines(PdfOutline outline, IDictionary<string, PdfObject> names, PdfDocument pdfDocument)
        {
            if (outline.getDestination() != null)
            {
                string bookmarkTitle = outline.getTitle();
                int pageNumber = pdfDocument.getPageNumber((PdfDictionary)outline.getDestination().getDestinationPage(names));
                Console.WriteLine($"{bookmarkTitle}: page {pageNumber}");
            }

            foreach (PdfOutline child in outline.getAllChildren())
            {
                WalkOutlines(child, names, pdfDocument);
            }
        }
        */
    }
}
