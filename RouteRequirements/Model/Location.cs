using RouteRequirements.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteRequirements.Model
{
    public class Location
    {
		private string _name;

		public string Name
		{
			get { return _name; }
			set 
			{ 
				if (string.IsNullOrWhiteSpace(value)) 
				{
					throw new RouteException("The name of the location cannot be blank or null.");
				}

				if (!char.IsUpper(value[0]))
				{
					throw new RouteException("The name of the location must start with a capital letter.");
				}

				_name = value;
			}
		}

        public Location(string naam)
        {
            Name = naam;                      
        }

    }
}
