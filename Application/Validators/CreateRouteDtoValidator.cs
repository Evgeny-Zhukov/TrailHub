using FluentValidation;
using TrailHub.API.Application.DTOs;
using TrailHub.Application.DTOs;

namespace TrailHub.Application.Validators
{
    public class CreateRouteDtoValidator : AbstractValidator<CreateRouteDto>
    {
        public CreateRouteDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название обязательно")
                .MinimumLength(3).WithMessage("Минимальная длина названия - 3 символа")
                .MaximumLength(200).WithMessage("Максимальная длина названия - 200 символов");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание обязательно")
                .MinimumLength(10).WithMessage("Минимальная длина описания - 10 символов")
                .MaximumLength(5000).WithMessage("Максимальная длина описания - 5000 символов");

            RuleFor(x => x.Difficulty)
                .IsInEnum().WithMessage("Неверное значение сложности");

            RuleFor(x => x.Region)
                .NotEmpty().WithMessage("Регион обязателен")
                .MaximumLength(100).WithMessage("Максимальная длина региона - 100 символов");

            RuleFor(x => x.DistanceKm)
                .GreaterThan(0).WithMessage("Дистанция должна быть больше 0")
                .LessThanOrEqualTo(1000).WithMessage("Дистанция не может превышать 1000 км");

            RuleFor(x => x.Coordinates)
                .Must(coordinates => coordinates == null || coordinates.Count >= 2)
                .WithMessage("Для создания маршрута требуется как минимум 2 координаты");

            RuleForEach(x => x.Coordinates).SetValidator(new CoordinateDtoValidator());
        }
    }

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