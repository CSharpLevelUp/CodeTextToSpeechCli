using System.Security;
using System.IO;
using System.Text;

namespace Shared
{
    public class CliFileHelper
    {
        public readonly string CurrentPath;
        public readonly bool IsDirectory;
        public CliFileHelper(string path, bool createIfNotFound=false)
        {
            path = Path.GetFullPath(path);
            if (!PathExists(path))
            {
                if (createIfNotFound) CreatePath(path);
                else throw new CliFileHelperException($"{path} doesn't exist");
            }
            IsDirectory = IsDir(path);
            CurrentPath = path;
        }

        private static bool PathExists(string path) { return Path.Exists(path); }
        

        private static bool IsDir(string path)
        {
            try
            {
                return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
            } catch (Exception e) when (e is FileNotFoundException || e is PathTooLongException || e is SecurityException || e is UnauthorizedAccessException)
            {
                throw new CliFileHelperException(e.Message);
            }
        }

        private static void CreatePath(string path)
        {
            if (!PathExists(path)) 
            {
                if (Path.EndsInDirectorySeparator(path)) Directory.CreateDirectory(path);
                else File.Create(path);
            } else throw new CliFileHelperException($"{path} already exists");
        }

        public string CreateInPath(string name)
        {
            // This is assuming the name parameter is just the name of the file to be created and not an absolute path
            // or trying to create a nested structure
            string newPath = Path.Join([CurrentPath, name]);
            CreatePath(newPath);
            return newPath;
        }

        public static string CreateInLocalAppData(string name)
        {
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);
            string newPath = Path.Join(localAppDataPath, name);
            CreatePath(newPath);
            return newPath;
        }

        public CliFileHelperSearchInfo? SearchInLowestDirectory(string name, bool checkParentDirectoryIfFile=false)
        {
            try 
            {
                string? workingPath;
                if (IsDirectory) workingPath = CurrentPath; 
                else if(checkParentDirectoryIfFile)
                {
                    workingPath = Path.GetDirectoryName(CurrentPath);
                    if (workingPath == null) throw new ArgumentException($"Null returned when getting {CurrentPath} parent directory"); 
                }
                else throw new CliFileHelperException($"{CurrentPath} Is not a directory");
                string path = Path.Join([workingPath, name]);
                return new CliFileHelperSearchInfo
                (
                    File.GetAttributes(path).HasFlag(FileAttributes.Directory) 
                        ? CliFileType.Directory 
                        : CliFileType.File,
                    path
                );
            } catch (FileNotFoundException) {
                return null;
            }   
        }

        private string GetDirectoryName()
        {
            var directoryPath = ((!IsDirectory) ? CurrentPath : Path.GetDirectoryName(CurrentPath)) ?? throw new ArgumentException($"Null returned when getting {CurrentPath} parent directory");
            var parentDirectoryPath = Path.GetDirectoryName(directoryPath) ?? throw new ArgumentException($"Null returned when getting {directoryPath} parent directory");
            return Path.GetRelativePath(parentDirectoryPath, directoryPath);
        }

        private string GetFilePath(string? filename=null)
        {
            if (IsDirectory && filename is null) throw new ArgumentException($"{CurrentPath} is a directory, and a file wasn't specified");
            else if (IsDirectory && filename is not null) return Path.Join(CurrentPath, filename);
            else return CurrentPath;
        }

        public string ReadFile(string? filename=null)
        {
            string content = string.Empty;
            using(StreamReader file = new(GetFilePath(filename)))
            {
                string? line;
                StringBuilder builder = new();

                while((line = file.ReadLine()) != null)
                {
                    builder.Append(line);
                }
                content = builder.ToString();
                builder.Clear();
                file.Close();
            }
            return content;
        }

        public void UpdateFileContents(string newContent, string? filename=null)
        {
            File.WriteAllText(GetFilePath(filename), newContent);
        }
    }

    public enum CliFileType
    {
        File = 0,
        Directory = 1
    }

    public class CliFileHelperSearchInfo
    {
        public readonly CliFileType Type;
        public readonly string AbsoulutePath;

        public CliFileHelperSearchInfo(CliFileType type, string path) 
        {
            Type = type;
            AbsoulutePath = path;
        }
    }

    public class CliFileHelperException(string message) : Exception(message) 
    {
    }

}