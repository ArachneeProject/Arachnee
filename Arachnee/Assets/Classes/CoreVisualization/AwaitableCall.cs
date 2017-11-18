using System;
using System.Collections;

namespace Assets.Classes.CoreVisualization
{
    /// <summary>
    /// Allows to wait for a unity object to be retrieved.
    /// </summary>
    public class AwaitableCall<TResult>
    {
        private readonly Func<IEnumerator> _asyncFunc;
        private readonly Func<TResult> _getResultFunc;

        public AwaitableCall(Func<IEnumerator> asyncFunc, Func<TResult> getResultFunc)
        {
            _asyncFunc = asyncFunc;
            _getResultFunc = getResultFunc;
        }

        public TResult Result { get; private set; }
        
        public IEnumerator Await()
        {
            yield return _asyncFunc.Invoke();
            Result = _getResultFunc.Invoke();
        }
    }
}