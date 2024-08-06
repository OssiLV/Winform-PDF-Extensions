using Aspose.Pdf.Facades;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PDFExtension
{
    public partial class Home
    {
        // ============================================= START TAB 3 ============================================================
        private Tuple<string, string> T3_CheckOuputFileName()
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
                bool validatePDF = Extensions.PDF.ValidatePermissionPDFfile(reader);
                if (!validatePDF)
                    return;

                Tuple<bool, int> validateIntervalPage = Extensions.PDF.ValidateIntervalPage(reader.NumberOfPages, t3_txt_interval_page.Text);
                if (!validateIntervalPage.Item1)
                {
                    reader.Close();
                    return;
                }
                int count = validateIntervalPage.Item2;


                for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber += validateIntervalPage.Item2)
                {
                    string finalFileName = $"{tuple.Item1}Part {count}{tuple.Item2}";
                    Extensions.PDF.SplitAndSaveInterval(
                        reader,
                        pathFolder,
                        pageNumber,
                        validateIntervalPage.Item2,
                        finalFileName
                        );

                    count += validateIntervalPage.Item2;
                }


                OpenFolder(pathFolder);
               
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
            if (Extensions.PDF.IsMatchSpecialChar(t3_txt_prefix.Text))
            {
                t3_txt_prefix.Text = string.Empty;
                prefix = string.Empty;

                Extensions.CustomMessageBox.WarningMessage("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t3_txt_postfix_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (Extensions.PDF.IsMatchSpecialChar(t3_txt_postfix.Text))
            {
                t3_txt_postfix.Text = string.Empty;
                postfix = string.Empty;

                Extensions.CustomMessageBox.WarningMessage("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t3_txt_seperate_custom_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (Extensions.PDF.IsMatchSpecialChar(t3_txt_seperate_custom.Text))
            {
                t3_txt_seperate_custom.Text = string.Empty;
                seperateCustom = string.Empty;

                Extensions.CustomMessageBox.WarningMessage("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
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
