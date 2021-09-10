using System;
using System.IO;
using TCTracking.Service.Interface;

namespace TCTracking.Service.Implement
{
    public class FileManagerService : IFileManagerService
    {
        private bool CreateFolder(string name)
        {
            try
            {
                if (!Directory.Exists(name))
                {
                    Directory.CreateDirectory(name);
                }
                return true;
            }
            catch (Exception ex) { }

            return false;
        }

        public bool MoveFile(string fileName, string destinationFolder)
        {
            try
            {
                destinationFolder = Path.Combine("Resources", "Images", destinationFolder);
                var isFoderCreated = CreateFolder(destinationFolder);
                if (isFoderCreated)
                {
                    var sourceFile = Path.Combine("Resources", "Images", fileName);    
                    fileName = Path.GetFileName(sourceFile);
                    var destFile = Path.Combine(destinationFolder, fileName);
                    File.Move(sourceFile, destFile, true);
                    return true;
                }
            }
            catch (Exception ex) { }
            return false;

        }
    }
}
