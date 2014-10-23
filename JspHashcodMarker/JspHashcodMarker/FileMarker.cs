using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JspHashcodMarker
{
    public class FileMarker
    {

        //This marker is the heading label to help us identify if the file has been marked
        private readonly string FILE_ID_MARKER = "<!-- File ID: ";

        FileNameToHashcode _fileNameToHashcode = new FileNameToHashcode();
        static Dictionary<string, int> _hashcodeDictionary = new Dictionary<string, int>();

        public Action<string, string> onNewHashcode { get; set; }

        public bool Mark(FileInfo file)
        {
            List<String> linesIn = new List<string>();
            using (StreamReader reader = new StreamReader(file.FullName))
            {
                while (reader.Peek() != -1)
                {
                    string line = reader.ReadLine();

                    //Do not mark files that have been previously marked
                    if (line.Contains(FILE_ID_MARKER))
                    {
                        return false;  //It has been marked previously so no need to mark it
                    }

                    linesIn.Add(line);
                } 
            }

            File.Delete(file.FullName);

            System.Threading.Thread.Sleep(50); //sleep for one second to allow deletion of file

            bool hasBeenMarked = false;

            DateTime dtNow = DateTime.Now;

            using (StreamWriter writer = new StreamWriter(file.FullName))
            {
                 for(int i = 0; i < linesIn.Count; i++)
                {
                    if(!hasBeenMarked && IsHeadTag(linesIn[i]))
                    {
                        //Trim the root folder so that it is excluded fromt hash computation
                        //e.g. instead of C:\dev\spring\dev\workspace\pbuse\WebContent\gcss\jsp\myproject\myfile.jsp
                        //the hash only uses gcss\jsp\myproject\myfile.jsp
                        //in this manner the hash computations will be the exact same no matter who does it on their 
                        //box even if they have the project in a different path than mine.
                        string fileId = CreateFileId(file.FullName.Replace(JspHashcodMarker.Program.RootFolder, "\\"));

                        //To avoid having IDs with the same hash... Just in case of a collision
                        //The last five numbers
                        string sequenceNumber = GetFileIdSequenceNumer(fileId).ToString().PadLeft(5, '0');

                        writer.WriteLine(FILE_ID_MARKER + fileId + "-" + sequenceNumber + " Date Marked: " +
                                         dtNow.ToString("MMMM dd, yyyy ") + " -->");
                        hasBeenMarked = true;
                    }

                    writer.WriteLine(linesIn[i]);
                }                
            }

            return true;
        }

        //sequence number is an additional salt to 
        private int GetFileIdSequenceNumer(string fileId)
        {
            int sequenceId = 1;

            if (_hashcodeDictionary.ContainsKey(fileId))
            {
                sequenceId = _hashcodeDictionary[fileId] = _hashcodeDictionary[fileId] + 1;
            }
            else
            {
                _hashcodeDictionary.Add(fileId, sequenceId);
            }

            return sequenceId;
        }

        private string CreateFileId(string value)
        {
            string fileId = _fileNameToHashcode.SHA1(value);

            if (onNewHashcode != null)
                onNewHashcode(fileId, value);

            return fileId;
        }

        private bool IsHeadTag(string line)
        {
            if (!string.IsNullOrEmpty(line) && line.ToLower().Contains("<head>"))
            {
                return true;
            }

            return false;
        }
    }
}
