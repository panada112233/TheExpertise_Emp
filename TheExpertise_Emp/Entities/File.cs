using System;
using System.Collections.Generic;

namespace TheExpertise_Emp.Entities;

public partial class File
{
    public int FileId { get; set; }

    public int UserId { get; set; }

    public string FilePath { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public string? Category { get; set; }

    public string? Description { get; set; }

    public DateTime UploadDate { get; set; }

    public bool? IsActive { get; set; }
}
