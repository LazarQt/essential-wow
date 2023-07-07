using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

var lines = File.ReadAllLines(@"C:\dev\New Text Document.txt");
var wowclass = "";

foreach (var e in lines)
{
    if (e.StartsWith("Class"))
    {
        wowclass = e.Split(":")[1];
    }
    else
    {
        DoThing(wowclass, e);
    }
}

static void DoThing(string wowclass, string w)
{
    using (var client = new HttpClient())
    {
        w = w.Replace(" ", "+");
        var response = client
            .GetAsync(
                "https://www.wowdb.com/spells/class-abilities/"
                    + wowclass.ToLower().Replace(" ", "-")
                    + "?filter-search="
                    + w
            )
            .GetAwaiter()
            .GetResult();
        if (response.IsSuccessStatusCode)
        {
            var responseContent = response.Content;
            var x = responseContent.ReadAsStringAsync().GetAwaiter().GetResult();

            string pattern =
                @"(http|ftp|https):\/\/([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:\/~+#-]*[\w@?^=%&\/~+#-]).jpg";

            RegexOptions options = RegexOptions.Singleline;
            Console.WriteLine(w);
            foreach (
                Match m in Regex.Matches(
                    Between(
                        x,
                        "<img class=\"icon-36 \" data-cfsrc=\"",
                        "\" width=\"36\" height=\"36\" alt=\"\" style=\"display:none;visibility:hidden;\" /><noscript><im"
                    ),
                    pattern,
                    options
                )
            )
            {
                Console.WriteLine(m.Value);
                var p = @"c:\dev\" + wowclass + "\\";
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
                w = w.Replace("+", "");
                var ws = w.Split("/");
                foreach (var wss in ws)
                {
                    var finalFilePath = p + wss;
                    if (ws.Length > 1)
                    {
                        finalFilePath += "_PART";
                    }

                    finalFilePath += ".jpg";
                    if (File.Exists(finalFilePath))
                    {
                        continue;
                    }

                    Dl(m.Value, finalFilePath);
                }

                break;
            }
        }
    }
}

static void Dl(string url, string path)
{
    using (WebClient wclient = new WebClient())
    {
        wclient.DownloadFile(new Uri(url), path);
    }
}

static string Between(string str, string f, string l)
{
    int startPoint = str.IndexOf(f) + f.Length;
    int endPOint = str.LastIndexOf(l);
    if (endPOint <= 0)
    {
        return "";
    }

    String result = str.Substring(startPoint, endPOint - startPoint);
    return result;
}
