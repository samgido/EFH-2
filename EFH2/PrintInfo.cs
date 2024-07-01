using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigraDoc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Visitors;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Extensions;
using OxyPlot;

namespace EFH2
{
	public class PrintInfo
	{
		public static void Print(MainViewModel model, string filename)
		{
			try
			{
				Document document = CreateDocument(model);

				PdfDocumentRenderer renderer = new PdfDocumentRenderer();

				renderer.Document = document;

				renderer.RenderDocument();

				renderer.PdfDocument.Save(filename);
			}
			// keep trying till it works, only happens once over
			catch (Exception ex) { Print(model, filename); }
		}

		private static Document CreateDocument(MainViewModel model)
		{
			Document document = new Document();

			Section section = document.AddSection();

			CreateHeader(section);

			CreateBasicDataInfoTable(model, section);
			CreateBasicDataTable(model, section);	

			CreateRainfallDischargeTable(model, section);

			if (RcnDataViewModel.Used)
			{
				section.AddPageBreak();

				Paragraph p = new Paragraph();
				p.AddText("Curve number Computation");
				p.Format.Alignment = ParagraphAlignment.Center;
				section.Add(p);
				section.AddTextFrame().Height = 15;

				CreateBasicDataInfoTable(model, section);
				CreateRcnTable(model, section);
			}

			return document;
		}

		private static void CreateHeader(Section section)
		{
			Paragraph p1 = section.Headers.Primary.AddParagraph();
			p1.AddText("EFH-2");
			p1.AddSpace(15);
			p1.AddText("ESTIMATING RUNOFF VOLUME AND PEAK DISCHARGE");
			p1.AddSpace(15);
			p1.AddText("Version 2.0.12");
			p1.Format.Alignment = ParagraphAlignment.Center;
		}

		private static void CreateBasicDataInfoTable(MainViewModel model, Section section)
		{
			Table table = section.AddTable();
			table.Borders.Color = Color.Parse("White");

			Column col = table.AddColumn("5cm"); col = table.AddColumn(); col = table.AddColumn(); col = table.AddColumn();

			Row row = table.AddRow();
			row.Borders.Color = Color.Parse("White");
			Paragraph p = new Paragraph();
			p.AddText("Client:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText(model.BasicDataViewModel.Client);
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[1].Add(p);
			p = new Paragraph();

			row = table.AddRow();
			p.AddText("County:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText(model.BasicDataViewModel.selectedCounty);
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[1].Add(p);
			p = new Paragraph();
			p.AddText("State:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[2].Add(p);
			p = new Paragraph();
			p.AddText(model.BasicDataViewModel.selectedState);
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[3].Add(p);
			p = new Paragraph();

			row = table.AddRow();
			row.Borders.Color = Color.Parse("White");
			p = new Paragraph();
			p.AddText("Practice:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText(model.BasicDataViewModel.Practice);
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[1].Add(p);
			p = new Paragraph();

			row = table.AddRow();
			p.AddText("Calculated By:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText(model.BasicDataViewModel.By);
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[1].Add(p);
			p = new Paragraph();
			p.AddText("Date:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[2].Add(p);
			p = new Paragraph();
			p.AddText(model.BasicDataViewModel.Date.ToString());
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[3].Add(p);
			p = new Paragraph();

			row = table.AddRow();
			p.AddText("Checked By:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText("__________");
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[1].Add(p);
			p = new Paragraph();
			p.AddText("Date:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[2].Add(p);
			p = new Paragraph();
			p.AddText("__________");
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[3].Add(p);
			p = new Paragraph();
		}

		private static void CreateBasicDataTable(MainViewModel model, Section section)
		{
			Table table = section.AddTable();

			Column col = table.AddColumn("6cm"); col = table.AddColumn(); col = table.AddColumn(); col = table.AddColumn();

			Paragraph p = new Paragraph();
			Row row = table.AddRow();
			row.TopPadding = 10;
			p.AddText("Drainage Area:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText(Format(model.BasicDataViewModel.DrainageArea));
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[1].Add(p);
			p = new Paragraph();
			p.AddText("Acres");
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[2].Add(p);
			p = GetCenteredParagraph();
			p.AddText(GetStatus(model.BasicDataViewModel.drainageAreaEntry.InputStatus));
			row.Cells[3].Add(p);
			p = new Paragraph();

			row = table.AddRow();
			p.AddText("Curve Number:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText(Format(model.BasicDataViewModel.RunoffCurveNumber));
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[1].Add(p);
			p = new Paragraph();
			p.AddText("");
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[2].Add(p);
			p = GetCenteredParagraph();
			p.AddText(GetStatus(model.BasicDataViewModel.runoffCurveNumberEntry.InputStatus));
			row.Cells[3].Add(p);
			p = new Paragraph();

			row = table.AddRow();
			p.AddText("Watershed Length:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText(Format(model.BasicDataViewModel.WatershedLength));
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[1].Add(p);
			p = new Paragraph();
			p.AddText("Feet");
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[2].Add(p);
			p = GetCenteredParagraph();
			//p.AddText(model.BasicDataViewModel.watershedLengthEntry.Status);
			row.Cells[3].Add(p);
			p = new Paragraph();

			row = table.AddRow();
			p.AddText("Watershed Slope");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText(Format(model.BasicDataViewModel.WatershedSlope));
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[1].Add(p);
			p = new Paragraph();
			p.AddText("Percent");
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[2].Add(p);
			p = GetCenteredParagraph();
			//p.AddText(model.BasicDataViewModel.watershedSlopeEntry.Status);
			row.Cells[3].Add(p);
			p = new Paragraph();

			row = table.AddRow();
			p.AddText("Time of Concentration:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText(Format(model.BasicDataViewModel.TimeOfConcentration));
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[1].Add(p);
			p = new Paragraph();
			p.AddText("Hours");
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[2].Add(p);
			p = GetCenteredParagraph();
			p.AddText(GetStatus(model.BasicDataViewModel.timeOfConcentrationEntry.InputStatus));
			row.Cells[3].Add(p);
			p = new Paragraph();

			row = table.AddRow();
			p.AddText("Rainfall Distribution - Type:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText(model.RainfallDischargeDataViewModel.selectedRainfallDistributionType);
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[1].Add(p);
			p = new Paragraph();
			p.AddText("");
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[2].Add(p);
			p = GetCenteredParagraph();
			//p.AddText(model.RainfallDischargeDataViewModel.RainfallDistributionTypeStatus);
			row.Cells[3].Add(p);
			p = new Paragraph();

			row = table.AddRow();
			p.AddText("Dimensionless Unit Hydrograph:");
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[0].Add(p);
			p = new Paragraph();
			p.AddText(model.RainfallDischargeDataViewModel.selectedDuhType);
			p.Format.Alignment = ParagraphAlignment.Right;
			row.Cells[1].Add(p);
			p = new Paragraph();
			p.AddText("");
			p.Format.Alignment = ParagraphAlignment.Left;
			row.Cells[2].Add(p);
			p = GetCenteredParagraph();

			p.AddText(model.RainfallDischargeDataViewModel.DuhTypeStatus);
			row.Cells[3].Add(p);
			p = new Paragraph();

		}

		private static void CreateRainfallDischargeTable(MainViewModel model, Section section)
		{
			section.AddTextFrame().Height = ".5cm";
			Table table = section.AddTable();
			table.Style = "Table";
			table.Borders.Color = Color.Parse("black");

			Column column = table.AddColumn("4cm");
			column.Format.Alignment = ParagraphAlignment.Center;

			for (int i = 0; i < MainViewModel.NumberOfStorms; i++)
			{
				column = table.AddColumn("1.5cm");
				column.Format.Alignment = ParagraphAlignment.Center;
			}

			// Make header row
			Row row = table.AddRow();
			row.HeadingFormat = true;
			row.Format.Alignment = ParagraphAlignment.Center;
			row.Cells[0].AddParagraph("Storm Number");

			for (int i = 0; i < MainViewModel.NumberOfStorms; i++)
			{
				row.Cells[i + 1].AddParagraph((i + 1).ToString());
			}

			// Frequency row
			row = table.AddRow();
			row.Cells[0].AddParagraph("Frequency (yrs)");
			row.Format.Alignment = ParagraphAlignment.Center;

			row.Borders.Bottom = new Border() { Color = Color.FromArgb(0, 0, 0, 0) };

			for (int i = 0; i < MainViewModel.NumberOfStorms; i++)
			{
				row.Cells[i + 1].AddParagraph(Format(model.RainfallDischargeDataViewModel.Storms[i].Frequency));
			}

			// 24-hour rainfall row
			row = table.AddRow();
			row.Cells[0].AddParagraph("24-hr rainfall (in)");
			row.Format.Alignment = ParagraphAlignment.Center;

			for (int i = 0; i < MainViewModel.NumberOfStorms; i++)
			{
				row.Cells[i + 1].AddParagraph(Format(model.RainfallDischargeDataViewModel.Storms[i].DayRain));
			}

			// First runoff row 
			row = table.AddRow();
			row.Cells[0].AddParagraph("Runoff (in)");
			row.Format.Alignment = ParagraphAlignment.Center;

			row.Borders.Bottom = new Border() { Color = Color.FromArgb(0, 0, 0, 0) };

			for (int i = 0; i < MainViewModel.NumberOfStorms; i++)
			{
				row.Cells[i + 1].AddParagraph(Format(model.RainfallDischargeDataViewModel.Storms[i].Runoff));
			}

			// second runoff row
			row = table.AddRow();
			row.Cells[0].AddParagraph("(ac-ft)");
			row.Format.Alignment = ParagraphAlignment.Center;

			for (int i = 0; i < MainViewModel.NumberOfStorms; i++)
			{
				double runoffInAcreFeet = (model.RainfallDischargeDataViewModel.Storms[i].Runoff * model.BasicDataViewModel.DrainageArea) / 12;
				row.Cells[i + 1].AddParagraph(Format(runoffInAcreFeet));
			}

			// second runoff row
			row = table.AddRow();
			row.Cells[0].AddParagraph("Peak Discharge (cfs)");
			row.Format.Alignment = ParagraphAlignment.Center;

			for (int i = 0; i < MainViewModel.NumberOfStorms; i++)
			{
				double runoffInAcreFeet = (model.RainfallDischargeDataViewModel.Storms[i].Runoff * model.BasicDataViewModel.DrainageArea) / 12;
				row.Cells[i + 1].AddParagraph(Format(runoffInAcreFeet));
			}
		}

		private static void CreateRcnTable(MainViewModel model, Section section)
		{
			section.AddTextFrame().Height = "1cm";

			Table table = section.AddTable();
			table.Borders.Color = Color.Parse("Black");

			Column col = table.AddColumn("10cm");
			col = table.AddColumn("6cm");

			Paragraph p = GetCenteredParagraph();
			Row row = table.AddRow();
			p.AddText("Acres (CN)");
			row.Cells[1].Add(p);
			p = GetCenteredParagraph();
			row.Cells[0].Borders.Bottom.Color = Color.FromArgb(0, 0, 0, 0);

			row = table.AddRow();
			p.AddText("COVER DESCRIPTION");
			row.Cells[0].Borders.Bottom.Color = Color.FromArgb(0, 0, 0, 0);
			row.Cells[1].Borders.Bottom.Color = Color.FromArgb(0, 0, 0, 0);
			row.Cells[0].Add(p);
			p = GetCenteredParagraph();

			table = section.AddTable();
			table.Borders.Color = Color.Parse("Black");
			table.AddColumn("10cm");
			table.AddColumn("1.5cm");
			table.AddColumn("1.5cm");
			table.AddColumn("1.5cm");
			table.AddColumn("1.5cm");

			row = table.AddRow();
			row.Cells[0].Borders.Top.Color = Color.FromArgb(0,0,0,0);
			p.AddText("A");
			row.Cells[1].Add(p);
			p = GetCenteredParagraph();
			p.AddText("B");
			row.Cells[2].Add(p);
			p = GetCenteredParagraph();
			p.AddText("C");
			row.Cells[3].Add(p);
			p = GetCenteredParagraph();
			p.AddText("D");
			row.Cells[4].Add(p);
			p = GetCenteredParagraph();

			PrintableMainViewModel printableModel = new PrintableMainViewModel(model);

            foreach (var category in printableModel.Categories)
            {
				row = table.AddRow();
				row.Borders.Top.Color = Color.FromArgb(0, 0, 0, 0);
				row.Borders.Bottom.Color = Color.FromArgb(0, 0, 0, 0);

				Paragraph label = new Paragraph();
				label.AddText(category.Label);
				label.GetFont().Size = 8;
				label.GetFont().Bold = true;
				row.Cells[0].Add(label);

				foreach (RcnRowWrapper dataRow in category.Rows)
				{
					row = table.AddRow();	
					row.Borders.Top.Color = Color.FromArgb(0, 0, 0, 0);
					row.Borders.Bottom.Color = Color.FromArgb(0, 0, 0, 0);
					Paragraph rowText = new Paragraph();
					rowText.GetFont().Size = 8;
					rowText.AddSpace(2);
					rowText.AddText(dataRow.BaseRow.Text);
					row.Cells[0].Add(rowText);

					Paragraph aValue = new Paragraph();
					aValue.GetFont().Size = 8;
					aValue.Format.Alignment = ParagraphAlignment.Center;
					aValue.AddText(dataRow.EntryA.Summary);
					row.Cells[1].Add(aValue);

					Paragraph bValue = new Paragraph();
					bValue.GetFont().Size = 8;
					bValue.Format.Alignment = ParagraphAlignment.Center;
					bValue.AddText(dataRow.EntryB.Summary);
					row.Cells[2].Add(bValue);

					Paragraph cValue = new Paragraph();
					cValue.GetFont().Size = 8;
					cValue.Format.Alignment = ParagraphAlignment.Center;
					cValue.AddText(dataRow.EntryC.Summary);
					row.Cells[3].Add(cValue);

					Paragraph dValue = new Paragraph();
					dValue.GetFont().Size = 8;
					dValue.Format.Alignment = ParagraphAlignment.Center;
					dValue.AddText(dataRow.EntryD.Summary);
					row.Cells[4].Add(dValue);
				}

				foreach (var subCategory in category.Subcategories)
				{
					foreach (RcnRowWrapper subRow in subCategory.Rows)
					{
						row = table.AddRow();
						row.Borders.Top.Color = Color.FromArgb(0, 0, 0, 0);
						row.Borders.Bottom.Color = Color.FromArgb(0, 0, 0, 0);

						Paragraph text = new Paragraph();
						text.AddText("\t" + subCategory.Label + "  " + subRow.BaseRow.Text);
						text.GetFont().Size = 8;
						row.Cells[0].Add(text);

						Paragraph aValue = new Paragraph();
						aValue.GetFont().Size = 8;
						aValue.Format.Alignment = ParagraphAlignment.Center;
						aValue.AddText(subRow.EntryA.Summary);
						row.Cells[1].Add(aValue);

						Paragraph bValue = new Paragraph();
						bValue.GetFont().Size = 8;
						bValue.Format.Alignment = ParagraphAlignment.Center;
						bValue.AddText(subRow.EntryB.Summary);
						row.Cells[2].Add(bValue);

						Paragraph cValue = new Paragraph();
						cValue.GetFont().Size = 8;
						cValue.Format.Alignment = ParagraphAlignment.Center;
						cValue.AddText(subRow.EntryC.Summary);
						row.Cells[3].Add(cValue);

						Paragraph dValue = new Paragraph();
						dValue.GetFont().Size = 8;
						dValue.Format.Alignment = ParagraphAlignment.Center;
						dValue.AddText(subRow.EntryD.Summary);
						row.Cells[4].Add(dValue);
					}
				}
			}

			row = table.AddRow();
			row = table.AddRow();
			row.Borders.Top.Color = Color.Parse("black");

			p = GetCenteredParagraph();
			p.GetFont().Size = 9;
			p.AddText("Total Area (by Hydrologic Soil Group)");
			p.Format.Alignment = ParagraphAlignment.Center;
			row.Cells[0].Add(p);
			p = GetCenteredParagraph();

			p.GetFont().Size = 9;
			p.AddText(Format(printableModel.GroupAAccumulatedArea));
			p.Format.Alignment = ParagraphAlignment.Center;
			row.Cells[1].Add(p);
			p = GetCenteredParagraph();

			p.GetFont().Size = 9;
			p.AddText(Format(printableModel.GroupBAccumulatedArea));
			p.Format.Alignment = ParagraphAlignment.Center;
			row.Cells[2].Add(p);
			p = GetCenteredParagraph();

			p.GetFont().Size = 9;
			p.AddText(Format(printableModel.GroupCAccumulatedArea));
			p.Format.Alignment = ParagraphAlignment.Center;
			row.Cells[3].Add(p);
			p = GetCenteredParagraph();

			p.GetFont().Size = 9;
			p.AddText(Format(printableModel.GroupDAccumulatedArea));
			p.Format.Alignment = ParagraphAlignment.Center;
			row.Cells[4].Add(p);
			p = GetCenteredParagraph();

			table = section.AddTable();
			col = table.AddColumn("8cm");
			col = table.AddColumn("8cm");

			row = table.AddRow();
			row.Borders.Color = Color.Parse("black");
			row.Borders.Top.Color = Color.FromArgb(0, 0, 0, 0);
			row.Cells[0].Borders.Right.Color = Color.FromArgb(0, 0, 0, 0);
			row.Cells[1].Borders.Left.Color = Color.FromArgb(0, 0, 0, 0);

			p.AddText("TOTAL DRAINAGE AREA: " + printableModel.MainViewModel.RcnDataViewModel.AccumulatedArea.ToString() + " Acres");
			p.GetFont().Size = 9;
			row.Cells[0].Add(p);
			p = GetCenteredParagraph();

			p.AddText("WEIGHTED CURVE NUMBER: " + printableModel.MainViewModel.RcnDataViewModel.WeightedCurveNumber.ToString());
			p.GetFont().Size = 9;
			row.Cells[1].Add(p);
			p = GetCenteredParagraph();
        }

		private static string Format(double d)
		{
			if (double.IsNormal(d)) return Math.Round(d, 2).ToString();
			else return string.Empty;
		}

		private static string FormatRcn(double d)
		{
			if (double.IsNormal(d)) return d.ToString();
			else return "-";
		}

		private static string GetStatus(InputStatus status)
		{
			switch (status)
			{
				case InputStatus.None:
					return string.Empty;
				case InputStatus.FromRcnCalculator:
					return "(provided from RCN Calculator";
				default:
					return "(user entered value)";
			}
		}

		private static Paragraph GetCenteredParagraph() => new Paragraph() { Format = { Alignment = ParagraphAlignment.Center } };	
	}
}
