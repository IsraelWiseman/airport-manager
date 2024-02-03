using AirportLogic.Models;

namespace AirportLogic.Services
{
    public class Logic
    {
        public List<Flight> Flights = new();
        private readonly Locations _locations;

        public Logic(Locations locations)
        {
            _locations = locations;
        }
        public Graph GetDepartureRoute()
        {
            return _locations.GetDepartureRoute();
        }


        public Graph GetArivalRoute()
        {
            return _locations.GetArrivalRoute();
        }





    }


}