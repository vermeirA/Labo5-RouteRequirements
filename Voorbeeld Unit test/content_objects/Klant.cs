using BestellingBL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BestellingBL.Model
{
    public class Klant
    {

        public Klant(int id, string naam, string email)
        {
            Id = id;
            Naam = naam;
            Email = email;
        }

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
        private string naam;
        public string Naam
        {
            get { return naam; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new BestellingException("Naam niet correct");
                naam = value;
            }
        }
        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value,@"[A-Za-z]+@[A-Za-z]+\.[A-Za-z]+"))
                    throw new BestellingException("email niet correct");
                email = value;
            }
        }
    }
}
