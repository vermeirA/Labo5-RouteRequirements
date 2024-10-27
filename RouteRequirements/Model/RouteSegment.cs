using RouteRequirements.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteRequirements.Model
{
    public class RouteSegment
    {
		private LocationSegment _locStart;

		public LocationSegment LocStart
		{
			get { return _locStart; }
			set 
			{
				if (string.IsNullOrEmpty(value.Name)) throw new RouteException("Startlocation cannot be empty.");
				_locStart = value; 
			}
		}

		private LocationSegment _locEnd;

		public LocationSegment LocEnd
		{
			get { return _locEnd; }
			set 
			{
                if (string.IsNullOrEmpty(value.Name)) throw new RouteException("Endlocation cannot be empty.");
                _locEnd = value; 
			}
		}

		private double _distance;
  
        public double Distance
		{
			get { return _distance; }
			set
			{
				if (value < 0) throw new RouteException("Distance - Distance can not be negative.");

				_distance = value;
			}
		}

        public RouteSegment(LocationSegment locStart, LocationSegment locEnd, double distance)
        {
            LocStart = locStart;         
            LocEnd = locEnd;           
            Distance = distance;
        }
    }
}
