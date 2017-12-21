using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Generic
{
    public static class SafeListLinq
    {
        public static int Count<T>(this SafeList<T> list, Func<T, bool> selector)
        {
            lock (list.SyncRoot)
            {
                return list.InternalList.Count(selector);
            }
        }
    }

    public class SafeList<T> : IList<T>
    {
        public readonly object SyncRoot = new object();
        private List<T> _internalList = new List<T>();

        public SafeList(IEnumerable<T> array)
        {
            _internalList = new List<T>(array);
        }

        public SafeList()
        {
        }

        public int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return _internalList.Count;
                }
            }
        }

        public List<T> InternalList
        {
            get { return _internalList; }
            set { _internalList = value; }
        }

        public bool IsReadOnly { get { return false; } }

        public T this[int index]
        {
            get
            {
                lock (SyncRoot)
                {
                    return _internalList[index];
                }
            }
            set
            {
                lock (SyncRoot)
                {
                    _internalList[index] = value;
                }
            }
        }

        public void Add(T item)
        {
            lock (SyncRoot)
            {
                _internalList.Add(item);
            }
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                _internalList.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (SyncRoot)
            {
                return _internalList.Contains(item);
            }
        }

        public void CopyTo(T[] output, int index)
        {
            lock (SyncRoot)
            {
                _internalList.CopyTo(output, index);
            }
        }

        //public IEnumerator<T> GetEnumerator()
        //{
        //    lock (Lock)
        //    {
        //        return _internalList.GetEnumerator();
        //    }
        //    //for (int i = 0; i < Count; i++)
        //    //    yield return _array[i];
        //}
        public T FirstOrDefault(Func<T, bool> selector)
        {
            lock (SyncRoot)
            {
                return _internalList.FirstOrDefault(selector);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)_internalList).GetEnumerator();
        }

        //IEnumerator<T> IEnumerable<T>.GetEnumerator()
        //{
        //    // call the generic version of the method
        //    return this.GetEnumerator();
        //}
        //public int Count(Func<T,bool> selector)
        //{
        //    lock (SyncRoot)
        //    {
        //        return _internalList.Count(selector);
        //    }
        //}
        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)_internalList).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            lock (SyncRoot)
            {
                return _internalList.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (SyncRoot)
            {
                _internalList.Insert(index, item);
            }
        }

        public bool Remove(T item)
        {
            lock (SyncRoot)
            {
                return _internalList.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (SyncRoot)
            {
                _internalList.RemoveAt(index);
            }
        }

        public void RePopulate(IEnumerable<T> array)
        {
            lock (SyncRoot)
            {
                _internalList.Clear();
                _internalList.AddRange(array);
            }
        }
    }
}