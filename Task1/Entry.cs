using System;

namespace Task1
{
    public class Entry
    {
        public double[] EntryParams { get; set; }
        public string EntryClass { get; set; }
        public string Prediction { get; set; }

        public double GetDistance(Entry e)
        {
            double distance = .0;
			
            for (var i = 0; i < e.EntryParams.Length; i++)
            {
                distance += Math.Pow(EntryParams[i] - e.EntryParams[i], 2);
            }

            return distance;
        }

        public override string ToString()
        {
            var str = String.Join(",", EntryParams) + $" ({EntryClass})";

            if (Prediction != null)
            {
                str += $" - our prediction={Prediction}";
            }
			
            return str;
        }
    }
}