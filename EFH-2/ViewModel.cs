/* ViewModel.cs
 * Author: Samuel Gido
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    public class ViewModel
    {
        public string Client { get; set; }

        public string State { get; set; }

        public string County { get; set; }

        public string By { get; set; }

        public DateTimeOffset Date { get; set; }

        public int DrainageArea { get; set; }

        public float CurveNumber { get; set; }

        public int WatershedLength { get; set; }

        public float WatershedSlope { get; set; }

        public float TimeOfConcentration { get; set; }

        private List<string> BasicDataString
        {
            get
            {
                List<string> l = new();

                l.Add(By);
                l.Add(Date.ToString());
                l.Add(Client);
                l.Add(County);
                l.Add(State);
                l.Add(DrainageArea.ToString());
                l.Add(CurveNumber.ToString());
                l.Add(WatershedLength.ToString());
                l.Add(WatershedSlope.ToString());
                l.Add(TimeOfConcentration.ToString());

                return l;
            }
        }

        public string RainfallDistType { get; set; }

        public string DUHType { get; set; }

        public int[] _years = new int[MainWindow._numberOfStorms];

        public float[] _dayRain = new float[MainWindow._numberOfStorms];

        public float[] _peakFlow = new float[MainWindow._numberOfStorms];

        public float[] _runoff = new float[MainWindow._numberOfStorms];

        public bool[] _selectedGraphs = new bool[MainWindow._numberOfStorms];

        private List<string> RainfallDistData
        {
            get
            {
                List<string> l = new();



                return l;
            }
        }

        private StreamWriter writer;

        public void SaveToFile(StreamWriter w)
        {
            writer = w;
            //Version # here

            foreach (string s in BasicDataString)
            {
                Write(s);
            }


        }

        private void Write(string s)
        {
            writer.WriteLine('"' + s + '"');
        }
    }
}
