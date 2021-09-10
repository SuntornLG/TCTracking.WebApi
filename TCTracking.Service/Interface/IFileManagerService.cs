

namespace TCTracking.Service.Interface
{
    public interface IFileManagerService
    {
        bool MoveFile(string fileName, string destinationFolder);
    }
}
