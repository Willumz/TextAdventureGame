using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class AsciiMapGenerator
{
    private static int MAXWIDTH = 40;

    public static string GenMap(Map map, Player player)
    {
        int currentrowlength = 0;
        List<string> maprows = new List<string>();
        int rownum = 1;

        IPlace place = map.GetObjByPath(player.Location);
        switch (place.GetType().ToString())
        {
            case "Map":
                maprows.Add("You are 'PLAYER'\n"+place.Name + " (" + ((Obj)place).ID + "):");
                foreach (Castle castle in ((Map)place).Castles.Arr)
                {
                    if (currentrowlength + GetWidthOfPlace(castle) < MAXWIDTH)
                    {
                        currentrowlength += GetWidthOfPlace(castle);
                        if (maprows.Count <= rownum) maprows.Add("\n\n\n");
                        maprows[rownum] = AddPlaceHorizontal(maprows[rownum], castle);
                    }
                    else
                    {
                        rownum++;
                        currentrowlength = 0;
                        maprows.Add("\n\n\n");
                        maprows[rownum] = AddPlaceHorizontal(maprows[rownum], castle);
                    }
                }
                break;
            case "Castle":
                maprows.Add("You are 'PLAYER'\n" + place.Name + " (" + ((Obj)place).ID + "):");
                foreach (Room room in ((Castle)place).Rooms.Arr)
                {
                    if (currentrowlength + GetWidthOfPlace(room) < MAXWIDTH)
                    {
                        currentrowlength += GetWidthOfPlace(room);
                        if (maprows.Count <= rownum) maprows.Add("\n\n\n");
                        maprows[rownum] = AddPlaceHorizontal(maprows[rownum], room);
                    }
                    else
                    {
                        rownum++;
                        currentrowlength = 0;
                        maprows.Add("\n\n\n");
                        maprows[rownum] = AddPlaceHorizontal(maprows[rownum], room);
                    }
                }
                break;
            case "Room":
                Castle parentcastle = (Castle) map.GetObjByPath(string.Join("/",
                    player.Location.Split('/').Take(player.Location.Split('/').Length - 1)));
                maprows.Add("You are 'PLAYER'\n" + parentcastle.Name + " (" + parentcastle.ID + "):");
                foreach (Room room in parentcastle.Rooms.Arr)
                {
                    if (currentrowlength + GetWidthOfPlace(room) < MAXWIDTH)
                    {
                        currentrowlength += GetWidthOfPlace(room);
                        if (maprows.Count <= rownum) maprows.Add("\n\n\n");
                        maprows[rownum] = AddPlaceHorizontal(maprows[rownum], room);
                    }
                    else
                    {
                        rownum++;
                        currentrowlength = 0;
                        maprows.Add("\n\n\n");
                        maprows[rownum] = AddPlaceHorizontal(maprows[rownum], room);
                    }
                }
                break;

        }

        string maptext = string.Join("\n", maprows.ToArray());
        string locationID = ((Obj) map.GetObjByPath(player.Location)).ID.ToString();
        if (maptext.Contains(locationID + new string(' ', 6 - locationID.Length)))
            maptext = maptext.Replace(locationID + new string(' ', 6 - locationID.Length), "PLAYER");
        else maptext = maptext.Replace(locationID, "PLAYER");
        return maptext;
    }

    static string AddPlaceHorizontal(string row, Obj place)
    {
        string[] rowarr = new string[4];
        int width = 0;
        if (place.Name.Length < 6) width = 8;
        else width = place.Name.Length + 2;
        rowarr[0] = "+" + new string('-', width - 2) + "+";
        rowarr[1] = "|" + place.Name + new string(' ', width-2-place.Name.Length) + "|";
        rowarr[2] = "|" + place.ID + new string(' ', width - 2 - place.ID.ToString().Length) + "|";
        rowarr[3] = "+" + new string('-', width - 2) + "+";

        string[] existingrow = row.Split('\n');
        existingrow[0] += rowarr[0];
        existingrow[1] += rowarr[1];
        existingrow[2] += rowarr[2];
        existingrow[3] += rowarr[3];
        return string.Join("\n", existingrow);
    }

    static int GetWidthOfPlace(IPlace place)
    {
        return place.Name.Length + 2;
    }

}
