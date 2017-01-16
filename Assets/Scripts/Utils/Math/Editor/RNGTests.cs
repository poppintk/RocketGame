using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace TX.Test
{
    public class RNGTests
    {
        [Test]
        [Description("Test that the state of RNG can be replicated.")]
        public void StateTest()
        {
            RNG original = new RNG(0);
            for (int i = 0; i < 10; i++)
                original.NextUInt();

            RNG.State state = original.state;
            RNG resumed = new RNG(state);

            List<uint> originalValues = new List<uint>();
            List<uint> resumedValues = new List<uint>();
            for (int i = 0; i < 10; i++)
            {
                originalValues.Add(original.NextUInt());
                resumedValues.Add(resumed.NextUInt());
            }

            CollectionAssert.AreEquivalent(originalValues, resumedValues);
        }
    }
}
