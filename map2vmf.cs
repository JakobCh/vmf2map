using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Entity
{
    public Dictionary<string, string> values;
    public List<Brush> brushes;

    public Entity()
    {
        values = new Dictionary<string, string>();
        brushes = new List<Brush>();
    }

    public Entity(string[] lines, ref int index)
    {
        while (lines[index].StartsWith("//")) { index++; } //skip comments
        Console.WriteLine("Entity: " + lines[index]);
        values = new Dictionary<string, string>();
        brushes = new List<Brush>();

        
        index++; //skip {
        while (lines[index].StartsWith("\""))
        {
            var split = lines[index].Split("\" \"");
            values[split[0].Replace("\"", "")] = split[1].Replace("\"", "");
            index++;
        }

        while (lines[index].StartsWith("//")) { index++; } //skip comments

        while (lines[index].StartsWith("{"))
        {
            var brush = new Brush();
            index++; //skip {
            while (lines[index].StartsWith("("))
            {
                brush.planes.Add(new Plane(lines[index]));
                //Console.WriteLine(lines[index]);
                index++;
            }
            index++; //skip }
            while (lines[index].StartsWith("//")) { index++; } //skip comments
            brushes.Add(brush);
        }

        index++; //skip {

        /*foreach (var key in values.Keys)
        {
            Console.WriteLine(key + " : " + values[key]);
        }

        foreach (var brush in brushes)
        {
            foreach (var plane in brush.planes)
            {
                plane.Print();
            }
            break;
        }*/

        
    }

    public string Output()
    {
        string output = "";
        if (values.ContainsKey("classname") && values["classname"] == "worldspawn")
        {
            output += "world\n";
        }
        else
        {
            output += "entity\n";
        }
        output += "{\n";

        output += "\t\"id\" \"" + Map2Vmf.currentId.ToString() + "\"\n";
        Map2Vmf.currentId += 1;

        foreach (var key in values.Keys) {
            if (key != "id")
                output += "\t\"" + key + "\" \"" + values[key] + "\"\n";
        }

        foreach (var brush in brushes) {
            output += brush.Output(1);
        }


        output += "}\n";

        return output;
    }
}

public class Brush
{
    public List<Plane> planes;

    public Brush()
    {
        planes = new List<Plane>();
    }

    public string Output(int height)
    {
        string output = "";
        string padding = "";
        for (int i = 0; i < height; i++) padding += "\t";

        output += padding + "solid\n";
        output += padding + "{\n";
        output += padding + "\t\"id\" \"" + Map2Vmf.currentId.ToString() + "\"\n";
        Map2Vmf.currentId += 1;

        foreach (var plane in planes) {
            output += plane.Output(height+1);
        }

        output += padding + "}\n";

        return output;
    }
}

public class Plane
{
    public string pos1;
    public string pos2;
    public string pos3;
    public string texture;
    public string Xtexture;
    public string Ytexture;
    public string rotation;
    public string Xscale;
    public string Yscale;

    public Plane(string line) {
        Console.WriteLine("Plane: " + line);
        Regex t;
        MatchCollection matches;

        t = new Regex("(\\([\\s\\S]+?\\))");
        matches = t.Matches(line);

        pos1 = matches[0].Value.Replace("( ", "(").Replace(" )", ")");
        pos2 = matches[1].Value.Replace("( ", "(").Replace(" )", ")");
        pos3 = matches[2].Value.Replace("( ", "(").Replace(" )", ")");

        t = new Regex("\\) ([\\S]+?) \\[");
        matches = t.Matches(line);

        texture = matches[0].Groups[1].Value;

        t = new Regex("(\\[[\\s\\S]+?\\])");
        matches = t.Matches(line);

        Xtexture = matches[0].Value.Replace("[ ", "[").Replace(" ]", "]");
        Ytexture = matches[1].Value.Replace("[ ", "[").Replace(" ]", "]");

        var split = line.Split("]")[2].Split(" ");
        rotation = split[1];
        Xscale = split[2];
        Yscale = split[3];


    }

    public string Output(int height)
    {
        string output = "";
        string padding = "";
        for (int i = 0; i < height; i++) padding += "\t";

        output += padding + "side\n";
        output += padding + "{\n";
        output += padding + "\t\"id\" \"" + Map2Vmf.currentId.ToString() + "\"\n";
        Map2Vmf.currentId += 1;
        output += padding + "\t\"plane\" \"" + pos1 + " " + pos2 + " " + pos3 + "\"\n";
        //output += padding + "\t\"smoothing_groups\" \"0\"\n";
        output += padding + "\t\"material\" \"" + texture + "\"\n";
        output += padding + "\t\"uaxis\" \"" + Xtexture + " " + Xscale + "\"\n";
        output += padding + "\t\"vaxis\" \"" + Ytexture + " " + Yscale + "\"\n";
        output += padding + "\t\"lightmapscale\" \"16\"\n";

        output += padding + "}\n";
        return output;
    }
}

public static class Map2Vmf
{
    public static int currentId = 0;

    public static void Help()
    {
        Console.WriteLine("Usage: map2vmf.exe [half-life map file] [Source vmf file]");
        Console.WriteLine("Example: map2vmf.exe crossfire.map crossfire.vfm");
    }

    public static void Main(string[] args)
    {
        if (args.Length != 2) {
            Help();
            return;
        }


        string[] lines = File.ReadAllLines(args[0]);
        int index = 0;

        List<Entity> entities = new List<Entity>();

        while (index < lines.Length-1) {
            entities.Add(new Entity(lines, ref index));
        }

        string output = "";
        foreach (var ent in entities) {
            output += ent.Output();
        }

        File.WriteAllText(args[1], output);


        //Console.WriteLine(index);

        //Console.WriteLine("Hello World");
    }
}