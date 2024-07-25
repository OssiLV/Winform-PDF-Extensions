using GemBox.Pdf;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;

namespace SplitPDFByTableOfContents
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_upload_Click(object sender, EventArgs e)
        {
            // If using the Professional version, put your serial key below.
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            using (var source = PdfDocument.Load("Chapters.pdf"))
            using (var archiveStream = File.OpenWrite("OutputBookmarks.zip"))
            using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create))
            {
                Dictionary<PdfPage, int> pageIndexes = source.Pages
                    .Select((page, index) => new { page, index })
                    .ToDictionary(item => item.page, item => item.index);

                // Iterate through document outlines.
                var outlines = source.Outlines;
                for (int index = 0; index < outlines.Count; ++index)
                {
                    var currentOutline = outlines[index];
                    var nextOutline = index + 1 < outlines.Count ? outlines[index + 1] : null;

                    int pageIndex = pageIndexes[currentOutline.Destination.Page];
                    int pageCount = nextOutline == null ? source.Pages.Count : pageIndexes[nextOutline.Destination.Page];

                    var entry = archive.CreateEntry($"{currentOutline.Title}.pdf");
                    using (var entryStream = entry.Open())
                    using (var destination = new PdfDocument())
                    {
                        // Add source pages from current bookmark till next bookmark to destination document.
                        while (pageIndex < pageCount)
                            destination.Pages.AddClone(source.Pages[pageIndex++]);

                        // Save destination document to the ZIP entry.
                        destination.Save(entryStream);
                    }
                }
            }




        }

        private void btn_output_Click(object sender, EventArgs e)
        {

        }
    }
}
