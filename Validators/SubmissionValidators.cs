using FluentValidation;
using FormEngineAPI.DTOs;

namespace FormEngineAPI.Validators;

public class CreateSubmissionDtoValidator : AbstractValidator<CreateSubmissionDto>
{
    public CreateSubmissionDtoValidator()
    {
        RuleFor(x => x.FormId)
            .GreaterThan(0).WithMessage("FormId deve ser maior que 0");

        RuleFor(x => x.DataJson)
            .NotEmpty().WithMessage("DataJson é obrigatório")
            .Must(BeValidJson).WithMessage("DataJson deve ser um JSON válido");
    }

    private bool BeValidJson(string json)
    {
        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
