using Aspose.Pdf.Facades;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace PDFExtension
{

    public partial class Home : System.Windows.Forms.Form
    {
        string
            pathFile,
            pathFolder,
            prefix,
            postfix,
            seperateCustom,
            errorValidateFileName
            = string.Empty;
        Dictionary<string, Tuple<string, int>> book_mark_page = null;
        PdfBookmarkEditor bookmarkEditor = null;
        Bookmarks bookmarks = null;
        PdfReader reader = null;



        public void StateReset()
        {
            pathFile = string.Empty;
            pathFolder = string.Empty;

            t1_txt_open_folder.Text = string.Empty;
            t1_txt_upload.Text = string.Empty;
            t1_trv_bookmark_structure.Nodes.Clear();
            t1_lb_file_name.Text = string.Empty;
            t1_rbtn_actions_all.Checked = true;
            t1_rbtn_actions_lv1.Checked = false;
            t1_rbtn_actions_lv2.Checked = false;
            t1_rbtn_actions_lv3.Checked = false;

            t2_txt_open_folder.Text = string.Empty;
            t2_txt_upload.Text = string.Empty;
            t2_lb_file_name.Text = string.Empty;
            t2_txt_frompage.Text = string.Empty;
            t2_txt_topage.Text = string.Empty;

            t3_txt_open_folder.Text = string.Empty;
            t3_txt_upload.Text = string.Empty;
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

        public void StateInit()
        {
            this.Icon = Properties.Resources.cut_icon;
            book_mark_page = new Dictionary<string, Tuple<string, int>>();
            errorValidateFileName = "File name invalid\nDon't use this special characters / : \\ * ? \" < > |";

            t1_rbtn_seperate_1.Checked = true;
            t1_rtxt_file_name_example.ReadOnly = true;
            t1_rtxt_file_name_example.ForeColor = Color.Red;
            t1_rbtn_actions_all.Checked = true;
            t1_rbtn_actions_lv1.Checked = false;
            t1_rbtn_actions_lv2.Checked = false;
            t1_rbtn_actions_lv3.Checked = false;


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

        public Home()
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

        // (EVENT) Clear current data wen change tab
        private void tc_split_by_bookmark_Selecting(object sender, TabControlCancelEventArgs e)
        {
            StateReset();
        }


        // (EVENT) Form load
        private void Home_Load(object sender, EventArgs e)
        {
            StateInit();
        }


    }
}
