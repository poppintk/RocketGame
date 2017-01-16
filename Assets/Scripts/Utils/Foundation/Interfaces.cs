using System;
using System.Collections.Generic;

namespace TX
{
    public interface IDeepCloneable
    {
        object DeepClone();
    }

    public interface IDeepCloneable<T> : IDeepCloneable
    {
        new T DeepClone();
    }

    public abstract class DeepCloneableBase<T> : IDeepCloneable<T>
    {
        public abstract T DeepClone();

        object IDeepCloneable.DeepClone()
        {
            return DeepClone();
        }
    }
}
