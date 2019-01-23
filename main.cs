using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

class MainClass
{
    public static void Main(string[] args)
    {

        // Parse Map
        Map map = MapParser.ParseMap("map.json");

        // Make Player object
        Player player = new Player(map.ID.ToString(), 0, 9);

        // Game Loop
        while (player.Lives > 0)
        { ListOptions(ref player, ref map); }

        if (player.Lives == 0)
        {
            Console.WriteLine("Game Over! You ran out of lives!");
        }
        else if (map.TreasuresLeft() == 0)
        {
            Console.WriteLine("You won! You collected all the treasures!");
        }


        Console.Read();
    }

    static void ListOptions(ref Player player, ref Map map)
    {
        Console.WriteLine("What would you like to do? ");
        // Get Location Object
        IPlace place = map.GetObjByPath(player.Location);
        Console.WriteLine("0) Other");
        Console.WriteLine("1) Move");

        string choice = "";

        // Do different things depending on type of location.
        switch (place.GetType().ToString())
        {
            case "Map":
                // If 1, GoTo, else if 0, OtherFunctions
                choice = Console.ReadLine();
                if (choice == "1") GoTo(ref player, ref map);
                else if (choice == "0") ListOtherFunctions(ref player, ref map);
                break;
            case "Castle":
                // If 1, GoTo, else if 0, OtherFunctions
                choice = Console.ReadLine();
                if (choice == "1") GoTo(ref player, ref map);
                else if (choice == "0") ListOtherFunctions(ref player, ref map);
                break;
            case "Room":
                Room room = (Room) place;
                // If Monster Present
                if (room.Monster.Present)
                {
                    Console.WriteLine("2) Fight " + room.Monster.Name);
                    Console.WriteLine("3) Bluff " + room.Monster.Name);
                    choice = Console.ReadLine();
                    // Choose Fight or Bluff
                    switch (choice)
                    {
                        case "0":
                            ListOtherFunctions(ref player, ref map);
                            break;
                        case "1":
                            GoTo(ref player, ref map);
                            break;
                        case "2":
                            if (room.Monster.Fight()) Console.WriteLine("Success!");
                            else
                            {
                                // Failed Fight
                                Console.WriteLine("Failed! You lost 1 life!");
                                player.Lives--;
                            }
                            break;
                        case "3":
                            if (room.Monster.Bluff()) Console.WriteLine("Success!");
                            else
                            {
                                // Failed Bluff
                                Console.WriteLine("Failed!");
                            }
                            break;
                    }
                }
                else
                {
                    // Monster not Present, Check if Treasure Present.
                    if (room.Treasure.Present) Console.WriteLine("2) Take " + room.Treasure.Name);
                    choice = Console.ReadLine();
                    if (choice == "1") GoTo(ref player, ref map);
                    else if (choice == "2" && room.Treasure.Present)
                    {
                        player.Points += room.Treasure.Points;
                        Console.WriteLine("Points increased by " + room.Treasure.Points + "!");
                        room.Treasure.Present = false;
                    }
                    else if (choice == "0") ListOtherFunctions(ref player, ref map);
                }
                break;
        }
    }

    static void GoTo(ref Player player, ref Map map)
    {
        if (player.Location != map.ID.ToString()) Console.WriteLine("0) Go Back");
        List<int> index = new List<int>();
        if (map.GetObjByPath(player.Location).GetType() == typeof(Castle))
        {
            int x = 1;
            foreach (Room i in ((Castle) map.GetObjByPath(player.Location)).Rooms.Arr)
            {
                Console.WriteLine(x + ") " + i.Name);
                index.Add(i.ID);
                x++;
            }
        }
        else if (map.GetObjByPath(player.Location).GetType() == typeof(Map))
        {
            int x = 1;
            foreach (Castle i in ((Map)map.GetObjByPath(player.Location)).Castles.Arr)
            {
                Console.WriteLine(x + ") " + i.Name);
                index.Add(i.ID);
                x++;
            }
        }
        int choice = GetNumber();
        if (choice == 0 && player.Location != map.ID.ToString())
        {
            string[] loc = player.Location.Split('/');
            player.Location = string.Join("/", loc.Take(loc.Length-1));
        }
        else if (choice <= index.Count && choice >= 0 && (map.GetObjByPath(player.Location).GetType() != typeof(Map) || choice != 0))
        {
            player.Location += "/" + index[choice - 1];
        }

        MoveDescription(ref player, ref map);
    }

    static void ListOtherFunctions(ref Player player, ref Map map)
    {
        Console.WriteLine("1) List Points");
        Console.WriteLine("2) Number of Treasures Remaining");
        Console.WriteLine("3) Number of Lives Remaining");
        Console.WriteLine("4) Show Map");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                Console.WriteLine("You have " + player.Points + " points.");
                break;
            case "2":
                Console.WriteLine("There are " + map.TreasuresLeft() + " treasures remaining.");
                break;
            case "3":
                Console.WriteLine("You have " + player.Lives + " lives remaining.");
                break;
            case "4":
                Console.WriteLine(AsciiMapGenerator.GenMap(map, player));
                break;
        }
    }

    static void MoveDescription(ref Player player, ref Map map)
    {
        IPlace place = map.GetObjByPath(player.Location);
        switch (place.GetType().ToString())
        {
            case "Map":
                Console.WriteLine("You are outside.");
                break;
            case "Room":
                Room r = (Room) place;
                Console.WriteLine("You are in " + r.Name + ".");
                if (r.Monster.Present) Console.WriteLine("There is a " + r.Monster.Name + ".");
                if (r.Treasure.Present) Console.WriteLine("There is a " + r.Treasure.Name + ".");
                else Console.WriteLine("The room is empty.");
                break;
            case "Castle":
                Castle c = (Castle) place;
                Console.WriteLine("You are in " + c.Name + ".");
                break;
        }
    }



    static int GetNumber()
    {
        int num = 0;
        bool success = false;
        success = int.TryParse(Console.ReadLine(), out num);
        while (!success)
        {
            Console.WriteLine("Invalid number, try again.");
            success = int.TryParse(Console.ReadLine(), out num);
        }
        return num;
    }

}