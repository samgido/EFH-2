using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigraDoc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Visitors;
using MigraDoc.Rendering;

namespace EFH2
{
	public class PrintDoc
	{
		public static void PrintInfo(MainViewModel model)
		{
			Document document = CreateDocument(model);

			PdfDocumentRenderer renderer = new PdfDocumentRenderer();

			renderer.Document = document;

			renderer.RenderDocument();

			try
			{
				string filename = "C:\\Users\\samue\\Downloads\\test.pdf";
				renderer.PdfDocument.Save(filename);
				Process.Start(filename);
			}
			catch (Exception ex) { Debug.WriteLine(ex.Message); }
		}

		private static Document CreateDocument(MainViewModel model)
		{
			Document document = new Document();

			Section section = document.AddSection();

			Paragraph p1 = section.Headers.Primary.AddParagraph();
			p1.AddText("EFH-2");
			p1.AddSpace(15);
			p1.AddText("ESTIMATING RUNOFF VOLUME AND PEAK DISCHARGE");
			p1.AddSpace(15);
			p1.AddText("Version 2.0.12");
			p1.Format.Alignment = ParagraphAlignment.Center;

			Table basicDataTable = section.AddTable();
			basicDataTable.Style = "Table";

			Column column = basicDataTable.AddColumn("5cm");

			return document;
		}

		private string Convert(double d)
		{
			if (double.IsNormal(d)) return d.ToString();
			else return string.Empty;
		}
	}
}
