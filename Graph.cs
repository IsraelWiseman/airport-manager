using AirportLogic.Models;

namespace AirportLogic.Services
{
    public class Graph
    {
        private readonly List<Location> _nodes = new();
        private readonly List<(Location? from, Location to)> _edges = new();

        public void AddNode(Location node)
        {
            _nodes.Add(node);
        }
        public void AddEdge(Location? from, Location to)
        {
            _edges.Add((from, to));
        }

        public List<Location> GetNextNodes(string node)
        {
            return _edges.Where(e => e.from?.Id == node).Select(e => e.to).ToList();
        }

        public Location? GetFirst()
        {
            return _edges.Where(e => e.from?.Id == null).Select(e => e.to).FirstOrDefault();
        }

        public Location? GetDepartureFirst()
        {
            return _edges
                .Where(e => e.from?.Id == "5" || e.from?.Id == "6")
                .Select(e => e.from)
                .FirstOrDefault();
        }
    }
}