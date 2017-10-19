using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.EntryProviders.OnlineDatabase.Tmdb;
using Assets.Classes.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects;
using Assets.Classes.GraphElements;
using Newtonsoft.Json;

namespace Assets.Classes.EntryProviders.OnlineDatabase
{
    public class TmdbProxy
    {
        private const char IdSeparator = '-';
        
        private readonly TmdbClient _client = new TmdbClient();
        
        public bool TryToBuild(string entryId, out Entry entry)
        {
            var split = entryId.Split(IdSeparator);

            if (split.Length != 2)
            {
                throw new ArgumentException($"\"{entryId}\" is not a valid id.", nameof(entryId));
            }

            var entryType = split[0];
            var tmdbId = split[1];

            if (entryType == nameof(Movie))
            {
                var tmdbMovie = _client.GetMovie(tmdbId);
                if (tmdbMovie.Id == default(long))
                {
                    entry = DefaultEntry.Instance;
                    return false;
                }

                entry = ConvertToMovie(tmdbMovie);
                return true;
            }
            else if (entryType == nameof(Artist))
            {
                var tmdbPerson = _client.GetPerson(tmdbId);
                if (tmdbPerson.Id == default(long))
                {
                    entry = DefaultEntry.Instance;
                    return false;
                }

                entry = ConvertToArtist(tmdbPerson);
                return true;
            }
            else
            {
                entry = DefaultEntry.Instance;
                return false;
            }
        }

        private Artist ConvertToArtist(TmdbPerson tmdbPerson)
        {
            return null;
        }

        private Movie ConvertToMovie(TmdbMovie tmdbMovie)
        {
            // create the Movie from the tmdbMovie
            var movie = JsonConvert.DeserializeObject<Movie>(JsonConvert.SerializeObject(tmdbMovie, TmdbJsonSettings.Instance), TmdbJsonSettings.Instance);
            movie.Id = nameof(Movie) + IdSeparator + movie.Id;

            // create the connections
            movie.Connections = new List<Connection>();

            var dictionary = new Dictionary<string, ConnectionFlags>();

            foreach (var cast in tmdbMovie.CombinedCredits.Cast)
            {
                string artistId = nameof(Artist) + IdSeparator + cast.Id;
                dictionary.Add(artistId, ConnectionFlags.Actor);
            }

            foreach (var cast in tmdbMovie.CombinedCredits.Crew)
            {
                string artistId = nameof(Artist) + IdSeparator + cast.Id;

                if (!dictionary.ContainsKey(artistId))
                {
                    dictionary[artistId] = ConvertJobToFlag(cast.Job);
                }

                dictionary[artistId] |= ConvertJobToFlag(cast.Job);
            }

            movie.Connections = dictionary.Select(kvp => new Connection
            {
                ConnectedId = kvp.Key,
                Flags = kvp.Value
            }).ToList();

            return movie;
        }

        private ConnectionFlags ConvertJobToFlag(string jobName)
        {
            ConnectionFlags flag;
            return Enum.TryParse(jobName, true, out flag)
            ? flag
            : 0;
        }
    }
}