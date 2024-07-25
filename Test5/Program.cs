using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using Aspose.Pdf;
using Aspose.Pdf.Facades;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace Test5
{
    class Program
    {
        static void Main(string[] args)
        {
            var pdfStamper = new PdfStamper(reader, outputStream);
            pdfStamper.SetEncryption(null,)
            string file = "file2.pdf";
            string outputPath = "output\\file2";
            PdfReader reader = new PdfReader(file);
            PdfBookmarkEditor bookmarkEditor = new PdfBookmarkEditor();
            bookmarkEditor.BindPdf(file);
            Bookmarks bookmarks = bookmarkEditor.ExtractBookmarks();

            Dictionary<string, int> book_mark_page = new Dictionary<string, int>();
            
            foreach (Bookmark bookmark in bookmarks) if(bookmark.Level == 1) 
            {
                    book_mark_page.Add(bookmark.Title, bookmark.PageNumber);
            }
            var listBookmarks = book_mark_page.ToList();
            int y = 1;
            for (int i = 0; i <= listBookmarks.Count - 1; i++)
            {

                //string extractTitle = "(" + y++.ToString() + ")" + " - " + list[i].Key;
                string titleResult = RemoveSpecialChar($"({y++}).{listBookmarks[i].Key}");
                //string titleResult = "Bookmark - " + y++.ToString();
                int fromPage = listBookmarks[i].Value;
                int toPage = 0;
                if (i + 1 == listBookmarks.Count)
                {
                    toPage = reader.NumberOfPages;
                }
                else
                {
                    toPage = listBookmarks[i + 1].Value - 1;
                }

                Console.WriteLine(titleResult + "\t" + "From: " + fromPage + "\t" + "To: " + toPage.ToString());
                ExtractPages(reader, $"{outputPath}\\{titleResult}.pdf", fromPage, toPage);
            }
            reader.Close();
            bookmarkEditor.Close();
            Console.ReadLine();
        }
        static string RemoveSpecialChar(string value)
        {
            Regex reg = new Regex("[*'\",_&#^@?/:<>|]");
            return reg.Replace(value, string.Empty);
        }
        public static void ExtractPages(PdfReader reader, string outputPdfPath,
            int startPage, int endPage)
        {
            iTextSharp.text.Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            try
            {
                // Intialize a new PdfReader instance with the contents of the source Pdf file:


                // For simplicity, I am assuming all the pages share the same size
                // and rotation as the first page:
                sourceDocument = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(startPage));

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
    }
}
