using System;
using System.Diagnostics;
using System.Linq;

namespace Stackdump
{
    public class ProcessInspector
    {
        private const string ManagedMainTreadPrefix = "mscor";

        public static void ListProcesses()
        {
            foreach (var process in Process.GetProcesses().OrderBy(process => process.ProcessName))
            {
                if (IsCurrent(process) || !IsManaged(process)) continue;

                PrintMethodInfo(process);
                Console.Out.WriteLine();
            }
        }

        private static bool IsCurrent(Process process)
        {
            return Process.GetCurrentProcess().Id == process.Id;
        }

        private static bool IsManaged(Process process)
        {
            try
            {
                foreach (ProcessModule pm in process.Modules)
                {
                    if (pm.ModuleName.StartsWith(ManagedMainTreadPrefix, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // If the process can't list modules skip it.
            }

            return false;
        }

        public static void PrintMethodInfo(Process p)
        {
            try
            {
                Console.Out.Write($" {p.Id}\t{p.ProcessName} ");
            }
            catch
            {
                Console.Out.Write($" {p.Id}\t? ");
            }

            try
            {
                Console.Out.Write($" ({p.MainModule?.FileName ?? "[Null]"}) ");
            }
            catch
            {
                // If the process cant provide a file name skip it.
            }
        }
    }
}