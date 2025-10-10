using FluentValidation;
using FormEngineAPI.DTOs;

namespace FormEngineAPI.Validators;

public class CreateMenuDtoValidator : AbstractValidator<CreateMenuDto>
{
    public CreateMenuDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Tipo de conteúdo é obrigatório")
            .MaximumLength(50).WithMessage("Tipo de conteúdo não pode exceder 50 caracteres");

        RuleFor(x => x.UrlOrPath)
            .MaximumLength(500).WithMessage("URL/Path não pode exceder 500 caracteres");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("Ícone não pode exceder 100 caracteres");
    }
}

public class UpdateMenuDtoValidator : AbstractValidator<UpdateMenuDto>
{
    public UpdateMenuDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Tipo de conteúdo é obrigatório")
            .MaximumLength(50).WithMessage("Tipo de conteúdo não pode exceder 50 caracteres");

        RuleFor(x => x.UrlOrPath)
            .MaximumLength(500).WithMessage("URL/Path não pode exceder 500 caracteres");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("Ícone não pode exceder 100 caracteres");
    }
}
