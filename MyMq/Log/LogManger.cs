using System;

namespace MyMq
{
    /// <summary>
    /// Log 管理
    /// </summary>
    class LogManger
    {
        public static void Info(object info, Type type)
        {
            log4net.LogManager.GetLogger(type).Info(info);
        }

        public static void Debug(object info, Type type)
        {
            log4net.LogManager.GetLogger(type).Debug(info);
        }
        public static void Warn(object info, Type type)
        {
            log4net.LogManager.GetLogger(type).Warn(info);
        }
        public static void Fatal(object info, Type type)
        {
            log4net.LogManager.GetLogger(type).Fatal(info);
        }
        public static void Error(object info, Type type)
        {
            log4net.LogManager.GetLogger(type).Error(info);
        }
    }
}
