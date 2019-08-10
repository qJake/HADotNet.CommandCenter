using HADotNet.CommandCenter.Models.Config.Tiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HADotNet.CommandCenter.Utils
{
    public class TileTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(BaseTile).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var arrayToken = JToken.Load(reader);
            var tileList = new List<BaseTile>();

            if (arrayToken.Type == JTokenType.Array)
            {
                foreach (var token in (JArray)arrayToken)
                {
                    if (token.Type == JTokenType.Object && token["type"]?.Value<string>() != null)
                    {
                        var typeName = token["type"].Value<string>();

                        var tileType = Assembly.GetAssembly(typeof(TileTypeJsonConverter)).DefinedTypes.FirstOrDefault(t => t.IsClass && t.GetCustomAttribute<TileTypeAttribute>() != null && t.GetCustomAttribute<TileTypeAttribute>().Name.ToUpper() == typeName.ToUpper());

                        if (tileType == null)
                        {
                            return null;
                        }

                        tileList.Add(token.ToObject(tileType) as BaseTile);
                    }
                }
            }
            return tileList;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (var tile in (List<BaseTile>)value)
            {
                serializer.Serialize(writer, tile);
            }

            writer.WriteEndArray();
        }
    }
}
