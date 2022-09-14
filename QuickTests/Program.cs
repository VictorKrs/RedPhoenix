using System;
using System.Collections.Generic;
using System.IO;

namespace QuickTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles("./logs", "20??-??-??.txt");

            var filesList = new List<string>();

            for (int i = files.Length - 1; i >= 0; i--)
                filesList.Add(files[i]);

            var files2 = filesList.ToArray();

            Array.Sort(files, StringComparer.InvariantCulture);

            Console.ReadLine();
        }
    }
}
