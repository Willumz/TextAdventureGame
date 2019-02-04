using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class MapParser
{

    public static Map ParseMap(string filepath)
    {
        string mapJSON = File.ReadAllText(filepath);

        mapJSON = CleanMapJSON(mapJSON);

        JObject map = JsonConvert.DeserializeObject<JObject>(mapJSON);

        List<Room> rooms = new List<Room>();
        List<Castle> castles = new List<Castle>();
        var a = map.GetValue("castles").Children();
        foreach (JToken castle in map.GetValue("castles").Children())
        {
            // Castles
            rooms.Clear();
            foreach (JToken room in (castle.First()))
            {
                // Rooms
                Treasure t = new Treasure((string)room.First()["treasure"]["name"],
                    int.Parse((string)room.First()["treasure"]["points"]));

                Monster m = new Monster((string)room.First()["monster"]["name"],
                    bool.Parse((string)room.First()["monster"]["present"]));

                Room r = new Room((string)((JProperty)room).Name,
                    m, t);
                rooms.Add(r);
            }

            Castle c = new Castle((string)((JProperty)castle).Name,
                new ObjArray<Room>(rooms.ToArray()));
            castles.Add(c);
        }

        Map MAP = new Map(new ObjArray<Castle>(castles.ToArray()));

        return MAP;
    }

    static string CleanMapJSON(string map)
    {
        Random rnd = new Random();

        string[] mapGENPOINTS = map.Split(new string[] { "{GENPOINTS}" }, StringSplitOptions.None);
        string map2 = "";
        int x = 0;
        foreach (string i in mapGENPOINTS)
        {
            if (x < mapGENPOINTS.Length - 1)
            {
                map2 += i + rnd.Next(100).ToString();
                x++;
            }
            else
            {
                map2 += i;
                break;
            }
        }
        map = map2;

        string[] mapGENNAME = map.Split(new string[] { "{GENNAME}" }, StringSplitOptions.None);
        map2 = "";
        x = 0;
        foreach (string i in mapGENNAME)
        {
            if (x < mapGENNAME.Length - 1)
            {
                map2 += i + Monster.MonsterNames[rnd.Next(Monster.MonsterNames.Length)];
                x++;
            }
            else
            {
                map2 += i;
                break;
            }
        }
        map = map2;

        string[] mapBOOL = map.Split(new string[] { "{BOOL}" }, StringSplitOptions.None);
        map2 = "";
        x = 0;
        foreach (string i in mapBOOL)
        {
            if (x < mapBOOL.Length - 1)
            {
                map2 += i + (rnd.Next(2) == 1).ToString();
                x++;
            }
            else
            {
                map2 += i;
                break;
            }
        }
        map = map2;
        
        return map;
    }

}