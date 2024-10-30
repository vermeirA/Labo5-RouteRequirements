using RouteRequirements.Exceptions;
using RouteRequirements.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace RouteRequirements
{
    public class RouteFactory
    {
        public XRoute BuildRouteFromFile(string fileName)
        {
            List<string> locations = new List<string>();
            List<bool> stops = new List<bool>();
            List<double> distances = new List<double>();

            string logFilePath = "C:\\Users\\Gebruiker\\Desktop\\Graduaat\\SEM3\\Programmeren gevorderd 1\\Labo 5\\RouteRequirements\\Exceptions\\errorlog.log";
            //errorlog leegmaken bij het bouwen van een nieuwe route
            if (File.Exists(logFilePath)) File.WriteAllText(logFilePath, string.Empty);

            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                try
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                    
                        // Regex patterns voor verschillende formats
                        string pattern1 = @"^(.+?),(\d+),(true|false)$"; // Format: Location,Distance,IsStop
                        string pattern2 = @"^(.+?)\((stop|transit)\),(.+?)\((stop|transit)\),(\d+)$"; // Format: StartLocation(stop|transit),EndLocation(stop|transit),Distance

                        Match match1 = Regex.Match(line, pattern1);
                        Match match2 = Regex.Match(line, pattern2);

                        if (match1.Success)
                        {
                            string location = match1.Groups[1].Value;
                            if (string.IsNullOrEmpty(location))
                            {
                                throw new RouteException($"Invalid location format in this line: {line}");
                            }

                            if (!double.TryParse(match1.Groups[2].Value, out double distance))
                            {
                                throw new RouteException($"Invalid distance format in this line: {line}");
                            }

                            if (!bool.TryParse(match1.Groups[3].Value, out bool isStop))
                            {
                                throw new RouteException($"Invalid stop/transit format in this line: {line}");
                            }

                            locations.Add(location);
                            if (distance != 0) //dit is nodig!
                            {
                                distances.Add(distance);
                            }
                            stops.Add(isStop);
                        }
                        else if (match2.Success)
                        {
                            string startLocation = match2.Groups[1].Value;
                            string endLocation = match2.Groups[3].Value;

                            if (string.IsNullOrEmpty(startLocation) || string.IsNullOrEmpty(endLocation))
                            {
                                throw new RouteException($"Invalid location format in this line: {line}");
                            }

                            if (!double.TryParse(match2.Groups[5].Value, out double distance))
                            {
                                throw new RouteException($"Invalid distance format in this line: {line}");
                            }

                            //stops eerste locatie
                            bool isStartStop;
                            
                            switch (match2.Groups[2].Value)
                            {
                                case ("stop"):
                                    isStartStop = true;
                                    break;

                                case ("transit"):
                                    isStartStop = false;
                                    break;
                              
                                default:
                                    throw new RouteException("Unexpected value: " + match2.Groups[2].Value);
                            }

                            //stops tweede locatie
                            bool isEndStop;

                            switch (match2.Groups[4].Value)
                            {
                                case ("stop"):
                                    isEndStop = true;
                                    break;

                                case ("transit"):
                                    isEndStop = false;
                                    break;

                                default:
                                    throw new RouteException("Unexpected value: " + match2.Groups[4].Value);
                            }

                            if (!locations.Contains(startLocation)) 
                            {
                                locations.Add(startLocation);
                                stops.Add(isStartStop);  
                            }

                            distances.Add(distance);

                            if(!locations.Contains(endLocation))
                            {
                                locations.Add(endLocation);
                                stops.Add(isEndStop);
                            }                           
                        }
                        else
                        {
                            File.AppendAllText(logFilePath, $"Warning: Line did not match any pattern - {line}");
                        }
                    
                    }
                }
                catch (RouteException ex)
                {
                    File.AppendAllText(logFilePath, ex.Message);
                }

                XRoute route = BuildRoute(locations, stops, distances);
                return route;
            }
        }

        public XRoute BuildRoute(List<string> locations, List<bool> stops, List<double> distances) 
        {
            string logFilePath = "C:\\Users\\Gebruiker\\Desktop\\Graduaat\\SEM3\\Programmeren gevorderd 1\\Labo 5\\RouteRequirements\\Exceptions\\errorlog.log";
            //errorlog leegmaken bij het bouwen van een nieuwe route
            if (File.Exists(logFilePath)) File.WriteAllText(logFilePath, string.Empty);

            XRoute route = new XRoute();

            try
            {
                // exception als leeg
                if (locations == null || stops == null || distances == null)
                    throw new RouteException("Inputlijsten mogen niet leeg zijn.");

                // Voeg alle locaties toe
                for (int i = 0; i < distances.Count; i++)
                {
                    LocationSegment locStart = new LocationSegment(locations[i], stops[i]);
                    LocationSegment locEnd = new LocationSegment(locations[i + 1], stops[i + 1]);
                    RouteSegment segment = new RouteSegment(locStart, locEnd, distances[i]);
                    route._routeSegments.Add(segment);
                }

            } catch (RouteException ex) 
            {
                File.AppendAllText(logFilePath, ex.Message + Environment.NewLine);
            }

            return route;
        }

        public XRoute ReverseRoute(XRoute route) 
        {
            XRoute routeNew = new XRoute();

            //in segmenten locaties omwisselen, anders klopt de route niet.
            foreach (var segment in route._routeSegments.AsEnumerable().Reverse())
            {
                var reversedSegment = new RouteSegment(
                    new LocationSegment(segment.LocEnd.Name, segment.LocEnd.IsStop),
                    new LocationSegment(segment.LocStart.Name, segment.LocStart.IsStop),
                    segment.Distance
                );

                routeNew._routeSegments.Add(reversedSegment);
            }

            return routeNew;
        }
    }
}

