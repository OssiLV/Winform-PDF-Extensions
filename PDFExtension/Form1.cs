using Aspose.Pdf.Facades;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace PDFExtension
{
    public partial class f_home : System.Windows.Forms.Form
    {
        string pathFile, pathFolder, prefix, postfix, seperateCustom = string.Empty;
        Dictionary<string, int> book_mark_page = null;
        Regex reg = null;

        PdfBookmarkEditor bookmarkEditor = null;
        PdfReader reader = null;
        public f_home()
        {
            InitializeComponent();

            pathFile = string.Empty;
            pathFolder = string.Empty;
            prefix = string.Empty;
            postfix = string.Empty;
            reg = new Regex("[<>/\\\\:*?\"|]");
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
            Regex reg = new Regex("[<>/\\\\:*?\"|]");
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

        private bool ValidatePathFileAndFolder()
        {
            if (string.IsNullOrEmpty(pathFile))
            {
                MessageBox.Show("Please select a file pdf");
                return false;
            }

            if (string.IsNullOrEmpty(pathFolder))
            {
                MessageBox.Show("Please select a folder to save process");
                return false;
            }

            return true;
        }


        // (EVENT) Clear current data wen change tab
        private void tc_split_by_bookmark_Selecting(object sender, TabControlCancelEventArgs e)
        {
            pathFile = string.Empty;
            pathFolder = string.Empty;

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

            t1_rtxt_file_name_example.Text = string.Empty;
            t1_txt_prefix.Text = string.Empty;
            t1_txt_postfix.Text = string.Empty;
            t1_txt_seperate_custom.Text = string.Empty;

            t2_rtxt_file_name_example.Text = string.Empty;
            t2_txt_prefix.Text = string.Empty;
            t2_txt_postfix.Text = string.Empty;
            t2_txt_seperate_custom.Text = string.Empty;

            t3_rtxt_file_name_example.Text = string.Empty;
            t3_txt_prefix.Text = string.Empty;
            t3_txt_postfix.Text = string.Empty;
            t3_txt_seperate_custom.Text = string.Empty;

            prefix = string.Empty;
            postfix = string.Empty;
            seperateCustom = string.Empty;
        }


        // (EVENT) Form load
        private void f_home_Load(object sender, EventArgs e)
        {
            t1_rbtn_seperate_1.Checked = true;
            t1_rtxt_file_name_example.ReadOnly = true;
            t1_rtxt_file_name_example.ForeColor = Color.Red;

            t2_rbtn_seperate_1.Checked = true;
            t2_rtxt_file_name_example.ReadOnly = true;
            t2_rtxt_file_name_example.ForeColor = Color.Red;

            t3_rbtn_seperate_1.Checked = true;
            t3_rtxt_file_name_example.ReadOnly = true;
            t3_rtxt_file_name_example.ForeColor = Color.Red;

            //this.TopMost = true;
            //this.FormBorderStyle = FormBorderStyle.Fixed3D;
            //this.WindowState = FormWindowState.Maximized;
        }


        // ============================================= START TAB 1 ============================================================

        private Tuple<string, string> T1_CheckOuputFileName()
        {
            bool validatePathFileAndFolder = ValidatePathFileAndFolder();

            try
            {
                string fileName = Path.GetFileNameWithoutExtension(pathFile);
                string fileExtension = Path.GetExtension(pathFile);
                StringBuilder first = new StringBuilder();
                StringBuilder last = new StringBuilder();
                StringBuilder example = new StringBuilder();


                if (validatePathFileAndFolder)
                {
                    if (t1_rbtn_seperate_1.Checked && string.IsNullOrEmpty(t1_txt_seperate_custom.Text))
                    {
                        if (t1_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(t1_rbtn_seperate_1.Text);
                        }
                        first.Append(fileName);
                        first.Append(t1_rbtn_seperate_1.Text);

                        if (t1_cb_postfix_visible.Checked)
                        {
                            last.Append(t1_rbtn_seperate_1.Text);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[bookmar_name]");
                        example.Append(last.ToString());


                    }
                    else if (t1_rbtn_seperate_2.Checked && string.IsNullOrEmpty(t1_txt_seperate_custom.Text))
                    {
                        if (t1_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(t1_rbtn_seperate_2.Text);
                        }
                        first.Append(fileName);
                        first.Append(t1_rbtn_seperate_2.Text);
                        if (t1_cb_postfix_visible.Checked)
                        {
                            last.Append(t1_rbtn_seperate_2.Text);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[bookmar_name]");
                        example.Append(last.ToString());


                    }
                    else if (t1_rbtn_seperate_3.Checked && string.IsNullOrEmpty(t1_txt_seperate_custom.Text))
                    {
                        if (t1_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(t1_rbtn_seperate_3.Text);
                        }
                        first.Append(fileName);
                        first.Append(t1_rbtn_seperate_3.Text);
                        if (t1_cb_postfix_visible.Checked)
                        {
                            last.Append(t1_rbtn_seperate_3.Text);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[bookmar_name]");
                        example.Append(last.ToString());

                    }
                    else if (!string.IsNullOrEmpty(seperateCustom))
                    {
                        if (t1_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(seperateCustom);
                        }
                        first.Append(fileName);
                        first.Append(seperateCustom);
                        if (t1_cb_postfix_visible.Checked)
                        {
                            last.Append(seperateCustom);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[bookmar_name]");
                        example.Append(last.ToString());

                    }

                }


                t1_rtxt_file_name_example.Text = example.ToString();
                return Tuple.Create(first.ToString(), last.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Tuple.Create(string.Empty, string.Empty);
            }
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
                Tuple<string, string> tuple = T1_CheckOuputFileName();

                if(tuple.Item1 == string.Empty && tuple.Item2 == string.Empty)
                    return;

                reader = new PdfReader(pathFile);
                var listBookmarks = book_mark_page.ToList();
                int y = 1;
                for (int i = 0; i <= listBookmarks.Count - 1; i++)
                {
                    string title = RemoveSpecialChar($"{listBookmarks[i].Key}");
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

                    Console.WriteLine(title + "\t" + "From: " + fromPage + "\t" + "To: " + toPage.ToString());

                    if (t1_txt_upload.Text.Length >= 1 && t1_txt_open_folder.Text.Length >= 1)
                    {
                        string finalTitle = $"{tuple.Item1}{title}{tuple.Item2}";

                        if (finalTitle.Length > 250)
                        {
                            finalTitle = $"{tuple.Item1}{title.Substring(0, 40)}...{tuple.Item2}";
                        }

                        ExtractPages(reader, $"{pathFolder}\\{finalTitle}", fromPage, toPage);
                    }
                }
                reader.Close();
                bookmarkEditor.Close();
                reader.Close();
                OpenFolder(pathFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void t1_btn_check_file_name_output_Click(object sender, EventArgs e)
        {
            T1_CheckOuputFileName();
        }

        private void t1_txt_prefix_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (reg.IsMatch(t1_txt_prefix.Text))
            {
                t1_txt_prefix.Text = string.Empty;
                prefix = string.Empty;

                MessageBox.Show("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t1_txt_postfix_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (reg.IsMatch(t1_txt_postfix.Text))
            {
                t1_txt_postfix.Text = string.Empty;
                prefix = string.Empty;

                MessageBox.Show("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t1_txt_seperate_custom_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (reg.IsMatch(t1_txt_seperate_custom.Text))
            {
                t1_txt_seperate_custom.Text = string.Empty;
                seperateCustom = string.Empty;

                MessageBox.Show("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t1_txt_seperate_custom_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(t1_txt_seperate_custom.Text))
            {
                t1_rbtn_seperate_1.Checked = true;
                t1_rbtn_seperate_2.Checked = false;
                t1_rbtn_seperate_3.Checked = false;

                t1_rbtn_seperate_1.Enabled = true;
                t1_rbtn_seperate_2.Enabled = true;
                t1_rbtn_seperate_3.Enabled = true;

            }
            else
            {
                t1_rbtn_seperate_1.Checked = false;
                t1_rbtn_seperate_2.Checked = false;
                t1_rbtn_seperate_3.Checked = false;

                t1_rbtn_seperate_1.Enabled = false;
                t1_rbtn_seperate_2.Enabled = false;
                t1_rbtn_seperate_3.Enabled = false;
            }
            seperateCustom = t1_txt_seperate_custom.Text;
        }

        private void t1_txt_prefix_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(t1_txt_prefix.Text))
            {
                t1_cb_prefix_visible.Checked = false;
            }
            else
            {
                t1_cb_prefix_visible.Checked = true;
            }
            prefix = t1_txt_prefix.Text;
        }

        private void t1_txt_postfix_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(t1_txt_postfix.Text))
            {
                t1_cb_postfix_visible.Checked = false;
            }
            else
            {
                t1_cb_postfix_visible.Checked = true;
            }
            postfix = t1_txt_postfix.Text;
        }

        // ============================================= END TAB 1 ============================================================




        // ============================================= START TAB 2 ============================================================

        private Tuple<string, string> T2_CheckOuputFileName()
        {
            bool validatePathFileAndFolder = ValidatePathFileAndFolder();

            try
            {
                string fileName = Path.GetFileNameWithoutExtension(pathFile);
                string fileExtension = Path.GetExtension(pathFile);
                StringBuilder first = new StringBuilder();
                StringBuilder last = new StringBuilder();
                StringBuilder example = new StringBuilder();


                if (validatePathFileAndFolder)
                {
                    if (t2_rbtn_seperate_1.Checked && string.IsNullOrEmpty(t2_txt_seperate_custom.Text))
                    {
                        if (t2_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(t2_rbtn_seperate_1.Text);
                        }
                        first.Append(fileName);
                        first.Append(t2_rbtn_seperate_1.Text);

                        if (t2_cb_postfix_visible.Checked)
                        {
                            last.Append(t2_rbtn_seperate_1.Text);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[page xx-yy]");
                        example.Append(last.ToString());


                    }
                    else if (t2_rbtn_seperate_2.Checked && string.IsNullOrEmpty(t2_txt_seperate_custom.Text))
                    {
                        if (t2_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(t2_rbtn_seperate_2.Text);
                        }
                        first.Append(fileName);
                        first.Append(t2_rbtn_seperate_2.Text);
                        if (t2_cb_postfix_visible.Checked)
                        {
                            last.Append(t2_rbtn_seperate_2.Text);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[page xx-yy]");
                        example.Append(last.ToString());


                    }
                    else if (t2_rbtn_seperate_3.Checked && string.IsNullOrEmpty(t2_txt_seperate_custom.Text))
                    {
                        if (t2_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(t2_rbtn_seperate_3.Text);
                        }
                        first.Append(fileName);
                        first.Append(t2_rbtn_seperate_3.Text);
                        if (t2_cb_postfix_visible.Checked)
                        {
                            last.Append(t2_rbtn_seperate_3.Text);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[page xx-yy]");
                        example.Append(last.ToString());

                    }
                    else if (!string.IsNullOrEmpty(seperateCustom))
                    {
                        if (t2_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(seperateCustom);
                        }
                        first.Append(fileName);
                        first.Append(seperateCustom);
                        if (t2_cb_postfix_visible.Checked)
                        {
                            last.Append(seperateCustom);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[page xx-yy]");
                        example.Append(last.ToString());

                    }

                }


                t2_rtxt_file_name_example.Text = example.ToString();
                return Tuple.Create(first.ToString(), last.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Tuple.Create(string.Empty, string.Empty);
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
                Tuple<string, string> tuple = T2_CheckOuputFileName();

                if (tuple.Item1 == string.Empty && tuple.Item2 == string.Empty)
                    return;

                reader = new PdfReader(pathFile);

                if (t2_txt_upload.Text != string.Empty
                    && t2_txt_open_folder.Text != string.Empty
                    && t2_txt_frompage.Text != string.Empty
                    )
                {
                    string finalTitle = $"{tuple.Item1}{tuple.Item2}";
                    if (t2_txt_topage.Text.Length < 1)
                    {
                        ExtractPages(reader, $"{t2_txt_open_folder.Text}/{tuple.Item1}Page {t2_txt_frompage.Text}-{reader.NumberOfPages}{tuple.Item2}", int.Parse(t2_txt_frompage.Text), reader.NumberOfPages);
                    }
                    else
                    {
                        ExtractPages(reader, $"{t2_txt_open_folder.Text}/{tuple.Item1}Page {t2_txt_frompage.Text}-{t2_txt_topage.Text}{tuple.Item2}", int.Parse(t2_txt_frompage.Text), int.Parse(t2_txt_topage.Text));
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
                if (t2_txt_frompage.Text.Length < 1 && t2_txt_topage.Text.Length >= 1)
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
            if ((e.KeyChar == '.') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf('.') > -1))
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
            if ((e.KeyChar == '.') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void t2_txt_prefix_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(t2_txt_prefix.Text))
            {
                t2_cb_prefix_visible.Checked = false;
            }
            else
            {
                t2_cb_prefix_visible.Checked = true;
            }
            prefix = t2_txt_prefix.Text;
        }

        private void t2_txt_postfix_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(t2_txt_postfix.Text))
            {
                t2_cb_postfix_visible.Checked = false;
            }
            else
            {
                t2_cb_postfix_visible.Checked = true;
            }
            postfix = t2_txt_postfix.Text;
        }

        private void t2_txt_seperate_custom_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(t2_txt_seperate_custom.Text))
            {
                t2_rbtn_seperate_1.Checked = true;
                t2_rbtn_seperate_2.Checked = false;
                t2_rbtn_seperate_3.Checked = false;

                t2_rbtn_seperate_1.Enabled = true;
                t2_rbtn_seperate_2.Enabled = true;
                t2_rbtn_seperate_3.Enabled = true;

            }
            else
            {
                t2_rbtn_seperate_1.Checked = false;
                t2_rbtn_seperate_2.Checked = false;
                t2_rbtn_seperate_3.Checked = false;

                t2_rbtn_seperate_1.Enabled = false;
                t2_rbtn_seperate_2.Enabled = false;
                t2_rbtn_seperate_3.Enabled = false;
            }
            seperateCustom = t2_txt_seperate_custom.Text;
        }

        private void t2_txt_prefix_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (reg.IsMatch(t2_txt_prefix.Text))
            {
                t2_txt_prefix.Text = string.Empty;
                prefix = string.Empty;

                MessageBox.Show("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t2_txt_postfix_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (reg.IsMatch(t2_txt_postfix.Text))
            {
                t2_txt_postfix.Text = string.Empty;
                postfix = string.Empty;

                MessageBox.Show("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t2_txt_seperate_custom_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (reg.IsMatch(t2_txt_seperate_custom.Text))
            {
                t2_txt_seperate_custom.Text = string.Empty;
                seperateCustom = string.Empty;

                MessageBox.Show("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t2_btn_check_file_name_output_Click(object sender, EventArgs e)
        {
            T2_CheckOuputFileName();
        }


        // ============================================= END TAB 2 ============================================================




        // ============================================= START TAB 3 ============================================================

        private Tuple<string, string> T3_CheckOuputFileName()
        {
            bool validatePathFileAndFolder = ValidatePathFileAndFolder();

            try
            {
                string fileName = Path.GetFileNameWithoutExtension(pathFile);
                string fileExtension = Path.GetExtension(pathFile);
                StringBuilder first = new StringBuilder();
                StringBuilder last = new StringBuilder();
                StringBuilder example = new StringBuilder();


                if (validatePathFileAndFolder)
                {
                    if (t3_rbtn_seperate_1.Checked && string.IsNullOrEmpty(t3_txt_seperate_custom.Text))
                    {
                        if (t3_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(t3_rbtn_seperate_1.Text);
                        }
                        first.Append(fileName);
                        first.Append(t3_rbtn_seperate_1.Text);

                        if (t3_cb_postfix_visible.Checked)
                        {
                            last.Append(t3_rbtn_seperate_1.Text);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[Part n]");
                        example.Append(last.ToString());


                    }
                    else if (t3_rbtn_seperate_2.Checked && string.IsNullOrEmpty(t3_txt_seperate_custom.Text))
                    {
                        if (t3_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(t3_rbtn_seperate_2.Text);
                        }
                        first.Append(fileName);
                        first.Append(t3_rbtn_seperate_2.Text);
                        if (t3_cb_postfix_visible.Checked)
                        {
                            last.Append(t3_rbtn_seperate_2.Text);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[Part n]");
                        example.Append(last.ToString());


                    }
                    else if (t3_rbtn_seperate_3.Checked && string.IsNullOrEmpty(t3_txt_seperate_custom.Text))
                    {
                        if (t3_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(t3_rbtn_seperate_3.Text);
                        }
                        first.Append(fileName);
                        first.Append(t3_rbtn_seperate_3.Text);
                        if (t3_cb_postfix_visible.Checked)
                        {
                            last.Append(t3_rbtn_seperate_3.Text);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[Part n]");
                        example.Append(last.ToString());

                    }
                    else if (!string.IsNullOrEmpty(seperateCustom))
                    {
                        if (t3_cb_prefix_visible.Checked)
                        {
                            first.Append(prefix);
                            first.Append(seperateCustom);
                        }
                        first.Append(fileName);
                        first.Append(seperateCustom);
                        if (t3_cb_postfix_visible.Checked)
                        {
                            last.Append(seperateCustom);
                            last.Append(postfix);
                        }
                        last.Insert(last.ToString().Length, fileExtension);

                        example.Append(first.ToString());
                        example.Append("[Part n]");
                        example.Append(last.ToString());

                    }

                }


                t3_rtxt_file_name_example.Text = example.ToString();
                return Tuple.Create(first.ToString(), last.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Tuple.Create(string.Empty, string.Empty);
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
                Tuple<string, string> tuple = T3_CheckOuputFileName();

                if (tuple.Item1 == string.Empty && tuple.Item2 == string.Empty)
                    return;

                reader = new PdfReader(pathFile);

                if (t3_txt_upload.Text != string.Empty
                    && t3_txt_open_folder.Text != string.Empty
                    && t3_txt_interval_page.Text != string.Empty
                    )
                {
                    int interval = int.Parse(t3_txt_interval_page.Text);
                    int count = interval;

                    for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber += interval)
                    {
                        string finalFileName = $"{tuple.Item1}Part {count}{tuple.Item2}";
                        SplitAndSaveInterval(
                            reader,
                            pathFolder,
                            pageNumber,
                            interval,
                            finalFileName
                            );

                        count += interval;
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
            if ((e.KeyChar == '.') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void t3_txt_prefix_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(t3_txt_prefix.Text))
            {
                t3_cb_prefix_visible.Checked = false;
            }
            else
            {
                t3_cb_prefix_visible.Checked = true;
            }
            prefix = t3_txt_prefix.Text;
        }

        private void t3_txt_postfix_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(t3_txt_postfix.Text))
            {
                t3_cb_postfix_visible.Checked = false;
            }
            else
            {
                t3_cb_postfix_visible.Checked = true;
            }
            postfix = t3_txt_postfix.Text;
        }

        private void t3_txt_seperate_custom_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(t3_txt_seperate_custom.Text))
            {
                t3_rbtn_seperate_1.Checked = true;
                t3_rbtn_seperate_2.Checked = false;
                t3_rbtn_seperate_3.Checked = false;

                t3_rbtn_seperate_1.Enabled = true;
                t3_rbtn_seperate_2.Enabled = true;
                t3_rbtn_seperate_3.Enabled = true;

            }
            else
            {
                t3_rbtn_seperate_1.Checked = false;
                t3_rbtn_seperate_2.Checked = false;
                t3_rbtn_seperate_3.Checked = false;

                t3_rbtn_seperate_1.Enabled = false;
                t3_rbtn_seperate_2.Enabled = false;
                t3_rbtn_seperate_3.Enabled = false;
            }
            seperateCustom = t3_txt_seperate_custom.Text;
        }

        private void t3_txt_prefix_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (reg.IsMatch(t3_txt_prefix.Text))
            {
                t3_txt_prefix.Text = string.Empty;
                prefix = string.Empty;

                MessageBox.Show("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t3_txt_postfix_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (reg.IsMatch(t3_txt_postfix.Text))
            {
                t3_txt_postfix.Text = string.Empty;
                postfix = string.Empty;

                MessageBox.Show("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t3_txt_seperate_custom_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (reg.IsMatch(t3_txt_seperate_custom.Text))
            {
                t3_txt_seperate_custom.Text = string.Empty;
                seperateCustom = string.Empty;

                MessageBox.Show("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t3_btn_check_file_name_output_Click(object sender, EventArgs e)
        {
            T3_CheckOuputFileName();
        }

        // ============================================= END TAB 3 ============================================================

    }
}
