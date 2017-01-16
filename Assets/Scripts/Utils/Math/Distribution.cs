using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TX
{
    /// <summary>
    /// Generic weighted selection.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    public class WeightedSelection<T>
    {
        public List<T> Items;
        public List<int> WeightCDF;
        public int TotalWeights;
        private readonly Func<T, int> GetWeight;

        public WeightedSelection(IEnumerable<T> items, Func<T, int> getWeight)
        {
            GetWeight = getWeight;

            Items = new List<T>();
            WeightCDF = new List<int>();
            foreach (T item in items)
            {
                int w = getWeight(item);
                if (w > 0)
                {
                    Items.Add(item);
                    TotalWeights += w;
                    WeightCDF.Add(TotalWeights);
                }
            }

            if (TotalWeights == 0)
            {
                // usually indicates building is too small
                throw new ArgumentException("Total weight is zero");
            }
        }

        /// <summary>Randomly select one item.</summary>
        /// <param name="rng">The RNG.</param>
        /// <returns>Index of the selected item.</returns>
        public int Select(RNG rng)
        {
            int sample = GetSample(rng);
            return Select(sample);
        }

        /// <summary>Randomly select one item.</summary>
        /// <param name="rng">The RNG.</param>
        /// <returns>Selected item.</returns>
        public T SelectItem(RNG rng)
        {
            int index = Select(rng);
            return Items[index];
        }

        /// <summary>Selects item based on specified sample.</summary>
        /// <param name="sample">The sample.</param>
        /// <returns>Index of the selected item.</returns>
        public int Select(int sample)
        {
            int index = WeightCDF.BinarySearch(sample);
            if (index < 0)
                index = ~index;
            return index;
        }

        /// <summary>Randomly select one item based on specified sample.</summary>
        /// <param name="sample">The sample in range [1, sumWeights]. </param>
        /// <returns>Selected item.</returns>
        public T SelectItem(int sample)
        {
            int index = Select(sample);
            return Items[index];
        }

        /// <summary>Removes the item at the given index.</summary>
        /// <param name="i">The index.</param>
        public void Remove(int i)
        {
            T toRemove = Items[i];
            int w = GetWeight(toRemove);
            for (int j = i; j < Items.Count; j++)
                WeightCDF[j] -= w;
            TotalWeights -= w;
            Items.RemoveAt(i);
            WeightCDF.RemoveAt(i);
        }

        protected int GetSample(RNG rng)
        {
            // must be 1-based sample
            return rng.NextIntRange(TotalWeights) + 1;
        }
    }
}
