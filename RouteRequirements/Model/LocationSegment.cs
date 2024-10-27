using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteRequirements.Model
{
    public class LocationSegment : Location
    {
        public bool IsStop { get; set; }

        public LocationSegment(string name,  bool isStop) : base(name)
        {
            IsStop = isStop;
        }
    }
}
