namespace SFFJunkie.Astral
{
    public class LocalGeocoder : IGeocoder
    {
        private readonly LocalDatabase _db = new();
        public LocationInfo Lookup(string name, string region = "")
        {
            return _db.Lookup(name);
        }
    }
}
