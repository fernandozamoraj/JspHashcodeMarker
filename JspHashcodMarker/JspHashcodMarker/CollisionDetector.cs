using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JspHashcodMarker
{
    public class CollisionDetector
    {
        public Dictionary<string, List<string>> Collisions { get; private set; }

        public void ScanForCollisions(string rootPath)
        {
            SHAConverter hasher = new SHAConverter();
            Dictionary<string, List<string>> allFiles = new Dictionary<string, List<string>>();

            FileRepo fileRepo = new FileRepo();

            foreach (var file in fileRepo.GetJspFiles(rootPath))
            {
                string hashCode = hasher.SHA1(file.Name);

                if (!allFiles.ContainsKey(hashCode))
                {
                    allFiles.Add(hashCode, new List<String>());
                }

                allFiles[hashCode].Add(file.FullName);
            }

            //Create a dictionary with all collisions
            //Collission are determine by
            Dictionary<string, List<string>> collisionsOnly = new Dictionary<string, List<string>>();

            foreach (var keyValuePair in allFiles)
            {
                if (allFiles[keyValuePair.Key].Count > 1)
                {
                    collisionsOnly.Add(keyValuePair.Key, new List<string>());
                    collisionsOnly[keyValuePair.Key].AddRange(allFiles[keyValuePair.Key]);
                }
            }

            this.Collisions = collisionsOnly;
        }
    }
}
