using GpxReader;
using NetTopologySuite.Geometries;

namespace TrailHub.Application.Services
{
    public interface IGpxParserService
    {
        Task<(LineString LineString, decimal[] ElevationProfile, double DistanceKm)> ParseGpxAsync(Stream stream);
    }

    public class GpxParserService : IGpxParserService
    {
        public async Task<(LineString LineString, decimal[] ElevationProfile, double DistanceKm)> ParseGpxAsync(Stream stream)
        {
            var reader = new GpxReader();
            var gpx = await reader.ReadAsync(stream);

            var coordinates = new List<Coordinate>();
            var elevations = new List<decimal>();

            // Собираем точки из треков
            foreach (var track in gpx.Tracks)
            {
                foreach (var segment in track.Segments)
                {
                    foreach (var point in segment.Points)
                    {
                        coordinates.Add(new Coordinate(point.Longitude, point.Latitude));
                        elevations.Add((decimal)(point.Elevation ?? 0));
                    }
                }
            }

            if (coordinates.Count < 2)
            {
                throw new ArgumentException("GPX файл должен содержать как минимум 2 точки.");
            }

            var lineString = new LineString(coordinates.ToArray()) { SRID = 4326 };

            // Расчет расстояния (Haversine)
            double distanceKm = 0;
            for (int i = 0; i < coordinates.Count - 1; i++)
            {
                distanceKm += CalculateDistance(
                    coordinates[i].Y, coordinates[i].X,
                    coordinates[i + 1].Y, coordinates[i + 1].X
                );
            }

            return (lineString, elevations.ToArray(), distanceKm);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Радиус Земли в км
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}