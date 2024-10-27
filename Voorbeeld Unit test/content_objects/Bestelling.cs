using BestellingBL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestellingBL.Model
{
    public class Bestelling
    {
        private int id;
        public int Id
        {
            get { return id; }
            set
            {
                if (value <= 0) throw new BestellingException("id<0");
                id = value;
            }
        }
        private Klant klant;
        public Klant Klant
        {
            get { return klant; }
            set { if (value == null) throw new BestellingException("klant is niet correct"); klant = value; }
        }
        private Dictionary<Product, int> producten = new();

        public Bestelling(int id, Klant klant)
        {
            Id = id;
            Klant = klant;
        }

        public List<(Product product,int aantal)> Producten()=>producten.Select(x=>(x.Key,x.Value)).ToList();
        public void VoegProductToe(Product product,int aantal)
        {
            if ((product == null) || (aantal <= 0)) throw new BestellingException("VoegProductToe - parameters niet correct");
            if (producten.ContainsKey(product)) producten[product] += aantal;
            else producten.Add(product, aantal);
        }
        public void VerwijderProduct(Product product, int aantal)
        {
            if ((product == null) || (aantal <= 0)) throw new BestellingException("VerwijderProduct - parameters niet geldig");
            if (!producten.ContainsKey(product) || producten[product]<aantal) throw new BestellingException("VerwijderProduct - parameters niet correct");
            else if (producten[product]>aantal)
                producten[product] -= aantal;
            else producten.Remove(product);
        }
        public double Prijs()
        {
            double prijs = 0;
            foreach(KeyValuePair<Product,int> kvp in producten) prijs += (kvp.Value*kvp.Key.Prijs);
            return prijs;
        }
    }
}
