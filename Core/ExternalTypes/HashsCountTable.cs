using System.Collections;

namespace Core.ExternalTypes
{
    public class KeyCountHashTable : Hashtable
    {
        public int TotalCount
        {
            get
            {
                int count = 0;
                lock (this)
                {
                    foreach (DictionaryEntry pair in this)
                    {
                        int keyCount = (int)pair.Value;
                        count += keyCount;
                    }
                }
                return count;
            }
        }

        public void Add(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                lock (this)
                {
                    if (this.ContainsKey(key))
                    {
                        int keyCount = (int)this[key];
                        this[key] = ++keyCount;
                    }
                    else 
                        base.Add(key, 1);
                }
            }
        }

        public void RemoveOneHost(string host)
        {
            lock (this)
            {
                if (!string.IsNullOrEmpty(host) && this.ContainsKey(host))
                {
                    int keyCount = (int)this[host];
                    if (keyCount <= 1)
                    {
                        base.Remove(host);
                    }
                    else
                    {
                        this[host] = --keyCount;
                    }
                }
            }
        }

        public override bool ContainsKey(object key)
        {
            lock (this)
            {
                if (key == null) 
                    return false;

                return base.ContainsKey(key);
            }
        }

        public int GetCountByKey(string key)
        {
            lock (this)
            {
                if (!string.IsNullOrEmpty(key) && ContainsKey(key))
                {
                    return (int)this[key];
                }
                else
                    return 0;
            }
        }
    }
}
