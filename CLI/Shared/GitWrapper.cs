using System.Diagnostics;

namespace Shared
{
    public class GitWrapper
    {
        private bool _isSubmodule = false;
        public bool IsSubmodule
        {
            get { return _isSubmodule;}
        }
        private ProcessRunner _gitProcess;
        public ProcessRunner GitProcess
        {
            get { return _gitProcess;}
        }
        private GitSubmodule? _submodule;
        public GitSubmodule? Submodule
        {
            get { return _submodule; }
        }
        public string WorkingDirectory;
        public GitWrapper(string path)
        {
            Setup(path);
        }

        public GitWrapper()
        {
            string? path = Path.GetDirectoryName(new ProcessRunner("git", ".").RunCommand("rev-parse --absolute-git-dir")) ?? throw new GitWrapperException("Got null trying to instantiate GitWrapper from current path");
            Setup(path);
        }
        private void Setup(string path)
        {
            CliFileHelper fileHelper = new(path);
            CliFileHelperSearchInfo searchInfo = fileHelper.SearchInLowestDirectory(".git") ?? throw new GitWrapperException($"{path} is not a git repo");
            _isSubmodule = searchInfo.Type is CliFileType.File;
            _gitProcess = new ProcessRunner("git", fileHelper.CurrentPath);
            WorkingDirectory = fileHelper.CurrentPath;
            if (IsSubmodule) _submodule = new GitSubmodule(fileHelper.CurrentPath);
        }

        public string GetDiffForPreviousCommit(string filename="")
        {
            if (filename is null) throw new GitWrapperException("Filename argument is null, can't diff null file");
            // Has some async issues when you return it directly
            string diff = GitProcess.RunCommand(@$"log -1 HEAD -p --no-merges --pretty=format: --no-color -- {filename}");
            return diff;
        }

        public string[] GetFileRevisionCommitHashes(string FileName, int range=1) 
        {
            if (range < 1) throw new GitWrapperException($"Range: {range} is invalid. Value should be greater than 1");
            string[] hashes = GitProcess.RunCommand(@$" log -{range} HEAD --pretty=format:%H --reverse -- {FileName}", true).Trim().Split('\n');
            return hashes;
        }

        public GitCommitFiles GetCommitHashAndFiles(int stepsFromHead=0)
        {
            if (stepsFromHead < 0) throw new GitWrapperException($"Steps from HEAD: {stepsFromHead} is invalid. Value should be greater than 0");
            string commitFileBlock = GitProcess.RunCommand($"log -1 HEAD^{stepsFromHead} --pretty=tformat:%H --name-only --no-merges", true);
            return new GitCommitFiles(commitFileBlock);
        }
    }

    public class GitCommitFiles
    {
        public readonly string Hash;
        public readonly string[] Files;
        public GitCommitFiles(string commitLogBlock)
        {
            var commitFiles = commitLogBlock.Split('\n');
            Files = new string[commitFiles.Length - 1];
            Hash = commitFiles[0].Trim();
            for (int idx = 1; idx < commitFiles.Length; idx++) Files[idx - 1] = commitFiles[idx].Trim();
        }
    }

    public class GitSubmodule 
    {
        public readonly string ParentRepoDirectory;
        public readonly string GitDirectory;

        public GitSubmodule(string submodulePath)
        {
            ParentRepoDirectory = new ProcessRunner("git", submodulePath).RunCommand("rev-parse --show-superproject-working-tree");
            GitDirectory = Path.Join([..ParentRepoDirectory.Split(['/', '\\']), ".git", "modules", Path.GetRelativePath(ParentRepoDirectory, submodulePath)]);
            CliFileHelper fileHelper = new(GitDirectory);
            var _ = fileHelper.SearchInLowestDirectory("hooks") ?? throw new GitWrapperException($"{GitDirectory} is not a valid git submodule");
        }
    }

    public class GitWrapperException(string message) : Exception(message)
    {
    }

}