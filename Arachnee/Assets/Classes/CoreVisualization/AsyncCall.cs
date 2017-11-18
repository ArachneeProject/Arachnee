using System;
using System.Collections;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Classes.CoreVisualization
{
    /// <summary>
    /// Allows to call a method that takes some time to retrieve some data, and then use this data to build a Unity object.
    /// </summary>
    /// <typeparam name="TData">Type of the data to retrieve.</typeparam>
    /// <typeparam name="TUnityObject">Type of Unity object.</typeparam>
    public class AsyncCall<TData, TUnityObject> where TUnityObject : Object
    {
        private readonly object _lock = new object();

        private readonly Func<TData> _getDataFunc;
        private readonly Func<TData, TUnityObject> _createObjectFunc;

        private TData _objectData;

        private TUnityObject _result;

        [CanBeNull]
        public TUnityObject Result
        {
            get
            {
                TUnityObject result;
                lock (_lock)
                {
                    result = _result;
                }

                return result;
            }
        }

        private bool _isRunning;

        public bool IsRunning
        {
            get
            {
                bool running;
                lock (_lock)
                {
                    running = _isRunning;
                }

                return running;
            }

            set
            {
                lock (_lock)
                {
                    _isRunning = value;
                }
            }
        }

        /// <param name="getObjectData">The method that should be run asynchronously in order to retrieve the data (it must not contain any call to the Unity API).</param>
        /// <param name="createObject">The method that should run on the Unity main thread in order to create the object with the retrieved data (it can contain some calls to the Unity API).</param>
        public AsyncCall(Func<TData> getObjectData, Func<TData,TUnityObject> createObject)
        {
            _getDataFunc = getObjectData;
            _createObjectFunc = createObject;
        }
        
        public IEnumerator Execute()
        {
            Restart();

            var thread = new Thread(() =>
            {
                _objectData = _getDataFunc.Invoke();
            });

            thread.Start();

            yield return new WaitWhile(() => thread.IsAlive);

            StoreResult(_createObjectFunc(_objectData));
        }

        private void StoreResult(TUnityObject result)
        {
            lock (_lock)
            {
                _result = result;
                _isRunning = false;
            }
        }

        private void Restart()
        {
            lock (_lock)
            {
                _result = null;
                _isRunning = true;
            }
        }
    }
}