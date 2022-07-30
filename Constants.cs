namespace FileCopyProgram
{
    /// <summary>
    /// class <c>Constants</c> contains all constants
    /// </summary>
    class Constants
    {
        public const string Percentage = "%";
        public const string DestinationError = "Please enter the destinaton folder.";
        public const string DestinationPathError = "Please enter correct destination path.";
        public const string SourcePathError = "Please enter correct source file path.";
        public const string SourceFileError = "Please fill or delete the empty row.";
        public const string FileNameError = "Can't copy paste two files of same name.";
        public const string NonOverwriteError = "File is present in destination.";
        public const string DirectorySameError = "Can't copy paste in same folder.";
        public const string ReadOnly = "UnauthorizedAccessException: file is not accessiable";
        public const string FileNotFound = "FileNotFoundException: source file is not found";
        public const string Time = "{0:00}:{1:00}:{2:00}";
        public const string Failed = "Failed";
        public const string Passed = "Passed";
    }
}