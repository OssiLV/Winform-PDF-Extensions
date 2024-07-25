using Aspose.Pdf.Facades;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PDFExtension
{
    public partial class f_home : System.Windows.Forms.Form
    {
        string pathFile = string.Empty;
        string pathFolder = string.Empty;
        Dictionary<string, int> book_mark_page = null;

        PdfBookmarkEditor bookmarkEditor = null;
        PdfReader reader = null;
        public f_home()
        {
            InitializeComponent();
        }

        static void OpenFolder(string pathFolder)
        {
            if (Directory.Exists(pathFolder))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = pathFolder,
                        UseShellExecute = true,
                        Verb = "open" // This ensures it opens in Windows Explorer
                    });

                    Console.WriteLine("Folder opened successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error opening folder: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", pathFolder));
            }


        }

        static string RemoveSpecialChar(string value)
        {
            Regex reg = new Regex("[*'\",_&#^@?/:<>|]");
            return reg.Replace(value, string.Empty);
        }

        static void ExtractPages(PdfReader reader, string outputPdfPath,
            int startPage, int endPage)
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
        public void SplitAndSaveInterval(PdfReader reader, string outputPath, int startPage, int interval, string pdfFileName)
        {
            iTextSharp.text.Document document = new iTextSharp.text.Document();
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

        static bool IsPasswordValid(string pdfFullname, byte[] password)
        {
            try
            {
                using (var pdfReader = new PdfReader(pdfFullname, password))
                {
                    // Successfully opened the PDF; it's password protected
                    return true;
                }
            }
            catch (BadPasswordException)
            {
                // Failed to open the PDF; password is incorrect
                return false;
            }
        }


        // (EVENT) Form load
        private void f_home_Load(object sender, EventArgs e)
        {

            //this.TopMost = true;
            //this.FormBorderStyle = FormBorderStyle.Fixed3D;
            //this.WindowState = FormWindowState.Maximized;
        }


        // Upload file path
        private void t1_btn_upload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.AddExtension = true;
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string pathFileDialog in openFileDialog.FileNames)
                {
                    // Run the file
                    //Process.Start(fileName);
                    string fileName = Path.GetFileName(pathFileDialog);
                    pathFile = pathFileDialog;

                    if (fileName.Length >= 26)
                    {
                        t1_lb_file_name.Text = fileName.Substring(0, 26) + "...";
                    }
                    else
                    {
                        t1_lb_file_name.Text = fileName;
                    }

                    t1_txt_upload.Text = pathFile;


                    //byte[] bytes = Encoding.ASCII.GetBytes(password);
                    //pdfDocument = new Document(pathFile, "123456");

                    //Console.WriteLine(IsPasswordValid(pathFile, bytes));



                    book_mark_page = new Dictionary<string, int>();
                    bookmarkEditor = new PdfBookmarkEditor();
                    bookmarkEditor.BindPdf(pathFile);
                    Bookmarks bookmarks = bookmarkEditor.ExtractBookmarks();

                    int i = 0;
                    foreach (var bookmark in bookmarks) if (bookmark.Level == 1)
                        {
                            // Level 1
                            t1_trv_bookmark_structure.Nodes.Add(bookmark.Title);
                            book_mark_page.Add(bookmark.Title, bookmark.PageNumber);
                        }


                }
            }
        }


        // Upload folder path
        private void t1_btn_open_folder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            DialogResult result = folder.ShowDialog();
            if (result == DialogResult.OK)
            {
                pathFolder = folder.SelectedPath;
                t1_txt_open_folder.Text = pathFolder;
            }

        }


        // RUN
        private void t1_btn_run_Click(object sender, EventArgs e)
        {
            try
            {
                reader = new PdfReader(pathFile);
                var listBookmarks = book_mark_page.ToList();
                int y = 1;
                for (int i = 0; i <= listBookmarks.Count - 1; i++)
                {
                    string titleResult = RemoveSpecialChar($"({y++}).{listBookmarks[i].Key}");
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
                    if (t1_txt_upload.Text.Length >= 1 && t1_txt_open_folder.Text.Length >= 1)
                    {
                        ExtractPages(reader, $"{pathFolder}\\{titleResult}.pdf", fromPage, toPage);
                    }
                }
                reader.Close();
                bookmarkEditor.Close();
                reader.Close();
                OpenFolder(pathFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        // Upload file path
        private void t2_btn_upload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.AddExtension = true;
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string pathFileDialog in openFileDialog.FileNames)
                {
                    // Run the file
                    //Process.Start(fileName);
                    string fileName = Path.GetFileName(pathFileDialog);

                    pathFile = pathFileDialog;

                    if (fileName.Length >= 26)
                    {
                        t2_lb_file_name.Text = fileName.Substring(0, 26) + "...";

                    }
                    else
                    {
                        t2_lb_file_name.Text = fileName;
                    }

                    t2_txt_upload.Text = pathFile;


                    //byte[] bytes = Encoding.ASCII.GetBytes(password);
                    //pdfDocument = new Document(pathFile, "123456");

                    //Console.WriteLine(IsPasswordValid(pathFile, bytes));



                    book_mark_page = new Dictionary<string, int>();
                    bookmarkEditor = new PdfBookmarkEditor();
                    bookmarkEditor.BindPdf(pathFile);
                    Bookmarks bookmarks = bookmarkEditor.ExtractBookmarks();


                    foreach (var bookmark in bookmarks) if (bookmark.Level == 1)
                        {
                            // Level 1
                            t2_trv_bookmark_structure.Nodes.Add(bookmark.Title);
                            book_mark_page.Add(bookmark.Title, bookmark.PageNumber);
                        }


                }
            }
        }


        // Upload folder path
        private void t2_btn_open_folder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            DialogResult result = folder.ShowDialog();
            if (result == DialogResult.OK)
            {
                pathFolder = folder.SelectedPath;
                t2_txt_open_folder.Text = pathFolder;
            }
        }


        // RUN
        private void t2_btn_run_Click(object sender, EventArgs e)
        {
            try
            {
                reader = new PdfReader(pathFile);

                if (t2_txt_upload.Text != string.Empty
                    && t2_txt_open_folder.Text != string.Empty
                    && t2_txt_frompage.Text != string.Empty
                    )
                {
                    if (t2_txt_topage.Text.Length < 1)
                    {
                        ExtractPages(reader, $"{t2_txt_open_folder.Text}/(edited) - {Path.GetFileName(pathFile)}", int.Parse(t2_txt_frompage.Text), reader.NumberOfPages);
                    }
                    else
                    {
                        ExtractPages(reader, $"{t2_txt_open_folder.Text}/(edited) - {Path.GetFileName(pathFile)}", int.Parse(t2_txt_frompage.Text), int.Parse(t2_txt_topage.Text));
                    }

                    OpenFolder(pathFolder);
                }
                else
                {
                    MessageBox.Show("Please upload file and folder path \nmaybe you must check from page value");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                if (t2_txt_frompage.Text.Length >= 1 && t2_txt_topage.Text.Length < 1)
                {
                    MessageBox.Show("Invalid to page value");
                }
                else if (t2_txt_frompage.Text.Length < 1 && t2_txt_topage.Text.Length >= 1)
                {
                    MessageBox.Show("Invalid from page value");

                }
                else
                {
                    MessageBox.Show(ex.Message);

                }
            }
        }


        // (EVENT) Just accpet number in textbox
        private void t2_txt_frompage_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }


        // (EVENT) Just accpet number in textbox
        private void t2_txt_topage_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }


        // Upload file path
        private void t3_btn_upload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.AddExtension = true;
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string pathFileDialog in openFileDialog.FileNames)
                {
                    // Run the file
                    //Process.Start(fileName);
                    string fileName = Path.GetFileName(pathFileDialog);

                    pathFile = pathFileDialog;

                    if (fileName.Length >= 26)
                    {
                        t3_lb_file_name.Text = fileName.Substring(0, 26) + "...";

                    }
                    else
                    {
                        t3_lb_file_name.Text = fileName;
                    }

                    t3_txt_upload.Text = pathFile;


                    book_mark_page = new Dictionary<string, int>();
                    bookmarkEditor = new PdfBookmarkEditor();
                    bookmarkEditor.BindPdf(pathFile);
                    Bookmarks bookmarks = bookmarkEditor.ExtractBookmarks();


                    foreach (var bookmark in bookmarks) if (bookmark.Level == 1)
                        {
                            // Level 1
                            t3_trv_bookmark_structure.Nodes.Add(bookmark.Title);
                            book_mark_page.Add(bookmark.Title, bookmark.PageNumber);
                        }


                }
            }
        }


        // Upload folder path
        private void t3_btn_open_folder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            DialogResult result = folder.ShowDialog();
            if (result == DialogResult.OK)
            {
                pathFolder = folder.SelectedPath;
                t3_txt_open_folder.Text = pathFolder;
            }
        }


        // RUN
        private void t3_btn_run_Click(object sender, EventArgs e)
        {
            try
            {
                reader = new PdfReader(pathFile);

                if (t3_txt_upload.Text != string.Empty
                    && t3_txt_open_folder.Text != string.Empty
                    && t3_txt_interval_page.Text != string.Empty
                    )
                {
                    int interval = int.Parse(t3_txt_interval_page.Text);
                    int pageNameSuffix = 0;
                    FileInfo file = new FileInfo(pathFile);
                    string pdfFileName = file.Name.Substring(0, file.Name.LastIndexOf(".")) + "-";
                    for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber += interval)
                    {
                        pageNameSuffix++;
                        string newPdfFileName = string.Format( "{0} " + pdfFileName, pageNameSuffix);
                        SplitAndSaveInterval(
                            reader,
                            pathFolder,
                            pageNumber,
                            interval,
                            newPdfFileName
                            );
                    }

                    OpenFolder(pathFolder);
                }
                else
                {
                    MessageBox.Show("Please upload file and folder path \nmaybe you must check interval page value");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                if (t2_txt_frompage.Text.Length >= 1 && t2_txt_topage.Text.Length < 1)
                {
                    MessageBox.Show("Invalid to page value");
                }
                else if (t2_txt_frompage.Text.Length < 1 && t2_txt_topage.Text.Length >= 1)
                {
                    MessageBox.Show("Invalid from page value");

                }
                else
                {
                    MessageBox.Show(ex.Message);

                }
            }
        }


        // (EVENT) Just accpet number in textbox
        private void t3_txt_interval_page_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }


        // (EVENT) Clear current data wen change tab
        private void tc_split_by_bookmark_Selecting(object sender, TabControlCancelEventArgs e)
        {
            pathFile = string.Empty;
            pathFolder  = string.Empty;

            t1_txt_open_folder.Text = string.Empty;
            t1_txt_upload.Text = string.Empty;
            t1_trv_bookmark_structure.Nodes.Clear();
            t1_lb_file_name.Text = string.Empty;

            t2_txt_open_folder.Text = string.Empty;
            t2_txt_upload.Text = string.Empty;
            t2_trv_bookmark_structure.Nodes.Clear();
            t2_lb_file_name.Text = string.Empty;
            t2_txt_frompage.Text = string.Empty;
            t2_txt_topage.Text = string.Empty;

            t3_txt_open_folder.Text = string.Empty;
            t3_txt_upload.Text = string.Empty;
            t3_trv_bookmark_structure.Nodes.Clear();
            t3_lb_file_name.Text = string.Empty;
            t3_txt_interval_page.Text = string.Empty;
        }
    }
}
