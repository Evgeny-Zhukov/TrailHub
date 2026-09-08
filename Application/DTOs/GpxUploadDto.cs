using Microsoft.AspNetCore.Http;

namespace TrailHub.Application.DTOs
{
    public class GpxUploadDto
    {
        public IFormFile File { get; set; } = null!;
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}