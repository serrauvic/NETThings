using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace ConcurrentQueue
{
    /// <summary>
    /// Simple implementation of a thread safe concurrent FIFO queue, with basic .NET infrastructure.
    /// </summary>
    /// <typeparam name="T">Queue elements type</typeparam>
    public class BlockingQueue<T> where T: class, new()
    {
        /// <summary>
        /// Thread synchronization.
        /// </summary>
        private readonly object _sync;
        /// <summary>
        /// Thread signaling.
        /// </summary>
        private ManualResetEvent _singal;
        /// <summary>
        /// Objects queue.
        /// </summary>
        private List<T> _queue;
        /// <summary>
        /// Cancel execution.
        /// </summary>
        private CancellationTokenSource _cancel;

        public BlockingQueue()
        {
            _sync   = new object();
            _queue  = new List<T>();
            _singal = new ManualResetEvent(false);
            _cancel = new CancellationTokenSource();
        }
        public void Shutdown()
        {
            _cancel.Cancel();
            _queue.Clear();
        }
        public T Take()
        {
            // Prepare cancelation token.
            CancellationToken cancelTkn = _cancel.Token; ;

            // While It's not shutting down work.
            while (!cancelTkn.IsCancellationRequested)
            {
                // Wait until signal comes (event signal, cancelation).
                // At this point current execution thread is blocked.
                WaitHandle.WaitAny(new[] { _singal, cancelTkn.WaitHandle });

                // Guard against shutting down.
                if (cancelTkn.IsCancellationRequested)
                    return null;

                T element = default(T);

                // Enter restricted area.
                lock (_sync)
                {
                    // If queue has elements....
                    if (_queue.Count > 0)
                    {
                        // Gets the first element
                        element = _queue.First();
                        // Remove the element from the queue.
                        _queue.RemoveAt(0);

                    } else
                    {
                        // Prepare next while execution.
                        _singal.Reset();
                    }
                } // Exit lcok
                // Return element.
                return element;
            }
            // on shutting down...
            return default(T);
        }
        /// <summary>
        /// Add T element to the end of the queue.
        /// </summary>
        /// <param name="element"></param>
        public void Add(T element)
        {
            // Enter restricted area.
            lock (_sync)
            {
                // Add element.
                _queue.Add(element);

                // "wake up" thread.
                _singal.Set();
            } // Exit lcok
        }
    }
}
