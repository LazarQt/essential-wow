using PuppeteerSharp;
using System.Text.Json;
using System.Text.RegularExpressions;

var allFiles = Directory.GetFiles(@"C:\dev\essential-wow\wow-tools\wow-tools\c");
var classes = allFiles.Select(f => Path.GetFileName(f).Split("-")[0]).Distinct();
foreach (var c in classes)
{
    var total = new Dictionary<string, int>();
    var files = allFiles.Where(f => Path.GetFileName(f).StartsWith(c));
    foreach (var f in files)
    {
        var u = new List<KeyValuePair<string, int>>();
        var content = JsonSerializer.Deserialize<List<KeyValuePair<string, int>>>(
            File.ReadAllText(f)
        );
        Console.WriteLine(f);
        var ordered = content.OrderByDescending(f => f.Value);
        foreach (var o in ordered)
        {
            if (total.ContainsKey(o.Key))
            {
                total[o.Key] += o.Value;
            }
            else
            {
                total.Add(o.Key, o.Value);
            }
            Console.WriteLine(o.Key);
        }
    }
    var orderedClass = total.OrderByDescending(f => f.Value);
    Console.WriteLine(c);
    foreach (var o in orderedClass)
    {
        Console.WriteLine(o.Key);
    }
    Console.WriteLine();
}

// old

var raidBase =
    "https://www.warcraftlogs.com/zone/rankings/33#boss={boss}&class={class}&spec={spec}&region=2&subregion=2&difficulty=4";
var dungeonBase =
    "https://www.warcraftlogs.com/zone/rankings/34#boss={boss}&region=2&subregion=2&class={class}&spec={spec}&leaderboards=1";
var raidBosses = new int[] { 2688, 2687, 2693, 2682, 2680, 2689, 2683, 2684 };
var dungeonBosses = new int[] { 12451, 12520, 12519, 12527, 61458, 61754, 61841, 10657 };

var dict = new Dictionary<string, string[]>()
{
    //{ "Death Knight", new string[] { "Frost", "Unholy" } },
    //{ "Demon Hunter", new string[] { "Havoc", "Vengeance" } },
    // { "Druid", new string[] { "Balance", "Feral", "Guardian", "Restoration" } },
    // { "Evoker", new string[] { "Devastation", "Preservation" } },
    // { "Hunter", new string[] { "Beast Mastery", "Marksmanship", "Survival" } },
    //{ "Mage", new string[] { "Arcane", "Fire", "Frost" } },
    // { "Monk", new string[] { "Brewmaster", "Mistweaver", "Windwalker" } },
    //{ "Paladin", new string[] { "Holy", "Protection", "Retribution" } },
    //{ "Priest", new string[] { "Discipline", "Holy", "Shadow" } },
    // { "Rogue", new string[] { "Assassination", "Outlaw", "Subtlety" } },
    { "Shaman", new string[] { "Elemental" } },
    // { "Warlock", new string[] { "Affliction", "Demonology", "Destruction" } },
    // { "Warrior", new string[] { "Arms", "Fury", "Protection" } }
};

foreach (var wowclass in dict)
{
    // var classCasts = new Dictionary<string, int>();
    foreach (var spec in wowclass.Value)
    {
        var specCasts = new Dictionary<string, int>();
        foreach (var raidBoss in raidBosses)
        {
            var encounterCasts = await GetEncounters(raidBase, raidBoss, wowclass.Key, spec);

            Merge(specCasts, encounterCasts);
            // Merge(classCasts, encounterCasts);
        }
        foreach (var dungeonBoss in dungeonBosses)
        {
            var encounterCasts = await GetEncounters(dungeonBase, dungeonBoss, wowclass.Key, spec);

            Merge(specCasts, encounterCasts);
            //Merge(classCasts, encounterCasts);
        }
        var specCastsOrdered = specCasts.OrderByDescending(c => c.Value);
        File.WriteAllText(
            $"{wowclass.Key}-{spec}.json",
            JsonSerializer.Serialize(specCastsOrdered)
        );
    }
    //var classCastsOrdered = classCasts.OrderByDescending(c => c.Value);
    // foreach (var e in classCastsOrdered)
    //{
    //    Console.WriteLine(e.Key);
    //}
    Console.WriteLine("finished  " + wowclass.Key);
}

void Merge(Dictionary<string, int> main, Dictionary<string, int> other)
{
    foreach (var c in other)
    {
        if (main.ContainsKey(c.Key))
        {
            main[c.Key] += c.Value;
        }
        else
        {
            main.Add(c.Key, c.Value);
        }
    }
}

async Task<Dictionary<string, int>> GetEncounters(
    string raidBase,
    int raidBoss,
    string wowclass,
    string spec
)
{
    while (true)
    {
        try
        {
            var baseRequest = raidBase
                .Replace("{boss}", raidBoss.ToString())
                .Replace("{class}", wowclass)
                .Replace("{spec}", spec)
                .Replace(" ", "");

            using var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions()
                {
                    Headless = true,
                    Timeout = 60000,
                    ExecutablePath = @"C:\Users\Jerry\AppData\Local\Chromium\Application\chrome.exe"
                }
            );
            using var page = await browser.NewPageAsync();
            Console.WriteLine("requesting " + baseRequest);
            await page.GoToAsync(baseRequest);
            await page.WaitForSelectorAsync(".rank.artifact.sorting_1");
            var actorSelection =
                @"Array.from(document.querySelectorAll('.main-table-link.main-table-player."
                + wowclass.Replace(" ", "")
                + "')).map(a => a.href +'|' +a.innerHTML)";
            var actorArray = (await page.EvaluateExpressionAsync<string[]>(actorSelection)).First(
                u => u.Contains("type=damage-done")
            );
            var actorSplit = actorArray.Split("|");
            var actorUrl = actorSplit[0];
            var actorName = actorSplit[1];
            await page.GoToAsync(actorUrl);
            await page.WaitForSelectorAsync(".dataTables_wrapper.dt-jqueryui");
            var actorIdSelection =
                "Array.from(document.querySelectorAll('a')).filter(function(element) {   return element.innerText === '"
                + actorName
                + "'; }).map(a => a.getAttribute('onclick'))";
            var nameStr = (await page.EvaluateExpressionAsync<string[]>(actorIdSelection))[
                0
            ].ToLower();
            var pos = nameStr.IndexOf("setfiltersource(");
            var charName = nameStr.Substring(pos + 1, nameStr.IndexOf(",") - pos - 1);

            charName = Regex.Match(charName, @"\d+").Value;
            var actorCasts = (actorSplit[0] + "&source=" + charName).Replace(
                "damage-done",
                "casts"
            );

            await page.GoToAsync(actorCasts);
            Thread.Sleep(5000);
            await page.WaitForSelectorAsync(".report-amount-percent");
            await page.WaitForSelectorAsync(".all.sorting.ui-state-default");
            var castsTables = await page.EvaluateExpressionAsync<string[]>(
                @"Array.from(document.querySelectorAll('.summary-table.report.dataTable')).map(a => a.innerHTML)"
            );
            var castsTable = castsTables[0];

            string castsPattern = "id=\"ability(.*?)\\/span>(.*?)\\$";
            MatchCollection rowMatches = Regex.Matches(
                castsTable,
                castsPattern,
                RegexOptions.Singleline
            );
            var encounterCasts = new Dictionary<string, int>();
            foreach (Match rowMatch in rowMatches.Cast<Match>())
            {
                var ab = rowMatch.Groups[1].Value;
                ab = ab.Substring(ab.IndexOf(">") + 1);
                ab = ab.Remove(ab.Length - 1);
                var cts = rowMatch.Groups[2].Value;
                cts = cts.Substring(cts.LastIndexOf(">") + 1);
                if (encounterCasts.ContainsKey(ab))
                {
                    continue;
                }
                encounterCasts.Add(ab, Convert.ToInt32(cts));
            }
            return await Task.FromResult(encounterCasts);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            if (e.Source is not null && !e.Source.Contains("PuppeteerSharp"))
            {
                Console.WriteLine("CRITCAL ISSUE");
                return await Task.FromResult(new Dictionary<string, int>());
            }
        }
    }
}
