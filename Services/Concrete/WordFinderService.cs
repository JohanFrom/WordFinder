using Newtonsoft.Json;
using System.Text;
using System.Xml;
using WordFinder.Enums;
using WordFinder.Models.ApiModels;
using WordFinder.Services.Abstract;
using WordFinder.Utilities;

namespace WordFinder.Services
{
    public class WordFinderService : IWordFinderService
    {
        private static readonly HttpClient _client = new();

        public List<string>? GetWords(string chars, string lang)
        {
            if (LanguageExist(lang) == false)
            {
                return new List<string>() { "Language does not exist!" };
            }

            string? filePath = VisualStudioProvider.FindWordsFile($"words{HelperMethods.Capitalize(lang.ToLower())}.txt") ?? null;
            if(filePath == null)
            {
                throw new Exception("Could not find the word file with the language " + "\"" + lang + "\"" + ".");
            }

            var words = File.ReadLines(filePath).ToList();
            var result = words.Where(x => chars.All(x.Contains) && x.Length == chars.Length) ?? null;

            if(result == null)
            {
                return null;
            }

            var matches = HelperMethods.IsMatchOnTwoStrings(chars, result) ?? null;

            return matches;   
        }

        public List<string> GetLanguages()
        {
            return Enum.GetNames(typeof(Languages)).ToList();
        }

        public async Task<List<string>> GetLanguageBasedOnISO2(string lang)
        {
            bool languageExists = LanguageExist(lang);  
            if(languageExists == false)
            {
                return new List<string>() {"Language does not exists in our list."};
            }

            string countryName = HelperMethods.ControlEnglishISO2Name(lang);
            if (countryName != string.Empty)
            {
                return new List<string>() { countryName };
            }

            string url = CreateApiUrlForISO2(lang);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            using (var response = await _client.SendAsync(request))
            {
                if (!response.IsSuccessStatusCode)
                {
                    return new List<string>() { "Something went wrong" };
                }

                var body = await response.Content.ReadAsStringAsync();
                var languageObj = JsonConvert.DeserializeObject<IEnumerable<ISO2Model>>(body) ?? null;

                if(languageObj == null)
                {
                    return new List<string>() { "Could not fetch country name" };
                }
                else
                {
                    foreach (var obj in languageObj)
                    {
                        return new List<string>() { obj.country_name };
                    }
                }
            }

            return new List<string>() { "Something went wrong" };
        }

        public async Task<string> AddWord(string word, string lang)
        {
            StringBuilder sb = new();
            bool langExist = LanguageExist(lang);
            if(langExist == false)
            {
                sb.Append("\"" + lang + "\" ");
                sb.Append("does not exists so could not add the word ");
                sb.Append("\"" + word + "\".");
                return sb.ToString();
            }

            bool wordExist = WordExist(word, lang);
            if(wordExist == true)
            {
                sb.Append("\"" + word + "\" ");
                sb.Append("exist which means the word could not be added.");
                return sb.ToString();
            }

            string? filePath = VisualStudioProvider.FindWordsFile($"words{HelperMethods.Capitalize(lang.ToLower())}.txt") ?? null;
            if (filePath == null)
            {
                throw new Exception("Could not find the word file with the language " + "\"" + lang + "\"" + ".");
            }

            using StreamWriter file = new(filePath, append: true);
            await file.WriteAsync(Environment.NewLine + word);
            file.Close();

            sb.Append("\"" + word + "\" ");
            sb.Append("has been added!");

            return sb.ToString();
        }

        public async Task<string> RemoveWord(string word, string lang)
        {
            StringBuilder sb = new();
            bool langExist = LanguageExist(lang);
            if (langExist == false)
            {
                sb.Append("\"" + lang + "\" ");
                sb.Append("does not exists so could not remove the word ");
                sb.Append("\"" + word + "\".");
                return sb.ToString();
            }

            bool wordExist = WordExist(word, lang);
            if (wordExist == false)
            {
                sb.Append("\"" + word + "\" ");
                sb.Append("does not exist which means the word could not be removed.");
                return sb.ToString();
            }

            string? filePath = VisualStudioProvider.FindWordsFile($"words{HelperMethods.Capitalize(lang.ToLower())}.txt") ?? null;
            if (filePath == null)
            {
                throw new Exception("Could not find the word file with the language " + "\"" + lang + "\"" + ".");
            }

            var tempFile = Path.GetTempFileName();
            var linesToKeep = File.ReadLines(filePath).Where(l => l != word.ToLower());

            await File.WriteAllLinesAsync(tempFile, linesToKeep);
            File.Delete(filePath);
            File.Move(tempFile, filePath);
            HelperMethods.RemoveWhiteSpaceFromFile(filePath);

            sb.Append("\"" + word + "\" ");
            sb.Append("has been removed!");

            return sb.ToString();
        }

        #region Boolean methods

        public bool WordExist(string word, string lang)
        {
            string? filePath = VisualStudioProvider.FindWordsFile($"words{HelperMethods.Capitalize(lang.ToLower())}.txt") ?? null;
            if(filePath == null)
            {
                throw new Exception("Could not find the word file with the language " + "\"" + lang + "\"" + ".");
            }

            var words = File.ReadAllLines(filePath).ToList().Where(x => x == word);

            return words.Any();
        }

        public bool LanguageExist(string lang)
        {
            var languages = Enum.GetNames(typeof(Languages)).ToList().Where(x => x == lang.ToLower());
            return languages.Any();
        }
        #endregion


        #region Static methods
        private static string CreateApiUrlForISO2(string lang)
        {
            StringBuilder sb = new();
            sb.Append("https://countrycode.dev/api/countries/iso2/");
            sb.Append(lang.ToUpper());
            return sb.ToString();
        }
        #endregion
    }
}
