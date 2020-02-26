#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;

namespace MightyAttributes.Editor
{
    public class MightyCache<T>
    {
        private readonly Dictionary<long, T> m_cache = new Dictionary<long, T>();

        public bool Contains(BaseMightyMember mightyMember) => Contains(mightyMember.ID);
        public bool Contains(long id) => m_cache.ContainsKey(id);

        public bool TryGetValue(BaseMightyMember mightyMember, out T value) => TryGetValue(mightyMember.ID, out value);
        public bool TryGetValue(long id, out T value) => m_cache.TryGetValue(id, out value);

        public void ClearCache() => m_cache.Clear();

        public T this[BaseMightyMember mightyMember]
        {
            get => this[mightyMember.ID];
            set => this[mightyMember.ID] = value;
        }

        public T this[long id]
        {
            get => m_cache[id];
            set => m_cache[id] = value;
        }

        public Dictionary<long, T>.ValueCollection Values => m_cache.Values;
        public int Count => m_cache.Count; 
    }

    public class MightyCache<Tk, Tv>
    {
        private readonly Dictionary<(long, Tk), Tv> m_cache = new Dictionary<(long, Tk), Tv>();

        public bool Contains(BaseMightyMember mightyMember, Tk keyItem) => Contains(mightyMember.ID, keyItem);
        public bool Contains(long id, Tk keyItem) => m_cache.ContainsKey((id, keyItem));

        public bool TryGetValue(BaseMightyMember mightyMember, Tk keyItem, out Tv value) =>
            TryGetValue(mightyMember.ID, keyItem, out value);

        public bool TryGetValue(long id, Tk keyItem, out Tv value) =>
            m_cache.TryGetValue((id, keyItem), out value);

        public void ClearCache() => m_cache.Clear();

        public Tv this[BaseMightyMember mightyMember, Tk keyItem]
        {
            get => this[mightyMember.ID, keyItem];
            set => this[mightyMember.ID, keyItem] = value;
        }

        public Tv this[long id, Tk keyItem]
        {
            get => m_cache[(id, keyItem)];
            set => m_cache[(id, keyItem)] = value;
        }

        public Dictionary<(long, Tk), Tv>.ValueCollection Values => m_cache.Values;
        public int Count => m_cache.Count; 
    }

    public class MightyMemberCache<T> where T : MemberInfo
    {
        private readonly List<MightyMember<T>> m_cache = new List<MightyMember<T>>();

        public List<MightyMember<T>> Values => m_cache;
        public int Count => m_cache.Count; 

        public MightyMember<T> Add(MightyMember<T> mightyMember)
        {
            m_cache.Add(mightyMember);
            return mightyMember;
        }

        public void ClearCache() => m_cache.Clear();
    }
}
#endif