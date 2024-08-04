using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Pdf;
using Spire.Pdf.Security;

namespace Permission_PDF
{
    internal class Program
    {
        [Obsolete]
        static void Main(string[] args)
        {
            //Create a PdfDocument object
            PdfDocument doc = new PdfDocument();

            //Load a sample PDF file
            doc.LoadFromFile(@"f1.pdf");

            //Specify open password to empty if you want users to able to open the document
            string openPsd = string.Empty;

            //Specify permission password
            string permissionPsd = "e-iceblue";

            //Encrypt the document with open password and permission password,
            //specifying the permission to None, which prevents users from performing any operations on the document
            doc.Security.Encrypt(openPsd, permissionPsd, PdfPermissionsFlags.None, PdfEncryptionKeySize.Key128Bit);

            //To allow users to preform some specific operations, such as fill fields and print the document, use the following line of code
            //doc.Security.Encrypt(openPsd, permissionPsd, PdfPermissionsFlags.FillFields | PdfPermissionsFlags.Print, PdfEncryptionKeySize.Key128Bit);

            //Save the document to another PDF file
            doc.SaveToFile("C:\\Users\\truongvo\\Desktop\\1.pdf");
        }
    }
}
