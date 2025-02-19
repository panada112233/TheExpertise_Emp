using System;
using System.Collections.Generic;

namespace TheExpertise_Emp.Entities;

public partial class Education
{
    public int EducationId { get; set; }

    public int UserId { get; set; }

    public string Level { get; set; } = null!;

    public string Institute { get; set; } = null!;

    public string FieldOfStudy { get; set; } = null!;

    public string? Year { get; set; }

    public decimal? Gpa { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
