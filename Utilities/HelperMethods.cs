namespace WordFinder.Utilities
{
    public static class HelperMethods
    {
        public static string Capitalize(string str)
        {
            char[] chars = str.ToCharArray();
            chars[0] = char.ToUpper(chars[0]);

            return new string(chars);
        }

        public static List<string>? IsMatchOnTwoStrings(string chars, IEnumerable<string> checkers)
        {
            List<string> returner = new();
            foreach (var check in checkers)
            {
                string s1 = chars.ToLowerInvariant();
                string s2 = check.ToLowerInvariant();

                if (s1.All(x => s2.Contains(x)) && s2.All(x => s1.Contains(x)))
                {
                    returner.Add(s2);
                }
                else
                {
                    continue;
                }
            }

            return returner;
        }

        public static void RemoveWhiteSpaceFromFile(string filename)
        {
            var tempFileName = Path.GetTempFileName();
            try
            {
                using var streamReader = new StreamReader(filename);
                using var streamWriter = new StreamWriter(tempFileName);

                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        streamWriter.WriteLine(line);
                }

                File.Copy(tempFileName, filename, true);
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }

        public static string ControlEnglishISO2Name(string lang)
        {
            if(lang == "en")
            {
                return "English";
            }

            return string.Empty;
        }
    }
}
