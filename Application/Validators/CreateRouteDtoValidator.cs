using FluentValidation;
using TrailHub.API.Application.DTOs;
using TrailHub.Application.DTOs;

namespace TrailHub.Application.Validators
{
    // Добавьте этот класс в существующий файл или создайте новый
    public class GpxUploadDtoValidator : AbstractValidator<GpxUploadDto>
    {
        public GpxUploadDtoValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("Файл GPX обязателен.")
                .Must(file => file != null && file.Length > 0).WithMessage("Файл не может быть пустым.")
                .Must(file => file.FileName.EndsWith(".gpx", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Файл должен иметь расширение .gpx");

            RuleFor(x => x.Name)
                .MaximumLength(100).When(x => !string.IsNullOrEmpty(x));

            RuleFor(x => x.Description)
                .MaximumLength(500).When(x => !string.IsNullOrEmpty(x));
        }
    }

    // Убедитесь, что CoordinateDtoValidator существует
    public class CoordinateDtoValidator : AbstractValidator<CoordinateDto>
    {
        public CoordinateDtoValidator()
        {
            RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
            RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        }
    }
}