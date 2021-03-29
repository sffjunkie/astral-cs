namespace SFFJunkie.Astral
{
    public interface IGeocoder
    {
        LocationInfo Lookup(string name, string region = "");
    }
}