using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization
{
    /// <summary>
    /// Allows to call a method that takes some time to retrieve a result, and use this result elsewhere, without blocking the main Unity Thread.
    /// </summary>
    /// <typeparam name="TResult">The type of result the  method returns.</typeparam>
    public class AsyncCall<TResult>
    {
        private readonly object _lock = new object();
        private bool _isFirstCall = true;
        
        private TResult _result;

        /// <summary>
        /// Executes the given GetResult method to retrieve a <see cref="TResult"/>, then executes the given ApplyResult method with the retrieved <see cref="TResult"/>.<br/>
        /// GetResult method is run asynchronously and shouldn't contain any call to the Unity API.<br/>
        /// ApplyResult method is run synchronously and can contain some call to the Unity API.
        /// </summary>
        /// <param name="getResult"></param>
        /// <param name="applyResult"></param>
        public IEnumerator Execute(Func<TResult> getResult, Action<TResult> applyResult)
        {
            bool funcWasNeverRun;
            lock (_lock)
            {
                funcWasNeverRun = _isFirstCall;
                _isFirstCall = false;
            }

            if (!funcWasNeverRun)
            {
                Logger.LogError("Function has already been called once.");
                yield break;
            }

            var thread = new Thread(() =>
            {
                _result = getResult.Invoke();
            });

            thread.Start();

            yield return new WaitWhile(() => thread.IsAlive);

            applyResult(_result);
        }
    }
}