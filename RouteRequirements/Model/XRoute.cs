using RouteRequirements.Exceptions;
using RouteRequirements.Interfaces;
using System.Net;
using System.Reflection.Metadata;

namespace RouteRequirements.Model
{
    public class XRoute : IRoute
    {
        public List<RouteSegment> _routeSegments;

        internal XRoute() //we zetten deze method internal omdat ze nooit van buitenaf mag aangemaakt worden! (enkel vanuit RouteFactory)
        {
            _routeSegments = new List<RouteSegment>();
        }

        public void AddLocation(string location, double distance, bool isStop)
        {
            if (_routeSegments.Any(loc => loc.LocStart.Name == location)) //kijken of de locatie reeds bestaat, geef error indien true
            {
                throw new RouteException("AddLocation - Location already exists.");
            }

            LocationSegment previousLocation = _routeSegments[_routeSegments.Count - 1].LocEnd;
            LocationSegment addLocation = new LocationSegment(location, isStop);

            if (!string.IsNullOrEmpty(previousLocation.Name))
            {
                previousLocation.Name = char.ToUpper(previousLocation.Name[0]) + previousLocation.Name.Substring(1);
            }

            if (!string.IsNullOrEmpty(addLocation.Name))
            {
                addLocation.Name = char.ToUpper(addLocation.Name[0]) + addLocation.Name.Substring(1);
            }

            _routeSegments.Add(new RouteSegment(previousLocation, addLocation, distance));
        }

        public double GetDistance()
        {
            double distance = _routeSegments.Sum(segment => segment.Distance);
            return distance;
        }

        public double GetDistance(string startLocation, string endLocation)
        {
            // checken of startlocatie voor eindlocatie ligt
            int indexStart = _routeSegments.FindIndex(loc => loc.LocStart.Name == startLocation);
            int indexEnd = _routeSegments.FindIndex(loc => loc.LocEnd.Name == endLocation);
            int indexFirst = _routeSegments.FindIndex(loc => loc.LocStart.Name == endLocation); //voor edge case
            double distance = 0;

            //check of ze bestaan
            if (indexStart == -1 || indexFirst == -1) 
            {
                throw new RouteException("GetDistance - One or more locations do(es) not exist.");
            }

            if (indexEnd < indexStart) //indexen mogen wel gelijk zijn.
            {
                throw new RouteException("GetDistance - Locations not succesive.");

            } else if (indexEnd == indexStart)
            {
                distance = _routeSegments[indexStart].Distance;

            } else
            {
                for (int i = indexStart; i <= indexEnd; i++)
                {
                    distance += _routeSegments[i].Distance;
                }
            }

            return distance;
        }

        public bool HasLocation(string location)
        {
            bool hasLocation = false;

            foreach (var segment in _routeSegments)
            {
                if (segment.LocStart.Name == location)
                {
                    hasLocation = true;
                }
            }

            return hasLocation;
        }

        public bool HasStop(string location)
        {
            int locIndex = _routeSegments.FindIndex(loc => loc.LocStart.Name == location);

            //lijkt magie, maar dit wilt zeggen: als locIndex gevonden is (!= -1) en de locatie is een stop (.IsStop == true), return true
            return locIndex != -1 && _routeSegments[locIndex].LocStart.IsStop;
        }

        public void InsertLocation(string location, double distance, string fromLocation, bool isStop)
        {
            int fromIndex = _routeSegments.FindIndex(segment => segment.LocStart.Name == fromLocation);

            if (fromIndex < 0)
            {
                throw new RouteException("InsertLocation - The fromLocation is not found.");
            }

            double currentSegmentDistance = _routeSegments[fromIndex].Distance;
            double updatedDistanceNext = currentSegmentDistance - distance;

            if (updatedDistanceNext <= 0)
            {
                throw new RouteException("InsertLocation - Distance must be smaller than the original distance between fromLocation and the next location.");
            }

            //nieuw segment maken en updaten
            LocationSegment insertStartLoc = new LocationSegment(location, isStop);
            LocationSegment insertEndLoc = new LocationSegment(_routeSegments[fromIndex].LocEnd.Name, _routeSegments[fromIndex].LocEnd.IsStop);
            RouteSegment insertSegment = new RouteSegment(insertStartLoc, insertEndLoc, updatedDistanceNext);
            _routeSegments.Insert(fromIndex + 1, insertSegment);

            //bestaand segment updaten
            _routeSegments[fromIndex].LocEnd = insertStartLoc;
            _routeSegments[fromIndex].Distance = distance;

        }

        public void RemoveLocation(string location)
        {
            // als het de eerste locatie
            if (_routeSegments[0].LocStart.Name == location)
            {
                _routeSegments.RemoveAt(0);
                return;
            }

            int indexRemove = _routeSegments.FindIndex(segment => segment.LocEnd.Name == location);

            if (indexRemove == -1)
                throw new RouteException("RemoveLocation - Location does not exist.");

            if (indexRemove > 0 && indexRemove < _routeSegments.Count - 1) // als het een midlocatie is (niet start of eind)
            {
                double newDistance =
                    _routeSegments[indexRemove].Distance +
                    _routeSegments[indexRemove + 1].Distance;

                _routeSegments[indexRemove] = new RouteSegment(_routeSegments[indexRemove].LocStart, _routeSegments[indexRemove + 1].LocEnd, newDistance);

                _routeSegments.RemoveAt(indexRemove + 1);
            }

            else if (indexRemove == _routeSegments.Count - 1) //als het de laatste locatie is
            {
                _routeSegments.RemoveAt(indexRemove);
            }

            Console.WriteLine($"{location} was deleted. Distance has been adjusted accordingly.");
        }

        public void SetDistance(double distance, string location1, string location2)
        {
            // de afstand tussen 2 locaties aanpassen, moeten controleren dat de locaties opeenvolgend zijn. (indexen checken) 
            int indexLoc1 = _routeSegments.FindIndex(segment => segment.LocStart.Name == location1);
            int indexLoc2 = _routeSegments.FindIndex(segment => segment.LocEnd.Name == location2);

            if (indexLoc1 != -1 && indexLoc1 == indexLoc2)
            {
                _routeSegments[indexLoc1].Distance = distance;
            }
            else
            {
                throw new RouteException("SetDistance - Locations are not successive or at least 1 does not exist.");
            }
        }

        public (string start, List<(double distance, string location)>) ShowFullRoute()
        {
            string startLocation = _routeSegments[0].LocStart.Name;

            List<(double distance, string location)> fullRoute = new List<(double distance, string location)>();

            for (int i = 0; i < _routeSegments.Count; i++)
            {
                //distance van segment ophalen en toevoegen samen met de volgende locatienaam 
                double distance = _routeSegments[i].Distance;
                fullRoute.Add((distance, _routeSegments[i].LocEnd.Name));
            }

            return (startLocation, fullRoute);
        }

        public (string start, List<(double distance, string location)>) ShowFullRoute(string startLocation, string endLocation)
        {
            
            int startIndex = _routeSegments.FindIndex(segment => segment.LocStart.Name == startLocation);
            int endIndex = _routeSegments.FindIndex(segment => segment.LocEnd.Name == endLocation);

            if (startIndex == -1 || endIndex == -1)
            {
                throw new RouteException("ShowFullRoute - Start or end location does not exist.");
            }

            if (endIndex < startIndex)
            {
                throw new RouteException("ShowFullRoute - End location must come after start location.");
            }

            List<(double distance, string location)> fullRoute = new List<(double distance, string location)>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                double distance = _routeSegments[i].Distance;
                fullRoute.Add((distance, _routeSegments[i].LocEnd.Name));
            }

            return (startLocation, fullRoute);
        }

        public List<string> ShowLocations()
        {
            HashSet<string> uniqueLocations = new HashSet<string>(); //HashSet zodat we de dubbele namen niet meekrijgen!

            foreach (var segment in _routeSegments)
            {
                uniqueLocations.Add(segment.LocStart.Name);
                uniqueLocations.Add(segment.LocEnd.Name);
            }

            return uniqueLocations.ToList();
        }

        public (string start, List<(double distance, string location)>) ShowRoute()
        {
            // De volledige route tonen met enkel stoplocaties (IsStop)
            // startLocatie is sowieso een Stop

            string startLocation = _routeSegments[0].LocStart.Name;
            double accumulatedDistance = 0;

            List<(double distance, string location)> stopLocations = new List<(double distance, string location)>();

            //enkel stoplocaties tonen, maar eerst de distance van de niet-stoplocaties bij de (respectievelijke) vorige wel-stop tellen
            for (int i = 0; i < _routeSegments.Count; i++)
            {
                accumulatedDistance += _routeSegments[i].Distance;

                if (_routeSegments[i].LocEnd.IsStop)
                {
                    stopLocations.Add((accumulatedDistance, _routeSegments[i].LocEnd.Name));
                    accumulatedDistance = 0;
                }
            }
           
            return (startLocation, stopLocations);
        }

        public (string start, List<(double distance, string location)>) ShowRoute(string startLocation, string endLocation)
        {
            int startIndex = _routeSegments.FindIndex(segment => segment.LocStart.Name == startLocation);
            int endIndex = _routeSegments.FindIndex(segment => segment.LocEnd.Name == endLocation);

            if (startIndex == -1 || endIndex == -1)
            {
                throw new RouteException("ShowRoute - Start or end location does not exist.");
            }

            if (endIndex < startIndex)
            {
                throw new RouteException("ShowRoute - End location must come after start location.");
            }

            List<(double distance, string location)> stopLocations = new List<(double distance, string location)>();

            double accumulatedDistance = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                accumulatedDistance += _routeSegments[i].Distance;

                if (_routeSegments[i].LocEnd.IsStop)
                {
                    stopLocations.Add((accumulatedDistance, _routeSegments[i].LocEnd.Name));
                    accumulatedDistance = 0;
                }
            }

            if (stopLocations.Count == 0)
            {
                Console.WriteLine("ShowRoute - No stop locations found between the specified start and end locations.");
            }

            return (startLocation, stopLocations);
        }

        public List<string> ShowStops()
        {
            HashSet<string> stops = new HashSet<string>();

            //we willen de naam van de stops weergeven (alle stops binnen route)
            for (int i = 0; i < _routeSegments.Count; i++)
            {
                if (_routeSegments[i].LocStart.IsStop)
                {
                    stops.Add(_routeSegments[i].LocStart.Name);
                }

                if (_routeSegments[i].LocEnd.IsStop)
                {
                    stops.Add(_routeSegments[i].LocEnd.Name);
                }
            }

            return stops.ToList();
        }

        public void UpdateLocation(string location, string newName, bool isStop)
        {
            //locationsegment object voor segment aan te passen
            LocationSegment updateLocation = new LocationSegment(newName, isStop);

            // als het de eerste locatie
            if (_routeSegments[0].LocStart.Name == location)
            {
                _routeSegments[0].LocStart = updateLocation;
                return;
            }

            int indexLoc = _routeSegments.FindIndex(segment => segment.LocEnd.Name == location);

            //checks
            if (indexLoc == -1) { throw new RouteException("UpdateLocation - The given location was not found."); }
            if (_routeSegments.Any(segment => segment.LocEnd.Name == newName)) { throw new RouteException("UpdateLocation - The new location name already exists."); }

            //beide segmenten aanpassen (locatie zal bestaan in 2 segmenten)
            _routeSegments[indexLoc].LocEnd = updateLocation;

            if (indexLoc < _routeSegments.Count - 1) //als het niet de laatste locatie is
            {
                _routeSegments[indexLoc + 1].LocStart = updateLocation;
            }
        }
    }
}
