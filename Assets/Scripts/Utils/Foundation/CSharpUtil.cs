using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TX
{
    /// <summary>
    /// Reflection utility.
    /// </summary>
    public static class ReflectionUtil
    {
        /// <summary>
        /// Equality comparer for <see cref="Type"/> s.
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IEqualityComparer{System.Type}"/>
        private class SimpleTypeComparer : IEqualityComparer<Type>
        {
            /// <inheritdoc/>
            bool IEqualityComparer<Type>.Equals(Type x, Type y)
            {
                return x.Assembly == y.Assembly &&
                    x.Namespace == y.Namespace &&
                    x.Name == y.Name;
            }

            /// <inheritdoc/>
            int IEqualityComparer<Type>.GetHashCode(Type obj)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets all types that inherit specified type.
        /// </summary>
        /// <param name="t"> The ancestor type. </param>
        /// <returns> All types that inherit from the ancestor. </returns>
        public static IEnumerable<Type> GetSubTypes(this Type t)
        {
            return Assembly.GetAssembly(t).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(t));
        }

        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                    return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static bool IsArrayOf(this Type toCheck, Type elementType)
        {
            if (!toCheck.IsArray)
                return false;
            Type elemType = toCheck.GetElementType();
            return elemType == elementType || elemType.IsSubclassOf(elementType);
        }

        public static bool IsListOf(this Type toCheck, Type elementType)
        {
            if (!toCheck.IsGenericType)
                return false;
            Type generic = toCheck.GetGenericTypeDefinition();
            if (!generic.IsAssignableFrom(typeof(List<>)))
                return false;
            var args = toCheck.GetGenericArguments();
            if (args.Length != 1)
                return false;
            Type genericArg = args[0];
            return genericArg == elementType || genericArg.IsSubclassOf(elementType);
        }

        /// <summary>
        /// Gets the field value of an object.
        /// </summary>
        /// <param name="source"> The source object. </param>
        /// <param name="name"> Field name. </param>
        /// <returns> The value of the object. </returns>
        public static object GetFieldValue(object source, string name)
        {
            if (source == null)
            {
                return null;
            }
            var type = source.GetType();
            do
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);
                type = type.BaseType;
            } while (type != null);
            return null;
        }

        /// <summary>
        /// Gets the value of an <see cref="IEnumerable"/> field of an object
        /// </summary>
        /// <param name="source"> The source object. </param>
        /// <param name="name"> Field name. </param>
        /// <param name="index"> Element index. </param>
        /// <returns> The value of the field. </returns>
        public static object GetFieldValue(object source, string name, int index)
        {
            var enumerable = GetFieldValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
            {
                enm.MoveNext();
            }
            return enm.Current;
        }

        /// <summary>
        /// Gets the generic method for a type.
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <param name="name"> The name. </param>
        /// <param name="parameterTypes"> The parameter types. </param>
        /// <returns> The method info object of the generic method. </returns>
        public static MethodInfo GetGenericMethod(this Type type, string name, Type[] parameterTypes)
        {
            var methods = type.GetMethods();
            foreach (var method in methods.Where(m => m.Name == name))
            {
                var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                if (methodParameterTypes.SequenceEqual(parameterTypes ?? new Type[0], new SimpleTypeComparer()))
                {
                    return method;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Enum utility.
    /// </summary>
    public static class EnumUtil
    {
        /// <summary>
        /// Gets all values of an <seealso cref="Enum"/> sorted in ascending order.
        /// </summary>
        /// <typeparam name="T"> Type of enum. </typeparam>
        /// <returns> All values of the enum. </returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Determines whether this instance has specified flag bits set.
        /// </summary>
        public static bool HasFlag(this Enum val, Enum flag)
        {
            if (!val.GetType().Equals(flag.GetType()))
            {
                throw new ArgumentException("Types mismatch");
            }
            var iFlag = Convert.ToInt32(flag);
            var iThis = Convert.ToInt32(val);
            return ((iThis & iFlag) == iFlag);
        }

        /// <summary>
        /// Determines whether this enum value is a flag (exactly 1 bit set).
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>Whether this enum value is a flag</returns>
        public static bool IsFlag(this Enum val)
        {
            var x = Convert.ToInt32(val);
            return x > 0 && (x & (x - 1)) == 0;
        }

        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example>string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;</example>
        public static T GetAttribute<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
    }

    public static class GenericUtil
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static bool UnorderedCheck<T>(T a, T b, Func<T, T, bool> check)
        {
            return check(a, b) || check(b, a);
        }
    }

    /// <summary>
    /// String utility.
    /// </summary>
    public static class StringUtil
    {
        private static Regex NonLeadingWordBegining = new Regex(@"(\B[A-Z]|(?<=\D)\d)");

        public static string SpaceWords(string str)
        {
            return TitleCase(NonLeadingWordBegining.Replace(str, " $1"));
        }

        public static string TitleCase(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        /// <summary>
        /// Gets the N'th index of a substring.
        /// </summary>
        /// <param name="target"> This string. </param>
        /// <param name="value"> The substring. </param>
        /// <param name="n"> The n. </param>
        /// <returns> The N'th index of the substring. </returns>
        public static int NthIndexOf(this string target, string value, int n)
        {
            Match m = Regex.Match(target, "((" + Regex.Escape(value) + ").*?){" + n + "}");

            if (m.Success)
            {
                return m.Groups[2].Captures[n - 1].Index;
            }
            else
            {
                return -1;
            }
        }
    }

    /// <summary>
    /// Array extension methods.
    /// </summary>
    public static class ArrayExtensions
    {
        public static IEnumerable<T> GetEnumerable<T>(this ArraySegment<T> seg)
        {
            var arr = seg.Array;
            int end = seg.Offset + seg.Count;
            for (int i = seg.Offset; i < end; i++)
            {
                yield return arr[i];
            }
        }

        /// <summary>
        /// Re-arranges the array with specified indices.
        /// </summary>
        /// <typeparam name="T"> Type of element. </typeparam>
        /// <param name="array"> The array. </param>
        /// <param name="newIndices"> The new indices. </param>
        /// <returns> The arranged array. </returns>
        /// <exception cref="InvalidOperationException">
        /// Length indices array does not match the target array.
        /// </exception>
        public static T[] Arranged<T>(this T[] array, int[] newIndices)
        {
            if (array.Length != newIndices.Length)
            {
                throw new InvalidOperationException("Length indices array does not match the target array.");
            }
            T[] result = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[newIndices[i]];
            }
            return result;
        }
    }

    public static class BitExtensions
    {
        /// <summary>
        /// Count the bits set in a bitarray.
        /// http://stackoverflow.com/questions/5063178/counting-bits-set-in-a-net-bitarray-class
        /// </summary>
        /// <param name="bitArray">The bit array.</param>
        /// <returns></returns>
        public static Int32 GetCardinality(this BitArray bitArray)
        {
            Int32[] ints = new Int32[(bitArray.Count >> 5) + 1];

            bitArray.CopyTo(ints, 0);

            Int32 count = 0;

            // fix for not truncated bits in last integer that may have been set to true with SetAll()
            ints[ints.Length - 1] &= ~(-1 << (bitArray.Count % 32));

            for (Int32 i = 0; i < ints.Length; i++)
            {
                Int32 c = ints[i];

                // magic (http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel)
                unchecked
                {
                    c = c - ((c >> 1) & 0x55555555);
                    c = (c & 0x33333333) + ((c >> 2) & 0x33333333);
                    c = ((c + (c >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
                }

                count += c;
            }

            return count;
        }
    }

    /// <summary>
    /// Extension methods for C#
    /// </summary>
    public static class CSharpExtensions
    {
        /// <summary>
        /// Performs action to each element of a sequence.
        /// </summary>
        /// <typeparam name="T"> Type of element. </typeparam>
        /// <param name="seq"> The sequence. </param>
        /// <param name="action"> The Action. </param>
        public static void ForEach<T>(this IEnumerable<T> seq, Action<int, T> action)
        {
            int i = 0;
            foreach (var item in seq)
            {
                action(i++, item);
            }
        }

        /// <summary>
        /// Casts the elements of an IEnumerable to the specified type.
        /// </summary>
        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable seq)
        {
            foreach (var s in seq)
            {
                yield return (TResult)s;
            }
        }

        /// <summary>
        /// Filters the elements of an IEnumerable based on a specified type.
        /// </summary>
        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable seq) where TResult : class
        {
            foreach (var s in seq)
            {
                TResult r = s as TResult;
                if (r != null)
                    yield return r;
            }
        }

        /// <summary>
        /// Repeats the sequence.
        /// </summary>
        /// <typeparam name="T"> Type of element. </typeparam>
        /// <param name="seq"> The sequence. </param>
        /// <param name="count"> How many times the sequence repeats. </param>
        /// <returns> The new sequence. </returns>
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> seq, int count)
        {
            return Enumerable.Repeat(seq, count).Aggregate((a, b) => a.Concat(b));
        }

        /// <summary>
        /// Repeats each element in the list. E.g. "abc" -&gt; "aaabbbccc"
        /// </summary>
        /// <typeparam name="T"> Type of element. </typeparam>
        /// <param name="seq"> The sequence. </param>
        /// <param name="count"> How many times each element repeats. </param>
        /// <returns> The new sequence. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> When count is negative. </exception>
        public static IEnumerable<T> RepeatElementWise<T>(this IEnumerable<T> seq, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(count.ToString());
            }
            foreach (var item in seq)
            {
                for (int i = 0; i < count; i++)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Appends a element to this sequence.
        /// </summary>
        /// <typeparam name="T"> Type of element. </typeparam>
        /// <param name="seq"> The sequence. </param>
        /// <param name="element"> The element. </param>
        /// <returns> The new sequence. </returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> seq, T element)
        {
            return seq.Concat(new[] { element });
        }

        /// <summary>
        /// Zips the specified seq a and seq b.
        /// </summary>
        /// <typeparam name="TA">The type of seq a.</typeparam>
        /// <typeparam name="TB">The type of seq b.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="seqA">The seq a.</param>
        /// <param name="seqB">The seq b.</param>
        /// <param name="func">The combining function.</param>
        /// <returns>Result sequence.</returns>
        public static IEnumerable<TResult> Zip<TA, TB, TResult>(this IEnumerable<TA> seqA, IEnumerable<TB> seqB, Func<TA, TB, TResult> func)
        {
            if (seqA == null) throw new ArgumentNullException("seqA");
            if (seqB == null) throw new ArgumentNullException("seqB");

            using (var iteratorA = seqA.GetEnumerator())
            using (var iteratorB = seqB.GetEnumerator())
            {
                while (iteratorA.MoveNext() && iteratorB.MoveNext())
                {
                    yield return func(iteratorA.Current, iteratorB.Current);
                }
            }
        }

        /// <summary>
        /// Shuffles the list in-place using specified <see cref="RNG"/>.
        /// </summary>
        /// <typeparam name="T"> Type of element. </typeparam>
        /// <param name="list"> The list. </param>
        /// <param name="rng"> The RNG. </param>
        /// <returns> This shuffled list. </returns>
        public static List<T> Shuffle<T>(this List<T> list, RNG rng)
        {
            // http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.NextIntRange(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        /// <summary>
        /// Removes any element matching the criteria.
        /// </summary>
        /// <typeparam name="T"> Type of element. </typeparam>
        /// <param name="list"> The list. </param>
        /// <param name="predicate"> Match predicate. </param>
        public static void RemoveIf<T>(this LinkedList<T> list, Func<T, bool> predicate)
        {
            if (list.Count == 0)
            {
                return;
            }
            var curr = list.First;
            while (curr != null)
            {
                var next = curr.Next;
                if (predicate(curr.Value))
                {
                    list.Remove(curr);
                }
                curr = next;
            }
        }

        /// <summary>
        /// Binary search that does not require an item in a sorted List.
        /// Throws exception if key doesn't exist.
        /// </summary>
        /// <typeparam name="T"> Type of element. </typeparam>
        /// <typeparam name="TKey"> Type of key associated with elements. </typeparam>
        /// <param name="list"> A list. </param>
        /// <param name="keySelector"> Key selector. </param>
        /// <param name="key"> Key to search for. </param>
        /// <returns> Index of the first element with matching key. If not found, returns complement of index of the first element that has a larger key. </returns>
        public static int BinarySearch<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key) where TKey : IComparable<TKey>
        {
            if (list.Count == 0)
            {
                throw new InvalidOperationException("Item not found");
            }
            int min = 0;
            int max = list.Count - 1;
            while (min < max)
            {
                int mid = min + ((max - min) / 2);
                T midItem = list[mid];
                TKey midKey = keySelector(midItem);
                int comp = midKey.CompareTo(key);
                if (comp < 0)
                {
                    min = mid + 1;
                }
                else if (comp > 0)
                {
                    max = mid - 1;
                }
                else
                {
                    return mid;
                }
            }

            if (min == max)
            {
                int cmp = keySelector(list[min]).CompareTo(key);
                if (cmp == 0)
                    return min;
                else if (cmp > 0)
                    return ~min;
                else
                    return ~(min + 1);
            }

            throw new InvalidOperationException("Item not found");
        }

        /// <summary>
        /// Search a linked list for a node containing value that satisfies predicate.
        /// </summary>
        /// <typeparam name="T"> Type of element. </typeparam>
        /// <param name="list"> This linked list. </param>
        /// <param name="predicate"> Search predicate. </param>
        /// <returns> The first node containing the matching element. </returns>
        public static LinkedListNode<T> Find<T>(this LinkedList<T> list, Func<T, bool> predicate)
        {
            for (var it = list.First; it != null;)
            {
                var next = it.Next;
                if (predicate(it.Value))
                {
                    return it;
                }
                it = next;
            }
            return null;
        }

        /// <summary>Appends the value to the list mapped by the key, creates the list if not already.</summary>
        /// <typeparam name="K">Key type.</typeparam>
        /// <typeparam name="V">Value type.</typeparam>
        /// <param name="map">The map of keys to lists of values.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to append.</param>
        public static void AppendOrNewList<K, V>(this Dictionary<K, List<V>> map, K key, V value)
        {
            List<V> values;
            if (!map.TryGetValue(key, out values))
            {
                values = new List<V>();
                map.Add(key, values);
            }
            values.Add(value);
        }
    }

    public class TempValue<T> : IDisposable
    {
        private T prevVal;
        private Action<T> set;

        public TempValue(T newVal, Action<T> set, Func<T> get)
        {
            prevVal = get();
            set(newVal);
            this.set = set;
        }

        void IDisposable.Dispose()
        {
            set(prevVal);
        }
    }

    #region Exceptions

    /// <summary>
    /// Stupid exceptions that are not caused by me.
    /// </summary>
    public class KnownBug : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KnownBug"/> class.
        /// </summary>
        /// <param name="msg"> The message. </param>
        public KnownBug(string msg = null) : base(msg)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownBug"/> class.
        /// </summary>
        /// <param name="msg"> The message. </param>
        /// <param name="inner"> The inner exception. </param>
        public KnownBug(string msg, Exception inner) : base(msg, inner)
        {
        }
    }

    /// <summary>
    /// Indicates a certain object or a field of the object lacks required attribute.
    /// </summary>
    /// <typeparam name="T"> Type of attribute. </typeparam>
    public class MissingAttributeException<T> : Exception where T : Attribute
    {
        /// <summary>
        /// Type of object that lacks the attribute.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Name of the field that lacks the attribute.
        /// </summary>
        public readonly string FieldName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingAttributeException{T}"/> class.
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <param name="fieldName"> Name of the field. </param>
        public MissingAttributeException(Type type, string fieldName = "")
        {
            Type = type;
            FieldName = fieldName;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string msg = string.Format("Type {0} requires {1}", Type.Name, typeof(T).Name);
            if (FieldName != null)
            {
                msg += " on field " + FieldName;
            }
            return msg;
        }
    }

    #endregion
}
