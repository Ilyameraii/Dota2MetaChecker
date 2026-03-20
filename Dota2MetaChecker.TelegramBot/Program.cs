using Services.Stratz;

var stratzHeroDataOrchestrator = new StratzHeroDataOrchestrator(
    new StratzApiService(
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJTdWJqZWN0IjoiYTEyN2I2NGEtOGZkYS00OWI4LTljYTQtOWE4N2NiMDRhMWZiIiwiU3RlYW1JZCI6IjQwMDc0OTg3NSIsIkFQSVVzZXIiOiJ0cnVlIiwibmJmIjoxNzYxOTM3MjAwLCJleHAiOjE3OTM0NzMyMDAsImlhdCI6MTc2MTkzNzIwMCwiaXNzIjoiaHR0cHM6Ly9hcGkuc3RyYXR6LmNvbSJ9.mANblEuwrQqxRxFmXClJBoLo34PkyUZCTi6YUjRgLYI"),
    new StratzHeroParser());

var heroesList = await stratzHeroDataOrchestrator.GetHeroesAsync();

foreach (var hero in heroesList)
{
    Console.WriteLine($"{hero.Name} - {100.0 * hero.WinCount / hero.MatchCount:F2} % winrate, rank - {hero.Rank}, position - {hero.Role}");
}