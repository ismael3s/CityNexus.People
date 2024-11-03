namespace CityNexus.People.Domain.VO;

public sealed record Name(string Value)
{
    public override string ToString() => this.Value;

    public static Name From(string value)
    {
        var results = value.Split(" ").Select(n => $"{n[0].ToString().ToUpper()}{n[1..]}");
        return new Name(string.Join(' ', results));
    }
};
