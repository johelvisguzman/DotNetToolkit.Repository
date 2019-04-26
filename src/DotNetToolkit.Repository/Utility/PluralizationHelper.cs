namespace DotNetToolkit.Repository.Utility
{
    using System;
    using System.Collections.Generic;

    internal static class PluralizationHelper
    {
        // https://gist.github.com/andrewjk/3186582
        public static string Pluralize(string text)
        {
            var exceptions = GetExceptions();
            if (exceptions.ContainsKey(text.ToLowerInvariant()))
                return exceptions[text.ToLowerInvariant()];

            if (text.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
                !text.EndsWith("ay", StringComparison.OrdinalIgnoreCase) &&
                !text.EndsWith("ey", StringComparison.OrdinalIgnoreCase) &&
                !text.EndsWith("iy", StringComparison.OrdinalIgnoreCase) &&
                !text.EndsWith("oy", StringComparison.OrdinalIgnoreCase) &&
                !text.EndsWith("uy", StringComparison.OrdinalIgnoreCase))
                return text.Substring(0, text.Length - 1) + "ies";

            // http://en.wikipedia.org/wiki/Plural_form_of_words_ending_in_-us
            if (text.EndsWith("us", StringComparison.CurrentCultureIgnoreCase))
                return text + "es";

            if (text.EndsWith("ss", StringComparison.CurrentCultureIgnoreCase))
                return text + "es";

            if (text.EndsWith("s", StringComparison.CurrentCultureIgnoreCase))
                return text;

            if (text.EndsWith("x", StringComparison.CurrentCultureIgnoreCase) ||
                text.EndsWith("ch", StringComparison.CurrentCultureIgnoreCase) ||
                text.EndsWith("sh", StringComparison.CurrentCultureIgnoreCase))
                return text + "es";

            if (text.EndsWith("f", StringComparison.CurrentCultureIgnoreCase) && text.Length > 1)
                return text.Substring(0, text.Length - 1) + "ves";

            if (text.EndsWith("fe", StringComparison.CurrentCultureIgnoreCase) && text.Length > 2)
                return text.Substring(0, text.Length - 2) + "ves";

            return text + "s";
        }

        private static Dictionary<string, string> GetExceptions()
        {
            return new Dictionary<string, string> {
                { "abyss", "abysses" },
                { "alumnus", "alumni" },
                { "analysis", "analyses" },
                { "aquarium", "aquaria" },
                { "arch", "arches" },
                { "atlas", "atlases" },
                { "axe", "axes" },
                { "baby", "babies" },
                { "bacterium", "bacteria" },
                { "batch", "batches" },
                { "beach", "beaches" },
                { "brush", "brushes" },
                { "bus", "buses" },
                { "calf", "calves" },
                { "chateau", "chateaux" },
                { "cherry", "cherries" },
                { "child", "children" },
                { "church", "churches" },
                { "circus", "circuses" },
                { "city", "cities" },
                { "cod", "cod" },
                { "copy", "copies" },
                { "crisis", "crises" },
                { "curriculum", "curricula" },
                { "deer", "deer" },
                { "dictionary", "dictionaries" },
                { "domino", "dominoes" },
                { "dwarf", "dwarves" },
                { "echo", "echoes" },
                { "elf", "elves" },
                { "emphasis", "emphases" },
                { "family", "families" },
                { "fax", "faxes" },
                { "fish", "fish" },
                { "flush", "flushes" },
                { "fly", "flies" },
                { "foot", "feet" },
                { "fungus", "fungi" },
                { "half", "halves" },
                { "hero", "heroes" },
                { "hippopotamus", "hippopotami" },
                { "hoax", "hoaxes" },
                { "hoof", "hooves" },
                { "index", "indexes" },
                { "iris", "irises" },
                { "kiss", "kisses" },
                { "knife", "knives" },
                { "lady", "ladies" },
                { "leaf", "leaves" },
                { "life", "lives" },
                { "loaf", "loaves" },
                { "man", "men" },
                { "mango", "mangoes" },
                { "memorandum", "memoranda" },
                { "mess", "messes" },
                { "moose", "moose" },
                { "motto", "mottoes" },
                { "mouse", "mice" },
                { "nanny", "nannies" },
                { "neurosis", "neuroses" },
                { "nucleus", "nuclei" },
                { "oasis", "oases" },
                { "octopus", "octopi" },
                { "party", "parties" },
                { "pass", "passes" },
                { "penny", "pennies" },
                { "person", "people" },
                { "plateau", "plateaux" },
                { "poppy", "poppies" },
                { "potato", "potatoes" },
                { "quiz", "quizzes" },
                { "reflex", "reflexes" },
                { "runner-up", "runners-up" },
                { "scarf", "scarves" },
                { "scratch", "scratches" },
                { "series", "series" },
                { "sheaf", "sheaves" },
                { "sheep", "sheep" },
                { "shelf", "shelves" },
                { "species", "species" },
                { "splash", "splashes" },
                { "spy", "spies" },
                { "stitch", "stitches" },
                { "story", "stories" },
                { "syllabus", "syllabi" },
                { "tax", "taxes" },
                { "thesis", "theses" },
                { "thief", "thieves" },
                { "tomato", "tomatoes" },
                { "tooth", "teeth" },
                { "tornado", "tornadoes" },
                { "try", "tries" },
                { "volcano", "volcanoes" },
                { "waltz", "waltzes" },
                { "wash", "washes" },
                { "watch", "watches" },
                { "wharf", "wharves" },
                { "wife", "wives" },
                { "woman", "women" }
            };
        }
    }
}
