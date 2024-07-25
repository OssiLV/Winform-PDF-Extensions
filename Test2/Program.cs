


using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Test2
{
    internal class Program
    {

        static string valueBeforeWhitespace(string value)
        {
            string valueBeforeWhitespace;
            int firstWhitespaceIndex = value.IndexOf(' ');

            if (firstWhitespaceIndex != -1)
            {
                valueBeforeWhitespace = value.Substring(0, firstWhitespaceIndex);
                return valueBeforeWhitespace;
            }
            else
            {
                // Handle case where there's no whitespace
                valueBeforeWhitespace = value;
                return valueBeforeWhitespace;
            }
        }
        static void Main(string[] args)
        {
            string inputFilePath = "test2.pdf";
            string outputDirectory = "output";

            SplitPDFByBookMark(inputFilePath, outputDirectory);
        }

        static string extractNumberFromString(string value)
        {
            return Regex.Match(value, @"\d+").Value;
        }

        public static void SplitPDFByBookMark(string inputFilePath, string outputDirectory)
        {
            PdfReader reader = new PdfReader(inputFilePath);
            IList<Dictionary<string, object>> bookmarks = SimpleBookmark.GetBookmark(reader);


            for (int i = 0; i < bookmarks.Count; ++i)
            {
                IDictionary<string, object> bookmark = (IDictionary<string, object>)bookmarks[i];
                string title = bookmark["Title"].ToString();
                string titleFixed = title.Replace(":", " _");
                int page = 0;
                if (!bookmark.ContainsKey("Page"))
                { 
                    page = int.Parse(extractNumberFromString(bookmark["Named"].ToString()));
                } else
                {
                    page = int.Parse(extractNumberFromString(bookmark["Page"].ToString()));
                }
                    

  

                // Create a new PDF document for each bookmark
                Document document = new Document(reader.GetPageSizeWithRotation(page));
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(Path.Combine(outputDirectory, $"{titleFixed}.pdf"), FileMode.Create, FileAccess.ReadWrite));
                
                document.Open();

                PdfContentByte cb = writer.DirectContent;
                PdfImportedPage importedPage = writer.GetImportedPage(reader, page);
                cb.AddTemplate(importedPage, 0, 0);

                document.Close();
            }

            reader.Close();
        }

    }
}
