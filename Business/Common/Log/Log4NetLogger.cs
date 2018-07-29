using System;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Logging;
using ILogger = log4net.Core.ILogger;

namespace TestCore.Business.Common.Log
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _log;

        private readonly XmlElement _xmlElement;

        public Log4NetLogger(string name, XmlElement xmlElement)
        {
            Name = name;
            _xmlElement = xmlElement;
            Repository = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(Hierarchy));
            _log = LogManager.GetLogger(Repository.Name, name);
            XmlConfigurator.Configure(Repository, xmlElement);
        }

        public void Log(Type callerStackBoundaryDeclaringType, Level level, object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Log(LoggingEvent logEvent)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabledFor(Level level)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
        public ILoggerRepository Repository { get; }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return _log.IsFatalEnabled;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return _log.IsDebugEnabled;
                case LogLevel.Error:
                    return _log.IsErrorEnabled;
                case LogLevel.Information:
                    return _log.IsInfoEnabled;
                case LogLevel.Warning:
                    return _log.IsWarnEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            string message = null;
            message = formatter(state, exception);
            if (string.IsNullOrEmpty(message) && exception == null) return;
            switch (logLevel)
            {
                case LogLevel.Critical:
                    _log.Fatal(message);
                    break;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    _log.Debug(message);
                    break;
                case LogLevel.Error:
                    _log.Error(message);
                    break;
                case LogLevel.Information:
                    _log.Info(message);
                    break;
                case LogLevel.Warning:
                    _log.Warn(message);
                    break;
                case LogLevel.None:
                    break;
                default:
                    _log.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                    _log.Info(message, exception);
                    break;
            }
        }
    }
}