public record Zone(string Name, List<Groups> Groups);

public record Groups(string Location, List<Effect> Effects);

public record Effect(string Type, string Id, string What);
