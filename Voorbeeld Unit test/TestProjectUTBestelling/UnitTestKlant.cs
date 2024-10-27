using BestellingBL.Exceptions;
using BestellingBL.Model;
using System.Numerics;

namespace TestProjectUTBestelling
{
    public class UnitTestKlant
    {
        [Theory] // Theory om meerde waarden met dezelfde code te testen
        [InlineData(1)]
        [InlineData(100)] 
        public void Test_Id_Valid(int id) //indien input valid (id meegeven aangezien InlineData gebruikt wordt)
        {
            // Valid klant aanmaken
            Klant klant = new Klant(10, "Jos", "jos@gmail.com");

            //We gaan testen of het Id juist aangepast wordt
            klant.Id = id;
            Assert.Equal(id, klant.Id);
        }

        [Fact] // Fact om 1 waarde te testen (exception is simpel), geen InlineData nodig, niks meegeven aan 'method'
        public void Test_Name_Valid()
        {
            Klant klant = new Klant(10, "Jos", "jos@gmail.com");

            klant.Naam = "Tom";
            Assert.Equal("Tom", klant.Naam);
        }

        [Fact]
        public void Test_Email_Valid()
        {
            Klant klant = new Klant(10, "Jos", "jos@gmail.com");

            klant.Email = "josdheer@gmail.com";
            Assert.Equal("josdheer@gmail.com", klant.Email);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_Id_Invalid(int id) // indien de waarde is invalid
        {
            Klant klant = new Klant(10, "Jos", "jos@gmail.com");

            //NIET Klant.Id = id -> Test zal niet werken!

            //Exceptions opvangen
            //Indien we de message willen vergelijken (geeft exception juiste message?) (anders enkel Assert.Throws)
            var ex = Assert.Throws<BestellingException>(() => klant.Id = id);
            Assert.Equal("id<0", ex.Message);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("111")]
        [InlineData(null)]
        // [InlineData("a1gg@gmail.com")] Deze zal werken, check voorwaarden
        [InlineData("jos@gmail")]
        [InlineData("jos@gmail.")]
        [InlineData("@gmail.com")]
        public void Test_Email_Invalid(string email)
        {
            Klant klant = new Klant(10, "Jos", "jos@gmail.com");
            Assert.Throws<BestellingException>( () => klant.Email = email);
        }

        // Ook de constructor moet getest worden (alles testen)
        [Theory]
        [InlineData(0, "Jos", "jos@gmail.com")]
        [InlineData(-1, "Jos", "jos@gmail.com")]
        [InlineData(10, "", "jos@gmail.com")]
        [InlineData(10, null, "jos@gmail.com")]
        [InlineData(10, "   ", "jos@gmail.com")]
        [InlineData(10, "Jos", "@gmail.com")]
        [InlineData(10, "Jos", "jos@gmailcom")]
        [InlineData(10, "Jos", "josgmail.com")]
        [InlineData(10, "Jos", "jos@gmail.")]
        [InlineData(10, "Jos", "")]
        [InlineData(10, "Jos", null)]
        // ...
        public void Test_ctor_Invalid(int id, string naam, string email)
        {
            Assert.Throws<BestellingException>(() => new Klant(id, naam, email));
        }
    }
}