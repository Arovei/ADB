using System;
using System.Linq;
using System.IO;

namespace ADB
{
    class ReportFile
    {
        public static long[] ReportStorage(long[] report, bool firstBoot)
        {
            string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "d20report.txt");
            if (firstBoot)
            {
                if (File.Exists(destPath))
                {
                    // Read the file as one string.
                    StreamReader myFile = new StreamReader(destPath);
                    string inputFile = myFile.ReadToEnd();

                    myFile.Close();

                    report = inputFile.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();                    
                }
                else
                {
                    using (StreamWriter sr = new StreamWriter(destPath))
                    {
                        foreach (var amount in report)
                        {
                            sr.Write(amount + " ");
                        }
                    }
                }                
            }
            else
            {
                using (StreamWriter sr = new StreamWriter(destPath))
                {
                    foreach (long amount in report)
                    {
                        sr.Write(amount.ToString() + " ");
                    }
                }
            }
            return report;
        }
    }
}
