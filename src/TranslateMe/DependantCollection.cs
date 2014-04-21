using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace TranslateMe
{
    public class DependantCollection<TOuter, TInner> :
        ObservableCollection<TOuter>,
        IDisposable
        where TOuter : class
        where TInner : class
    {
        private readonly Func<TInner, TOuter, bool> _comparer;
        private readonly Func<TInner, TOuter> _converter;
        private readonly ObservableCollection<TInner> _innerCollection;

        public DependantCollection(
            ObservableCollection<TInner> innerCollection,
            Func<TInner, TOuter, bool> comparer,
            Func<TInner, TOuter> converter)
            : base(from item in innerCollection select converter(item))
        {
            _comparer = comparer;
            _converter = converter;
            _innerCollection = innerCollection;

            innerCollection.CollectionChanged += InnerCollectionOnCollectionChanged;
        }

        public void Dispose()
        {
            _innerCollection.CollectionChanged -= InnerCollectionOnCollectionChanged;
        }

        private void InnerCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.OldItems != null)
            {
                eventArgs.OldItems.
                    Cast<TInner>().
                    ForEach(inner => this.Remove(outer => _comparer(inner, outer)));
            }

            if (eventArgs.NewItems != null)
            {
                var newItems = eventArgs.NewItems.
                    Cast<TInner>().
                    Select(_converter);

                this.AddRange(newItems);
            }
        }
    }
}