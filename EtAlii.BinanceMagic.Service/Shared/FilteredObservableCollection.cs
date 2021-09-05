namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;

    public static class ObservableCollectionFilterExtensions
    {
        public static void SubscribeFiltered<TInstance>( 
            this IList<TInstance> target,
            ObservableCollection<TInstance> source,
            Func<TInstance, bool> filter)
        {
            source.CollectionChanged += (_, e) => OnCollectionChanged(e, source, target, filter);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), source, target, filter);
        }

        public static void FilterWhenNeeded<TInstance>( 
            this IList<TInstance> target,
            TInstance instance,
            Func<TInstance, bool> filter)
        {
            if (filter(instance))
            {
                if (!target.Contains(instance))
                {
                    target.Add(instance);
                }
            }
            else
            {
                if (target.Contains(instance))
                {
                    target.Remove(instance);
                }
            }
        }

        private static void OnCollectionChanged<TInstance>(
            NotifyCollectionChangedEventArgs e, 
            ObservableCollection<TInstance> source, 
            IList<TInstance> target,
            Func<TInstance, bool> filter)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems!.OfType<TInstance>())
                    {
                        if (filter(newItem))
                        {
                            if (!target.Contains(newItem))
                            {
                                target.Add(newItem);
                            }
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var oldItem in e.OldItems!.OfType<TInstance>())
                    {
                        if (filter(oldItem))
                        {
                            if (target.Contains(oldItem))
                            {
                                target.Remove(oldItem);
                            }
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    target.Clear();

                    foreach (var item in source)
                    {
                        if (filter(item))
                        {
                            target.Add(item);
                        }
                    }
                    break;
            }
        }
    }
}