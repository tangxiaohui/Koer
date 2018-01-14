using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateSystem.Log
{
    public delegate void DefaultLog(string msg);
    public delegate void WarnLog(string msg);
    public delegate void ErrorLog(string msg);

    public class UpdateLog
    {
        public static int LOG_LEVEL = 0;
        private static int INFO_LEVEL = 0;
        private static int DEBUG_LEVEL = 1;
        private static int WARN_LEVEL = 2;
        private static int ERROR_LEVEL = 3;
        private static StringBuilder _builder = new StringBuilder();
        private static string _builderError = "";
        private static string _currentLog = "";

        private static DefaultLog _defaultLogFunc;
        private static WarnLog _warnLogFunc;
        private static ErrorLog _errorLogFunc;

        //注册日志回调
        public static void RegisterLogCallback(DefaultLog dLog, WarnLog wLog, ErrorLog eLog)
        {
            _defaultLogFunc = dLog;
            _warnLogFunc = wLog;
            _errorLogFunc = eLog;
        }

        public static void DEBUG_LOG(string log)
        {
            if (LOG_LEVEL <= DEBUG_LEVEL)
            {
                LOG("DEBUG_LOG: " + log);
                //System.Diagnostics.Trace.Write(log);
                if (_defaultLogFunc != null)
                {
                    _defaultLogFunc(log);
                }
            }
        }

        public static void ERROR_LOG(string log)
        {
            if (LOG_LEVEL <= ERROR_LEVEL)
            {
                LOG("ERROR_LOG: " + log);
                _currentLog = "ERROR_LOG: " + log;
                _builderError = _currentLog;

                //System.Diagnostics.Trace.Fail(log);

                if (_errorLogFunc != null)
                {
                    _errorLogFunc(log);
                }
            }
        }

        public static void EXCEPTION_LOG(Exception ex)
        {
            _builderError = ex.Message;
            //System.Diagnostics.Trace.Fail(ex.Message + "\n" + ex.StackTrace);
        }

        public static void INFO_LOG(string log)
        {
            if (LOG_LEVEL <= INFO_LEVEL)
            {
                LOG("INFO_LOG:  " + log);
                //System.Diagnostics.Trace.Write(log);
                if (_defaultLogFunc != null)
                {
                    _defaultLogFunc(log);
                }
            }
        }

        public static void WARN_LOG(string log)
        {
            if (LOG_LEVEL <= WARN_LEVEL)
            {
                LOG("WARN_LOG:  " + log);
                //System.Diagnostics.Trace.Write(log);
                if (_warnLogFunc != null)
                {
                    _warnLogFunc(log);
                }
            }
        }

        public static string getLog()
        {
            return _builder.ToString();
        }

        public static string getCurrentErrorLog()
        {
            return _builderError.ToString();
        }

        public static void init()
        {
            _builder.Remove(0, _builder.Length);
            _builderError = "";
        }

        private static void LOG(string log)
        {
            return;
            //string time = "" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            //_builder.Append(time + "    " + log).Append("\n");
            //if (log.Contains("ERROR_LOG"))
            //{
            //    //UpdateLog.ERROR_LOG(log);
            //    //Form1.setMsg(log + " count = " + _count);
            //    //Common.ULogFile.sharedInstance.LogError(string.Format("{0} {1}", time, log));
            //}
        }
    }
}
