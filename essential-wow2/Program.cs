using HtmlAgilityPack;
using System;
using System.Net.Http;
using SixLabors.ImageSharp;

namespace WowheadScraper;

public record Zone(string Name, List<Groups> Groups);

public record Groups(string Location, List<Effect> Effects);

public record Effect(string Type, string Id, string What);

class Program
{
    static void Main(string[] args)
    {
        string filePath = "s2.csv";

        var zones = new List<Zone>();
        var lines = new List<string[]>();

        using (StreamReader sr = new StreamReader(filePath))
        {
            var t0 = "";
            var t1 = "";
            var t5 = "";
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');

                //if (values[0] != "" && !zones.Any(z => z.Name == values[0]))
                //{
                //    zones.Add(new Zone(values[0], new List<Groups>()));
                //}

                //if (values[5] != "")
                //{
                //    var grp = new Groups(values[5],new List<Effect>());
                //    zones
                //}

                if (values[0] == "")
                {
                    values[0] = t0;
                    values[1] = t1;
                    values[5] = t5;
                }
                else
                {
                    t0 = values[0];
                    t1 = values[1];
                    t5 = values[5];
                }
                lines.Add(values);
            }

            //            < div class="container" style="width:1164px;height:776px;">
            //    <img src = "VP-1.jpg" />
            //    < div style="position:relative; top:-20%;left:-79%;" dir="rtl">
            //        <img src = "https://wow.zamimg.com/images/wow/icons/large/spell_nature_purge.jpg" style="width:16px;height:16px" />
            //        <br><a href = "https://www.wowhead.com/spell=410760" ></ a >
            //        < br >< a href="https://www.wowhead.com/spell=410760"></a>
            //        <br><a href = "https://www.wowhead.com/spell=410760" ></ a >
            //    </ div >
            //</ div >

            var groups = lines.GroupBy(l => $"{l[0]}");
            var html = "";
            foreach (var grp in groups)
            {
                using var image = Image.Load("assets/"+grp.Key+".jpg");

                int width = image.Width;
                int height = image.Height;

      

                html += $"<div class=\"container\" style=\"width:{width}px;height:{height}px;\">";
                html += $"<img src=\"assets/{grp.Key}.jpg\" />";
                var z = grp.GroupBy(x => x[5]);
                foreach(var g in z)
                {
                    var cords = g.Key.Split("|");
                    var t = 100.00d / (double)height * Convert.ToDouble(cords[1]);
                    var w = 100.00d / (double)width * Convert.ToDouble(cords[0]);
                    t = Math.Round(t, 0);
                    w = Math.Round(w, 0);
                    html += $"<div  style=\" box-shadow: 5px 10px grey;border:1px solid black; position:absolute; top:{t}%;left:{w}%;\">";
                    var u = g.GroupBy(x => x[4]);
                    foreach(var xd in u)
                    {
                        html += $"{xd.Key}<br>";
                        foreach(var i in xd)
                        {
                            html += $"<a href=\"https://www.wowhead.com/{i[2].ToLower()}={i[3]}\"></a><br>";
                        }
                    }
                    html += "   </div>";
                }
                html += " </div>";
            }
            var newhtml = File.ReadAllText("assets/temp.html");
            newhtml = newhtml.Replace("{replace}", html);
            File.WriteAllText("omgtest.html", newhtml);
        }
    }
}
