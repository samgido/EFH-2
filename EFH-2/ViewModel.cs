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
        public string Client { get; 
            set
            {
                Client = value;
            }
        }

    public string State { get; set; }

        public string County { get; set; }

        public string Practice { get; set; }

        public string By { get; set; }

        public DateTimeOffset Date { get; set; }

        public int DrainageArea { get; set; }

        public float CurveNumber { get; set; }

        public int WatershedLength { get; set; }

        public float WatershedSlope { get; set; }

        public float TimeOfConcentration { get; set; }

        private List<object> BasicDataString
        {
            get
            {
                List<object> l = new();

                l.Add(By);
                l.Add(Date);
                l.Add(Client);
                l.Add(County);
                l.Add(State);
                l.Add(Practice);
                l.Add(DrainageArea);
                l.Add(CurveNumber);
                l.Add(WatershedLength);
                l.Add(WatershedSlope);
                l.Add(TimeOfConcentration);

                return l;
            }
        }

        public string RainfallDistType { get; set; }

        public string DUHType { get; set; }

        public int[] _freq = new int[MainWindow._numberOfStorms];

        public float[] _dayRain = new float[MainWindow._numberOfStorms];

        public float[] _peakFlow = new float[MainWindow._numberOfStorms];

        public float[] _runoff = new float[MainWindow._numberOfStorms];

        public bool[] _selectedGraphs = new bool[MainWindow._numberOfStorms];

        private List<object> RainfallDistData
        {
            get
            {
                List<object> l = new();

                l.Add(RainfallDistType + ',' + DUHType);

                for (int i = 0; i < _freq.Length; i++)
                {
                    l.Add(_freq[i]);
                    l.Add(_dayRain[i]);
                }

                l.Add("");


                return l;
            }
        }

        public void SaveToFile(StreamWriter w)
        {
            //Version # here

            foreach (object s in BasicDataString)
            {
                WriteLine(s, w);
            }

            WriteLine("", w);
            WriteLine("", w);
            WriteLine("", w);


            // Last bit

            for (int i = 0; i < _freq.Length; i++)
            {
                Write(_freq[i], w);
                w.Write(',');
                Write(_dayRain[i], w);
                w.Write(',');
                Write(_peakFlow[i], w);
                w.Write(',');
                WriteLine(_runoff[i], w);
            }

        }

        private void WriteLine(object s, StreamWriter writer)
        {
            if (s == null) { s = ""; }
            writer.WriteLine('"' + s.ToString() + '"');
        }

        private void Write(object s, StreamWriter writer)
        {
            if (s == null) { s = ""; }
            writer.Write('"' + s.ToString() + '"');
        }

        public void OpenFile(string fn)
        {
            using (StreamReader reader = new StreamReader(fn))
            {
                string _ = reader.ReadLine();
                By = reader.ReadLine();
            }
        }
    }
}
