namespace EtAlii.CryptoMagic.Surfing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Serilog;

    public class Persistence<TItem> : IPersistence<TItem>
    {
        private readonly ILogger _logger = Log.ForContext<Persistence<TItem>>();

        public IReadOnlyList<TItem> Items { get; } 
        private readonly List<TItem> _items;
        private readonly string _folder;

        public Persistence(string storageFolder, string identifier)
        {
            _folder = Path.Combine(storageFolder, identifier);
            
            _items = new List<TItem>();
            Items = _items.AsReadOnly();
        }

        public void Load()
        {
            if (!Directory.Exists(_folder))
            {
                _logger.Information("Creating folder {FolderName}", _folder);
                Directory.CreateDirectory(_folder);
            }
            
            _logger.Information("Reading previous items from {FolderName}", _folder);

            var files = Directory.GetFiles(_folder);

            var items = files
                .Select(File.ReadAllText)
                .Select(t => JsonSerializer.Deserialize<TItem>(t))
                .ToArray();
            
            _items.AddRange(items);
            _logger.Information("Read {NumberOfItems}", items.Length);
        }
        
        public void Add(TItem item)
        {
            var fileName = Path.Combine(_folder, $"{DateTime.Now.Ticks}.txt");
            using var sw = File.CreateText(fileName);
            var json = JsonSerializer.Serialize(item);
            sw.Write(json);

            item = JsonSerializer.Deserialize<TItem>(json);
            _items.Add(item);
        }
    }
}