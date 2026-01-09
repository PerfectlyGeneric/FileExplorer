using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;

namespace FileExplorer.CommonHelpers
{
    public class FileAlgorithms
    {
        public static ObservableCollection<File> BasicCombinedSearch(List<File> sourceFiles, ObservableCollection<string> input)
        {
            if (sourceFiles == null || !sourceFiles.Any())
                return new ObservableCollection<File>();

            if (input == null || !input.Any() || input[0] == "Add Search Options")
                return new ObservableCollection<File>(sourceFiles);

            string rawQuery = input[0];
            rawQuery = rawQuery.Trim('[', ']');
            var segments = rawQuery.Split(new[] { " && " }, StringSplitOptions.RemoveEmptyEntries);
            List<string> nameQueries = new List<string>();
            List<string> tagQueries = new List<string>();

            foreach (var segment in segments)
            {
                bool isExclusion = segment.StartsWith("!");
                string content = segment.TrimStart('!').Trim('(', ')');
                var parts = content.Split(':');

                if (parts.Length < 2) continue;

                string type = parts[0].Trim().ToLower();
                var values = parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());

                foreach (var val in values)
                {
                    string formattedTerm = isExclusion ? "NOT" + val : val;

                    if (type == "name")
                        nameQueries.Add(formattedTerm);
                    else if (type == "tag")
                        tagQueries.Add(formattedTerm);
                }
            }

            List<File> nameResults = sourceFiles;
            if (nameQueries.Any())
            {
                nameResults = BasicNameSearch(sourceFiles, nameQueries);
            }

            List<File> finalResults = nameResults;
            if (tagQueries.Any())
            {
                finalResults = BasicTagSearch(nameResults, tagQueries);
            }

            return new ObservableCollection<File>(finalResults);
        }

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
