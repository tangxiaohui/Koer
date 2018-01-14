using System;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Common
{
    public static class UDebug
    {

        delegate void LogDelegate(String msg);
        delegate void LogExceptionDelegate(Exception e);
        public delegate void AssertDelegate(bool condition, String msg);
        static LogDelegate _Log = NoAction;
        static LogDelegate _LogError = NoAction;
        static LogDelegate _LogWarning = NoAction;
        static LogExceptionDelegate _LogException = NoActionException;
        static AssertDelegate _AssertDelegate = null;

        public static bool _enabled = true;
        public static bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (_enabled)
                    {
                        _Log = UnityEngine.Debug.Log;
                        _LogError = UnityEngine.Debug.LogError;
                        _LogException = UnityEngine.Debug.LogException;
                        _LogWarning = UnityEngine.Debug.LogWarning;
                    }
                    else
                    {
                        _Log = NoAction;
                        _LogError = NoAction;
                        _LogWarning = NoAction;
                        _LogException = NoActionException;
                    }
                }
            }
        }

        public static AssertDelegate assertDelegate
        {
            set
            {
                _AssertDelegate = value;
            }
        }
        public static String GetCallStack(int skipFrame = 1)
        {
            StackTrace stackTrace = new StackTrace(skipFrame, true); // get call stack
            StackFrame[] stackFrames = stackTrace.GetFrames(); // get method calls (frames)
                                                               // write call stack method names
            String ret = String.Empty;
            var sb = new StringBuilder(200);
            for (int i = 0; i < stackFrames.Length; ++i)
            {
                var frame = stackFrames[i];
                sb.Append(frame.GetMethod());
                sb.Append("(at ");
                sb.Append(frame.GetFileName());
                sb.Append(": ");
                sb.Append(frame.GetFileLineNumber());
                sb.Append(")\n");
            }
            return sb.ToString();
        }

        public static int GetCallStackSize()
        {
            StackTrace stackTrace = new StackTrace(1, false); // get call stack
            return stackTrace.FrameCount;
        }

        public static String SafeFormat(String format, params object[] args)
        {
            if (format != null && args != null)
            {
                try
                {
                    return String.Format(format, args);
                }
                catch (Exception e)
                {
                    UDebug.LogException(e);
                }
            }
            return String.Empty;
        }

        [Conditional("_DEBUG_INFO"), Conditional("UNITY_EDITOR")]
        static public void Assert(bool condition, string assertString = null)
        {
            if (!condition)
            {
                if (_AssertDelegate != null)
                {
                    _AssertDelegate(condition, assertString);
                }
            }
        }

        #region Base logger
        [Conditional("_DEBUG_INFO")]
        public static void Log(String arg)
        {
            _Log(arg);
        }

        [Conditional("_DEBUG_INFO")]
        public static void Log<T>(T arg)
        {
            if (arg != null)
            {
                _Log(arg.ToString());
            }
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogError(String arg)
        {
            _LogError(arg);
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogError<T>(T arg)
        {
            if (arg != null)
            {
                _LogError(arg.ToString());
            }
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogWarning(String arg)
        {
            _LogWarning(arg);
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogWarning<T>(T arg)
        {
            if (arg != null)
            {
                _LogWarning(arg.ToString());
            }
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogException(Exception e)
        {
            _LogException(e);
        }
        #endregion

        #region LogEx
        [Conditional("_DEBUG_INFO")]
        public static void LogEx(String arg)
        {
            Log(arg);
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogEx<T>(T arg)
        {
            if (arg != null)
            {
                Log(arg.ToString());
            }
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogEx<T>(String format, T arg)
        {
            Log(SafeFormat(format, arg));
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogEx<T1, T2>(String format, T1 arg1, T2 arg2)
        {
            Log(SafeFormat(format, arg1, arg2));
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogEx<T1, T2, T3>(String format, T1 arg1, T2 arg2, T3 arg3)
        {
            Log(SafeFormat(format, arg1, arg2, arg3));
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogEx(String format, params object[] args)
        {
            Log(SafeFormat(format, args));
        }
        #endregion

        #region LogErrorEx
        [Conditional("_DEBUG_INFO")]
        public static void LogErrorEx(String arg)
        {
            LogError(arg);
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogErrorEx<T>(T arg)
        {
            if (arg != null)
            {
                LogError(arg.ToString());
            }
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogErrorEx<T>(String format, T arg)
        {
            LogError(SafeFormat(format, arg));
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogErrorEx<T1, T2>(String format, T1 arg1, T2 arg2)
        {
            LogError(SafeFormat(format, arg1, arg2));
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogErrorEx<T1, T2, T3>(String format, T1 arg1, T2 arg2, T3 arg3)
        {
            LogError(SafeFormat(format, arg1, arg2, arg3));
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogErrorEx(String format, params object[] args)
        {
            LogError(SafeFormat(format, args));
        }
        #endregion

        #region LogWarningEx
        [Conditional("_DEBUG_INFO")]
        public static void LogWarningEx(String arg)
        {
            LogWarning(arg);
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogWarningEx<T>(T arg)
        {
            LogWarning(arg.ToString());
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogWarningEx<T>(String format, T args)
        {
            LogWarning(SafeFormat(format, args));
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogWarningEx<T1, T2>(String format, T1 arg1, T2 arg2)
        {
            LogWarning(SafeFormat(format, arg1, arg2));
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogWarningEx<T1, T2, T3>(String format, T1 arg1, T2 arg2, T3 arg3)
        {
            LogWarning(SafeFormat(format, arg1, arg2, arg3));
        }

        [Conditional("_DEBUG_INFO")]
        public static void LogWarningEx(String format, params object[] args)
        {
            LogWarning(SafeFormat(format, args));
        }
        #endregion

        [Conditional("_DEBUG_INFO")]
        public static void BreakPoint()
        {
#if UNITY_EDITOR
            UnityEngine.Debug.DebugBreak();
#else
          System.Diagnostics.Debugger.Break();
#endif
        }

        private static void NoAction(String msg)
        {
#if !UNITY_EDITOR
          Console.WriteLine( msg );
#endif
        }

        private static void NoActionException(Exception e)
        {
#if !UNITY_EDITOR
          Console.WriteLine( e.ToString() );
#endif
        }

        public static void Print(String arg)
        {
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
          UnityEngine.Debug.Log( arg );
#else
            Console.WriteLine(arg);
#endif
        }

        public static void Print<T>(T arg)
        {
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
          UnityEngine.Debug.Log( arg.ToString() );
#else
            Console.WriteLine(arg.ToString());
#endif
        }

        public static void Print<T>(String format, T arg)
        {
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
          UnityEngine.Debug.Log( SafeFormat( format, arg ) );
#else
            Console.WriteLine(SafeFormat(format, arg));
#endif
        }

        public static void Print<T1, T2>(String format, T1 arg1, T2 arg2)
        {
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
          UnityEngine.Debug.Log( SafeFormat( format, arg1, arg2 ) );
#else
            Console.WriteLine(SafeFormat(format, arg1, arg2));
#endif
        }

        public static void Print<T1, T2, T3>(String format, T1 arg1, T2 arg2, T3 arg3)
        {
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
          UnityEngine.Debug.Log( SafeFormat( format, arg1, arg2, arg3 ) );
#else
            Console.WriteLine(SafeFormat(format, arg1, arg2, arg3));
#endif
        }

        public static void Print(String format, params object[] args)
        {
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
          UnityEngine.Debug.Log( SafeFormat( format, args ) );
#else
            Console.WriteLine(format, args);
#endif
        }

        static UDebug()
        {
            enabled = true;
            //enabled = false;
            if (UnityEngine.Debug.isDebugBuild)
            {
                if (enabled)
                {
                    _Log = UnityEngine.Debug.Log;
                    _LogError = UnityEngine.Debug.LogError;
                    _LogException = UnityEngine.Debug.LogException;
                    _LogWarning = UnityEngine.Debug.LogWarning;
                }
            }
        }
    }
}