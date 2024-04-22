using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace IssueTrackerApi.Controllers.Issues;

public class Api : ControllerBase
{
    // GET /issues
    [HttpGet("/issues")]
    public async Task<ActionResult> GetTheIssuesAsync()
    {
        return Ok();
    }

    [HttpPost("/issues")]
    public async Task<ActionResult> AddIssueAsync(
        [FromBody] CreateIssueRequestModel request,
        [FromServices] IValidator<CreateIssueRequestModel> validator)
    {
        var results = await validator.ValidateAsync(request);
        if (results.IsValid)
        {
            return Ok(request);
        }

        return BadRequest(results.ToDictionary());

    }
}

public record CreateIssueRequestModel
{
    public string Software { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateIssueRequestModelValidator : AbstractValidator<CreateIssueRequestModel>
{
    private readonly IReadOnlyList<string> _supportedSoftware = ["excel", "powerpoint", "word"];

    public CreateIssueRequestModelValidator()
    {
        RuleFor(i => i.Description)
            .NotEmpty()
            .MaximumLength(1024);

        RuleFor(i => i.Software)
            .NotEmpty()
            .Must(i =>
            {
                return _supportedSoftware.Any(s => s.Equals(i, StringComparison.InvariantCultureIgnoreCase));
            })
            .WithMessage("Unsupported Software. Good Luck!");
    }
}