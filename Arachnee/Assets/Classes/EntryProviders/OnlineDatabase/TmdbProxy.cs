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

        public Entry GetEntry(string entryId)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException($"\"{entryId}\" is not a valid id.", nameof(entryId));
            }

            var split = entryId.Split(IdSeparator);

            if (split.Length != 2)
            {
                throw new ArgumentException($"\"{entryId}\" is not a valid id.", nameof(entryId));
            }

            var entryType = split[0];
            var tmdbId = split[1];

            if (string.IsNullOrEmpty(entryType) || string.IsNullOrEmpty(tmdbId))
            {
                throw new ArgumentException($"\"{entryId}\" is not a valid id.", nameof(entryId));
            }

            // convert the tmdb object to its corresponding Entry
            Entry entry;
            switch (entryType)
            {
                case nameof(Movie):
                    var tmdbMovie = _client.GetMovie(tmdbId);
                    entry = ConvertToMovie(tmdbMovie);
                    break;

                case nameof(Artist):
                    var tmdbPerson = _client.GetPerson(tmdbId);
                    entry = ConvertToArtist(tmdbPerson);
                    break;
                default:
                    throw new ArgumentException($"\"{entryId}\" cannot be processed because \"{entryType}\" is not a handled entry type.", nameof(entryId));
            }

            return entry;
        }

        private Entry ConvertToArtist(TmdbPerson tmdbPerson)
        {
            return DefaultEntry.Instance;
        }

        private Movie ConvertToMovie(TmdbMovie tmdbMovie)
        {
            // create the Movie from the tmdbMovie
            var movie = JsonConvert.DeserializeObject<Movie>(
                JsonConvert.SerializeObject(tmdbMovie, TmdbJsonSettings.Instance), TmdbJsonSettings.Instance);
            movie.Id = nameof(Movie) + IdSeparator + movie.Id;

            // create the connections
            movie.Connections = new List<Connection>();

            var dictionary = new Dictionary<string, ConnectionFlags>();

            foreach (var cast in tmdbMovie.Credits.Cast)
            {
                string artistId = nameof(Artist) + IdSeparator + cast.Id;
                dictionary.Add(artistId, ConnectionFlags.Actor);
            }

            foreach (var cast in tmdbMovie.Credits.Crew)
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