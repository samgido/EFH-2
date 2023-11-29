using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2.Misc
{

    public class FileOperations
    {
        private const string InvalidEntryReplacerMessage = "Invalid Entry";

        public static void PrintData(BasicDataViewModel BasicVM, RainfallDataViewModel RainfallVM, string fn)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            
            Document.Merge(
                GeneratePageOne(BasicVM, RainfallVM), 
                GeneratePageTwo(BasicVM, RainfallVM))
                .GeneratePdf(fn);


            static Document GeneratePageOne(BasicDataViewModel BasicVM, RainfallDataViewModel RainfallVM)
            {
                return Document.Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Content().Background(Colors.Grey.Medium);
                        page.Size(PageSizes.A4);
                        page.Margin(0.5f, Unit.Centimetre);
                        page.DefaultTextStyle(style => style.FontSize(11).WrapAnywhere(false));

                        page.Header().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });


                            table.Cell().Row(1).Column(1).Text(text =>
                            {
                                text.Span("EFH-2");
                            });

                            table.Cell().Row(1).Column(2).Text(text =>
                            {
                                text.AlignCenter();
                                text.Span("ESTIMATING RUNOFF VOLUME AND PEAK DISCHARGE");
                            });

                            table.Cell().Row(1).Column(3).Text(text =>
                            {
                                text.AlignRight();
                                text.Span("Version _");
                            });

                            table.Cell().Row(2).ColumnSpan(3).Height(30).Canvas((canvas, size) =>
                            {
                                using var paint = new SKPaint
                                {
                                    Color = SKColors.Black,
                                    StrokeWidth = 2,
                                    IsStroke = true
                                };

                                canvas.DrawLine(10, 10, size.Width - 10, 10, paint);
                            });
                        });

                        page.Content().Layers(layers =>
                        {
                            layers.PrimaryLayer();

                            layers.Layer().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(130);
                                    columns.ConstantColumn(75);
                                    columns.ConstantColumn(75);
                                    columns.ConstantColumn(50);
                                    columns.ConstantColumn(20);
                                    columns.RelativeColumn();
                                });

                                // basic data text field labels

                                table.Cell().Row(1).Column(1).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Client:");
                                });

                                table.Cell().Row(2).Column(1).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("County:");
                                });

                                table.Cell().Row(2).Column(4).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("State:");
                                });

                                table.Cell().Row(3).Column(1).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Practice:");
                                });

                                table.Cell().Row(4).Column(1).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Calculate By:");
                                });

                                table.Cell().Row(4).Column(4).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Date:");
                                });

                                table.Cell().Row(5).Column(4).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Date:");
                                });

                                table.Cell().Row(5).Column(1).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Checked By:");
                                });

                                // basic data entry fields

                                table.Cell().Row(1).Column(2).ColumnSpan(3).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span(BasicVM.Client);
                                });

                                table.Cell().Row(2).Column(2).ColumnSpan(3).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span(BasicVM.SelectedCounty);
                                });

                                table.Cell().Row(3).Column(2).ColumnSpan(3).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span(BasicVM.Practice);
                                });

                                table.Cell().Row(4).Column(2).ColumnSpan(3).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span(BasicVM.By);
                                });

                                table.Cell().Row(5).Column(2).ColumnSpan(3).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span("_____________");
                                });

                                table.Cell().Row(2).Column(6).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span(BasicVM.SelectedState);
                                });

                                table.Cell().Row(4).Column(6).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");

                                    DateTime date = BasicVM.Date.Date;

                                    text.Span(date.ToString("MM/dd/yyyy"));
                                });

                                table.Cell().Row(5).Column(6).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span("______________________");
                                });

                                // basic data number field labels

                                table.Cell().Row(6).Text("");
                                table.Cell().Row(7).Text("");

                                table.Cell().Row(8).Column(1).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Drainage Area:");
                                });

                                table.Cell().Row(9).Column(1).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Curve Number:");
                                });

                                table.Cell().Row(10).Column(1).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Watershed Length:");
                                });

                                table.Cell().Row(11).Column(1).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Watershed Slope:");
                                });

                                table.Cell().Row(12).Column(1).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Time of Concentration:");
                                });

                                table.Cell().Row(13).Column(1).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Rainfall Distribution - Type:");
                                });

                                table.Cell().Row(14).Column(1).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Dimensionless Unit Hydrograph:");
                                });

                                // basic data number field entries

                                table.Cell().Row(8).Column(2).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span(BasicVM.DrainageArea.ToString());
                                });

                                table.Cell().Row(9).Column(2).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span(BasicVM.RunoffCurveNumber.ToString());
                                });

                                table.Cell().Row(10).Column(2).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span(BasicVM.WatershedLength.ToString());
                                });

                                table.Cell().Row(11).Column(2).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span(BasicVM.WatershedSlope.ToString());
                                });

                                table.Cell().Row(12).Column(2).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();

                                    double toc = BasicVM.TimeOfConcentration;
                                    toc = Math.Round(toc, 3);

                                    text.Span(toc.ToString());
                                });

                                table.Cell().Row(13).Column(2).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span(RainfallVM.SelectedRainfallDistributionType);
                                });

                                table.Cell().Row(14).Column(2).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span(RainfallVM.SelectedDUHType);
                                });

                                // basic data unit labels

                                table.Cell().Row(8).Column(4).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span("Acres");
                                });

                                table.Cell().Row(10).Column(4).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span("Feet");
                                });

                                table.Cell().Row(11).Column(4).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span("Percent");
                                });

                                table.Cell().Row(12).Column(4).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span("Hours");
                                });

                                // Status labels

                                table.Cell().Row(8).Column(5).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignLeft();
                                    string msg = BasicVM.DrainageAreaStatus;

                                    if(msg == MainWindow.DrainageAreaInvalidEntryMessage) { msg = InvalidEntryReplacerMessage; }
                                    text.Span("(" + msg + ")");
                                });

                                table.Cell().Row(9).Column(5).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignLeft();
                                    string msg = BasicVM.RunoffCurveNumberStatus;

                                    if(msg == MainWindow.RunoffCurveNumberInvalidEntryMessage) { msg = InvalidEntryReplacerMessage; }
                                    text.Span("(" + msg + ")");
                                });

                                table.Cell().Row(10).Column(5).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignLeft(); 
                                    string msg = BasicVM.WatershedLengthStatus;

                                    if(msg == MainWindow.WatershedLengthInvalidEntryMessage) { msg = InvalidEntryReplacerMessage; }
                                    text.Span("(" + msg + ")");
                                });

                                table.Cell().Row(11).Column(5).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignLeft();
                                    string msg = BasicVM.WatershedSlopeStatus;

                                    if(msg == MainWindow.WatershedSlopeInvalidEntryMessage) { msg = InvalidEntryReplacerMessage; }
                                    text.Span("(" + msg + ")");
                                });

                                table.Cell().Row(12).Column(5).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignLeft();
                                    string msg = BasicVM.TimeOfConcentrationStatus;

                                    if(msg == MainWindow.TimeOfConcentrationInvalidEntryMessage) { msg = InvalidEntryReplacerMessage; }
                                    text.Span("(" + msg + ")");
                                });

                                table.Cell().Row(13).Column(5).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("(" + RainfallVM.RainfallDistributionTypeStatus + ")");
                                });

                                table.Cell().Row(14).Column(5).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("(" + RainfallVM.DUHTypeStatus + ")");
                                });

                            });

                            layers.Layer().AlignMiddle().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    int stormWidth = 55;

                                    columns.ConstantColumn(50);
                                    columns.ConstantColumn(110);
                                    columns.ConstantColumn(stormWidth);
                                    columns.ConstantColumn(stormWidth);
                                    columns.ConstantColumn(stormWidth);
                                    columns.ConstantColumn(stormWidth);
                                    columns.ConstantColumn(stormWidth);
                                    columns.ConstantColumn(stormWidth);
                                    columns.ConstantColumn(stormWidth);
                                });

                                table.Cell().Column(2).Row(1).Border(1).AlignMiddle().Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("Storm Number");
                                });

                                table.Cell().Row(2).Height(5);

                                table.Cell().Column(2).Row(2).RowSpan(3).Border(1).Height(40);

                                table.Cell().Column(2).Row(3).RowSpan(2).Text(text =>
                                {
                                    text.ParagraphSpacing(4f);

                                    text.Span("  ");
                                    text.Span("Frequency (yrs)");

                                    text.EmptyLine();

                                    text.Span("  ");
                                    text.Span("24-Hr rainfall (in)");
                                });

                                table.Cell().Column(2).Row(5).RowSpan(3).Border(1);
                                table.Cell().Column(2).Row(6).RowSpan(2).Text(text =>
                                {
                                    text.ParagraphSpacing(4f);

                                    text.Span("  ");
                                    text.Span("Runoff  (in)");

                                    text.EmptyLine();

                                    text.Span("  ");
                                    text.Span("               (ac-ft)");
                                });

                                table.Cell().Column(2).Row(8).Border(1).AlignMiddle().Text(text =>
                                {
                                    text.Span("  ");
                                    text.Span("Peak Discharge (cfs)");
                                });


                                for (int i = 0; i < 7; i++)
                                {
                                    uint col = (uint)(i + 3);

                                    table.Cell().Column(col).Row(1).Border(1).Height(25).AlignMiddle().Text(text =>
                                    {
                                        text.AlignCenter();
                                        text.Span((i + 1).ToString());
                                    });

                                    table.Cell().Column(col).Row(2).RowSpan(3).Border(1);

                                    table.Cell().Column(col).Row(3).RowSpan(2).Text(text =>
                                    {
                                        text.ParagraphSpacing(4f);
                                        text.AlignRight();

                                        double freq = RainfallVM.Storms[i].Frequency;
                                        freq = Math.Round(freq, 3);

                                        text.Span(freq.ToString());
                                        text.Span("  ");

                                        text.EmptyLine();

                                        double dayRain = RainfallVM.Storms[i].DayRain;
                                        dayRain = Math.Round(dayRain, 3);

                                        text.Span(dayRain.ToString());
                                        text.Span("  ");

                                    });

                                    table.Cell().Row(5).Height(5);
                                    table.Cell().Column(col).Row(5).RowSpan(3).Border(1);

                                    table.Cell().Column(col).Row(6).RowSpan(2).Text(text =>
                                    {
                                        text.ParagraphSpacing(4f);
                                        text.AlignRight();

                                        text.Span(RainfallVM.Storms[i].Runoff.ToString());
                                        text.Span("  ");

                                        text.EmptyLine();

                                        text.Span("ac-ft");
                                        text.Span("  ");
                                    });

                                    table.Cell().Column(col).Row(8).Border(1).Height(20).AlignMiddle().Text(text =>
                                    {
                                        text.AlignCenter();

                                        text.Span(RainfallVM.Storms[i].PeakFlow.ToString());
                                    });

                                }

                            });

                        });

                    });
                });

            }

            static Document GeneratePageTwo(BasicDataViewModel BasicVM, RainfallDataViewModel RainfallVM)
            {
                return Document.Create(document =>
                {
                    document.Page(page =>
                    {

                        page.Content().Background(Colors.Grey.Medium);
                        page.Size(PageSizes.A4);
                        page.Margin(0.5f, Unit.Centimetre);
                        page.DefaultTextStyle(style => style.FontSize(11).WrapAnywhere(false));

                        page.Header().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });


                            table.Cell().Row(1).Column(1).Text(text =>
                            {
                                text.Span("EFH-2");
                            });

                            table.Cell().Row(1).Column(2).Text(text =>
                            {
                                text.AlignCenter();
                                text.Span("ESTIMATING RUNOFF VOLUME AND PEAK DISCHARGE");
                            });

                            table.Cell().Row(1).Column(3).Text(text =>
                            {
                                text.AlignRight();
                                text.Span("Version _");
                            });

                            table.Cell().Row(2).ColumnSpan(3).Height(30).Canvas((canvas, size) =>
                            {
                                using var paint = new SKPaint
                                {
                                    Color = SKColors.Black,
                                    StrokeWidth = 2,
                                    IsStroke = true
                                };

                                canvas.DrawLine(10, 10, size.Width - 10, 10, paint);
                            });
                        });

                        page.Content().Layers(layers =>
                        {

                            layers.PrimaryLayer();

                            layers.Layer().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(130);
                                    columns.ConstantColumn(75);
                                    columns.ConstantColumn(75);
                                    columns.ConstantColumn(50);
                                    columns.ConstantColumn(20);
                                    columns.RelativeColumn();
                                });

                                // basic data text field labels

                                table.Cell().Row(1).Column(1).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Client:");
                                });

                                table.Cell().Row(2).Column(1).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("County:");
                                });

                                table.Cell().Row(2).Column(4).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("State:");
                                });

                                table.Cell().Row(3).Column(1).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Practice:");
                                });

                                table.Cell().Row(4).Column(1).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Calculate By:");
                                });

                                table.Cell().Row(4).Column(4).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Date:");
                                });

                                table.Cell().Row(5).Column(4).ColumnSpan(2).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Date:");
                                });

                                table.Cell().Row(5).Column(1).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("Checked By:");
                                });

                                // basic data entry fields

                                table.Cell().Row(1).Column(2).ColumnSpan(3).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span(BasicVM.Client);
                                });

                                table.Cell().Row(2).Column(2).ColumnSpan(3).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span(BasicVM.SelectedCounty);
                                });

                                table.Cell().Row(3).Column(2).ColumnSpan(3).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span(BasicVM.Practice);
                                });

                                table.Cell().Row(4).Column(2).ColumnSpan(3).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span(BasicVM.By);
                                });

                                table.Cell().Row(5).Column(2).ColumnSpan(3).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span("______________________");
                                });

                                table.Cell().Row(2).Column(6).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span(BasicVM.SelectedState);
                                });

                                table.Cell().Row(4).Column(6).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");

                                    DateTime date = BasicVM.Date.Date;

                                    text.Span(date.ToString("MM/dd/yyyy"));
                                });

                                table.Cell().Row(5).Column(6).Text(text =>
                                {
                                    text.AlignLeft();
                                    text.Span("  ");
                                    text.Span("______________________");
                                });
                            });

                            layers.Layer().AlignTop().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    uint repeatingCol = 60;

                                    columns.ConstantColumn(35);
                                    columns.ConstantColumn(250);
                                    columns.ConstantColumn(repeatingCol);
                                    columns.ConstantColumn(repeatingCol);
                                    columns.ConstantColumn(repeatingCol);
                                    columns.ConstantColumn(repeatingCol);
                                });

                                table.Cell().Row(1).Height(120);

                                table.Cell().Row(2).Column(3).ColumnSpan(4).Border(1).Height(20).Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("Acres (CN)");
                                });

                                table.Cell().Row(3).Column(3).ColumnSpan(4).Border(1).Height(20).Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("Hydrologic Soil Group");
                                });

                                // a b c d 

                                table.Cell().Row(4).Column(3).Border(1).Height(20).Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("A");
                                });
                                table.Cell().Row(4).Column(4).Border(1).Height(20).Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("B");
                                });
                                table.Cell().Row(4).Column(5).Border(1).Height(20).Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("C");
                                });
                                table.Cell().Row(4).Column(6).Border(1).Height(20).Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("D");
                                });

                                table.Cell().Row(2).RowSpan(3).Column(2).Border(1).AlignMiddle().Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("COVER DESCRIPTION");
                                });

                                table.Cell().Column(2).Row(5).RowSpan(4).Border(1).Height(60);
                                table.Cell().Column(2).Row(5).AlignMiddle().Text(text =>
                                {
                                    text.EmptyLine();
                                    text.Span("  ");
                                    text.Span("CULTIVATED AGRICULTURAL LANDS").Bold();

                                    text.EmptyLine();

                                    text.Span("     ");
                                    text.Span("Row crops");

                                    text.Span("                   ");
                                    text.Span("Contoured (C)");

                                    text.Span("                   ");
                                    text.Span("good");

                                });

                                table.Cell().Column(3).Row(5).RowSpan(4).Border(1).AlignMiddle().Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("-");
                                });
                                table.Cell().Column(4).Row(5).RowSpan(4).Border(1).AlignMiddle().Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("160(75)");
                                });
                                table.Cell().Column(5).Row(5).RowSpan(4).Border(1).AlignMiddle().Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("80(82)");
                                });
                                table.Cell().Column(6).Row(5).RowSpan(4).Border(1).AlignMiddle().Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("-");
                                });

                                table.Cell().Column(2).Row(9).Border(1).Height(20).Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("Total Area (by Hydrological Soil Group");
                                });

                                table.Cell().Column(3).Row(9).Border(1).Text(text =>
                                {
                                    text.AlignCenter();
                                });
                                table.Cell().Column(4).Row(9).Border(1).Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("160");
                                });
                                table.Cell().Column(5).Row(9).Border(1).Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("80");
                                });
                                table.Cell().Column(6).Row(9).Border(1).Text(text =>
                                {
                                    text.AlignCenter();
                                });

                                table.Cell().Row(10).Column(2).ColumnSpan(5).Border(1).Height(20);

                                table.Cell().Row(10).Column(2).Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("TOTAL DRAINAGE AREA: 240 Acres");
                                });

                                table.Cell().Row(10).Column(3).ColumnSpan(4).Text(text =>
                                {
                                    text.AlignRight();
                                    text.Span("WEIGHTED CURVE NUMBER: 77");
                                    text.Span("      ");
                                });
                            });

                        });

                    });
                });
            }
        }

    }

    /// <summary>
    /// Helper class that writes to the save file
    /// </summary>
    public class Writer
    {
        /// <summary>
        /// Writes to a file
        /// </summary>
        private readonly StreamWriter _writer;

        public Writer(StreamWriter w)
        {
            this._writer = w;
        }

        /// <summary>
        /// Writes an object's .ToString(), wrapped with quotes, to the file,
        /// optionally moves the writer to the next line
        /// </summary>
        /// <param name="s">Object who's ToString will be written</param>
        /// <param name="next">Whether or not the writer moves to the next line</param>
        public void WriteQuoted(object s, bool next)
        {
            string content = '"' + s.ToString() + '"';
            this._writer.Write(content);
            if(next) { _writer.WriteLine(""); }
        }

        /// <summary>
        /// Writes an object's .ToString() to the file,
        /// optionally moves the writer to the next line
        /// </summary>
        /// <param name="s">Object who's ToString will be written</param>
        /// <param name="next">Whether or not the writer moves to the next line</param>
        public void Write(object s, bool next)
        {
            this._writer.Write(s.ToString());
            if(next) { _writer.WriteLine(""); }
        }
    }


    public class Reader
    {
        private StreamReader _r;

        public Reader(StreamReader r)
        {
            _r = r;
        }

        /// <summary>
        /// Reads a line from the reader, but removes at the beginning and end of the line
        /// </summary>
        /// <returns>The unquoted line</returns>
        public string ReadQuoted()
        {
            string line = this._r.ReadLine();

            line.Remove(0);
            line.Remove(line.Length - 1);

            line = line.Replace("\"", "");

            return line.Trim();
        }

        /// <summary>
        /// Reads a line from the reader
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            return this._r.ReadLine().Trim();
        }

        public int ParseInt(string s)
        {
            if (Int32.TryParse(s, out var temp))
            {
                return temp;
            }
            else { return 0; }
        }

        public double ParseDouble(string s)
        {
            if (double.TryParse(s, out var temp))
            {
                return temp;
            }
            else { return 0; }
        }

        public float ParseFloat(string s)
        {
            if (float.TryParse(s, out var temp))
            {
                return temp;
            }
            else { return 0; }
        }
    }

}
