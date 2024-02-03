using AirportModels;
using Microsoft.AspNetCore.SignalR;

namespace AirportWebApi
{
    public class FlightUpdateHub : Hub
    {
        public async Task SendFlightStatusUpdate(string flightNumber, PlaneStatus status)
        {
            await Clients.All.SendAsync("FlightStatusUpdate", flightNumber, status);
        }
    }
}
