using BestellingBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectUTBestelling
{
    public class UnitTestBestelling
    {
        //props en ctor checks komen hier normaal nog boven

        private Klant klant;
        private Product productA, productB, productC;

        // maken constructor zodat deze waarden niet telkens herschreven moeten worden
        public UnitTestBestelling()
        {
            klant = new Klant(1, "Jos", "jos@gmail.com");
            productA = new Product(5, "AAA", 10);
            productB = new Product(7, "BBB", 12);
            productC = new Product(6, "CCC", 15);
        }

        //methods checken
        [Fact]
        public void Test_Prijs()
        {
            //ingewikkelder aangezien Bestelling Klant en Product objecten bezit, moeten aangemaakt worden om te testen.           
            Bestelling bestelling = new Bestelling(10, klant);
            bestelling.VoegProductToe(productA, 2);
            bestelling.VoegProductToe(productB, 3);

            Assert.Equal(56, bestelling.Prijs());
        }

        [Fact]
        public void Test_VoegProductToe_Valid()
        {
            Bestelling bestelling = new Bestelling(10, klant);

            bestelling.VoegProductToe(productA, 2);
            //Checken of er 1 product is toegevoegd
            Assert.Equal(1, bestelling.Producten().Count());
            //Checken of het product A is
            Assert.Contains((productA, 2), bestelling.Producten());

            //Ook bij een extra toevoeging checken of de waarde correct weggeschreven wordt
            bestelling.VoegProductToe(productB, 3);
            Assert.Equal(2, bestelling.Producten().Count());
            Assert.Contains((productB, 3), bestelling.Producten());
            //Ook checken of eerste toevoeging blijft bestaan
            Assert.Contains((productA, 2), bestelling.Producten());

            //worden er meerdere producten A toegevoegd, zelfde checks
            bestelling.VoegProductToe(productA, 3);
            Assert.Equal(2, bestelling.Producten().Count());
            Assert.Contains((productB, 3), bestelling.Producten());
            //Ook checken of eerste toevoeging blijft bestaan (en nu 5 units heeft)
            Assert.Contains((productA, 5), bestelling.Producten());
        }
    }
}
