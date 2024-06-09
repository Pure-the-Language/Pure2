namespace Pure.Core.Helpers
{
    public static class StringHelper
    {
        public static string[] SplitArgumentsLikeCsv(this string line, char separator = ',', bool ignoreEmptyEntries = false)
        {
            // Remark-cz: We use Csv library mainly to allow automatic handling of double quotes
            string[] arguments = Csv.CsvReader.ReadFromText(line, new Csv.CsvOptions()
            {
                HeaderMode = Csv.HeaderMode.HeaderAbsent,
                Separator = separator
            }).First().Values;

            if (ignoreEmptyEntries)
                return arguments.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
            else
                return arguments;
        }
    }
}
