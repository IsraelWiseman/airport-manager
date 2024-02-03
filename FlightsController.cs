using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirportLogic.Models;
using AirportWebApi.Data;
using AirportLogic.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Location = AirportLogic.Models.Location;

namespace AirportWebApi.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly AirportContext _context;
        private readonly Logic _logic;
        private readonly IHubContext<FlightUpdateHub> _hubContext;

        public FlightsController(AirportContext context, Locations locations, IHubContext<FlightUpdateHub> hubContext)
        {
            _context = context;
            _logic = new Logic(locations);
            _hubContext = hubContext;

        }



        [HttpPost]
        public async Task<IActionResult> LandHandler([FromBody] EventPayload payload)
        {
            if (_context.Flights == null)
            {
                return Problem("Entity set 'AirportContext.Flights' is null.");
            }

            var flight = payload.Flight;
            var plane = payload.Plane;

            var landingRoute = _logic.GetArivalRoute();
            await flight!.FlightLogic(landingRoute);

            plane!.CurrentLocation = landingRoute.GetFirst()?.Id;


            _context.Flights.Add(flight);
            _context.Planes.Add(plane!);
            await _context.SaveChangesAsync();

            var landingLocations = new List<string>();
            var currentNode = landingRoute.GetFirst();
            Location? finalDestination = null;
            Location? fromLocation = null;
            Location? toLocation = null;

            while (currentNode != null)
            {
                fromLocation = currentNode;

                if (currentNode!.Id == "6")
                {
                    finalDestination = currentNode;
                }



                currentNode = landingRoute.GetNextNodes(currentNode.Id!).FirstOrDefault();

                if (currentNode != null)
                {
                    toLocation = currentNode;
                    await MovePlane(plane, fromLocation, toLocation);

                    landingLocations.Add(fromLocation.Id!);

                    var flightDetails = new
                    {
                        flight.FlightNumber,
                        plane.PlaneCode,
                        plane.Status,
                        fromLocation.Id,
                        toLocation
                    };

                    await _hubContext.Clients.All.SendAsync("PlaneMoved", flight.FlightNumber, plane.PlaneCode, fromLocation.Id, toLocation.Id, flight.Plane!.Status);
                }
                else
                {
                    landingLocations.Add(fromLocation.Id!);
                }

                plane!.CurrentLocation = fromLocation.Id;
            }

            if (finalDestination != null)
            {
                var planeAtFinalDestination = _context.Planes.SingleOrDefault(p => p.LocationId == finalDestination.Id);
                if (planeAtFinalDestination != null)
                {
                    await _context.SaveChangesAsync();
                    _context.Planes.Remove(planeAtFinalDestination);
                }
            }



            return Ok(new
            {

                Message = $"Landing process initiated for Flight: {flight.FlightNumber}, Status: {plane.Status}",
                LandingLocations = landingLocations
            });


        }




        [HttpPost]
        public async Task<IActionResult> DepartureHandler([FromBody] EventPayload payload)
        {
            if (_context.Flights == null)
            {
                return Problem("Entity set 'AirportContext.Flights' is null.");
            }

            var flight = payload.Flight;
            var plane = payload.Plane;

            var departureRoute = _logic.GetDepartureRoute();
            await flight!.FlightLogic(departureRoute);

            plane!.CurrentLocation = departureRoute.GetDepartureFirst()?.Id;

            _context.Flights.Add(flight);
            _context.Planes.Add(plane!);
            await _context.SaveChangesAsync();

            var departureLocations = new List<string>();
            var currentNode = departureRoute.GetDepartureFirst();
            Location? finalDestination = null;
            Location? fromLocation = null;
            Location? toLocation = null;

            while (currentNode != null)
            {
                fromLocation = currentNode;

                if (currentNode!.Id == "9")
                {
                    finalDestination = currentNode;
                }

                currentNode = departureRoute.GetNextNodes(currentNode.Id!).FirstOrDefault();

                if (currentNode != null)
                {
                    toLocation = currentNode;
                    await MovePlane(plane, fromLocation, toLocation);
                    departureLocations.Add(fromLocation.Id!);

                    var flightDetails = new
                    {
                        flight.FlightNumber,
                        plane.PlaneCode,
                        plane.Status,
                        fromLocation.Id,
                        toLocation

                    };

                    await _hubContext.Clients.All.SendAsync("PlaneMoved", flight.FlightNumber, plane.PlaneCode, fromLocation.Id, toLocation.Id, flight.Plane!.Status);


                }
                else
                {
                    departureLocations.Add(fromLocation.Id!);
                }

                plane!.CurrentLocation = fromLocation.Id;
            }

            if (finalDestination != null)
            {
                var planeAtFinalDestination = _context.Planes.SingleOrDefault(p => p.LocationId == finalDestination.Id);
                if (planeAtFinalDestination != null)
                {
                    await _context.SaveChangesAsync();
                    _context.Planes.Remove(planeAtFinalDestination);
                }
            }

            return Ok(new
            {
                Message = $"Departure process initiated for Flight: {flight.FlightNumber} At status: {plane.Status}",
                DepartureLocations = departureLocations
            });
        }


        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight([FromBody] Flight flight)
        {

            if (_context.Flights == null)
            {
                return Problem("Entity set 'AirportContext.Flights'  is null.");
            }
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlight", new { id = flight.Id }, flight);
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            if (_context.Flights == null)
            {
                return NotFound();
            }
            var flightsWithDetails = await _context.Flights
                .Select(flight => new
                {
                    flight.Id,
                    flight.FlightNumber,
                    flight.PlaneId,
                    flight.Plane!.Status,
                    flight.Plane.PlaneCode,

                })
                .ToListAsync();

            return Ok(flightsWithDetails);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(int id)
        {
            if (_context.Flights == null)
            {
                return NotFound();
            }
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }

        // PUT: api/Flights/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlight(int id, Flight flight)
        {
            if (id != flight.Id)
            {
                return BadRequest();
            }

            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }





        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            if (_context.Flights == null)
            {
                return NotFound();
            }
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlightExists(int id)
        {
            return (_context.Flights?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task MovePlane(Plane plane, Location fromLocation, Location toLocation)
        {
            plane.CurrentLocation = toLocation.Id;
            await _context.SaveChangesAsync();
        }

    }
}