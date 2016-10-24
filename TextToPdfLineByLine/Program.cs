using System;
using System.IO;
using System.Linq;
using dbAutoTrack.PDFWriter;
using dbAutoTrack.PDFWriter.Graphics;

namespace TextToPdfLineByLine
{
    class Program
    {
        private static class ReturnTypes
        {
            public const int Basarili = 0;
            public const int EksikParametre = 1;
            public const int BilinmeyenHata = -1;
        }

        private readonly static string BoslukYerineUygunKarakter = ((char)18).ToString();

        static int Main(string[] args)
        {
            if (args.Length == 0)
                return ReturnTypes.EksikParametre;

            string path = args[0];

            string[] textArray = textArrayAl(path);

            PdfOzellikleri pdfOzellikleri = new PdfOzellikleri(textArray.Take(100));

            Document doc = new Document();
            doc.Compress = false;
            TextArea textArea = textAreaAyarla(textArray, pdfOzellikleri);

            while (textArea != null)
            {
                Page page = pdfOzellikleri.YeniPageAl();

                doc.Pages.Add(page);
                PDFGraphics graphics = page.Graphics;

                //DrawText richtext at x = 50; y =75 and height = 650
                //OverflowText is return as RichText
                //Loop until there is no overflow.
                textArea = graphics.DrawTextArea(0, 0, textArea);
            }

            FileStream fs = kaydedilecekDosyaDon(path);

            try
            {
                doc.Generate(fs);
                return ReturnTypes.Basarili;
            }
            catch (Exception)
            {
                return ReturnTypes.BilinmeyenHata;
            }
            finally
            {
                fs.Flush();
                fs.Close();
            }
        }

        private static FileStream kaydedilecekDosyaDon(string path)
        {
            FileInfo fi = new FileInfo(path);
            string directory = fi.DirectoryName;
            int sondanKirpilacakKarakterSayisi = fi.Extension.Length;
            string fileName = fi.Name;
            fileName = fileName.Substring(0, fileName.Length - sondanKirpilacakKarakterSayisi);
            fileName += ".pdf";
            string kaydedilecekDosya = Path.Combine(directory, fileName);

            FileStream fs = new FileStream(kaydedilecekDosya, FileMode.Create, FileAccess.Write);
            return fs;
        }

        private static TextArea textAreaAyarla(string[] textArray, PdfOzellikleri pdfOzellikleri)
        {
            TextArea textArea = pdfOzellikleri.TextAreaDon();
            TextStyle style = PdfOzellikleri.Style;

            foreach (string satir in textArray)
            {
                string yazilacakText;

                if (satir == string.Empty)
                {
                    yazilacakText = " ";
                }
                else
                {
                    if (satir.StartsWith(" "))
                        yazilacakText = BoslukYerineUygunKarakter + satir.Substring(1);
                    else
                        yazilacakText = satir;
                }

                textArea.NewLine();
                textArea.AddContent(yazilacakText, style);
            }

            return textArea;
        }

        private static string[] textArrayAl(string path)
        {
            string[] text = File.ReadAllLines(path, System.Text.Encoding.Default);
            return text;
        }
    }
}
