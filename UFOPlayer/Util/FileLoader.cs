using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.Design.AxImporter;
using UFOPlayer.Script;
using Avalonia.Platform.Storage;
using Windows.Storage;
using System.Diagnostics;

namespace UFOPlayer.Util
{
    public class FileLoader
    {

        public async Task<List<ScriptAction>> loadFile(Avalonia.Platform.Storage.IStorageFile file)
        {
            await using var stream = await file.OpenReadAsync();
            return await loadFile(stream);
            
        }
        public async Task<List<ScriptAction>> loadFile(String filepath)
        {
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filepath);
            Stream stream = (await storageFile.OpenReadAsync()).AsStream();
            return await loadFile(stream);
        }

        public async Task<String[]> getCorrespondingScriptPaths(String fullPath)
        {
            String relatedScriptPath = "";
            String fileType = ".csv";
            try
            {
                // Combine the provided path and filename into a full path
                Debug.WriteLine(Path.GetFileNameWithoutExtension(fullPath));
                // Get all files in the directory of the combined path with the specified file type
                string[] allFiles = Directory.GetFiles(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath) + "*" + fileType);

                // Use LINQ to filter files based on the specified criteria
                string[] matchingFiles = allFiles
                    .Where(file => Path.GetFileName(file).ToLower().Contains("ufo") 
                    && Path.GetExtension(file).Equals(fileType, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                return matchingFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new string[] {""};
            }
            
            
        }

        private async Task<List<ScriptAction>> loadFile(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            // Reads all the content of file as a text.
            String fileContent = await streamReader.ReadToEndAsync();
            String[] lines = fileContent.Split('\n');
            List<ScriptAction> list = new List<ScriptAction>();
            foreach (String line in lines)
            {
                try
                {
                    ScriptAction action;
                    String[] values = line.Split(",");
                    int time = int.Parse(values[0]) * 100;
                    if (values.Length == 3)
                    {
                        sbyte pow = getPowerValue(values[1], values[2]);
                        action = new ScriptAction(time, pow, pow);

                    }
                    else if (values.Length == 5)
                    {
                        sbyte left = getPowerValue(values[1], values[2]);
                        sbyte right = getPowerValue(values[3], values[4]);
                        action = new ScriptAction(time, left, right);
                    }
                    else
                    {
                        return new List<ScriptAction>();
                    }
                    list.Add(action);
                }
                catch (Exception e)
                {
                    break;
                }

            }
            return list;
        }

        

        private sbyte getPowerValue(String polarity, String power)
        {
            sbyte value = sbyte.Parse(power);
            if (polarity == "1")
            {
                return (sbyte)(-value);
            }
            return value;
        }
    }
}
