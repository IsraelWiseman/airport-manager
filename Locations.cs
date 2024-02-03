using AirportLogic.Models;

namespace AirportLogic.Services
{
    public class Locations
    {
        public Location[] _locations { get; set; } = new Location[]
        {
           new Location("1"),
           new Location("2"),
           new Location("3"),
           new Location("4"),
           new Location("5"),
           new Location("6"),
           new Location("7"),
           new Location("8"),
           new Location("9")
        };
        internal Graph GetArrivalRoute()
        {
            var graph = new Graph();
            graph.AddNode(_locations[0]);
            graph.AddNode(_locations[1]);
            graph.AddNode(_locations[2]);
            graph.AddNode(_locations[3]);
            graph.AddNode(_locations[4]);
            graph.AddNode(_locations[5]);
            graph.AddNode(_locations[6]);
            graph.AddNode(_locations[7]);
            graph.AddNode(_locations[8]);

            //Landing route
            graph.AddEdge(null, _locations[0]);
            graph.AddEdge(_locations[0], _locations[1]);
            graph.AddEdge(_locations[1], _locations[2]);
            graph.AddEdge(_locations[2], _locations[3]);
            graph.AddEdge(_locations[3], _locations[4]);
            graph.AddEdge(_locations[4], _locations[5]);
            graph.AddEdge(_locations[4], _locations[6]);

            return graph;
        }

        internal Graph GetDepartureRoute()
        {
            var graph = new Graph();
            graph.AddNode(_locations[0]);
            graph.AddNode(_locations[1]);
            graph.AddNode(_locations[2]);
            graph.AddNode(_locations[3]);
            graph.AddNode(_locations[4]);
            graph.AddNode(_locations[5]);
            graph.AddNode(_locations[6]);
            graph.AddNode(_locations[7]);
            graph.AddNode(_locations[8]);

            //Departure route
            graph.AddEdge(_locations[5], _locations[7]);
            graph.AddEdge(_locations[6], _locations[7]);
            graph.AddEdge(_locations[7], _locations[3]);
            graph.AddEdge(_locations[3], _locations[8]);

            return graph;
        }
    }
}
