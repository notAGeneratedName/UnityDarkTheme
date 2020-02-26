#if UNITY_EDITOR
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    public abstract class BaseMightySinglePair
    {
        public object AttributeTarget { get; set; }

        public abstract bool AttributeIs<Ta>() where Ta : BaseMightyAttribute;
        public abstract Ta GetAttribute<Ta>() where Ta : BaseMightyAttribute;
        public abstract Td GetDrawer<Td, Ta>(out Ta attribute) where Td : BaseMightyDrawer where Ta : BaseMightyAttribute;

        public abstract bool GetRefreshDrawer(out IRefreshDrawer drawer, out BaseMightyAttribute attribute);
    }

    public abstract class BaseMightyPairArray
    {
        public abstract BaseMightySinglePair[] GetPairs<Ta>() where Ta : BaseMightyAttribute;

        public abstract Ta GetAttribute<Ta>() where Ta : BaseMightyAttribute;
        public abstract Td[] GetDrawers<Td, Ta>(out Ta[] attributes) where Td : BaseMightyDrawer where Ta : BaseMightyAttribute;

        public abstract bool GetRefreshDrawers(out IRefreshDrawer[] drawers, out BaseMightyAttribute[] attributes);
    }

    public class MightySinglePair<Td, Ta> : BaseMightySinglePair where Td : BaseMightyDrawer where Ta : BaseMightyAttribute
    {
        private readonly Td m_drawer;
        private readonly Ta m_attribute;

        public MightySinglePair(Td drawer, Ta attribute)
        {
            m_drawer = drawer;
            m_attribute = attribute;
        }

        public override bool AttributeIs<Ta1>() => m_attribute is Ta1;

        public override Ta1 GetAttribute<Ta1>() => m_attribute as Ta1;

        public override Td1 GetDrawer<Td1, Ta1>(out Ta1 attribute)
        {
            attribute = GetAttribute<Ta1>();
            return m_drawer as Td1;
        }

        public override bool GetRefreshDrawer(out IRefreshDrawer drawer, out BaseMightyAttribute attribute)
        {
            if (m_drawer is IRefreshDrawer refreshDrawer)
            {
                drawer = refreshDrawer;
                attribute = m_attribute;
                return true;
            }

            drawer = null;
            attribute = null;
            return false;
        }
    }

    public class MightyPairArray<Td, Ta> : BaseMightyPairArray where Td : BaseMightyDrawer where Ta : BaseMightyAttribute
    {
        private readonly MightySinglePair<Td, Ta>[] m_mightySinglePairs;

        public MightyPairArray(Td[] drawers, Ta[] attributes)
        {
            m_mightySinglePairs = new MightySinglePair<Td, Ta>[attributes.Length];
            for (var i = 0; i < m_mightySinglePairs.Length; i++)
                m_mightySinglePairs[i] = new MightySinglePair<Td, Ta>(drawers[i], attributes[i]);
        }

        public override BaseMightySinglePair[] GetPairs<Ta1>() => InternalGetPairs<Ta1>().ToArray();

        private IEnumerable<BaseMightySinglePair> InternalGetPairs<Ta1>() where Ta1 : BaseMightyAttribute
        {
            foreach (var pair in m_mightySinglePairs)
                if (pair.AttributeIs<Ta1>())
                    yield return pair;
        }

        public override Ta1 GetAttribute<Ta1>()
        {
            foreach (var singlePair in m_mightySinglePairs)
                if (singlePair.GetAttribute<Ta1>() is Ta1 attribute)
                    return attribute;
            return null;
        }

        public override Td1[] GetDrawers<Td1, Ta1>(out Ta1[] attributes)
        {
            var length = m_mightySinglePairs.Length;
            attributes = new Ta1[length];
            var drawers = new Td1[length];
            for (var i = 0; i < length; i++)
                drawers[i] = m_mightySinglePairs[i].GetDrawer<Td1, Ta1>(out attributes[i]);

            return drawers;
        }

        public override bool GetRefreshDrawers(out IRefreshDrawer[] drawers, out BaseMightyAttribute[] attributes)
        {
            var drawersList = new List<IRefreshDrawer>();
            var attributesList = new List<BaseMightyAttribute>();

            var value = false;
            foreach (var pair in m_mightySinglePairs)
            {
                if (!pair.GetRefreshDrawer(out var drawer, out var attribute)) continue;

                drawersList.Add(drawer);
                attributesList.Add(attribute);
                value = true;
            }

            drawers = drawersList.ToArray();
            attributes = attributesList.ToArray();
            return value;
        }
    }

    public abstract class BaseMightyMember
    {
        private string m_groupName;
        private string m_groupId;

        public long ID { get; }
        public bool IsSerialized { get; }

        public bool IsGrouped { get; set; }
        public bool IsConditionnalGrouped { get; set; }

        public string GroupID => m_groupId;

        public string GroupName
        {
            get => m_groupName;
            set
            {
                m_groupId = $"{Context.GetInstanceID()}.{Target.GetHashCode()}.{value}";
                m_groupName = value;
            }
        }

        public Object Context { get; }
        public object Target { get; }
        
        public abstract string MemberName { get; }

        public SerializedProperty Property { get; }
        public SerializedProperty GetElement(int index) => !Property.isArray ? Property : Property.GetArrayElementAtIndex(index);

        public Type PropertyType { get; }

        protected BaseMightyMember(Object context, object target, SerializedProperty property = null)
        {
            ID = this.GetUniqueID();
            
            Context = context;
            Target = target;
            IsSerialized = property != null;
            Property = property;
            if (IsSerialized) PropertyType = property.GetSystemType();
        }

        public abstract bool HasAttribute<Ta>() where Ta : BaseMightyAttribute;

        public abstract Ta GetAttribute<Ta>() where Ta : BaseMightyAttribute;

        public abstract bool GetDrawer<Td, Ta>(out Td drawer, out Ta attribute)
            where Td : BaseMightyDrawer where Ta : BaseMightyAttribute;

        public abstract void SetDrawer<Td, Ta>(Td drawer, Ta attribute) where Td : BaseMightyDrawer where Ta : BaseMightyAttribute;

        public abstract bool GetDrawers<Td, Ta>(out Td[] drawers, out Ta[] attributes)
            where Td : BaseMightyDrawer where Ta : BaseMightyAttribute;

        public abstract void SetDrawers<Td, Ta>(Td[] drawers, Ta[] attributes) where Td : BaseMightyDrawer where Ta : BaseMightyAttribute;

        public abstract object GetAttributeTarget<Ta>() where Ta : BaseMightyAttribute;
        public abstract object InitAttributeTarget<Ta>() where Ta : BaseMightyAttribute;

        public abstract (IRefreshDrawer[], BaseMightyAttribute[]) GetRefreshDrawers();
    }

    public class MightyMember<T> : BaseMightyMember where T : MemberInfo
    {
        private readonly Dictionary<Type, BaseMightySinglePair> m_singlePairByAttributeType = new Dictionary<Type, BaseMightySinglePair>();
        private readonly Dictionary<Type, BaseMightyPairArray> m_pairArrayByAttributeType = new Dictionary<Type, BaseMightyPairArray>();

        public T MemberInfo { get; }

        public MightyMember(T memberInfo, Object context, object target, SerializedProperty property = null)
            : base(context, target, property)
        {
            MemberInfo = memberInfo;
            MemberName = MemberInfo.Name;
        }

        public override string MemberName { get; }

        public override bool HasAttribute<Ta>() =>
            m_singlePairByAttributeType.ContainsKey(typeof(Ta)) || m_pairArrayByAttributeType.ContainsKey(typeof(Ta));

        public override Ta GetAttribute<Ta>() =>
            m_singlePairByAttributeType.TryGetValue(typeof(Ta), out var singlePair) ? singlePair.GetAttribute<Ta>() :
            m_pairArrayByAttributeType.TryGetValue(typeof(Ta), out var pairArray) ? pairArray.GetAttribute<Ta>() : null;

        public override bool GetDrawer<Td, Ta>(out Td drawer, out Ta attribute)
        {
            if (m_singlePairByAttributeType.TryGetValue(typeof(Ta), out var singlePair))
            {
                drawer = singlePair.GetDrawer<Td, Ta>(out attribute);
                return true;
            }

            foreach (var pair in m_singlePairByAttributeType.Values)
            {
                if (!pair.AttributeIs<Ta>()) continue;

                drawer = pair.GetDrawer<Td, Ta>(out attribute);
                return true;
            }

            drawer = default;
            attribute = default;
            return false;
        }

        public override void SetDrawer<Td, Ta>(Td drawer, Ta attribute)
        {
            m_singlePairByAttributeType[typeof(Ta)] = new MightySinglePair<Td, Ta>(drawer, attribute);
            drawer.InitDrawer(this, attribute);
        }

        public override bool GetDrawers<Td, Ta>(out Td[] drawers, out Ta[] attributes)
        {
            if (m_pairArrayByAttributeType.TryGetValue(typeof(Ta), out var pairArray))
            {
                drawers = pairArray.GetDrawers<Td, Ta>(out attributes);
                return true;
            }

            var drawersList = new List<Td>();
            var attributesList = new List<Ta>();

            foreach (var pairs in m_pairArrayByAttributeType.Values)
            foreach (var pair in pairs.GetPairs<Ta>())
            {
                drawersList.Add(pair.GetDrawer<Td, Ta>(out var attribute));
                attributesList.Add(attribute);
            }

            if (drawersList.Count > 0)
            {
                drawers = drawersList.ToArray();
                attributes = attributesList.ToArray();
                return true;
            }

            drawers = default;
            attributes = default;
            return false;
        }

        public override void SetDrawers<Td, Ta>(Td[] drawers, Ta[] attributes)
        {
            m_pairArrayByAttributeType[typeof(Ta)] = new MightyPairArray<Td, Ta>(drawers, attributes);
            for (var i = 0; i < drawers.Length; i++) drawers[i].InitDrawer(this, attributes[i]);
        }

        public override object GetAttributeTarget<Ta>()
        {
            if (Property == null) return null;

            foreach (var singlePair in m_singlePairByAttributeType.Values)
                if (singlePair.AttributeIs<Ta>())
                    return singlePair.AttributeTarget;

            foreach (var pairArray in m_pairArrayByAttributeType.Values)
            foreach (var singlePair in pairArray.GetPairs<Ta>())
                return singlePair.AttributeTarget;

            return null;
        }

        public override object InitAttributeTarget<Ta>()
        {
            if (Property == null) return null;
            var target = Property.GetAttributeTarget<Ta>();
            foreach (var singlePair in m_singlePairByAttributeType.Values)
                if (singlePair.AttributeIs<Ta>())
                    singlePair.AttributeTarget = target;

            foreach (var pairArray in m_pairArrayByAttributeType.Values)
            foreach (var singlePair in pairArray.GetPairs<Ta>())
                singlePair.AttributeTarget = target;

            return target;
        }

        public override (IRefreshDrawer[], BaseMightyAttribute[]) GetRefreshDrawers()
        {
            var drawersList = new List<IRefreshDrawer>();
            var attributesList = new List<BaseMightyAttribute>();
            foreach (var mightySinglePair in m_singlePairByAttributeType.Values)
            {
                if (!mightySinglePair.GetRefreshDrawer(out var drawer, out var attribute)) continue;

                drawersList.Add(drawer);
                attributesList.Add(attribute);
            }

            foreach (var mightyPairArray in m_pairArrayByAttributeType.Values)
            {
                if (!mightyPairArray.GetRefreshDrawers(out var drawers, out var attributes)) continue;

                drawersList.AddRange(drawers);
                attributesList.AddRange(attributes);
            }

            return (drawersList.ToArray(), attributesList.ToArray());
        }
    }
}
#endif