using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Object2Table
{
    public class ObjectPrinter
    {
        public static string Print<T>(T data) where T : class
        {
            return Print(new object[] { data }, typeof(T));
        }

        public static string Print<T>(IEnumerable<T> data) where T : class
        {
            return Print(data.Cast<object>().ToArray(), typeof(T));
        }

        public static string Print(IEnumerable<object> data, Type elementType)
        {
            if (elementType.AssemblyQualifiedName is null || elementType.AssemblyQualifiedName.StartsWith("System"))
                return string.Join(Environment.NewLine, data.Select(PrintSystemObject));

            var properties = elementType.GetPropertyEmitters();
            var printableData = data.Select(item => properties.Select(i => Print(i.Emitter.DynamicInvoke(item)).Split(Environment.NewLine)).ToArray()).ToArray();

            var columnsLength = properties
                .Select((i, j) =>
                {
                    var maxLengthData = printableData.Any() ? printableData.Max(k => k[j].Any() ? k[j].Max(l => l.Length) : 0) : 0;
                    var nameLength = i.Property.Length;
                    return maxLengthData > nameLength ? maxLengthData + 2 : nameLength + 2;
                })
                .ToArray();
            var divider = new string('-', columnsLength.Sum() + properties.Length + 1).AsSpan();
            var sb = new StringBuilder();
            sb.Append(divider);
            sb.AppendLine();
            
            for (var prop = 0; prop < properties.Length; prop++)
            {
                sb.Append('|');
                sb.Append(PrintCell(properties[prop].Property, columnsLength[prop]));
            }

            sb.Append('|');
            sb.AppendLine();
            sb.Append(divider);

            foreach (var printableRow in printableData)
            {
                sb.AppendLine();
                var max = printableRow.Max(i => i.Length);

                for (var lineNumber = 0; lineNumber < max; lineNumber++)
                {
                    for (var column = 0; column < printableRow.Length; column++)
                    {
                        sb.Append('|');
                        sb.Append(PrintCell(lineNumber < printableRow[column].Length ? printableRow[column][lineNumber] : "", columnsLength[column]));
                    }

                    sb.Append('|');
                    sb.AppendLine();
                }

                sb.Append(divider);
            }

            return sb.ToString();
        }

        public static string Print(object data)
        {
            if (data is null)
                return "-";

            if (data.IsEnumerable(out var enumerable))
                return Print(enumerable.Cast<object>(), enumerable.GetType().ToElementType());

            return Print(new[] { data }, data.GetType());
        }

        private static ReadOnlySpan<char> PrintCell(ReadOnlySpan<char> value, int length)
        {
            Span<char> result = stackalloc char[length];
            result.Fill(' ');
            
            var padding = length - value.Length;
            var leftPadding = padding / 2;
            foreach (var character in value)
            {
                result[leftPadding] = character;
                leftPadding++;
            }
            
            return result.ToString();
        }

        private static string PrintSystemObject(object data)
        {
            var sb = new StringBuilder();
            switch (data)
            {
                case IDictionary dictionary:
                    var keys = new List<string>();
                    var values = new List<string>();
                    foreach (DictionaryEntry dictionaryEntry in dictionary)
                    {
                        keys.Add(Print(dictionaryEntry.Key));
                        values.Add(Print(dictionaryEntry.Value));
                    }

                    var maxKey = keys.SelectMany(i => i.Split(Environment.NewLine)).Max(i => i.Length);
                    var maxValue = values.SelectMany(i => i.Split(Environment.NewLine)).Max(i => i.Length);
                    for (var i = 0; i < dictionary.Count; i++)
                    {
                        var key = keys[i].Split(Environment.NewLine);
                        var value = values[i].Split(Environment.NewLine);
                        var length = key.Length > value.Length ? key.Length : value.Length;
                        for (var j = 0; j < length; j++)
                        {
                            if (i != 0)
                                sb.AppendLine();
                            sb.Append(PrintCell(j < key.Length ? key[j] : "", maxKey + 2));
                            sb.Append(' ');
                            sb.Append(PrintCell(j < value.Length ? value[j] : "", maxValue + 2));
                        }
                    }
                    break;
                case null:
                    sb.Append('-');
                    break;
                default:
                    sb.Append(data);
                    break;
            }

            return sb.ToString();
        }
    }
}