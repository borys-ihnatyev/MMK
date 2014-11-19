// ReSharper disable once CheckNamespace
namespace System.IO
{
    public partial class FileDirectoryProcessing
    {
        private readonly string initDir;
        private readonly bool processInputFolders;

        private ResultInfo lastResult;
        
        public event Action<string> OnDirectoryProcess;
        public event Action<string> OnFileProcess;
        public event Action OnDone;

        public FileDirectoryProcessing(string initDir = "", bool processInputFolders = false)
        {
            this.initDir = initDir;
            this.processInputFolders = processInputFolders;
            lastResult = null;
        }

        public ResultInfo StartProcess()
        {
            if (OnFileProcess == null && OnDirectoryProcess == null)
                throw new NullReferenceException(
                    "Must be set almost one of events : OnDirectoryProcess || OnFileProcess"
                    );

            lastResult = new ResultInfo(1, 0);

            ProcessDirectory(initDir);

            if (OnDone != null)
                OnDone();
            return lastResult;
        }

        private void ProcessDirectory(string path)
        {
            var files = Directory.GetFiles(path);
            if (files.Length > 0)
                foreach (var file in files)
                {
                    if (OnFileProcess != null)
                        OnFileProcess(file);
                    ++lastResult.filesProcessed;
                }

            if (!processInputFolders) return;
            
            var dirs = Directory.GetDirectories(path);
            if (dirs.Length <= 0) return;

            foreach (var dir in dirs)
            {
                ProcessDirectory(dir);
                if (OnDirectoryProcess != null)
                    OnDirectoryProcess(dir);
                ++lastResult.directoriesProcessed;
            }
        }
    }
}