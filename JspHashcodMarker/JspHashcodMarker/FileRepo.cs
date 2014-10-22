using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JspHashcodMarker
{
    public class FileRepo
    {
        public Action<string> OnProgress { get; set; }

        public List<FileInfo> GetJspFiles(String rootPath)
        {
            FileTreeWalker walker = new FileTreeWalker();
            List<FileInfo> files = new List<FileInfo>();

            walker.FileFound = (x) =>
            {
                files.Add(x);
                if (OnProgress != null)
                {
                    OnProgress("File Added: " + x.FullName);
                }
            };

            //get all JSP and HTML files
            walker.Walk(rootPath, "*.jsp");
            walker.Walk(rootPath, "*.htm");  //this also includes .html 

            return files;
        }
    }
}
