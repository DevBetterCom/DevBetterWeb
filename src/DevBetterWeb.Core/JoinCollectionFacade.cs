using DevBetterWeb.Core.SharedKernel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevBetterWeb.Core
{
    //class JoinCollectionFacade<T, TJoin> : ICollection<T>
    //{
    //    // based on class from this blog post: https://blog.oneunicorn.com/2017/09/25/many-to-many-relationships-in-ef-core-2-0-part-3-hiding-as-icollection/

    //    private readonly ICollection<TJoin> _collection;
    //    private readonly Func<TJoin, T> _selector;
    //    private readonly Func<T, TJoin> _creator;

    //    public JoinCollectionFacade(
    //        ICollection<TJoin> collection,
    //        Func<TJoin, T> selector,
    //        Func<T, TJoin> creator)
    //    {
    //        _collection = collection;
    //        _selector = selector;
    //        _creator = creator;
    //    }

    //    public int Count => _collection.Count;

    //    public bool IsReadOnly => _collection.IsReadOnly;

    //    public void Add(T item)
    //    {
    //        _collection.Add(_creator(item));
    //    }

    //    public void Clear()
    //    {
    //        _collection.Clear();
    //    }

    //    public bool Contains(T item)
    //    {
    //        return _collection.Any(e => Equals(_selector(e), item));
    //    }

    //    public void CopyTo(T[] array, int arrayIndex)
    //    {
    //        this.ToList().CopyTo(array, arrayIndex);
    //    }

    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        return _collection.Select(e => _selector(e)).GetEnumerator();
    //    }

    //    public bool Remove(T item)
    //    {
    //        return _collection.Remove(_collection.FirstOrDefault(e => Equals(_selector(e), item)));
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }
    //}
}
