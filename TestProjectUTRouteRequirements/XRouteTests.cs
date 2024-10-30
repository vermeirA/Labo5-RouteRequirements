using RouteRequirements;
using RouteRequirements.Model;
using RouteRequirements.Exceptions;
using static System.Formats.Asn1.AsnWriter;


namespace TestProjectUTRouteRequirements
{
    public class XRouteTests
    {
        private XRoute _route;
        private RouteFactory _routeFactory;

        public XRouteTests()
        {
            //doordat Xroute enkel in de RouteFactory aangemaakt mag worden, doe ik het op deze manier
            _routeFactory = new RouteFactory();
        }

        private void InitializeRoute()
        {
            var locations = new List<string> { "A", "B", "C", "D", "E" };
            var stops = new List<bool> {true, true, false, false, true};
            var distances = new List<double> {10.0, 15.0, 12.0, 18.0};
            _route = _routeFactory.BuildRoute(locations, stops, distances);
        }

        [Fact]
        public void AddLocation_Should_Add_Location_Correctly()
        {
            InitializeRoute();

            _route.AddLocation("Location1", 10.0, true);

            Assert.Equal(5, _route._routeSegments.Count);
            Assert.Equal("E", _route._routeSegments[4].LocStart.Name);
            Assert.True(_route._routeSegments[4].LocEnd.IsStop);
            Assert.Equal(10.0, _route._routeSegments[4].Distance);           
        }

        [Fact]
        public void AddLocation_Should_Throw_Exception_When_Location_Exists()
        {
            InitializeRoute();

            string location = "A";
            double distance = 10.0;
            bool isStop = true;

            var ex = Assert.Throws<RouteException>(() => _route.AddLocation(location, distance, isStop));
            Assert.Equal("AddLocation - Location already exists.", ex.Message);
        }

        [Fact]
        public void AddLocation_Should_Throw_Exception_When_Location_NotCapital()
        {
            InitializeRoute();

            var ex = Assert.Throws<RouteException>(() => _route.AddLocation("location2", 15.0, false));
            Assert.Equal("The name of the location must start with a capital letter.", ex.Message);
        }

        [Fact]
        public void GetDistance_Should_Return_Correct_TotalDistance()
        {
            InitializeRoute();          

            double distanceTotal = _route.GetDistance();

            Assert.Equal(55.0, distanceTotal);
        }

        [Fact]
        public void GetDistanceBetween_Should_Return_Correct_DistanceBetween()
        {
            InitializeRoute();

            double distanceBetween = _route.GetDistance("B", "D");

            Assert.Equal(27, distanceBetween);
        }

        [Fact]
        public void GetDistanceBetween_Should_Throw_Exception_When_NoLocation()
        {
            InitializeRoute();

            var ex = Assert.Throws<RouteException>(() => _route.GetDistance("C", "F"));
            Assert.Equal("GetDistance - One or more locations do(es) not exist.", ex.Message);
        }

        [Fact]
        public void GetDistanceBetween_Should_Throw_Exception_When_NoLocationSuccession()
        {
            InitializeRoute();

            Assert.Throws<RouteException>(() => _route.GetDistance("C", "A"));
        }

        [Fact]
        public void HasLocation_Should_Return_True_If_Location_Exists()
        {
            InitializeRoute();
            Assert.False(_route.HasLocation("Location"));
            Assert.True(_route.HasLocation("C"));
        }

        [Fact]
        public void HasStop_Should_Return_True_If_IsStop()
        {
            InitializeRoute();

            Assert.True(_route.HasStop("A"));
            Assert.False(_route.HasStop("C"));
        }

        [Fact]
        public void InsertLocation_Should_Insert_Location_Correctly()
        {
            InitializeRoute();

            string locationInsert = "LocationInsert";
            double distanceInsert = 9.0;
            bool isStopInsert = false;

            _route.InsertLocation(locationInsert, distanceInsert, "A", isStopInsert);

            Assert.Equal(distanceInsert, _route._routeSegments[0].Distance);
            Assert.Equal(1, _route._routeSegments[1].Distance);

            Assert.Equal("LocationInsert", _route._routeSegments[0].LocEnd.Name); // Ensure one location was added
            Assert.Equal("LocationInsert", _route._routeSegments[1].LocStart.Name);
        }

        [Fact]
        public void InsertLocation_Should_Throw_Exception_When_LocationNotFound()
        {
            InitializeRoute();

            string locationInsert = "LocationInsert";
            double distanceInsert = 10.0;
            bool isStopInsert = false;

            var ex = Assert.Throws<RouteException>(() => _route.InsertLocation(locationInsert, distanceInsert, "F", isStopInsert));
            Assert.Equal("InsertLocation - The fromLocation is not found.", ex.Message);
        }

        [Fact]
        public void InsertLocation_Should_Throw_Exception_When_DistanceInvalid()
        {
            InitializeRoute();

            string locationInsert = "LocationInsert";
            double distanceInsert = 16.0;
            bool isStopInsert = false;

            var ex = Assert.Throws<RouteException>(() => _route.InsertLocation(locationInsert, distanceInsert, "B", isStopInsert));
            Assert.Equal("InsertLocation - Distance must be smaller than the original distance between fromLocation and the next location.", ex.Message);
        }

        [Fact]
        public void RemoveLocation_Should_Remove_FirstLocation_Correctly()
        {
            InitializeRoute();

            _route.RemoveLocation("A");

            Assert.Equal(3, _route._routeSegments.Count);
            Assert.Equal("B", _route._routeSegments[0].LocStart.Name);
            Assert.Equal("C", _route._routeSegments[0].LocEnd.Name);
            Assert.Equal(15.0, _route._routeSegments[0].Distance);
        }

        [Fact]
        public void RemoveLocation_Should_Remove_LastLocation_Correctly()
        {
            InitializeRoute();

            _route.RemoveLocation("E");

            Assert.Equal(3, _route._routeSegments.Count);
            Assert.Equal("D", _route._routeSegments[2].LocEnd.Name);
        }

        [Fact]
        public void RemoveLocation_Should_Remove_MiddleLocation_Correctly()
        {
            InitializeRoute();

            _route.RemoveLocation("C");

            Assert.Equal(3, _route._routeSegments.Count);
            Assert.Equal("B", _route._routeSegments[1].LocStart.Name);
            Assert.Equal("D", _route._routeSegments[1].LocEnd.Name);
            Assert.Equal(27.0, _route._routeSegments[1].Distance);
        }

        [Fact]
        public void SetDistance_Should_Set_Correctly()
        {
            InitializeRoute();
            _route.SetDistance(15.0, "C", "D");

            Assert.Equal(15.0, _route._routeSegments[2].Distance);
        }

        [Fact]
        public void SetDistance_Should_Except_Correctly()
        {
            InitializeRoute();

            var ex = Assert.Throws<RouteException>(() => _route.SetDistance(15.0, "A", "D"));
            Assert.Equal("SetDistance - Locations are not successive or at least 1 does not exist.", ex.Message);
        }

        [Fact]
        public void ShowFullRoute_Should_Work_Correctly()
        {
            InitializeRoute();
            var (start, fullRoute) = _route.ShowFullRoute();

            Assert.Equal("A", start);
            Assert.Equal(4, fullRoute.Count);
            Assert.Equal(10.0, fullRoute[0].distance);
            Assert.Equal("B", fullRoute[0].location);
            Assert.Equal(15.0, fullRoute[1].distance); 
            Assert.Equal("C", fullRoute[1].location);
            Assert.Equal(12.0, fullRoute[2].distance);
        }

        [Fact]
        public void ShowFullRouteBetween_Should_Work_Correctly()
        {
            InitializeRoute();
            var (start, fullRoute) = _route.ShowFullRoute("B", "E");

            Assert.Equal("B", start);
            Assert.Equal(3, fullRoute.Count);
            Assert.Equal(15, fullRoute[0].distance);
            Assert.Equal("C", fullRoute[0].location);
            Assert.Equal(12, fullRoute[1].distance);
            Assert.Equal("D", fullRoute[1].location);
        }

        [Fact]
        public void ShowFullRouteBetween_Should_Except_Correctly()
        {
            InitializeRoute();

            var ex = Assert.Throws<RouteException>(() => _route.ShowFullRoute("D", "C"));
            Assert.Equal("ShowFullRoute - End location must come after start location.", ex.Message);

            var ex2 = Assert.Throws<RouteException>(() => _route.ShowFullRoute("C", "E"));
            Assert.Equal("ShowRoute - Start and endlocation must be a stoplocation.", ex2.Message);
        }

        [Fact]
        public void ShowLocations_Should_Work_Correctly()
        {
            InitializeRoute();
            List<string> locations = _route.ShowLocations();

            Assert.Equal("A", locations[0]);
            Assert.Equal("B", locations[1]);
            Assert.Equal("C", locations[2]);
            Assert.Equal("D", locations[3]);
        }

        [Fact]
        public void ShowRoute_Should_Work_Correctly()
        {
            InitializeRoute();
            var (start, route) = _route.ShowRoute();

            Assert.Equal("A", start);
            Assert.Equal(2, route.Count);
            Assert.Equal(10, route[0].distance);
            Assert.Equal("B", route[0].location);
            Assert.Equal(45, route[1].distance);
            Assert.Equal("E", route[1].location);
        }

        [Fact]
        public void ShowRouteBetween_Should_Work_Correctly()
        {
            InitializeRoute();
            var (start, route) = _route.ShowRoute("B", "E");

            Assert.Equal("B", start);
            Assert.Single(route);
            Assert.Equal(45.0, route[0].distance);
            Assert.Equal("E", route[0].location);
            
        }

        [Fact]
        public void ShowRouteBetween_Should_Except_Correctly()
        {
            InitializeRoute();

            var ex = Assert.Throws<RouteException>(() => _route.ShowRoute("D", "F"));
            Assert.Equal("ShowRoute - Start or end location does not exist.", ex.Message);

            var ex2 = Assert.Throws<RouteException>(() => _route.ShowRoute("D", "B"));
            Assert.Equal("ShowRoute - End location must come after start location.", ex2.Message);
        }

        [Fact]
        public void ShowStops_Should_Work_Correctly()
        {
            InitializeRoute();
            List<string> stops = _route.ShowStops();

            Assert.Equal(3, stops.Count);
            Assert.Equal("A", stops[0]);
            Assert.Equal("B", stops[1]);
            Assert.Equal("E", stops[2]);
        }

        [Fact]
        public void UpdateLocation_Should_Work_Correctly()
        {
            InitializeRoute();
            _route.UpdateLocation("B", "P", false);

            Assert.Equal("P", _route._routeSegments[0].LocEnd.Name);
            Assert.False(_route._routeSegments[0].LocEnd.IsStop);
            Assert.Equal("P", _route._routeSegments[1].LocStart.Name);

            //edge case 1
            InitializeRoute();
            _route.UpdateLocation("A", "L", true);

            Assert.Equal("L", _route._routeSegments[0].LocStart.Name);

            //edge case 2
            InitializeRoute();
            _route.UpdateLocation("E", "L", true);
            Assert.Equal("L", _route._routeSegments[3].LocEnd.Name);
        }
    }
}   
