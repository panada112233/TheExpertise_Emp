using System;
using System.Collections.Generic;

namespace TheExpertise_Emp.Entities;

public partial class AdminUser
{
    public int AdminId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public string Email { get; set; } = null!;
}
