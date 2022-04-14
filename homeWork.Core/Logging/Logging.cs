using System;
using System.IO;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;

namespace DotNetHomeWork.Core.Logging
{
    public static class Logging
    {
        public static ILog GetLog()
        {
            var consoleLog = new ConsoleLog();
            var fileLog = new FileLog(new FileLogSettings()
            {
                FilePath = Path.Combine("logs", $"log.{{RollingSuffix}}.{DateTime.Now:HH-mm-ss}.log"),
                RollingStrategy = new RollingStrategyOptions
                {
                    Type = RollingStrategyType.ByTime,
                    Period = RollingPeriod.Day,
                    MaxFiles = 5
                },
                FileOpenMode = FileOpenMode.Append,
            });

            return new CompositeLog(fileLog, consoleLog);
        }
    }
}
