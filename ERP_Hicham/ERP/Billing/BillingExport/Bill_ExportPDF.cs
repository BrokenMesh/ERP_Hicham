using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Fonts;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Windows.Controls;

namespace ERP_Hicham.ERP.Billing.BillingExport
{
    class Bill_ExportPDF
    {
        public static void Demo() {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);
            
            XFont font = new XFont("Sans", 20.0, PdfSharp.Drawing.XFontStyle.Italic);

            gfx.DrawString("Hello, World!", font, XBrushes.Black,
              new XRect(0, 0, page.Width, page.Height),
              XStringFormat.TopLeft);

            string filename = "HelloWorld.pdf";
            document.Save(filename);
        }

    }
}
