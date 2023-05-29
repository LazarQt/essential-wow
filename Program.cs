using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using CsvHelper;
using HtmlAgilityPack;
using PuppeteerSharp;

public class Program
{
    private async static Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var wowheadDungeonGuides = new List<string>()
        {
            "https://www.wowhead.com/guide/dungeons/brackenhide-hollow-strategy",
            "https://www.wowhead.com/guide/mythic-plus-dungeons/freehold-strategy",
            "https://www.wowhead.com/guide/dungeons/halls-of-infusion-strategy",
            "https://www.wowhead.com/guide/mythic-plus-dungeons/neltharions-lair-strategy",
            "https://www.wowhead.com/guide/dungeons/neltharus-strategy",
            "https://www.wowhead.com/guide/dungeons/uldaman-legacy-of-tyr-strategy",
            "https://www.wowhead.com/guide/mythic-plus-dungeons/underrot-strategy",
            "https://www.wowhead.com/guide/mythic-plus-dungeons/vortex-pinnacle-strategy"
        };

        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

        var options = new LaunchOptions { Headless = true };
        string pattern = @"spell=(\d+)";
        using (var browser = await Puppeteer.LaunchAsync(options))
        using (var page = await browser.NewPageAsync())
        {
            await page.GoToAsync(
                "https://www.wowhead.com/guide/dungeons/brackenhide-hollow-strategy"
            );
            string html = await page.GetContentAsync();
            HtmlDocument guideDoc = new HtmlDocument();
            guideDoc.LoadHtml(html);

            var xxx = guideDoc.DocumentNode.SelectNodes("//*[@class='tabbed-contents']");
            foreach (var node in xxx)
            {
                Console.WriteLine("node found");
                // Console.WriteLine(node.InnerText);
                var links = node.Descendants("a")
                    .Where(a => a.GetAttributeValue("href", "").Contains("wowhead.com/spell="))
                    .Select(a =>
                    {
                        string hrefValue = a.GetAttributeValue("href", "");
                        Match match = Regex.Match(hrefValue, pattern);
                        if (match.Success)
                        {
                            return match.Groups[1].Value;
                        }
                        return null;
                    })
                    .Where(number => number != null);

                // Output the extracted links
                foreach (var link in links)
                {
                    //  Console.WriteLine(link);
                }
            }
            // Process the downloaded HTML here
            //  Console.WriteLine(html);
        }
        using var client = new HttpClient();

        foreach (var url in wowheadDungeonGuides)
        {
            var guideHtml = await client.GetStringAsync(url);
            HtmlDocument guideDoc = new HtmlDocument();
            guideDoc.LoadHtml(guideHtml);

            var xxx = guideDoc.DocumentNode.SelectNodes("//*[@class='tabbed-contents']");
            foreach (var node in xxx)
            {
                Console.WriteLine(node.InnerText);
            }
        }

        var content = await client.GetStringAsync("https://www.wowhead.com/spell=374073");

        Console.WriteLine(content);

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(content);

        // Use XPath to find the spell name
        HtmlNode nameNode = doc.DocumentNode.SelectSingleNode("//*[@class='whtt-name']");
        Console.WriteLine(nameNode.InnerText);

        HtmlNode tableNode = doc.DocumentNode.SelectSingleNode("//table[@id='spelldetails']");

        // Extract the table rows
        HtmlNodeCollection rows = tableNode.SelectNodes(".//tr");

        // Create a dictionary to store the table data
        Dictionary<string, string> spellData = new Dictionary<string, string>();

        // Iterate over the rows and extract the key-value pairs
        foreach (HtmlNode row in rows)
        {
            HtmlNodeCollection cells = row.SelectNodes(".//th|td");
            if (cells != null && cells.Count == 2)
            {
                string key = cells[0].InnerText.Trim();
                string value = cells[1].InnerText.Trim();
                spellData[key] = value;
            }
        }

        var o = new SpellDetails(
            spellData["Duration"],
            spellData["School"],
            spellData["Mechanic"],
            spellData["Dispel type"],
            spellData["GCD category"],
            spellData["Cost"],
            spellData["Range"],
            spellData["Cast time"],
            spellData["Cooldown"],
            spellData["GCD"],
            spellData["Effect"],
            spellData["Flags"]
        );

        // Display the spell data
        foreach (var kvp in spellData)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }

        //string json = JsonSerializer.Serialize(spellData);
        // SpellDetails spell = JsonSerializer.Deserialize<SpellDetails>(spellData);

        var list = new List<Row>();

        using (var reader = new StreamReader("s2.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            // Use the ReadRecords method to read all the rows in the CSV file
            var records = csv.GetRecords<Row>();
            list.AddRange(records);
        }
        var grp = list.GroupBy(x => x.SpellCategory).ToDictionary(t => t.Key, t => t.Count());
        var x = grp.Where(i => i.Value <= 3);
        foreach (var i in x)
        {
            Console.WriteLine($"are you sure about {i}");
        }
    }
}

public record Row(
    string SpellName,
    string SpellId,
    string SpellCategory,
    string Blank,
    string NeedsInterrupt,
    string Dungeon
);

public record SpellDetails(
    string Duration,
    string School,
    string Mechanic,
    string DispelType,
    string GCDCategory,
    string Cost,
    string Range,
    string CastTime,
    string Cooldown,
    string GCD,
    string Effect,
    string Flags
);
