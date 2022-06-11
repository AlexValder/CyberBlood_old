using System;
using System.IO;
using Godot;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace CyberBlood.Scripts.Utils.GodotSink {
    public class GodotSink: ILogEventSink {
        private readonly MessageTemplateTextFormatter _formatter;
        private const string DEFAULT_TEMPLATE =
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        public GodotSink(IFormatProvider formatProvider, string template = DEFAULT_TEMPLATE) {
            if (template == null) {
                throw new ArgumentNullException(nameof(template));
            }

            _formatter = new MessageTemplateTextFormatter(template, formatProvider);
        }

        public void Emit(LogEvent logEvent) {
            using var writer = new StringWriter();
            _formatter.Format(logEvent, writer);
            var message = writer.ToString();

            if (logEvent.Level >= LogEventLevel.Error) {
                GD.PrintErr(message);
            } else {
                GD.Print(message);
            }
        }
    }
}