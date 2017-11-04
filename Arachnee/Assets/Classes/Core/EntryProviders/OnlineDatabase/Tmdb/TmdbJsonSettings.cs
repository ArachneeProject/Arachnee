using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb
{
    public class TmdbJsonSettings
    {
        private static JsonSerializerSettings _instance;

        public static JsonSerializerSettings Instance => _instance ?? (_instance = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new PascalCasePropertyNamesContractResolver()
        });

        private class PascalCasePropertyNamesContractResolver : DefaultContractResolver
        {
            private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

            protected override string ResolvePropertyName(string propertyName)
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    return string.Empty;
                }

                var sb = new StringBuilder(propertyName.Length);
                sb.Append(char.ToLower(propertyName[0]));
                
                foreach (var character in propertyName.Substring(1))
                {
                    if (char.IsUpper(character))
                    {
                        sb.Append('_');
                        sb.Append(char.ToLower(character, Culture));
                        continue;
                    }

                    sb.Append(character);
                }

                return sb.ToString();
            }
        }
    }
}