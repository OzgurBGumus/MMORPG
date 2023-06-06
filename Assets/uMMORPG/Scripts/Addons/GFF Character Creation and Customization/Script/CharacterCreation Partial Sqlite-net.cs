using SQLite; // from https://github.com/praeclarum/sqlite-net
using UnityEngine;

public partial class Database
{
    public class character_customization
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string character { get; set; }
        public string values { get; set; }
        public float scale { get; set; }
    }

    public void Connect_Customization()
    {
        // create tables if they don't exist yet or were deleted
        connection.CreateTable<character_customization>();
    }

    

    
}
