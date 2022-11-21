using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;

namespace Reserve.Data
{
    public class JsonObjectComparer<T> : ValueComparer<T> where T : class
    {
        private static int HashcodeOf(T target)
        {
            var jt = JToken.FromObject(target);
            return new JTokenEqualityComparer().GetHashCode(jt);
        }
        
        private static T SnapshotOf(T target)
        {
            if (target is ICloneable c)
                return c.Clone() as T;

            return JToken.FromObject(target)
                .DeepClone()
                .ToObject<T>();
        }
        
        private static bool IsEquals(T left, T right)
        {
            if (left is IEquatable<T> e)
                return e.Equals(right);

            return JToken.DeepEquals(JToken.FromObject(left), JToken.FromObject(right));
        }

        public JsonObjectComparer() 
            : base(
                (l, r) => IsEquals(l, r),
                it => HashcodeOf(it),
                it => SnapshotOf(it)
            ) { }
    }
}