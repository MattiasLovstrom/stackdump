using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Stackdump
{
    public class Debugger
    {
        private readonly int _processId;
        
        public Debugger(int processId)
        {
            _processId = processId;
        }

        public IEnumerable<Regex> IncludeRegexs { get; set; } = new Regex[0];

        public IEnumerable<Regex> ExcludeRegexs { get; set; } = new Regex[0];

        public StringBuilder DumpThreads()
        {
            var formattedThreads = new StringBuilder();
            try
            {
                using var dataTarget = DataTarget.AttachToProcess(_processId, true);
                var runtimeInfo = dataTarget.ClrVersions[0];
                var process = runtimeInfo.CreateRuntime();

                foreach (var thread in process.Threads)
                {
                    formattedThreads
                        .Append(Filter(DumpThread(thread)))
                        .AppendLine();
                }
            }
            catch (Exception ex)
            {
                formattedThreads.AppendFormat("  Stackdump error ({0})", ex.Message);
            }

            return formattedThreads;
        }

        private StringBuilder DumpThread(ClrThread thread)
        {
            var formattedThread = new StringBuilder();
            formattedThread.AppendFormat("OS Thread Id:{0}", thread.OSThreadId).AppendLine();
            var maxLines = 1024;
            foreach (var frame in thread.EnumerateStackTrace())
            {
                formattedThread.Append(DumpFrame(frame));
                maxLines--;
                if (maxLines >= 0) continue;

                formattedThread.Append($"...\n[Maximum number of calls reached");
                break;
            }

            return formattedThread;
        }

        private static StringBuilder DumpFrame(ClrStackFrame frame)
        {
            if (frame == null) return null;

            var formattedFrame = new StringBuilder();

            try
            {
                var methodName = frame.Method?.Type?.Name;
                if (string.IsNullOrEmpty(methodName)) return null;

                formattedFrame.AppendFormat($"  {methodName}(");
                var first = true;
                foreach (var field in frame.Method?.Type?.Fields)
                {
                    if (!first)
                    {
                        formattedFrame.Append(", ");
                    }

                    formattedFrame.Append(field?.Type?.Name ?? "N/A");
                    first = false;
                }

                formattedFrame.AppendLine(")");
            }
            catch (Exception ex)
            {
                formattedFrame.Append($"  Stackdump error ({ex.Message})");
            }

            return formattedFrame;
        }

        private StringBuilder Filter(StringBuilder dumpThread)
        {
            if (IncludeRegexs.Any())
            {
                if (!IncludeRegexs.Any(regex => regex.IsMatch(dumpThread.ToString())))
                {
                    return null;
                }
            }

            if (ExcludeRegexs.Any())
            {
                if (ExcludeRegexs.Any(regex => regex.IsMatch(dumpThread.ToString())))
                {
                    return null;
                }
            }

            return dumpThread;
        }
    }
}