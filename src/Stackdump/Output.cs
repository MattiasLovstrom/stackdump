using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Stackdump
{
    public class Output
    {
        public delegate void PrintUsageDelegate();

        public static void HandleException(
            Exception ex,
            string programName,
            ArgumentParser arguments,
            PrintUsageDelegate printUsage)
        {
            switch (ex)
            {
                case Win32Exception _:
                    Console.Out.WriteLine("Error:");
                    Console.Out.WriteLine("Can't find process with process Id='" + arguments.ProcessId + "'");
                    Console.Out.WriteLine(
                        "Run {0} as an administrator or as the same user that execute the process to debug has.",
                        programName);
                    Console.Out.WriteLine(
                        "({0} will not see processes that runs under a build-in service account like Network service)",
                        programName);
                    Console.Out.WriteLine();
                    Console.Out.WriteLine("To list the processes that {0} sees execute it without any arguments.",
                        programName);
                    Environment.Exit(3);
                    break;
                case NullReferenceException _ when ex.StackTrace.Contains("MDbgProcess.Detach()"):
                    Console.Out.WriteLine("The process to debug has been stopped");
                    break;
                case COMException externalException when externalException.ErrorCode == -2146233554:
                    Console.Out.WriteLine("Unable to attach to the Process. A debugger s already attached.");
                    break;
                case ArgumentException _:
                    Console.Out.WriteLine("Error: \n" + ex.Message);
                    break;
                default:
                    printUsage();
                    Console.Out.WriteLine("Error:");
                    Console.Out.WriteLine(ex);
                    Environment.Exit(2);
                    break;
            }
        }
    }
}