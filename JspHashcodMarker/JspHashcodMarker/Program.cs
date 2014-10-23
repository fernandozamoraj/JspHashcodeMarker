using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 *  Author: Fernando Zamora
 *  Description: This program assigns a file ID to each of our JSP files. 
 *  This is so that we can identify each file with the corresponding file 
 *  in the source code. I considered using filenames with paths but decided
 *  agains using file names since that could reveal more information than
 *  necessary making a security risk (e.g. revealing the path and name of
 *  a jsp could reveal the entire file structure to a hacker for whatever
 *  malicious purposes a hacker may have). 
 *  
 *  I also decided against using a simple sequence number to each file.
 *  The sequence number concept works fine as an ID. However as newer files
 *  are added to the system and the IDs are required to be assigned. The
 *  IDs become arbitrary. What I mean by that is that a hascode belongs to
 *  exactly one file. The hashcode is converted from the full file path. Even 
 *  in cases where a hashcode may be duplicated to two different files the
 *  hashcode remains unique because of the additional five character sequence
 *  number padded at the end.  In this manner this program can used to not
 *  only assign the initial hashcodes but also to update the system as new
 *  JSP files are added.
 *  
 *  
 * 
 */
namespace JspHashcodMarker
{
    class Program
    {
        static Dictionary<string, int> _hashCodes = new Dictionary<string, int>();
        public static string RootFolder = "C:\\dev\\spring\\dev\\workspace\\pbuse\\WebContent\\";

        static void Main(string[] args)
        {
            bool hasErrors = false;

            try 
            {
                //Change this path to your local path
                MarkJsps(RootFolder);
                ReportCollisions();
            }
            catch (Exception e)
            {
                hasErrors = true;
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Process Complete" + (hasErrors ? " with errors." : "."));
            Console.WriteLine();
            Console.WriteLine("Press the Enter key to exit the program.");
            Console.Read();
        }

        static void ReportCollisions()
        {
            int collisionCount = 0;

            Console.WriteLine("*************Reporting Hashcode Collisions**************");
            foreach (var hashCode in _hashCodes)
            {
                if (hashCode.Value > 1)
                {
                    collisionCount++;
                    Console.WriteLine(hashCode.Key + ": " + hashCode.Value.ToString().PadLeft(5));
                }
            }

            Console.WriteLine(collisionCount + " collisions detected.");
            Console.WriteLine();
            Console.WriteLine("*************End of Collision Report**************");

        }
        static void MarkJsps(string rootPath)
        {
            FileRepo fileRepo = new FileRepo();
            FileMarker marker = new FileMarker();

            System.Console.WriteLine("Scanning Directory......");

            marker.onNewHashcode = (hashCode, fileName) =>
            {
                System.Console.WriteLine("New hash: " + hashCode + " " + fileName);
                if (_hashCodes.ContainsKey(hashCode))
                    _hashCodes[hashCode] = _hashCodes[hashCode] + 1;
                else
                    _hashCodes.Add(hashCode, 1);
            };

            fileRepo.OnProgress = (progressMessage) =>
            {
                System.Console.WriteLine(progressMessage);
            };

            int count = 0;
            foreach (var file in fileRepo.GetJspFiles(rootPath))
            {
                System.Console.WriteLine("Marking File " + (count++));
                System.Console.WriteLine("  " + file.FullName);
                if(marker.Mark(file))
                    System.Console.WriteLine("  " + "file marked");
                else
                    System.Console.WriteLine("  " + "file skipped");
            }
        }
    }
}
