using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PDFExtension
{
    namespace Extensions
    {

        static class PDF
        {
            internal static void ExtractPages(PdfReader reader, string outputPdfPath, int startPage, int endPage)
            {
                iTextSharp.text.Document sourceDocument = null;
                PdfCopy pdfCopyProvider = null;
                PdfImportedPage importedPage = null;

                try
                { 
                    var stream = new FileStream(outputPdfPath, FileMode.Create);

                    sourceDocument = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(startPage));
                    pdfCopyProvider = new PdfCopy(sourceDocument, stream);

                    sourceDocument.Open();

                    // Walk the specified range and add the page copies to the output file:
                    for (int i = startPage; i <= endPage; i++)
                    {
                        importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                        pdfCopyProvider.AddPage(importedPage);
                    }
                    sourceDocument.Close();
                    pdfCopyProvider.Close();
                    stream.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    reader.Close();
                    MessageBox.Show(ex.Message);
                    //throw ex;
                }
            }

            internal static void SplitAndSaveInterval(PdfReader reader, string outputPath, int startPage, int interval, string pdfFileName)
            {
                try
                {
                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    using (var stream = new FileStream(outputPath + "\\" + pdfFileName + ".pdf", FileMode.Create))
                    {
                        PdfCopy copy = new PdfCopy(document, stream);
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

                        copy.Close();
                        document.Close();
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            internal static bool ValidatePermissionPDFfile(PdfReader reader)
            {
                if (!reader.IsOpenedWithFullPermissions || reader.IsEncrypted())
                {
                    CustomMessageBox.ErrorMessage("PDF has not perrmission");
                    return false;
                }

                return true;
            }

            internal static bool ValidatePathFileAndFolder(string pathFile, string pathFolder)
            {
                if (string.IsNullOrEmpty(pathFile))
                {
                    CustomMessageBox.WarningMessage("You forgot to select a file pdf");
                    return false;
                }

                if (string.IsNullOrEmpty(pathFolder))
                {
                    CustomMessageBox.WarningMessage("You forgot to select a folder to save process");
                    return false;
                }

                return true;
            }

            internal static bool ValidateFromPageToPage(string fromPage, string toPage)
            {
                if (string.IsNullOrEmpty(fromPage))
                {
                    CustomMessageBox.WarningMessage("From page value invalid");
                    return false;
                }

                if (!string.IsNullOrEmpty(fromPage) && !string.IsNullOrEmpty(toPage))
                {
                    int fp = int.Parse(fromPage);
                    int tp = int.Parse(toPage);

                    if (fp <= 0 || tp <= 0)
                    {
                        CustomMessageBox.WarningMessage("To page value or from page value invalid");
                        return false;
                    }

                    if (tp < fp)
                    {
                        CustomMessageBox.WarningMessage("To page value cannot be smaller than from page");
                        return false;
                    }
                }

                return true;
            }

            internal static Tuple<bool, int> ValidateIntervalPage(int numberOfPages, string intervalPage)
            {
                if (string.IsNullOrEmpty(intervalPage))
                {
                    CustomMessageBox.WarningMessage("Interval page value cannot be empty");
                    return Tuple.Create(false, 0);
                }

                int intervalPageParsed = int.Parse(intervalPage);

                if (intervalPageParsed > numberOfPages)
                {
                    CustomMessageBox.WarningMessage("Interval page value cannot be greeter than total page of document");
                    return Tuple.Create(false, 0);
                }

                if (intervalPageParsed <= 0)
                {
                    CustomMessageBox.WarningMessage("Interval page value invalid");
                    return Tuple.Create(false, 0);
                }

                return Tuple.Create(true, intervalPageParsed);
            }

            internal static string RemoveSpecialChar(string value)
            {
                Regex reg = new Regex("[<>/\\\\:*?\"|]");
                return reg.Replace(value, string.Empty);
            }

            internal static bool IsMatchSpecialChar(string value)
            {
                Regex reg = new Regex("[<>/\\\\:*?\"|]");
                return reg.IsMatch(value);
            }

            internal static string RandomGUID()
            {
                return Guid.NewGuid().ToString();
            }
        }

        static class CustomMessageBox
        {
            internal static void WarningMessage(string content, string caption = "Warning")
            {
                MessageBox.Show(content, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            internal static void InfoMessage(string content, string caption = "Info")
            {
                MessageBox.Show(content, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            internal static void ErrorMessage(string content, string caption = "Error")
            {
                MessageBox.Show(content, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }

}
