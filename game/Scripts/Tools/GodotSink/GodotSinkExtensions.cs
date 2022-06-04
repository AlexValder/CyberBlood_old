using System;
using Serilog;
using Serilog.Configuration;

namespace CyberBlood.Scripts.Tools.GodotSink {
    public static class GodotSinkExtensions {
        public static LoggerConfiguration GodotSink(
            this LoggerSinkConfiguration loggerConfiguration,
            IFormatProvider formatProvider = null
        ) {
            return loggerConfiguration.Sink(new GodotSink(formatProvider));
        }

        public static LoggerConfiguration GodotSink(
            this LoggerSinkConfiguration loggerConfiguration,
            string outputTemplate,
            IFormatProvider formatProvider = null
        ) {
            return loggerConfiguration.Sink(new GodotSink(formatProvider, outputTemplate));
        }
    }
}
