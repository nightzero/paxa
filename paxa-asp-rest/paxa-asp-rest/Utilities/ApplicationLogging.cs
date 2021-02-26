using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace paxa.Utilities
{
    public class ApplicationLogging
    {
        private static ILoggerFactory _Factory = null;

        public static ILoggerFactory LoggerFactoryCreator
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = LoggerFactory.Create(builder => builder.AddConsole());
                }
                return _Factory;
            }
            set { _Factory = value; }
        }

        public static ILogger CreateLogger() => LoggerFactoryCreator.CreateLogger("Debug");
    }
}
