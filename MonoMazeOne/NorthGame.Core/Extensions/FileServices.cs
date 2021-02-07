// Rapbit Game development
//
using Newtonsoft.Json;
using NorthGame.Core.Abstractions;
using System;

namespace NorthGame.Core.Extensions
{
    public static class FileExtensions
    {
        public static string ExecutingLocation { get; } = FindExecutingLocation();

        private static string FindExecutingLocation()
        {
            var file = new System.IO.FileInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            return file.DirectoryName;
        }

        public static void SaveGameElement<T>(this T element, string path) where T : IGameElement
        {
            var jsonData = JsonConvert.SerializeObject(element, Formatting.Indented);
            System.IO.File.WriteAllText(System.IO.Path.Combine(ExecutingLocation, path), jsonData);
        }

        public static T Populate<T>(this T element) where T : IGameElement
        {
            return element.Populate(element.Layout);
        }

        public static T Populate<T>(this T element, string path) where T : IGameElement
        {
            // TODO : Abstraction for FileSystem, create unit tests
            // TODO : Optimize with caching

            if (string.IsNullOrEmpty(path)) return element;
            var filePath = System.IO.Path.Combine(ExecutingLocation, path + ".json");
            if (!System.IO.File.Exists(filePath))
            {
                // TODO : <ove all string literals to the constants class.
                throw new ArgumentException($"Design file not found: {filePath}. Check the 'Copy to outputfolder' settings for the design file ");
            }
            JsonConvert.PopulateObject(System.IO.File.ReadAllText(filePath), element);
            return element;
        }
    }
}
