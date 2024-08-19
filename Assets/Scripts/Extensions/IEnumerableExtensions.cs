using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class IEnumerableExtensions
    {
        public static T GetRandomElement<T>(this IEnumerable<T> list, Func<T, bool> predicate = null)
        {
            var array = list as T[] ?? list.ToArray();

            if (array.Length == 0)
                throw new NullReferenceException();

            var tempList = predicate != null ? array.Where(predicate.Invoke).ToList() : array.ToList();
            
            if (tempList.Count == 0)
                throw new NullReferenceException();
            
            return tempList[Random.Range(0, tempList.Count)];
        }
        
        public static T[] GetRandomElements<T>(this IEnumerable<T> list, int count, Func<T, bool> predicate = null)
        {
            var array = list as T[] ?? list.ToArray();

            if (array.Length == 0)
                throw new NullReferenceException();

            var returnedArray = new T[count];
            
            var tempList = predicate != null ? array.Where(predicate.Invoke).ToList() : array.ToList();

            for (int i = 0; i < count; i++)
            {
                var item = tempList[Random.Range(0, tempList.Count)];
                returnedArray[i] = item;
                tempList.Remove(item);
            }
            
            return returnedArray;
        }
    }
}