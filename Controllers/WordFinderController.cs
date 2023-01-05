using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;
using System.Xml;
using WordFinder.Services.Abstract;

namespace WordFinder.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WordFinderController : ControllerBase
    {
        private readonly IWordFinderService _wordFinderService;

        public WordFinderController(IWordFinderService wordFinderService)
        {
            _wordFinderService = wordFinderService;
        }

        /// <summary>
        /// Retrieves the list of word based on the given characters in any given order, based on a language and the length of the chars
        /// </summary>
        /// <param name="chars">Word/chars in any given order e.g. hello or lleoh</param>
        /// <param name="lang">Language as country code e.g. en is english, se is swedish</param>
        /// <returns>List of words</returns>
        [HttpGet("GetWords")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<string>>? GetWords(string chars, string lang)
        {
            var wordsFound = _wordFinderService.GetWords(chars, lang);
            if(wordsFound == null)
            {
                return NotFound();
            }

            if(wordsFound.Count == 0)
            {
                StringBuilder sb = new();
                sb.Append("Could not find any words based on the characters ");
                sb.Append("\"" + chars + "\"");
                sb.Append(" and the language of ");
                sb.Append("\"" + lang + "\"" + ".");
                return NotFound(sb.ToString());
            }

            return Ok(wordsFound);
        }

        /// <summary>
        /// Retrieves the current availavle languages to pick from
        /// </summary>
        /// <returns>Return a list of languages represented with their respective country code</returns>
        [HttpGet("GetLanguages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<string>> GetLanguages()
        {
            var languagesFound = _wordFinderService.GetLanguages();
            if(languagesFound == null)
            {
                return NotFound();
            }

            return Ok(languagesFound);
        }

        /// <summary>
        /// Adds a word to the words list based on the language
        /// </summary>
        /// <param name="word">A new word</param>
        /// <param name="lang">Language as country code e.g. en is english, sv is swedish</param>
        /// <returns>Return a string as a message, added or not</returns>
        [HttpPost("AddWord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<string> AddWord(string word, string lang)
        {
            string message = await _wordFinderService.AddWord(word, lang);
            return message;
        }

        /// <summary>
        /// Removes a word from the list if the word exists
        /// </summary>
        /// <param name="word">An existing word!</param>
        /// <param name="lang">Language as country code e.g. en is english, sv is swedish</param>
        /// <returns></returns>
        [HttpDelete("RemoveWord")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<string> RemoveWord(string word, string lang)
        {
            string message = await _wordFinderService.RemoveWord(word, lang);
            return message;
        }

        /// <summary>
        /// Gets a specific language based on ISO2
        /// </summary>
        /// <param name="lang">Language as country code e.g. en is english, sv is swedish</param>
        /// <returns>Returns the country name and ISO2</returns>
        [HttpGet("GetSpecificLanguage/{lang}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<string>>> GetSpecificLangugage(string lang)
        {
            var language = await _wordFinderService.GetLanguageBasedOnISO2(lang);
            if(language == null)
            {
                return NotFound();
            }

            return Ok(language);
        }
    }
}