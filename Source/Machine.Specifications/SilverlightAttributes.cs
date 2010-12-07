using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

#if SILVERLIGHT
namespace System
{
    /// <summary>
    /// Marker attribute to make code compilable to Silverlight
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Enum
      | AttributeTargets.Struct | AttributeTargets.Class,
      Inherited = false)]
    public sealed class SerializableAttribute : Attribute { }

    namespace Security
    {
        [AttributeUsageAttribute(AttributeTargets.Assembly, AllowMultiple = false,
            Inherited = false)]
        public sealed class AllowPartiallyTrustedCallersAttribute : Attribute { }
    }

    namespace Runtime.Serialization
    {
        public sealed class SerializationInfo { }
    }

    namespace Diagnostics
    {

        /// <summary>
        /// Stopwatch is used to measure the general performance of Silverlight functionality. Silverlight
        /// does not provide a high resolution timer as is available in many operating systems,
        /// so the resolution of this timer is limited to milliseconds. This class is best used to measure
        /// the relative performance of functions over many iterations.
        /// </summary>
        public sealed class Stopwatch : IDisposable
        {
            private long _startTick;
            private long _elapsed;
            private bool _isRunning;
            private string _name;
            private Action<Stopwatch> _startAction;
            private Action<Stopwatch> _stopAction;

            public Stopwatch() : this("stopwatch", null, null) { }

            /// <summary>
            /// Creates an instance of the Stopwatch class and starts the timer. By
            /// providing a value to the name parameter, the Debug Console automatically
            /// will include the current values when the timer was started and stopped with
            /// this name.
            /// </summary>
            /// <param name="name"></param>
            public Stopwatch(string name)
                : this(name, WriteStart, WriteResults) { } 

            /// <summary>
            /// Creates an instance of the Stopwatch class and starts the timer.
            /// </summary>
            /// <param name="stopAction">Action to take when the Stop method is called.</param>
            public Stopwatch(Action<Stopwatch> stopAction)
                :this(null, stopAction)
            {
            }

            /// <summary>
            /// Creates an instance of the Stopwatch class and starts the timer.
            /// </summary>
            /// <param name="startAction">Action to take when the timer starts.</param>
            /// <param name="stopAction">Action to take when the Stop method is called.</param>
            public Stopwatch(Action<Stopwatch> startAction,
                Action<Stopwatch> stopAction)
                :this(null, startAction, stopAction)
            {
            }

            /// <summary>
            /// Creates an instance of the Stopwatch class and starts the timer.
            /// </summary>
            /// <param name="name">Name of this stop watch (used as output)</param>
            /// <param name="startAction">Action to take when the timer starts.</param>
            /// <param name="stopAction">Action to take when the Stop method is called.</param>
            public Stopwatch(string name,
                Action<Stopwatch> startAction,
                Action<Stopwatch> stopAction)
            {
                _name = name;
                _startAction = startAction;
                _stopAction = stopAction;
                Start();
            }

            public string Name
            {
                get { return _name; }
            }

            /// <summary>
            /// Completely resets and deactivates the timer.
            /// </summary>
            public void Reset()
            {
                _elapsed = 0;
                _isRunning = false;
                _startTick = 0;
            }

            /// <summary>
            /// Begins the timer.
            /// </summary>
            public void Start()
            {
                if (!_isRunning)
                {
                    _startTick = GetCurrentTicks();
                    _isRunning = true;
                    if (_startAction != null)
                    {
                        _startAction(this);
                    }
                }
            }

            /// <summary>
            /// Stops the current timer.
            /// </summary>
            public void Stop()
            {
                if (_isRunning)
                {
                    _elapsed += GetCurrentTicks() - _startTick;
                    _isRunning = false;
                    if (_stopAction != null)
                    {
                        _stopAction(this);
                    }
                }
            }

            /// <summary>
            /// Gets a value indicating whether the instance is currently recording.
            /// </summary>
            public bool IsRunning
            {
                get { return _isRunning; }
            }

            /// <summary>
            /// Gets the Elapsed time as a Timespan.
            /// </summary>
            public TimeSpan Elapsed
            {
                get { return TimeSpan.FromMilliseconds(ElapsedMilliseconds); }
            }

            /// <summary>
            /// Gets the Elapsed time as the total number of milliseconds.
            /// </summary>
            public long ElapsedMilliseconds
            {
                get { return GetCurrentElapsedTicks() / TimeSpan.TicksPerMillisecond; }
            }

            /// <summary>
            /// Gets the Elapsed time as the total number of ticks (which is faked
            /// as Silverlight doesn't have a way to get at the actual "Ticks")
            /// </summary>
            public long ElapsedTicks
            {
                get { return GetCurrentElapsedTicks(); }
            }

            private long GetCurrentElapsedTicks()
            {
                return (long)(this._elapsed + (IsRunning ? (GetCurrentTicks() - _startTick) : 0));
            }

            private long GetCurrentTicks()
            {
                // TickCount: Gets the number of milliseconds elapsed since the system started.
                return Environment.TickCount * TimeSpan.TicksPerMillisecond;
            }

            #region IDisposable Members

            public void Dispose()
            {
                Stop();
            }

            #endregion

            private static void WriteStart(Stopwatch sw)
            {
                WriteStartInternal(sw);
            }

            // This is not called in a Release build
            [Conditional("DEBUG")]
            private static void WriteStartInternal(Stopwatch sw)
            {
                Debug.WriteLine("BEGIN\t{0}", sw._name);

            }

            private static void WriteResults(Stopwatch sw)
            {
                WriteResultsInternal(sw);
            }

            // This is not called in a Release build
            [Conditional("DEBUG")]
            private static void WriteResultsInternal(Stopwatch sw)
            {
                Debug.WriteLine("END\t{0}\t{1}", sw._name, sw.ElapsedMilliseconds);
            }
        }

    }
}
#endif