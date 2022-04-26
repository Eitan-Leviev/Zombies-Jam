using UnityEngine;
using System;
using System.Collections.Generic;

namespace Avrahamy.Collections {
    public static class CollectionsExtensions {
        /// <summary>
        /// Returns a value of type V from a data structure of type T.
        /// </summary>
        public delegate V GetValueDelegate<T, V>(T data);

        public static T Last<T>(this T[] array) {
            return array[array.Length - 1];
        }

        public static void SetLast<T>(this T[] array, T value) {
            array[array.Length - 1] = value;
        }

        public static T Last<T>(this IList<T> array) {
            return array[array.Count - 1];
        }

        public static void SetLast<T>(this IList<T> array, T value) {
            array[array.Count - 1] = value;
        }

        public static void SetSize<T>(this IList<T> list, int size, T item = default) {
            for (int i = list.Count; i < size; i++) {
                list.Add(item);
            }
        }

        /// <summary>
        /// Intersects the list with another list.
        /// Keeps the original items, only takes new items from 'other' and
        /// removes items that don't exist in 'other'.
        /// </summary>
        public static void StableIntersect<T>(this List<T> list, List<T> other, Func<T, T, bool> IsEqual) {
            foreach (var otherItem in other) {
                if (!list.Contains(otherItem, IsEqual)) {
                    list.Add(otherItem);
                }
            }

            // All values of 'other' are now in the list.
            if (other.Count == list.Count) return;

            // 'other' is shorter, so remove items from the list that are not
            // found in 'other'.
            for (int i = list.Count - 1; i >= 0; i--) {
                if (!other.Contains(list[i], IsEqual)) {
                    list.RemoveAt(i);
                }
            }
        }

        public static bool Contains<T>(this IEnumerable<T> list, T item, Func<T, T, bool> IsEqual) {
            foreach (var t in list) {
                if (IsEqual(t, item)) {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsType<T>(this IList<T> list, Type type) {
            foreach (var item in list) {
                var itemType = item.GetType();
                if (itemType != type) continue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Warning: this is a costly action!
        /// </summary>
        public static T[] Append<T>(this T[] array, T item) {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = item;
            return array;
        }

        public static bool Contains<T>(this T[] array, T item) {
            foreach (var element in array) {
                if (element == null) {
                    if (item == null) return true;
                    continue;
                }
                if (element.Equals(item)) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if added to list.
        /// </summary>
        public static bool AddIfNotContains<T>(this IList<T> list, T item) {
            if (list.Contains(item)) return false;
            list.Add(item);
            return true;
        }

        public static HashSet<T> AddAndReturn<T>(this HashSet<T> hashSet, T item) {
            hashSet.Add(item);
            return hashSet;
        }

        /// <summary>
        /// Returns the item at a given index or the last item if the index is
        /// out of range.
        /// </summary>
        public static T GetAtIndexOrLast<T>(this T[] array, int index) {
            index = Mathf.Min(index, array.Length - 1);
            return array[index];
        }

        public static IEnumerable<T> Reverse<T>(this LinkedList<T> list) {
            var node = list.Last;
            LinkedListNode<T> previous;
            while (node != null) {
                previous = node.Previous;
                yield return node.Value;
                if (previous == null) {
                    // In case a value was added.
                    previous = node.Previous;
                }
                node = previous;
            }
        }

        public static Dictionary<K, V> ToDictionary<K, V, T>(this T[] array, GetValueDelegate<T, K> getKey, GetValueDelegate<T, V> getValue) {
            var result = new Dictionary<K, V>();

            foreach (T data in array) {
                result[getKey(data)] = getValue(data);
            }

            return result;
        }

        public static Stack<T> ToStack<T>(this List<T> list) {
            var stack = new Stack<T>();
            for (var i = list.Count - 1; i >= 0; i--) {
                var el = list[i];
                stack.Push(el);
            }

            return stack;
        }

        public static TCast[] As<TOriginal, TCast>(this TOriginal[] array) where TCast : class {
            var newArray = new TCast[array.Length];
            for (int i = 0; i < array.Length; i++) {
                newArray[i] = array[i] as TCast;
            }
            return newArray;
        }

        /// <summary>
        /// Used on a Dictionary of lists, makes sure there is a list for the given
        /// key and returns this list.
        /// </summary>
        public static List<TItem> EnsureListForKey<TKey, TItem>(this Dictionary<TKey, List<TItem>> dict, TKey key) {
            return dict.EnsureCollectionForKey<TKey, TItem, List<TItem>>(key);
        }

        /// <summary>
        /// Used on a Dictionary of lists, makes sure there is a collection for the given
        /// key and returns this collection.
        /// </summary>
        public static TCollection EnsureCollectionForKey<TKey, TItem, TCollection>(this Dictionary<TKey, TCollection> dict, TKey key) where TCollection : ICollection<TItem>, new() {
            if (!dict.ContainsKey(key)) {
                dict[key] = new TCollection();
            }
            return dict[key];
        }
    }
}
