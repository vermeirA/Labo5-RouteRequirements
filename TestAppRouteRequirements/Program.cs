using RouteRequirements;
using RouteRequirements.Model;
using System.Security.Cryptography.X509Certificates;

namespace TestAppRouteRequirements
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RouteFactory factory = new RouteFactory();

            string data1 = @"C:\Users\Gebruiker\Desktop\Graduaat\SEM3\Programmeren gevorderd 1\Labo 5\TestAppRouteRequirements\Files\data1.txt";
            string data2 = @"C:\Users\Gebruiker\Desktop\Graduaat\SEM3\Programmeren gevorderd 1\Labo 5\TestAppRouteRequirements\Files\data2.txt";

            XRoute routeData1 = factory.BuildRouteFromFile(data1);
            XRoute routeData2 = factory.BuildRouteFromFile(data2);
           

            Console.WriteLine();

            for (int i = 0; i < routeData2._routeSegments.Count; i++)
            {
                Console.WriteLine($"{routeData2._routeSegments[i].LocStart.Name}({routeData2._routeSegments[i].LocStart.IsStop}) -> " +
                     $"{routeData2._routeSegments[i].LocEnd.Name}({routeData2._routeSegments[i].LocEnd.IsStop}): " +
                     $"{routeData2._routeSegments[i].Distance}");
            }

            Console.WriteLine();

            XRoute reversedRoute = factory.ReverseRoute(routeData1);

            for (int i = 0; i < reversedRoute._routeSegments.Count; i++)
            {
                Console.WriteLine($"{reversedRoute._routeSegments[i].LocStart.Name}({reversedRoute._routeSegments[i].LocStart.IsStop}) -> " +
                     $"{reversedRoute._routeSegments[i].LocEnd.Name}({reversedRoute._routeSegments[i].LocEnd.IsStop}): " +
                     $"{reversedRoute._routeSegments[i].Distance}");
            }

            Console.WriteLine();

            for (int i = 0; i < routeData2._routeSegments.Count; i++)
            {
                Console.WriteLine($"{routeData2._routeSegments[i].LocStart.Name}({routeData2._routeSegments[i].LocStart.IsStop}) -> " +
                     $"{routeData2._routeSegments[i].LocEnd.Name}({routeData2._routeSegments[i].LocEnd.IsStop}): " +
                     $"{routeData2._routeSegments[i].Distance}");
            } 
        }
    }
}
