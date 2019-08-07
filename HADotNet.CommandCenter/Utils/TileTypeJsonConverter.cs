using HADotNet.CommandCenter.Models.Config.Tiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace HADotNet.CommandCenter.Utils
{
    public class TileTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(BaseTile).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.Object && (token["type"]?.HasValues ?? false))
            {
                var typeName = token["type"].Value<string>();

                var tileType = Assembly.GetAssembly(typeof(TileTypeJsonConverter)).DefinedTypes.FirstOrDefault(t => t.IsClass && t.GetCustomAttribute<TileTypeAttribute>() != null && t.GetCustomAttribute<TileTypeAttribute>().Name.ToUpper() == typeName.ToUpper());

                if (tileType == null)
                {
                    return null;
                }

                return token.ToObject(tileType);
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!typeof(BaseTile).IsAssignableFrom(value.GetType()))
            {
                return;
            }

            var token = JToken.FromObject(value);

            token["type"] = value.GetType().GetCustomAttribute<TileTypeAttribute>().Name;

            serializer.Serialize(writer, value);
        }
    }
}
