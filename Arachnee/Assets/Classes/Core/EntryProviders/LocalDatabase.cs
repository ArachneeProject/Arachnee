using System;
using System.Collections.Generic;
using System.IO;
using Assets.Classes.Core.Models;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Classes.Core.EntryProviders
{
    public class LocalDatabase : EntryProvider
    {
        private readonly SqliteConnection _connection;

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };

        public LocalDatabase()
        {
             _connection = new SqliteConnection("URI=file:" + Path.Combine(Path.Combine(Application.dataPath, "Database"), "arachnee.db"));
        }

        public bool TrySave(Entry entry)
        {
            try
            {
                const string query = "INSERT OR REPLACE INTO Entries (Id, Data) VALUES (@id,@data);";
                
                _connection.Open();
                using (var cmd = this._connection.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", entry.Id);
                    cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(entry, _serializerSettings));
                    
                    var added = cmd.ExecuteNonQuery();

                    _connection.Close();
                    cmd.Dispose();

                    return added > 0;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                _connection.Close();
            }

            return false;
        }

        public override Queue<SearchResult> GetSearchResults(string searchQuery)
        {
            throw new NotImplementedException();
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            try
            {
                const string query = "SELECT Data FROM Entries WHERE Id=@id;";

                _connection.Open();
                using (var cmd = this._connection.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id", entryId);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        // if entry is not in the db, ask the bigger provider
                        if (!reader.Read())
                        {
                            reader.Dispose();
                            cmd.Dispose();
                            _connection.Close();
                            
                            if (BiggerProvider != null
                                && BiggerProvider.TryGetEntry(entryId, out entry))
                            {
                                TrySave(entry);
                                return true;
                            }

                            entry = DefaultEntry.Instance;
                            return false;
                        }

                        // entry is in the db
                        var data = reader["Data"] as string;
                        if (string.IsNullOrEmpty(data))
                        {
                            reader.Dispose();
                            cmd.Dispose();
                            _connection.Close();
                            entry = DefaultEntry.Instance;
                            return false;
                        }

                        entry = JsonConvert.DeserializeObject(data, _serializerSettings) as Entry;
                    }

                    _connection.Close();
                    cmd.Dispose();
                    return entry != null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                _connection.Close();
            }

            entry = DefaultEntry.Instance;
            return false;
        }
    }
}
