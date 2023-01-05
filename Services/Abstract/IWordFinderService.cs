namespace WordFinder.Services.Abstract
{
    public interface IWordFinderService
    {
        public List<string>? GetWords(string chars, string lang);
        public List<string> GetLanguages();
        public Task<List<string>> GetLanguageBasedOnISO2(string lang);
        public Task<string> AddWord(string word, string lang);
        public Task<string> RemoveWord(string word, string lang);
        public bool WordExist(string word, string lang);
        public bool LanguageExist(string lang);
    }
}
