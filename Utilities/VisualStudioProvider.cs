using WordFinder.Enums;

namespace WordFinder.Utilities
{
    public static class VisualStudioProvider
    {
        public static string? FindWordsFile(string fileName)
        {
            string startDirectory = new DirectoryInfo(Directory.GetCurrentDirectory()).ToString();
            string result = Directory.EnumerateFiles(startDirectory, fileName, SearchOption.AllDirectories).First();

            if(result == null)
            {
                return null;
            }
            
            return result;

        }
    }
}
