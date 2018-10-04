using System.IO;

namespace Fibrinogen
{
    public class Logger
    {
        public string Path { get; }

        public Stream FileW { get; }
        public Stream FileR { get; }
        public StreamWriter Writer { get; }
        public StreamReader Reader { get; }

        public Logger(string path)
        {
            Path = path;
            if (!File.Exists(path))
                FileW = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            else
                FileW = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);

            FileR = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Write);

            Writer = new StreamWriter(FileW);
            Reader = new StreamReader(FileR);
        }

        public void ClearReader()
        {
            FileR.Position = 0;
            Reader.DiscardBufferedData();
        }
    }
}
