using HtmlAgilityPack;
using System;
using System.Net.Http;
using SixLabors.ImageSharp;
using System.Text.Json;
using System.Text.Json.Serialization;
using essential_wow;

var folders = new string[] { "arcane", "fire","frost" };

foreach (var f in folders)
{
    Console.WriteLine(f);
    var dict = new Dictionary<string, int>();
    foreach (var files in Directory.GetFiles($"f/{f}"))
    {
        var csv = File.ReadAllLines(files).Skip(1).Select(x => x.Replace("\"", "").Split(","));
        foreach (var e in csv)
        {
            var a = e[0];
            var b = Convert.ToInt32(e[1].Split("$")[0]);
            //Console.WriteLine($"{}: {}");
            if (dict.ContainsKey(a))
            {
                dict[a] += b;
            }
            else
            {
                dict.Add(a, b);
            }
        }
    }

    var x = dict.OrderByDescending(d => d.Value);
    foreach (var e in x)
    {
        Console.WriteLine($"{e.Key}");
    }
}

return;
var db = new Dfs2Context();

var html = "";

foreach (var dung in db.Dungeons)
{
    var u = 0;
    foreach (var maps in dung.Maps.ToList())
    {
        u++;

        html +=
            $"<div class=\"container\" style=\"color:red;width:1013px;height:676px;margin-bottom:2em;\">";
        html += $"<img src=\"assets/{dung.Name}-{u}.jpg\" />";
        foreach (var g in maps.Blocks.ToList())
        {
            //var t = 100.00d / (double)1013 * Convert.ToDouble(g.LocX);
            // var w = 100.00d / (double)676 * Convert.ToDouble(g.LocY);

            html +=
                $"<div  style=\" box-shadow: 5px 10px grey;border:1px solid black; background-color:rgba(25,25,25,0.5); position:absolute; top:{g.LocY}%;left:{g.LocX}%;\">";
            html += $"<span style=\"text-decoration:underline;\">{g.Name}</span></br>";
            var grped = g.Entries.GroupBy(x => x.Action);
            foreach (var xd in grped)
            {
                html +=
                    $"<img style=\"width:16px;height:16px\" src=\"assets/{xd.Key}.jpg\" /> <span style=\"text-decoration:underline;\">{xd.Key}</span><br>";
                foreach (var i in xd)
                {
                    html +=
                        $"<a href=\"https://www.wowhead.com/{i.Type.ToLower()}={i.ExtId}\"></a><br>";
                }
                html += "<div class=\"spc\"></div>";
            }
            html += "   </div>";
        }
        html += " </div>";
    }
}
var newhtml = File.ReadAllText("assets/temp.html");
newhtml = newhtml.Replace("{replace}", html);
File.WriteAllText("omgtest.html", newhtml);
