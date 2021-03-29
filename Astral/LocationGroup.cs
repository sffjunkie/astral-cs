using System.Collections.Generic;

#nullable enable

namespace SFFJunkie.Astral
{
    public class LocationGroup
    {
        public string Name { get; set; } = "";
        private readonly Dictionary<string, List<LocationInfo>> _locations = new();

        public LocationGroup(string name)
        {
            Name = name;
        }

        public Dictionary<string, List<LocationInfo>> Locations { get => _locations;  }

        public void Add(LocationInfo location)
        {
            var name = location.Name;
            List<LocationInfo> list;
            if (!_locations.ContainsKey(name))
            {
                list = new List<LocationInfo>();
                _locations[name] = list;
            }
            else
            {
                list = _locations[name];
            }
            list.Add(location);
        }

        /// <summary>
        /// Look up a location name. If the name contains a comma then it assumed to be
        /// a name,region
        /// </summary>
        /// <param name="locationName"></param>
        /// <returns></returns>
        public LocationInfo? Lookup(string locationName)
        {
            string name;
            string region;
            if (locationName.Contains(","))
            {
                var elems = locationName.Split(",");
                name = elems[0];
                region = elems[1];
            }
            else
            {
                name = locationName;
                region = "";
            }

            if (_locations.Count == 0)
            {
                return null;
            }

            return LookupInList(region, _locations[name]);
        }

        /// <summary>
        /// Look for a location in the region specified.
        /// If no region specified or no location with the region can be found
        /// then the first item in the list is returned.
        /// </summary>
        /// <param name="region">The region to look up</param>
        /// <param name="list">The list of locations to look in</param>
        /// <returns></returns>
        private static LocationInfo LookupInList(string region, List<LocationInfo> list)
        {
            if (region == "")
            {
                return list[0];
            }

            LocationInfo? found = null;
            foreach (LocationInfo info in list)
            {
                if (info.Region == region)
                {
                    found = info;
                    break;
                }
            }
            if (found == null)
            {
                return list[0];
            }
            else
            {
                return found;
            }
        }
    }
}
