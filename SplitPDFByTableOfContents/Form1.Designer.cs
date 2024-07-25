namespace SplitPDFByTableOfContents
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_upload = new System.Windows.Forms.Button();
            this.txt_fileName = new System.Windows.Forms.TextBox();
            this.btn_output = new System.Windows.Forms.Button();
            this.txt_output = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_upload
            // 
            this.btn_upload.Location = new System.Drawing.Point(29, 40);
            this.btn_upload.Name = "btn_upload";
            this.btn_upload.Size = new System.Drawing.Size(75, 23);
            this.btn_upload.TabIndex = 0;
            this.btn_upload.Text = "upload";
            this.btn_upload.UseVisualStyleBackColor = true;
            this.btn_upload.Click += new System.EventHandler(this.btn_upload_Click);
            // 
            // txt_fileName
            // 
            this.txt_fileName.Location = new System.Drawing.Point(29, 12);
            this.txt_fileName.Name = "txt_fileName";
            this.txt_fileName.Size = new System.Drawing.Size(352, 22);
            this.txt_fileName.TabIndex = 1;
            // 
            // btn_output
            // 
            this.btn_output.Location = new System.Drawing.Point(29, 97);
            this.btn_output.Name = "btn_output";
            this.btn_output.Size = new System.Drawing.Size(136, 23);
            this.btn_output.TabIndex = 0;
            this.btn_output.Text = "output Directory";
            this.btn_output.UseVisualStyleBackColor = true;
            this.btn_output.Click += new System.EventHandler(this.btn_output_Click);
            // 
            // txt_output
            // 
            this.txt_output.Location = new System.Drawing.Point(29, 69);
            this.txt_output.Name = "txt_output";
            this.txt_output.Size = new System.Drawing.Size(352, 22);
            this.txt_output.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txt_output);
            this.Controls.Add(this.btn_output);
            this.Controls.Add(this.txt_fileName);
            this.Controls.Add(this.btn_upload);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_upload;
        private System.Windows.Forms.TextBox txt_fileName;
        private System.Windows.Forms.Button btn_output;
        private System.Windows.Forms.TextBox txt_output;
    }
}

