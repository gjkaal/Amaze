// Rapbit Game development
//
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace NorthGame.Tiled
{
    public class TiledMapManager
    {
        private readonly IFileSystem _fileSystem;
        public TiledMap TileSet { get; private set; }
        private readonly Dictionary<string, string> _tileSetFiles = new Dictionary<string, string>();

        public TiledMapManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IEnumerable<string> TileSetFiles() => _tileSetFiles.Keys;

        public bool LoadTileSet(string tileSetName)
        {
            var fileName = _tileSetFiles[tileSetName];
            if (string.IsNullOrEmpty(fileName)) return false;
            var tileset = new TiledMap();
            try
            {
                JsonConvert.PopulateObject(System.IO.File.ReadAllText(fileName), tileset);
                TileSet = tileset;
                return true;
            }
            catch (Exception e)
            {
                // TODO : Log warning and exception
                return false;
            }
        }

        public void LoadTileSetFiles(string directoryName)
        {
            _tileSetFiles.Clear();
            foreach (var file in ScanFiles(directoryName))
            {
                var shortName = _fileSystem.Path.GetFileNameWithoutExtension(file.Name);
                if (_tileSetFiles.ContainsKey(shortName)) continue;
                _tileSetFiles.Add(shortName, file.FullName);
            }
        }

        private IEnumerable<IFileInfo> ScanFiles(string directoryName)
        {
            var dir = _fileSystem.DirectoryInfo.FromDirectoryName(directoryName);
            if (!dir.Exists) yield break;

            foreach (var sub in dir.EnumerateDirectories())
            {
                foreach (var s in ScanFiles(sub.FullName))
                {
                    yield return s;
                }
            }
            foreach (var file in dir.EnumerateFiles("*.json"))
            {
                var content = file.OpenText().ReadToEnd();
                if (content.Contains("\"tiledversion\""))
                {
                    yield return file;
                }
            }
        }
    }
}
