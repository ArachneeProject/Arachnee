using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb;
using Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects;
using Assets.Classes.Core.Models;
using Newtonsoft.Json;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase
{
    public class TmdbProxy
    {
        private const char IdSeparator = '-';

        private readonly TmdbClient _client = new TmdbClient();

        private readonly Dictionary<string, ConnectionType> _handledCrewJobs = new Dictionary<string, ConnectionType>
        {
            {"Director", ConnectionType.Director},
            {"Boom Operator", ConnectionType.BoomOperator},
        };

        /// <summary>
        /// Returns a list of <see cref="SearchResult"/> corresponding to the given query.
        /// </summary>
        public List<SearchResult> GetSearchResults(string query)
        {
            var results = new List<SearchResult>();
            if (string.IsNullOrEmpty(query))
            {
                return results;
            }

            var tmdbResults = _client.GetCombinedSearchResults(query);
            var resultsWithImage = tmdbResults.Where(r => !(string.IsNullOrEmpty(r.PosterPath) && string.IsNullOrEmpty(r.ProfilePath)));

            foreach (var result in resultsWithImage)
            {
                switch (result.MediaType)
                {
                    case "movie":
                        results.Add(new SearchResult
                        {
                            EntryId = nameof(Movie) + IdSeparator + result.Id,
                            ImagePath = result.PosterPath,
                            Name = result.Title
                        });
                        break;
                        
                    case "person":
                        results.Add(new SearchResult
                        {
                            EntryId = nameof(Artist) + IdSeparator + result.Id,
                            ImagePath = result.ProfilePath,
                            Name = result.Name
                        });
                        break;

                    case "tv":
                        results.Add(new SearchResult
                        {
                            EntryId = nameof(Serie) + IdSeparator + result.Id,
                            ImagePath = result.PosterPath,
                            Name = result.Name
                        });
                        break;
                }
            }

            return results;
        }

        /// <summary>
        /// Returns the <see cref="Entry"/> corresponding to the given id.
        /// </summary>
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
            switch (entryType) // TODO: Handle Serie
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

        private Artist ConvertToArtist(TmdbPerson tmdbPerson)
        {
            // create the Artist from the tmdbPerson
            var artist = JsonConvert.DeserializeObject<Artist>(
                            JsonConvert.SerializeObject(tmdbPerson, TmdbJsonSettings.Instance), TmdbJsonSettings.Instance);
            artist.Id = nameof(Artist) + IdSeparator + artist.Id;
            artist.NickNames = tmdbPerson.AlsoKnownAs;

            // create the connections
            artist.Connections = new List<Connection>();

            foreach (var cast in tmdbPerson.CombinedCredits.Cast.Where(c => !string.IsNullOrEmpty(c.PosterPath)))
            {
                var id = cast.MediaType == "tv"
                    ? nameof(Serie) + IdSeparator + cast.Id
                    : nameof(Movie) + IdSeparator + cast.Id;

                artist.Connections.Add(new Connection
                {
                    ConnectedId = id,
                    Type = ConnectionType.Actor,
                    Label = cast.Character
                });
            }
            
            foreach (var cast in tmdbPerson.CombinedCredits.Crew.Where(c => !string.IsNullOrEmpty(c.PosterPath)))
            {
                ConnectionType type;
                if (_handledCrewJobs.TryGetValue(cast.Job, out type))
                {
                    artist.Connections.Add(new Connection
                    {
                        ConnectedId = nameof(Movie) + IdSeparator + cast.Id,
                        Type = type,
                        Label = cast.Job
                    });
                }
                else
                {
                    artist.Connections.Add(new Connection
                    {
                        ConnectedId = nameof(Movie) + IdSeparator + cast.Id,
                        Type = ConnectionType.Crew,
                        Label = cast.Job
                    });
                }
            }

            return artist;
        }

        private Movie ConvertToMovie(TmdbMovie tmdbMovie)
        {
            // create the Movie from the tmdbMovie
            var movie = JsonConvert.DeserializeObject<Movie>(JsonConvert.SerializeObject(tmdbMovie));
            movie.Id = nameof(Movie) + IdSeparator + movie.Id;
            movie.Tags = tmdbMovie.Genres.Select(g => g.Name).ToList();

            // create the connections
            movie.Connections = new List<Connection>();
            
            foreach (var cast in tmdbMovie.Credits.Cast.Where(c => !string.IsNullOrEmpty(c.ProfilePath)))
            {
                movie.Connections.Add(new Connection
                {
                    ConnectedId = nameof(Artist) + IdSeparator + cast.Id,
                    Type = ConnectionType.Actor,
                    Label = cast.Character
                });
            }

            foreach (var cast in tmdbMovie.Credits.Crew.Where(c => !string.IsNullOrEmpty(c.ProfilePath)))
            {
                ConnectionType type;
                if (_handledCrewJobs.TryGetValue(cast.Job, out type))
                {
                    movie.Connections.Add(new Connection
                    {
                        ConnectedId = nameof(Artist) + IdSeparator + cast.Id,
                        Type = type,
                        Label = cast.Job
                    });
                }
                else
                {
                    movie.Connections.Add(new Connection
                    {
                        ConnectedId = nameof(Artist) + IdSeparator + cast.Id,
                        Type = ConnectionType.Crew,
                        Label = cast.Job
                    });
                }
            }
            
            return movie;
        }
    }
}