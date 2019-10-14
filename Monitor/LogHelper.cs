using System;
using System.Collections.Generic;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace awaken
{
    public static class LogHelper
    {
        public static void WriteInfo(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Info(msg);
        }

        public static void WriteInfo(string msg)
        {
            WriteInfo(typeof(LogHelper), msg);
        }

        public static void Log(string msg)
        {
            WriteInfo(typeof(LogHelper), msg);
        }

        public static void WriteError(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error(msg);
        }

        public static void WriteError(Type t, Exception ex)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error(ex.Message, ex);
        }

        public static void Debug(string msg)
        {
            Log(msg);
        }
    }
}