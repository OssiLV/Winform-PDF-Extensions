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
        // ============================================= START TAB 2 ============================================================
        private Tuple<string, string> T2_CheckOuputFileName()
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
                        example.Append("[Page xx-yy]");
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
                        example.Append("[Page xx-yy]");
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
                        example.Append("[Page xx-yy]");
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
                        example.Append("[Page xx-yy]");
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

                bool validateFromPageToPage = Extensions.PDF.ValidateFromPageToPage(t2_txt_frompage.Text, t2_txt_topage.Text);
                if (!validateFromPageToPage)
                    return;

                reader = new PdfReader(pathFile);
                bool validatePDF = Extensions.PDF.ValidatePermissionPDFfile(reader);
                if (!validatePDF)
                    return;

                string finalTitle = $"{tuple.Item1}{tuple.Item2}";
                if (t2_txt_topage.Text.Length < 1)
                {
                    Extensions.PDF.ExtractPages(reader, $"{t2_txt_open_folder.Text}/{tuple.Item1}Page {t2_txt_frompage.Text}-{reader.NumberOfPages}{tuple.Item2}", int.Parse(t2_txt_frompage.Text), reader.NumberOfPages);
                }
                else
                {
                    Extensions.PDF.ExtractPages(reader, $"{t2_txt_open_folder.Text}/{tuple.Item1}Page {t2_txt_frompage.Text}-{t2_txt_topage.Text}{tuple.Item2}", int.Parse(t2_txt_frompage.Text), int.Parse(t2_txt_topage.Text));
                }

                OpenFolder(pathFolder);
               
                reader.Close();
            }
            catch (Exception ex)
            {  
                Extensions.CustomMessageBox.ErrorMessage(ex.Message);
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
            if (Extensions.PDF.IsMatchSpecialChar(t2_txt_prefix.Text))
            {
                t2_txt_prefix.Text = string.Empty;
                prefix = string.Empty;

                Extensions.CustomMessageBox.WarningMessage("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t2_txt_postfix_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (Extensions.PDF.IsMatchSpecialChar(t2_txt_postfix.Text))
            {
                t2_txt_postfix.Text = string.Empty;
                postfix = string.Empty;

                Extensions.CustomMessageBox.WarningMessage("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t2_txt_seperate_custom_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (Extensions.PDF.IsMatchSpecialChar(t2_txt_seperate_custom.Text))
            {
                t2_txt_seperate_custom.Text = string.Empty;
                seperateCustom = string.Empty;

                Extensions.CustomMessageBox.WarningMessage("Name file invalid\n Don't use this special characters / : \\ * ? \" < > |");
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }

        private void t2_btn_check_file_name_output_Click(object sender, EventArgs e)
        {
            T2_CheckOuputFileName();
        }


        // ============================================= END TAB 2 ============================================================


    }
}
