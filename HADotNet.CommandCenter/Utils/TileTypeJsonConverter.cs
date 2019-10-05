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
        public override bool CanConvert(Type objectType) => typeof(IList<BaseTile>).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var arrayToken = JToken.Load(reader);

            if (!arrayToken.HasValues)
            {
                return new List<BaseTile>();
            }
            
            if (arrayToken.Type == JTokenType.Array)
            {
                var tileList = new List<BaseTile>();

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

                return tileList;
            }
            else if (arrayToken.Type == JTokenType.Object && arrayToken["type"]?.Value<string>() != null)
            {
                var typeName = arrayToken["type"].Value<string>();

                var tileType = Assembly.GetAssembly(typeof(TileTypeJsonConverter)).DefinedTypes.FirstOrDefault(t => t.IsClass && t.GetCustomAttribute<TileTypeAttribute>() != null && t.GetCustomAttribute<TileTypeAttribute>().Name.ToUpper() == typeName.ToUpper());

                if (tileType == null)
                {
                    return null;
                }

                return arrayToken.ToObject(tileType) as BaseTile;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            if (value is BlankTile t)
            {
                serializer.Serialize(writer, t);
            }
            else if (value.GetType().IsAssignableFrom(typeof(List<BaseTile>)))
            {
                foreach (var tile in (List<BaseTile>)value)
                {
                    serializer.Serialize(writer, tile);
                }
            }

            writer.WriteEndArray();
        }
    }
}
