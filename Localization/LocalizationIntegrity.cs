using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Resources;
using System.Collections;

namespace HAF.Localization
{
    public static class LocalizationIntegrity
    {
        /// <summary>
        /// Check the integrity of the string resource files.
        /// </summary>
        /// <returns>List of errors</returns>
        public static List<string> CheckStrings()
        {
            var errors = new List<string>();
            var cultures = new string[] { "de-DE", "en-US" };
            var keys = new List<string>();

            // gather all keys
            foreach (string culture in cultures)
            {
                // get resource set
                var set = Strings.ResourceManager.GetResourceSet(new CultureInfo(culture), false, false);

                // add all keys in set to keys
                foreach (DictionaryEntry entry in set)
                {
                    if (!keys.Contains(entry.Key.ToString()))
                        keys.Add(entry.Key.ToString());
                }
            }

            // check each culture
            foreach (string culture in cultures)
            {
                // create temporary copy of keys
                var tkeys = keys.ToList();

                // get resource set
                var set = Strings.ResourceManager.GetResourceSet(new CultureInfo(culture), false, false);
                
                // remove avaliable keys
                foreach (DictionaryEntry entry in set)
                    tkeys.Remove(entry.Key.ToString());

                // add errors
                foreach (string key in tkeys)
                    errors.Add(culture + " : " + key + "is missing");
            }

            return errors;
        }
    }
}
