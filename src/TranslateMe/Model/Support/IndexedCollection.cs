using System;
using System.Collections.ObjectModel;

namespace TranslateMe.Model.Support
{
    public abstract class IndexedCollection<TItem, TKey> : ObservableCollection<TItem>
    {
        private readonly Func<TItem, TKey> _keySelector;

        protected IndexedCollection(Func<TItem, TKey> keySelector)
        {
            _keySelector = keySelector;
        }

        public TItem this[TKey key]
        {
            get
            {
                for (var i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];
                    var itemKey = _keySelector(item);
                    var match = itemKey.Equals(key);

                    if (match)
                        return item;
                }

                return default(TItem);
            }
        }
    }
}