﻿namespace System.Collections.Generic
{
    public static class IEnumratorExtensions
    {
        public static List<T> ToList<T>(this IEnumerator<T> e)
        {
            var list = new List<T>();
            while (e.MoveNext())
            {
                list.Add(e.Current);
            }
            return list;
        }
        
        public static void Add<T>(this IList<T> e,List<T> list)
        {
            foreach(T item in list)
            {
                e.Add(item);
            }
        }
        
        public static void Add<T>(this IList<T> e, IEnumerable<T> list)
        {
            foreach (T item in list)
            {
                e.Add(item);
            }
        }
    }
}

