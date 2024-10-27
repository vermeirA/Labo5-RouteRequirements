using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestellingBL.Model
{
    public class Product
    {
        public Product(int id, string naam, double prijs)
        {
            Id = id;
            Naam = naam;
            Prijs = prijs;
        }

        public int Id { get; set; }
        public string Naam { get; set; }
        public double Prijs { get; set; }
    }
}
