using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders
{
    public class JsonSettings
    {
        private static JsonSerializerSettings _tmdb;

        public static JsonSerializerSettings Tmdb => _tmdb ?? (_tmdb = new JsonSerializerSettings
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
                    if (char.IsLower(character))
                    {
                        sb.Append(character);
                        continue;
                    }

                    sb.Append('_');
                    sb.Append(char.ToLower(character, Culture));
                }

                return sb.ToString();
            }
        }
    }
}