using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JspHashcodMarker
{
    class Program
    {
        static void Main(string[] args)
        {
            bool hasErrors = false;

            try
            {
                MarkJsps("C:\\dev\\spring\\dev\\workspace\\pbuse\\WebContent\\");
            }
            catch (Exception e)
            {
                hasErrors = true;
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Process Complete" + (hasErrors ? " with errors" : ""));
            Console.Read();
        }

        static void MarkJsps(string rootPath)
        {
            FileRepo fileRepo = new FileRepo();
            FileMarker marker = new FileMarker();

            System.Console.WriteLine("Scanning Directory......");

            fileRepo.OnProgress = (x) =>
            {
                System.Console.WriteLine(x);
            };

            int count = 0;
            foreach (var file in fileRepo.GetJspFiles(rootPath))
            {
                System.Console.WriteLine("Marking File " + (count++));
                System.Console.WriteLine("  " + file.FullName);
                marker.Mark(file);
            }
        }
    }
}
