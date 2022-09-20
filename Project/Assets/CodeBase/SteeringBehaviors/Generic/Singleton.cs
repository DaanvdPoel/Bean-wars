using UnityEngine;

namespace Generic
{
    /// <summary>
    /// Generic singleton class managing a MonoBehavior.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// The one and only MonoBehavior instance managed by this singleton.
        /// </summary>
        private static T m_instance;

        /// <summary>
        /// True if we are quitting hence we do not return the MonoBehavior instance anymore.
        /// </summary>
        private static bool m_applicationIsQuitting = false;

        /// <summary>
        /// True if this Singleton has DontDestroyOnLoad() enabled.
        /// </summary>
        private readonly bool m_dontDestroyOnLoad;

        /// <summary>
        /// Constructor to allow enabling or disabling DontDestroyOnLoad() for a specific Singleton.
        /// </summary>
        /// <param name="dontDestroyOnLoad">True to enable DontDestroyOnLoad() for this Singleton.</param>
        public Singleton(bool dontDestroyOnLoad = true)
        {
            m_dontDestroyOnLoad = dontDestroyOnLoad;
        }

        /// <summary>
        /// Get the one and only MonoBehavior instance managed by this singleton.
        /// </summary>
        /// <returns>The single MonoBehavior instance.</returns>
        public static T GetInstance()
        {
            // stop returning the instance if we are quitting
            if (m_applicationIsQuitting) 
                return null; 

            // create the instance if it is not available yet
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();
                if (m_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    m_instance = obj.AddComponent<T>();
                }
            }

            // return the instance
            return m_instance;
        }

        /* IMPORTANT!!! To use Awake in a derived class you need to do it this way
         * protected override void Awake()
         * {
         *     base.Awake();
         *     //Your code goes here
         * }
         * */

        protected virtual void Awake()
        {
            // make sure to use this as the instance
            if (m_instance == null)
            {
                m_instance = this as T;

                // make sure to not destroy on load is enabled if requested.
                if (m_dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else if (m_instance != this as T)
            {
                Destroy(gameObject);
            }
            else 
            {
                // make sure to not destroy on load is enabled if requested.
                if (m_dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject); 
            }
        }

        private void OnApplicationQuit()
        {
            // remember we are quitting
            m_applicationIsQuitting = true;
        }
    }
}