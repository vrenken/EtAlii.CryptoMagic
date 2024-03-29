﻿namespace EtAlii.CryptoMagic
{
    using System;
    using System.Linq;

    public partial class OneOffView
    {
        protected override string GetListUrl() => "/one-off/list";
        
        protected override OneOffTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.OneOffTradings.Single(t => t.Id == id);
        }
    }
}