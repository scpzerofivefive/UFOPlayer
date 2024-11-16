using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace UFOPlayer.Scripts
{
    public class CSVFileLoader
    {
        public async Task<UFOScript> loadFile(string filepath)
        {
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filepath);
            Stream stream = (await storageFile.OpenReadAsync()).AsStream();
            return await loadFile(filepath, stream);
        }

        public async Task<string[]> getCorrespondingScriptPaths(string fullPath, string pattern)
        {
            string relatedScriptPath = "";
            string fileType = ".csv";
            string mediaFileName = Path.GetFileNameWithoutExtension(fullPath);

            pattern = String.Format(pattern, Regex.Escape(mediaFileName));
            
            try
            {
                // Combine the provided path and filename into a full path
                Debug.WriteLine("Searching for scripts for " + mediaFileName);
                Debug.WriteLine("Using pattern: " + pattern);
                // Get all files in the directory of the combined path with the specified file type
                string[] allFiles = Directory.GetFiles(Path.GetDirectoryName(fullPath),"*" + fileType);


                // Use LINQ to filter files based on the specified criteria
                string[] matchingFiles = allFiles
                    .Where(file =>
                        Regex.IsMatch(Path.GetFileName(file), pattern, RegexOptions.IgnoreCase) &&
                        Path.GetExtension(file).Equals(fileType, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                return matchingFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new string[] { "" };
            }
        }

        public async Task<UFOScript> loadFile(string fullPath, Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            // Reads all the content of file as a text.
            string fileContent = await streamReader.ReadToEndAsync();
            string[] lines = fileContent.Split('\n');

            List<ScriptAction> lactions = new List<ScriptAction>();
            List<ScriptAction> ractions = new List<ScriptAction>();
            foreach (string line in lines)
            {
                try
                {

                    if (line.Length == 0)
                    {
                        continue;
                    }
                    string[] values = line.Split(",");

                    TimeSpan timestamp = TimeSpan.FromMilliseconds(int.Parse(values[0]) * 100);
                    if (values.Length == 3) //UFOSA Format
                    {
                        sbyte pow = getPowerValue(values[1], values[2]);

                        if (lactions.Count() == 0 || lactions.Last().Intensity != pow)
                        {
                            lactions.Add(new ScriptAction(timestamp, pow));
                            ractions.Add(new ScriptAction(timestamp, pow));
                        }

                    }
                    else if (values.Length == 5) //UFOTW Format
                    {
                        sbyte left = getPowerValue(values[1], values[2]);
                        sbyte right = getPowerValue(values[3], values[4]);
                        if (lactions.Count() == 0 || lactions.Last().Intensity != left)
                        {
                            lactions.Add(new ScriptAction(timestamp, left));
                        }

                        if (ractions.Count() == 0 || ractions.Last().Intensity != right)
                        {
                            ractions.Add(new ScriptAction(timestamp, right));
                        }
                    }
                    else
                    {
                        return new UFOScript(fullPath);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    break;
                }


            }
            Debug.WriteLine(lines.Count() + " lines have been read.");
            Debug.WriteLine("Left has " + lactions.Count + " Actions.");
            Debug.WriteLine("Right has " + ractions.Count + " Actions.");
            return new UFOScript(fullPath, lactions, ractions);
        }



        private sbyte getPowerValue(string polarity, string power)
        {
            sbyte value = sbyte.Parse(power);
            if (polarity == "1")
            {
                return (sbyte)-value;
            }
            return value;
        }
    }
}
