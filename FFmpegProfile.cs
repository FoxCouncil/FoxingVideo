using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace FoxingVideo;

public class FFmpegProfile
{
    private const string ConnectionString = "Data Source=profiles.db";

    public string Key { get; set; }

    public string Arguments { get; set; }

    public FFmpegProfile(string key, string arguments)
    {
        Key = key.ToLower();

        Arguments = arguments;
    }

    public static void Initialize()
    {
        using var conn = new SQLiteConnection(ConnectionString);

        conn.Open();

        using var cmd = new SQLiteCommand(conn);

        cmd.CommandText = "CREATE TABLE IF NOT EXISTS Profiles (Key TEXT PRIMARY KEY, Arguments TEXT)";
        cmd.ExecuteNonQuery();
    }

    public static string StoreProfile(FFmpegProfile profile)
    {
        var exists = RetrieveProfile(profile.Key);

        if (exists != null)
        {
            return "Profile name already exists!";
        }

        using var conn = new SQLiteConnection(ConnectionString);

        conn.Open();

        using var cmd = new SQLiteCommand(conn);

        cmd.CommandText = "INSERT OR REPLACE INTO Profiles (Key, Arguments) VALUES (@key, @arguments)";
        cmd.Parameters.AddWithValue("@key", profile.Key);
        cmd.Parameters.AddWithValue("@arguments", profile.Arguments);
        cmd.ExecuteNonQuery();

        return "Successfully added profile!";
    }

    public static FFmpegProfile RetrieveProfile(string key)
    {
        using var conn = new SQLiteConnection(ConnectionString);

        conn.Open();

        using var cmd = new SQLiteCommand(conn);

        cmd.CommandText = "SELECT Arguments FROM Profiles WHERE Key = @key";
        cmd.Parameters.AddWithValue("@key", key.ToLower());

        var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new FFmpegProfile(key, reader.GetString(0));
        }
        else
        {
            return null;
        }
    }

    public static void DeleteProfile(string key)
    {
        using var conn = new SQLiteConnection(ConnectionString);

        conn.Open();

        using var cmd = new SQLiteCommand(conn);

        cmd.CommandText = "DELETE FROM Profiles WHERE Key = @key";
        cmd.Parameters.AddWithValue("@key", key.ToLower());
        cmd.ExecuteNonQuery();
    }

    public static List<FFmpegProfile> ListProfiles()
    {
        var profiles = new List<FFmpegProfile>();

        using var conn = new SQLiteConnection(ConnectionString);
        
        conn.Open();
            
        using var cmd = new SQLiteCommand(conn);

        cmd.CommandText = "SELECT Key, Arguments FROM Profiles ORDER BY Key ASC";

        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            profiles.Add(new FFmpegProfile(reader.GetString(0), reader.GetString(1)));
        }

        return profiles;
    }

    public override string ToString()
    {
        return Key;
    }
}

