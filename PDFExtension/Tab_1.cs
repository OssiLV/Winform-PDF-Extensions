using Aspose.Pdf.Facades;
using iTextSharp.text.pdf;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDFExtension
{

    public partial class Home
    {
        // ============================================= START TAB 1 ============================================================
        private Tuple<string, string> T1_CheckOuputFileName()
        {
            bool validatePathFileAndFolder = Extensions.PDF.ValidatePathFileAndFolder(pathFile, pathFolder);

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
        private void T1_ActionsBookmarkView(Bookmarks bookmarks)
        {
            book_mark_page.Clear();
            t1_trv_bookmark_structure.Nodes.Clear();
            try
            {

                Aspose.Pdf.Document document = new Aspose.Pdf.Document(pathFile);
                int i = 0; int y = 0;

                if (t1_rbtn_actions_all.Checked)
                {
                    foreach (var level_1 in document.Outlines)
                    {
                        t1_trv_bookmark_structure.Nodes.Add(level_1.Title).ForeColor = Color.Teal;

                        if (level_1.Count > 0)
                        {
                            foreach (var level_2 in level_1)
                            {
                                t1_trv_bookmark_structure.Nodes[i].Nodes.Add(level_2.Title).ForeColor = Color.BlueViolet;

                                if (level_2.Count > 0)
                                {
                                    foreach (var level_3 in level_2)
                                    {
                                        t1_trv_bookmark_structure.Nodes[i].Nodes[y].Nodes.Add(level_3.Title);
                                    }
                                }
                                y++;
                            }
                        }
                        i++;
                        y = 0;
                    }
                }
                else if (t1_rbtn_actions_lv1.Checked)
                {
                    foreach (var bookmark in bookmarks) if (bookmark.Level == 1)
                        {
                            book_mark_page.Add(Extensions.PDF.RandomGUID(), Tuple.Create(bookmark.Title, bookmark.PageNumber));
                            t1_trv_bookmark_structure.Nodes.Add(bookmark.Title).ForeColor = Color.Teal;
                        }
                }
                else if (t1_rbtn_actions_lv2.Checked)
                {

                    foreach (var bookmark in bookmarks) if (bookmark.Level == 2)
                        {
                            book_mark_page.Add(Extensions.PDF.RandomGUID(), Tuple.Create(bookmark.Title, bookmark.PageNumber));
                            t1_trv_bookmark_structure.Nodes.Add(bookmark.Title).ForeColor = Color.BlueViolet;
                        }
                }
                else if (t1_rbtn_actions_lv3.Checked)
                {
                    foreach (var bookmark in bookmarks) if (bookmark.Level == 3)
                        {
                            book_mark_page.Add(Extensions.PDF.RandomGUID(), Tuple.Create(bookmark.Title, bookmark.PageNumber));
                            t1_trv_bookmark_structure.Nodes.Add(bookmark.Title);
                        }
                }
            }
            catch
            {

            }

        }


        // Upload file path
        private void t1_btn_upload_Click(object sender, EventArgs e)
        {
            t1_trv_bookmark_structure.Nodes.Clear();

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
                        t1_lb_file_name.Text = fileName.Substring(0, 38) + "...";
                    }
                    else
                    {
                        t1_lb_file_name.Text = fileName;
                    }

                    t1_txt_upload.Text = pathFile;

                    bookmarkEditor = new PdfBookmarkEditor();
                    bookmarkEditor.BindPdf(pathFile);
                    bookmarks = bookmarkEditor.ExtractBookmarks();

                    t1_rbtn_actions_all.Checked = true;
                    t1_rbtn_actions_lv1.Checked = false;
                    t1_rbtn_actions_lv2.Checked = false;
                    t1_rbtn_actions_lv3.Checked = false;

                    T1_ActionsBookmarkView(bookmarks);

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

                if (tuple.Item1 == string.Empty && tuple.Item2 == string.Empty)
                    return;

                reader = new PdfReader(pathFile);

                bool validatePDF = Extensions.PDF.ValidatePermissionPDFfile(reader);
                if (!validatePDF)
                    return;


                if (t1_rbtn_actions_all.Checked)
                {
                    book_mark_page.Clear();
                    foreach (var bookmark in bookmarks) if (bookmark.Level == 1)
                        {
                            book_mark_page.Add(Extensions.PDF.RandomGUID(), Tuple.Create(bookmark.Title, bookmark.PageNumber));
                            t1_trv_bookmark_structure.Nodes.Add(bookmark.Title).ForeColor = Color.Teal;
                        }
                }

                var listBookmarks = book_mark_page.ToList();
                int y = 1;
                for (int i = 0; i <= listBookmarks.Count - 1; i++)
                {
                    string title = Extensions.PDF.RemoveSpecialChar($"{listBookmarks[i].Value.Item1}");
                    int fromPage = listBookmarks[i].Value.Item2;
                    int toPage = 0;
                    if (i + 1 == listBookmarks.Count)
                    {
                        toPage = reader.NumberOfPages;
                    }
                    else
                    {
                        toPage = listBookmarks[i + 1].Value.Item2 - 1;
                    }

                    Console.WriteLine(title + "\t" + "From: " + fromPage + "\t" + "To: " + toPage.ToString());

                    if (t1_txt_upload.Text.Length >= 1 && t1_txt_open_folder.Text.Length >= 1)
                    {
                        string finalTitle = $"({y}).{tuple.Item1}{title}{tuple.Item2}";

                        if (finalTitle.Length > 250)
                        {
                            finalTitle = $"{tuple.Item1}{title.Substring(0, 40)}...{tuple.Item2}";
                        }

                        Extensions.PDF.ExtractPages(reader, $"{pathFolder}\\{finalTitle}", fromPage, toPage);
                    }
                    y++;
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
            if (Extensions.PDF.IsMatchSpecialChar(t1_txt_prefix.Text))
            {
                t1_txt_prefix.Text = string.Empty;
                prefix = string.Empty;

                Extensions.CustomMessageBox.WarningMessage(errorValidateFileName);
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t1_txt_postfix_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (Extensions.PDF.IsMatchSpecialChar(t1_txt_postfix.Text))
            {
                t1_txt_postfix.Text = string.Empty;
                prefix = string.Empty;

                Extensions.CustomMessageBox.WarningMessage(errorValidateFileName);
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t1_txt_seperate_custom_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (Extensions.PDF.IsMatchSpecialChar(t1_txt_seperate_custom.Text))
            {
                t1_txt_seperate_custom.Text = string.Empty;
                seperateCustom = string.Empty;

                Extensions.CustomMessageBox.WarningMessage(errorValidateFileName);
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

        private void t1_rbtn_actions_all_CheckedChanged(object sender, EventArgs e)
        {
            if (t1_rbtn_actions_all.Checked)
            {
                T1_ActionsBookmarkView(bookmarks);
            }
        }

        private void t1_rbtn_actions_lv1_CheckedChanged(object sender, EventArgs e)
        {
            if (t1_rbtn_actions_lv1.Checked)
            {
                T1_ActionsBookmarkView(bookmarks);
            }
        }

        private void t1_rbtn_actions_lv2_CheckedChanged(object sender, EventArgs e)
        {
            if (t1_rbtn_actions_lv2.Checked)
            {
                T1_ActionsBookmarkView(bookmarks);
            }
        }

        private void t1_rbtn_actions_lv3_CheckedChanged(object sender, EventArgs e)
        {
            if (t1_rbtn_actions_lv3.Checked)
            {
                T1_ActionsBookmarkView(bookmarks);
            }
        }

        // ============================================= END TAB 1 ============================================================
    }
}
