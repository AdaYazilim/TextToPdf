using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using dbAutoTrack.PDFWriter;
using dbAutoTrack.PDFWriter.Graphics;

namespace TextToPdfLineByLine
{
	public class PdfOzellikleri
	{
		private const float FontSize = 8;
		private readonly static PDFFont Font = new PDFFont(StandardFonts.Courier, FontStyle.Regular);
		public readonly static TextStyle Style = new TextStyle(Font, FontSize, RGBColor.Black);

		private const float TopPad = 25;
		private const float BottomPad = 20;
		private const float LeftPad = 15;
		private const float RightPad = 15;

		private readonly PageSize _pagesize = PageSize.A4;
		private readonly PageOrientation _orientation;

		public PdfOzellikleri(IEnumerable<string> ornekTextArray)
		{
			Page page = new Page(PageSize.A4, PageOrientation.Portrait);
			float pageWidth = page.Width;

			float yazilabilirWidth = pageWidth - LeftPad - RightPad;

			_pagesize = PageSize.A4;

			_orientation =
				ornekTextArray.Any(txt => textGenisligiAl(txt) > yazilabilirWidth) ?
					PageOrientation.Landscape :
					PageOrientation.Portrait;
		}

		public Page YeniPageAl()
		{
			return new Page(_pagesize, _orientation);
		}

		public TextArea TextAreaDon()
		{
			Page page = YeniPageAl();
			float width = page.Width;
			float height = page.Height;

			TextArea retVal = new TextArea(width, height);
			retVal.TopPad = TopPad;
			retVal.LeftPad = LeftPad;
			retVal.RightPad = RightPad;
			retVal.BottomPad = BottomPad;
			//retVal.RightIndent = 50f;
			//retVal.Alignment = TextAlignment.Left;
			//retVal.ParaSpace = 10f;
			retVal.KeepTogether = true;   // Dikkat: Bu false olursa sayfa sonlarýndaki satýrlar bir sonraki sayfada tekrar yazýlabiliyor.
			return retVal;
		}

		private static float textGenisligiAl(string text)
		{
			return Font.GetTextWidth(text, FontSize);
		}
	}
}