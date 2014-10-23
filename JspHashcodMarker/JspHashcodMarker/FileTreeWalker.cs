using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JspHashcodMarker
{
    public class FileTreeWalker
    {
        public Action<FileInfo> OnFileFound{get;set;}  

        public void Walk(string rootPath, string searchPattern)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(rootPath);
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            FileInfo[] files = directoryInfo.GetFiles(searchPattern);

            foreach (FileInfo fileInfo in files)
            {
                if( OnFileFound != null) 
                    OnFileFound(fileInfo);
            }

            foreach (DirectoryInfo childDirectory in directories)
            {
                Walk(childDirectory.FullName, searchPattern);
            }
        }

    }
}
