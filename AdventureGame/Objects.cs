using System;
using System.Collections.Generic;

// Basic object, which others can inherit from.
class Obj
{
    private static int id = 0;
    public static int GenID()
    {
        id++;
        return id;
    }

    public int ID { get; set; }
    public string Name { get; set; }

}

class Castle : Obj, IPlace
{

    public ObjArray<Room> Rooms { get; set; }

    public Castle(string name, ObjArray<Room> rooms)
    {
        this.Name = name;
        this.Rooms = rooms;
        this.ID = Obj.GenID();
    }
}

// Interface which places inherit from.
public interface IPlace
{
    string Name { get; set; }
}

// An array of places.
class ObjArray<T> where T : Obj
{

    public T[] Arr { get; set; }

    public ObjArray(T[] arr)
    {
        this.Arr = arr;
    }
    public T GetObj(string name)
    {
        foreach (T i in this.Arr)
            if (i.Name == name)
                return i;
        throw new Exception("No such object.");
    }
    public bool ObjExists(string name)
    {
        foreach (T i in this.Arr)
            if (i.Name == name)
                return true;
        return false;
    }
}


class Room : Obj, IPlace
{

    public Monster Monster { get; set; }
    public Treasure Treasure { get; set; }

    public Room(string name, Monster monster, Treasure treasure)
    {
        this.Name = name;
        this.Monster = monster;
        this.Treasure = treasure;
        this.ID = Obj.GenID();
    }

}

class Treasure : Obj
{
    public string Name { get; set; }
    public int Points { get; set; }
    public bool Present { get; set; }

    public Treasure(string name, int points)
    {
        this.Name = name;
        this.Points = points;
        this.ID = Obj.GenID();
        this.Present = true;
    }

}

class Monster : Obj
{
    public string Name { get; set; }
    public bool Present { get; set; }
    public bool HasBluffed { get; set; }
    public bool IsDead { get; set; }

    // List of potential monster names.
    public static string[] MonsterNames = new string[]
    {
      "HYDRA", "KRAKEN", "DRAGON"
    };

    public Monster(string name, bool present)
    {
        this.Name = name;
        this.Present = present;
        this.ID = Obj.GenID();
    }
    // Fight. IF RANDOM (0, 10) > RANDOM(0,10)
    public bool Fight()
    {
        Random rnd = new Random();
        if (rnd.Next(20) > rnd.Next(20))
        {
            this.Present = false;
            return true;
        }
        return false;
    }
    // Bluff 30% chance.
    public bool Bluff()
    {
        Random rnd = new Random();
        if (rnd.Next(10) < 3)
        {
            this.Present = false;
            return true;
        }
        return false;
    }

}

class Map : Obj, IPlace
{

    public ObjArray<Castle> Castles { get; set; }

    public Map(ObjArray<Castle> castles)
    {
        this.Castles = castles;
        this.ID = Obj.GenID();
        this.Name = "Map";
    }

    // Get a place in Map by its ID
    public IPlace GetObjByID(int id)
    {
        if (this.ID == id) return this;
        foreach (Castle castle in Castles.Arr)
        {
            if (castle.ID == id) return castle;
            else
            {
                foreach (Room room in castle.Rooms.Arr)
                {
                    if (room.ID == id) return room;
                }
            }
        }
        throw new Exception("No such ID.");
    }

    // Get the path to a place by its ID
    public string GetPathByID(int id)
    {
        string path = "";
        if (this.ID == id) path = this.ID.ToString();
        foreach (Castle castle in Castles.Arr)
        {
            if (castle.ID == id) path = this.ID + "/" + castle.ID.ToString();
            else
            {
                foreach (Room room in castle.Rooms.Arr)
                {
                    if (room.ID == id) path = this.ID + "/" + castle.ID + "/" + room.ID;
                }
            }
        }
        return path;
    }

    // Get an object in map by its path
    public IPlace GetObjByPath(string path)
    {
        string[] path2 = path.Split('/');
        return this.GetObjByID(int.Parse(path2[path2.Length-1]));
    }

    // Count how many treasures are left in the map
    public int TreasuresLeft()
    {
        int total = 0;
        foreach (Castle i in this.Castles.Arr)
        {
            foreach (Room ii in i.Rooms.Arr)
            {
                if (ii.Treasure.Present) total++;
            }
        }
        return total;
    }

}

class Player
{
    public string Location { get; set; }
    public int Points { get; set; }
    private int _lives = 0;
    public int Lives
    {
        get { return this._lives; }
        set
        {
            if (this.Monsters.List.Count > 0 && this.Lives > value)
            {
                Console.WriteLine(this.Monsters.List[0].Name + " has died!");
                this.Monsters.List.RemoveAt(0);
            }
            this._lives = value;
        }
    }
    
    public MonsterPack Monsters { get; set; }

    public Player(string location, int points, int lives)
    {
        this.Monsters = new MonsterPack();
        this.Location = location;
        this.Points = points;
        this.Lives = lives;
    }
}

class MonsterPack
{
    public List<Monster> List { get; set; }

    public MonsterPack()
    {
        this.List = new List<Monster>();
    }

    public bool CatchMonster(Monster monster)
    {
        bool success = false;

        Random rnd = new Random();
        if (rnd.Next(50) > rnd.Next(100))
        {
            success = true;
            Console.WriteLine("You caught it! Would you like to rename it?");
            Console.WriteLine("1) Yes\n2) No");
            if (Console.ReadLine() == "1")
            {
                Console.WriteLine("Enter the name: ");
                monster.Name = Console.ReadLine();
            }
            List.Add(monster);
        }

        return success;
    }
}