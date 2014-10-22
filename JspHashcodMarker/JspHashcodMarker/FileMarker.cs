using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JspHashcodMarker
{
    public class FileMarker
    {
        public void Mark(FileInfo file)
        {
            List<String> linesIn = new List<string>();
            using (StreamReader reader = new StreamReader(file.FullName))
            {
                while (reader.Peek() != -1)
                {
                    linesIn.Add( reader.ReadLine() );
                } 
            }

            File.Delete(file.FullName);

            System.Threading.Thread.Sleep(50); //sleep for one second to allow deletion of file

            bool hasBeenMarked = false;

            using (StreamWriter writer = new StreamWriter(file.FullName))
            {
                FileNameToHashcode fileNameToHashcode = new FileNameToHashcode();

                for(int i = 0; i < linesIn.Count; i++)
                {
                    if(!hasBeenMarked && IsHeadTag(linesIn[i]))
                    {
                        writer.WriteLine("<!-- File ID: " + fileNameToHashcode.SHA1(file.FullName) + " -->");
                        hasBeenMarked = true;
                    }

                    writer.WriteLine(linesIn[i]);
                }                
            }
        }

        private bool IsHeadTag(string line)
        {
            if (line.ToLower().Contains("<head>"))
            {
                return true;
            }

            return false;
        }
    }
}
