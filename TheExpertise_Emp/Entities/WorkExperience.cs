using System;
using System.Collections.Generic;

namespace TheExpertise_Emp.Entities;

public partial class WorkExperience
{
    public int ExperienceId { get; set; }

    public int? UserId { get; set; }

    public string JobTitle { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string? StartDate { get; set; }

    public string? EndDate { get; set; }

    public string? Description { get; set; }

    public int? Salary { get; set; }

    public virtual User? User { get; set; }
}
