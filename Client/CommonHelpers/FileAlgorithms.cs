using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;

namespace FileExplorer.CommonHelpers
{
    public class FileAlgorithms
    {

        public static List<File> BasicTagSearch(List<File> Files, List<string> UserInput)
        {
            //Definitions
            List<string> NotOperators = new List<string> { "NOT", "!!" };
            char[] Delimiters = { ' ', ',', ';' };

            var hasTags = new List<string>();
            var doesntHaveTags = new List<string>();
            List<File> Output = new List<File>();

            foreach (var input in UserInput)
            {
                if (string.IsNullOrWhiteSpace(input)) continue;

                var Elements = input.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);

                foreach (var element in Elements)
                {
                    var Ele = element.Trim();
                    bool hasNot = false;

                    foreach (var op in NotOperators)
                    {
                        if (Ele.StartsWith(op, StringComparison.OrdinalIgnoreCase))
                        {
                            hasNot = true;
                            Ele = Ele.Substring(op.Length);

                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(Ele)) continue;

                    if (hasNot)
                    {
                        doesntHaveTags.Add(Ele);
                    }
                    else
                    {
                        hasTags.Add(Ele);
                    }
                }
            }

            Output = Files.Where(file =>
            {
                bool checkHasTags = hasTags.All(tag => file.Relic.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)));

                bool checkDoesntHaveTags = !doesntHaveTags.Any(tag => file.Relic.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)));

                return checkHasTags && checkDoesntHaveTags;
            }).ToList();

            return Output;
        }

        public static List<File> BasicNameSearch(List<File> Files, List<string> UserInput)
        {
            // Definitions
            List<string> NotOperators = new List<string> { "NOT", "!!" };
            char[] Delimiters = { ' ', ',', ';' };

            var hasName = new List<string>();
            var doesntHaveName = new List<string>();
            List<File> Output = new List<File>();

            foreach (var input in UserInput)
            {
                if (string.IsNullOrWhiteSpace(input)) continue;

                var Elements = input.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);

                foreach (var element in Elements)
                {
                    var Ele = element.Trim();
                    bool hasNot = false;

                    foreach (var op in NotOperators)
                    {
                        if (Ele.StartsWith(op, StringComparison.OrdinalIgnoreCase))
                        {
                            hasNot = true;
                            Ele = Ele.Substring(op.Length);

                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(Ele)) continue;

                    if (hasNot)
                    {
                        doesntHaveName.Add(Ele);
                    }
                    else
                    {
                        hasName.Add(Ele);
                    }
                }
            }

            Output = Files.Where(file =>
            {
                bool checkhasName = hasName.All(pattern => file.Name.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0);

                bool checkdoesntHaveName = !doesntHaveName.Any(pattern => file.Name.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0);

                return checkhasName && checkdoesntHaveName;
            }).ToList();

            return Output;
        }

    }
}
