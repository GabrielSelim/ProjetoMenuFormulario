using FluentValidation;
using FormEngineAPI.DTOs;

namespace FormEngineAPI.Validators;

public class CreateFormDtoValidator : AbstractValidator<CreateFormDto>
{
    public CreateFormDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres");

        RuleFor(x => x.SchemaJson)
            .NotEmpty().WithMessage("SchemaJson é obrigatório")
            .Must(BeValidJson).WithMessage("SchemaJson deve ser um JSON válido");

        RuleFor(x => x.Version)
            .MaximumLength(50).WithMessage("Versão não pode exceder 50 caracteres");
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

public class UpdateFormDtoValidator : AbstractValidator<UpdateFormDto>
{
    public UpdateFormDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres");

        RuleFor(x => x.SchemaJson)
            .NotEmpty().WithMessage("SchemaJson é obrigatório")
            .Must(BeValidJson).WithMessage("SchemaJson deve ser um JSON válido");

        RuleFor(x => x.Version)
            .MaximumLength(50).WithMessage("Versão não pode exceder 50 caracteres");
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
