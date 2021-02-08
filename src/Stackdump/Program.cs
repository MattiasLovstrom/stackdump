using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Stackdump
{
    public class Dumper
    {
        public static readonly string[] AcceptedArguments = { "e", "i" };

        private static void Main(string[] args)
        {
            var arguments = new ArgumentParser(AcceptedArguments, args);

            if (!arguments.HasProcessId)
            {
                PrintUsage();
                PrintProcesses();
                Environment.Exit(1);
            }

            try
            {
                var output = new StringBuilder();
                var debugger = new Debugger(arguments.ProcessId)
                {
                    IncludeRegexs = GenerateRegexs(arguments.GetArgument("i")),
                    ExcludeRegexs = GenerateRegexs(arguments.GetArgument("e"))
                };
                output.Append($"{DateTime.Now}\t{Environment.MachineName}\tpid='{arguments.ProcessId}");
                output.AppendLine();
                output.Append(debugger.DumpThreads());

                Console.Out.WriteLine((object)output);
            }
            catch (Exception ex)
            {
                Output.HandleException(ex, "stackdump.exe", arguments,
                    PrintUsage);
            }

            Environment.Exit(0);
        }

        private static void PrintProcesses()
        {
            Console.Out.WriteLine("Active .Net processes for current user:");
            ProcessInspector.ListProcesses();
        }

        private static IEnumerable<Regex> GenerateRegexs(IEnumerable<string> inputs)
        {
            return inputs.Select(input =>
                new Regex(input, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)).ToList();
        }

        private static void PrintUsage()
        {
            Console.Out.Write("Stackdump");
            Console.Out.WriteLine(" for .Net core by Mattias Lövström");
            Console.Out.WriteLine("Usage:");
            Console.Out.WriteLine(
                "StackDump   [/i:<regular expression to include threads with>] [/e:<regular expression to exclude threads with>] <process id>");
        }
    }
}