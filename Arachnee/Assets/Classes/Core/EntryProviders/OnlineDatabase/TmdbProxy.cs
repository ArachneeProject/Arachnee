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
        /// Returns the main image corresponding to the given <see cref="Entry"/>.
        /// </summary>
        public byte[] GetMainImage(Entry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            if (entry == DefaultEntry.Instance)
            {
                throw new ArgumentException("The given entry was the default entry.");
            }

            var movie = entry as Movie;
            if (movie != null)
            {
                return GetImage(ImageType.Poster, ImageSize.Large, movie.MainImagePath);
            }

            var artist = entry as Artist;
            if (artist != null)
            {
                return GetImage(ImageType.Profile, ImageSize.Large, artist.MainImagePath);
            }

            var tv = entry as TvSeries;
            if (tv != null)
            {
                return GetImage(ImageType.Poster, ImageSize.Large, tv.MainImagePath);
            }

            throw new ArgumentOutOfRangeException($"{entry.GetType().Name} is not handled.");
        }

        /// <summary>
        /// Returns the main image corresponding to the given <see cref="SearchResult"/>.
        /// </summary>
        public byte[] GetMainImage(SearchResult searchResult)
        {
            if (searchResult == null)
            {
                throw new ArgumentNullException(nameof(searchResult));
            }

            if (string.IsNullOrEmpty(searchResult.EntryId))
            {
                throw new ArgumentException($"\"Property {nameof(searchResult.EntryId)} of {nameof(SearchResult)} is empty.");
            }

            if (string.IsNullOrEmpty(searchResult.ImagePath))
            {
                throw new ArgumentException($"\"Property {nameof(searchResult.ImagePath)} of {nameof(SearchResult)} is empty.");
            }

            var split = searchResult.EntryId.Split(IdSeparator);

            if (split.Length != 2)
            {
                throw new ArgumentException($"\"{searchResult.EntryId}\" is not a valid id.");
            }

            var entryType = split[0];
            
            if (string.IsNullOrEmpty(entryType))
            {
                throw new ArgumentException($"\"{searchResult.EntryId}\" is not a valid id.");
            }
            
            switch (entryType)
            {
                case nameof(Movie):
                    return GetImage(ImageType.Poster, ImageSize.Small, searchResult.ImagePath);

                case nameof(Artist):
                    return GetImage(ImageType.Profile, ImageSize.Small, searchResult.ImagePath);

                case nameof(TvSeries):
                    return GetImage(ImageType.Poster, ImageSize.Small, searchResult.ImagePath);

                default:
                    throw new ArgumentException($"\"{searchResult.EntryId}\" cannot be processed because \"{entryType}\" is not a handled entry type.");
            }
        }

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
            var resultsWithImage = tmdbResults.Where(r =>
                !(string.IsNullOrEmpty(r.PosterPath) && string.IsNullOrEmpty(r.ProfilePath)));

            foreach (var result in resultsWithImage)
            {
                switch (result.MediaType)
                {
                    case "movie":
                        results.Add(new SearchResult
                        {
                            EntryId = nameof(Movie) + IdSeparator + result.Id,
                            ImagePath = result.PosterPath,
                            Name = result.Title,
                            Date = result.ReleaseDate
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
                            EntryId = nameof(TvSeries) + IdSeparator + result.Id,
                            ImagePath = result.PosterPath,
                            Name = result.Name,
                            Date = result.FirstAirDate
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

            ulong id;
            if (!ulong.TryParse(tmdbId, out id))
            {
                throw new ArgumentException($"\"{entryId}\" is not a valid id.", nameof(entryId));
            }

            // convert the tmdb object to its corresponding Entry
            Entry entry;
            switch (entryType) // TODO: Handle Serie
            {
                case nameof(Movie):
                    var tmdbMovie = _client.GetMovie(id);
                    entry = ConvertToMovie(tmdbMovie);
                    break;

                case nameof(Artist):
                    var tmdbPerson = _client.GetPerson(id);
                    entry = ConvertToArtist(tmdbPerson);
                    break;

                case nameof(TvSeries):
                    var tmdbTvSeries = _client.GetTvSeries(id);
                    entry = ConvertToTvSeries(tmdbTvSeries);
                    break;

                default:
                    throw new ArgumentException(
                        $"\"{entryId}\" cannot be processed because \"{entryType}\" is not a handled entry type.",
                        nameof(entryId));
            }

            return entry;
        }

        private TvSeries ConvertToTvSeries(TmdbTvSeries tmdbTvSeries)
        {
            var tvSeries = JsonConvert.DeserializeObject<TvSeries>(
                JsonConvert.SerializeObject(tmdbTvSeries, TmdbJsonSettings.Instance), TmdbJsonSettings.Instance);

            tvSeries.Id = nameof(TvSeries) + IdSeparator + tmdbTvSeries.Id;
            tvSeries.MainImagePath = tmdbTvSeries.PosterPath;

            foreach (var cast in tmdbTvSeries.Credits.Cast.Where(c => !string.IsNullOrEmpty(c.ProfilePath)))
            {
                tvSeries.Connections.Add(new Connection
                {
                    ConnectedId = nameof(Artist) + IdSeparator + cast.Id,
                    Type = ConnectionType.Actor,
                    Label = cast.Character
                });
            }

            foreach (var cast in tmdbTvSeries.Credits.Crew.Where(c => !string.IsNullOrEmpty(c.ProfilePath)))
            {
                ConnectionType type;
                if (_handledCrewJobs.TryGetValue(cast.Job, out type))
                {
                    tvSeries.Connections.Add(new Connection
                    {
                        ConnectedId = nameof(Artist) + IdSeparator + cast.Id,
                        Type = type,
                        Label = cast.Job
                    });
                }
                else
                {
                    tvSeries.Connections.Add(new Connection
                    {
                        ConnectedId = nameof(Artist) + IdSeparator + cast.Id,
                        Type = ConnectionType.Crew,
                        Label = cast.Job
                    });
                }
            }

            var creators = tmdbTvSeries.CreatedBy.Select(creator => nameof(Artist) + IdSeparator + creator.Id.ToString());
            tvSeries.Connections.AddRange(creators.Select(creator => new Connection
            {
                ConnectedId = creator,
                Label = "Created by",
                Type = ConnectionType.CreatedBy
            }));
            
            return tvSeries;
        }

        private Artist ConvertToArtist(TmdbPerson tmdbPerson)
        {
            // create the Artist from the tmdbPerson
            var artist = JsonConvert.DeserializeObject<Artist>(
                JsonConvert.SerializeObject(tmdbPerson, TmdbJsonSettings.Instance), TmdbJsonSettings.Instance);
            artist.Id = nameof(Artist) + IdSeparator + artist.Id;
            artist.NickNames = tmdbPerson.AlsoKnownAs;

            // set images
            artist.MainImagePath = tmdbPerson.ProfilePath;

            // create the connections
            artist.Connections = new List<Connection>();

            foreach (var cast in tmdbPerson.CombinedCredits.Cast.Where(c => !string.IsNullOrEmpty(c.PosterPath)))
            {
                var id = cast.MediaType == "tv"
                    ? nameof(TvSeries) + IdSeparator + cast.Id
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
			// clean up fields
            if (!tmdbMovie.Runtime.HasValue)
            {
                tmdbMovie.Runtime = 0;
            }
			
            // create the Movie from the tmdbMovie
            var movie = JsonConvert.DeserializeObject<Movie>(JsonConvert.SerializeObject(tmdbMovie));
            movie.Id = nameof(Movie) + IdSeparator + movie.Id;
            movie.Tags = tmdbMovie.Genres.Select(g => g.Name).ToList();

            // set images
            movie.MainImagePath = tmdbMovie.PosterPath;

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
        
        private byte[] GetImage(ImageType imageType, ImageSize imageSize, string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                throw new ArgumentNullException(nameof(imagePath));
            }

            string fileSize;

            switch (imageSize)
            {
                case ImageSize.Small:
                    switch (imageType)
                    {
                        case ImageType.Backdrop:
                            fileSize = "w300";
                            break;

                        case ImageType.Logo:
                        case ImageType.Profile:
                            fileSize = "w45";
                            break;

                        case ImageType.Poster:
                        case ImageType.Still:
                            fileSize = "w92";
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(imageType), $"Image type \"{imageType}\" is not handled.");
                    }
                    break; // small

                case ImageSize.Medium:
                    switch (imageType)
                    {
                        case ImageType.Backdrop:
                            fileSize = "w780";
                            break;

                        case ImageType.Logo:
                        case ImageType.Profile:
                        case ImageType.Poster:
                        case ImageType.Still:
                            fileSize = "w185";
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(imageType), $"Image type \"{imageType}\" is not handled.");
                    }
                    break; // medium

                case ImageSize.Large:
                    switch (imageType)
                    {
                        case ImageType.Backdrop:
                            fileSize = "w1280";
                            break;

                        case ImageType.Logo:
                            fileSize = "w500";
                            break;

                        case ImageType.Profile:
                            fileSize = "h632";
                            break;

                        case ImageType.Poster:
                            fileSize = "w780";
                            break;

                        case ImageType.Still:
                            fileSize = "w300";
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(imageType), $"Image type \"{imageType}\" is not handled.");
                    }
                    break; // large

                case ImageSize.Original:
                    fileSize = "original";
                    break; // original

                default:
                    throw new ArgumentOutOfRangeException(nameof(imageSize), $"Image size \"{imageSize}\" is not handled.");
            }

            var image = _client.GetImage(fileSize, imagePath);
            return image;
        }

        private enum ImageType
        {
            Backdrop,
            Logo,
            Poster,
            Profile,
            Still
        }

        private enum ImageSize
        {
            Small,
            Medium,
            Large,
            Original
        }
    }
}