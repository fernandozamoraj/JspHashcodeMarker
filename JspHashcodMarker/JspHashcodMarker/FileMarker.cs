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
        SHAConverter _fileNameToHashcodeConverter = new SHAConverter();
        CollisionDetector _collisionDetector;
        
        public Action<string, string> onNewHashcode { get; set; }

        public FileMarker(CollisionDetector collisionDetector)
        {
            _collisionDetector = collisionDetector;
        }

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
                        //string fileId = CreateFileId(file.FullName.Replace(JspHashcodMarker.Program.RootFolder, "\\"));
                        string fileId = CreateFileId(file.Name);

                        //To avoid having IDs with the same hash... Just in case of a collision
                        //The last five numbers
                        string sequenceNumber = GetFileIdSequenceNumber(fileId, file.FullName).ToString().PadLeft(5, '0');
                        string sequenceTotal = GetNumberOfTotalCollisions(fileId).ToString().PadLeft(5, '0');

                        writer.WriteLine(FILE_ID_MARKER + fileId + "-" + sequenceNumber + "of" + sequenceTotal + " Date Marked: " +
                                         dtNow.ToString("MMMM dd, yyyy ") + " -->");
                        hasBeenMarked = true;
                    }

                    writer.WriteLine(linesIn[i]);
                }                
            }

            return true;
        }
        

        /*
         * Figures out the sequence by matchin the full file path to the sequence in the list 
         * e.g.
         * abg4b34b34b3343b3eb334b has a list of 3 files
         *                          c:\temp\myfile1.jsp             has a position of 1
         *                          c:\temp\sub\myfile1.jsp         has a position of 2
         *                          c:\temp\sub2\myfile1.jsp        has a position of 3 
         * 
         * each of those files would have a sequence equal to their position 
         * 
         *          
         */
        int GetFileIdSequenceNumber(string fileId, string fullName){

            if(_collisionDetector.Collisions.ContainsKey(fileId)){
                List<string> fileNames = _collisionDetector.Collisions[fileId];

                for(int i = 0; i < fileNames.Count; i++)
                {
                    if(fileNames[i].CompareTo(fullName) == 0)
                    {
                        return i+1; //add 1 to avoid using 0 as a sequence
                    }
                }
            }

            return 1;
        }

        int GetNumberOfTotalCollisions(string fileId)
        {
            if (_collisionDetector.Collisions.ContainsKey(fileId))
            {
                return _collisionDetector.Collisions[fileId].Count;
            }

            return 1;
        }

        private string CreateFileId(string value)
        {
            string fileId = _fileNameToHashcodeConverter.SHA1(value);

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
