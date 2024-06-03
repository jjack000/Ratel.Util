using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ratel.Util
{
    static class Util
    {
        static public void SaveToFile<T>(T obj, string path)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            File.WriteAllText(path, JsonSerializer.Serialize(obj, options));
        }

        static public T? LoadFromFile<T>(string path)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path), options);
        }

        static public Stream SaveToStream<T>(T obj)
        {
            var ms = new MemoryStream();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var writer = new Utf8JsonWriter(ms);
            JsonSerializer.Serialize(writer, obj, options);
            writer.Flush();
            ms.Position = 0;
            return ms;
        }

        static public T? LoadFromStream<T>(Stream stream)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Deserialize<T>(new StreamReader(stream).ReadToEnd(), options);
        }

        static public T Clone<T>(T obj)
        {
            return LoadFromStream<T>(SaveToStream(obj))!;
        }
    }

}
