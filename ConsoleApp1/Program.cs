using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int iterations = 10;
            int reps = 100;
            string src = "big-file.txt";
            string dest = "big-file-copy.txt";
            string results = "results.txt";

            if (File.Exists(results)) File.Delete(results);
            List<string> resultsArray = new List<string>();
            Action<string> output = (line) => resultsArray.Add(line);

            for (int i = 0; i < iterations; i++)
            {
                TimeFunction("Old copying", reps, () => File.Copy(src, dest, true), output);
                foreach (int sqrtb in new int[] { 1024, 2048, 4096, 8192 })
                    TimeFunction($"Filestream copying ({sqrtb} * {sqrtb})", reps, () => FastCopy(src, dest), output);
            }

            File.WriteAllLines(results, resultsArray);

            Console.ReadLine();
        }

        public static void TimeFunction(string name, int reps, Action function, Action<string> output)
        {
            DateTime startTime = DateTime.Now;

            for (int i = 0; i < reps; i++)
                function();

            DateTime endTime = DateTime.Now;

            double duration = ((endTime - startTime).TotalMilliseconds) / reps;

            output($"{name}: {duration}ms");
        }

        public static void FastCopy(string inputFilePath, string outputFilePath, int sqrtBuffersize = 1024)
        {
            //  https://stackoverflow.com/questions/1246899/file-copy-vs-manual-filestream-write-for-copying-file/1247042#1247042

            int bufferSize = sqrtBuffersize * sqrtBuffersize;
            FileStream inStream = File.OpenRead(inputFilePath);

            using (FileStream fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fileStream.SetLength(inStream.Length);
                int bytesRead = -1;
                byte[] bytes = new byte[bufferSize];

                while ((bytesRead = inStream.Read(bytes, 0, bufferSize)) > 0)
                {
                    fileStream.Write(bytes, 0, bytesRead);
                }
            }
        }
    }
}
