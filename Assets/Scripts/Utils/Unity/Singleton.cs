using UnityEngine;

namespace TX.Game
{
    /// <summary>
    /// Base class for all singleton managers.
    /// </summary>
    /// <typeparam name="T"> Type of singleton manager. </typeparam>
    public class Singleton<T> : BaseBehaviour where T : BaseBehaviour
    {
        protected static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(T) +
                           " is needed in the scene, but there is none.");
                    }
                }

                return instance;
            }
        }
    }
}
