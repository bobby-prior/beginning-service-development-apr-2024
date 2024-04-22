using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace IssueTrackerApi.Controllers.Issues;

public class Api(IDocumentSession session) : ControllerBase
{

    [HttpGet("/issues")]
    public async Task<ActionResult> GetTheIssuesAsync([FromQuery] string software = "all")
    {
        IReadOnlyList<Issue> issues;

        if (software == "all")
        {
            issues = await session.Query<Issue>().ToListAsync();
        }
        else
        {
            issues = await session.Query<Issue>().Where(i => i.Software.Equals(software, StringComparison.InvariantCultureIgnoreCase)).ToListAsync();
        }

        return Ok(issues);
    }

    [HttpGet("/issues/{id:guid}")]
    public async Task<ActionResult> GetIssueByIdAsync(Guid id)
    {
        var issue = await session.Query<Issue>().SingleOrDefaultAsync(i => i.Id == id);

        if (issue is null)
        {
            return NotFound();
        }

        return Ok(issue);
    }

    [HttpPost("/issues")]
    public async Task<ActionResult> AddIssueAsync(
        [FromBody] CreateIssueRequestModel request,
        [FromServices] IValidator<CreateIssueRequestModel> validator)
    {
        var results = await validator.ValidateAsync(request);
        if (results.IsValid)
        {
            var response = new Issue
            {
                CreatedAt = DateTimeOffset.UtcNow,
                Description = request.Description,
                Software = request.Software,
                Id = Guid.NewGuid(),
                Status = IssueStatus.Created
            };

            session.Store(response);
            await session.SaveChangesAsync();

            return Ok(response);
        }

        return BadRequest(results.ToDictionary());

    }
}

public record Issue
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Software { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public IssueStatus Status { get; set; }
}

public enum IssueStatus
{
    Created
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