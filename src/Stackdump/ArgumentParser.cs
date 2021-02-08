using System;
using System.Collections;

namespace Stackdump
{
    public class ArgumentParser
    {
        private readonly Hashtable _arguments = new Hashtable();

        public ArgumentParser(string[] acceptedArguments, string[] arguments)
        {
            foreach (var s in arguments)
                if (s.StartsWith("/"))
                {
                    var str1 = s.Substring(1);
                    var str2 = str1;
                    var empty = string.Empty;
                    if (str1.IndexOf(':') >= 0)
                    {
                        var strArray = str1.Split(new[]{':'}, 2, StringSplitOptions.RemoveEmptyEntries);
                        str2 = strArray[0];
                        empty = strArray[1];
                    }

                    if (!_arguments.ContainsKey(str2))
                        _arguments.Add(str2, new ArrayList());
                    ((ArrayList) _arguments[str2]).Add(empty);
                }
                else
                {
                    if (!int.TryParse(s, out var result) || result <= 0)
                        throw new ArgumentException("Process id has to be a integer");
                    ProcessId = result;
                }

            var arrayList = new ArrayList(acceptedArguments);
            foreach (string key in _arguments.Keys)
                if (!arrayList.Contains(key))
                    throw new ArgumentException("The argument '" + key + "' is not accepted");
        }

        public int ProcessId { get; set; }

        public bool HasProcessId => ProcessId > 0; 

        public string[] GetArgument(string name)
        {
            if (!_arguments.ContainsKey(name))
                return new string[0];
            var strArray = new string[((ArrayList) _arguments[name]).Count];
            ((ArrayList) _arguments[name]).CopyTo(strArray);

            return strArray;
        }
    }
}