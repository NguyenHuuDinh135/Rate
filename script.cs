using System;
using System.IO;
using Npgsql;

class Program
{
    static void Main()
    {
        string connString = ""Host=localhost;Port=5432;Database=RateDb;Username=postgres;Password=postgres"";
        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            using var cmd = new NpgsqlCommand(""SELECT COUNT(*) FROM \""Bookings\"""", conn);
            var count = cmd.ExecuteScalar();
            Console.WriteLine($""Bookings count: {count}"");
            
            using var cmd2 = new NpgsqlCommand(""SELECT COUNT(*) FROM \""Payments\"""", conn);
            var pCount = cmd2.ExecuteScalar();
            Console.WriteLine($""Payments count: {pCount}"");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
