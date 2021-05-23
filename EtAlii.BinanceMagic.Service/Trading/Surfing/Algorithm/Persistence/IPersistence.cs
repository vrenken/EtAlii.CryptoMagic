namespace EtAlii.BinanceMagic.Service.Surfing
{
    using System.Collections.Generic;

    public interface IPersistence<TItem>
    {
        IReadOnlyList<TItem> Items { get; }
        
        void Load();
        void Add(TItem item);

    }
}