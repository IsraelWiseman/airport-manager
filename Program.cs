using AirportLogic.Models;
using AirportModels;
using Newtonsoft.Json;
using System.Text;

namespace AirportWebApiSimulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string baseUrl = "https://localhost:7130/";

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            Random random = new();

            try
            {
                while (true)
                {
                    // Generate a random FlightNumber (8 characters)
                    string flightNumber = Guid.NewGuid().ToString("N")[..8];
                    string planeCode = Guid.NewGuid().ToString("N")[..8];


                    // Generate a random PlaneStatus
                    PlaneStatus planeStatus = (PlaneStatus)random.Next(0, 2);

                    // Create a new Plane object
                    var plane = new Plane
                    {
                        Status = planeStatus,
                        PlaneCode = planeCode
                    };

                    // Create a new Flight object
                    var flight = new Flight
                    {
                        FlightNumber = flightNumber,
                        Plane = plane
                    };

                    // Serialize the payload to JSON
                    var payloadJson = JsonConvert.SerializeObject(new
                    {
                        Flight = flight,
                        Plane = plane
                    });

                    // Determine the endpoint based on PlaneStatus
                    string endpoint = planeStatus == PlaneStatus.Landing ? "api/LandHandler" : "api/DepartureHandler";

                    // Send a POST request to the appropriate endpoint
                    var response = await httpClient.PostAsync(endpoint, new StringContent(payloadJson, Encoding.UTF8, "application/json"));

                    response.EnsureSuccessStatusCode();

                    // Read the response from the server
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Output the response
                    Console.WriteLine($"Flight Number: {flightNumber}, Plane Status: {planeStatus}");
                    Console.WriteLine("Response:");
                    Console.WriteLine(responseContent);
                    Console.WriteLine();

                    // Wait for 3 seconds before creating the next Plane
                    await Task.Delay(3000);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
