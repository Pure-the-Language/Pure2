using Parcel.CoreEngine.Helpers;
using System.Text.RegularExpressions;

namespace Python
{
    internal class RuntimeHelper
    {
        internal static string FindPythonDLL()
        {
            foreach (string path in StringHelper.SplitCommandLineArguments(Environment.GetEnvironmentVariable("PATH"), ';'))
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles(path))
                    {
                        string filename = Path.GetFileName(file);
                        if (Regex.IsMatch(filename, @"python\d{2,3}\.dll"))
                            return file;
                    }
                }
                // Remark-cz: Certain paths might NOT be enumerable due to access issues
                catch (Exception) { continue; }
            }
            return null;
        }
    }
}
