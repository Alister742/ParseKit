using System.Collections.Generic;
using System.Collections;

namespace Core.ExternalTypes
{
    public class HashQueue<T> : Hashtable
    {
        public int FullCount
        {
            get
            {
                lock (this)
                {
                    int count = 0;
                    foreach (DictionaryEntry pair in this)
                    {
                        Queue<T> vipQueue = pair.Value as Queue<T>;
                        count += vipQueue.Count;
                    }
                    return count;
                }
            }
        }

        public void Add(object key, T value)
        {
            lock (this)
            {
                if (this.ContainsKey(key))
                {
                    Queue<T> vipQueue = this[key] as Queue<T>;
                    vipQueue.Enqueue(value);
                }
                else
                {
                    Queue<T> vipQueue = new Queue<T>();
                    vipQueue.Enqueue(value);
                    base.Add(key, vipQueue);
                }
            }
        }

        public T GetObj(string key)
        {
            lock (this)
            {
                if (this.ContainsKey(key))
                {
                    Queue<T> vipQueue = this[key] as Queue<T>;
                    T obj = vipQueue.Dequeue();
                    if (vipQueue.Count == 0)
                        this.Remove(key);
                    return obj;
                }
                return default(T);
            }
        }

        public override bool ContainsKey(object key)
        {
            lock (this)
            {
                if (key == null)
                    return false;
                else
                    return base.ContainsKey(key);
            }
        }
    }
}
