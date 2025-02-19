using System;
using System.Collections.Generic;

namespace TheExpertise_Emp.Entities;

public partial class PasswordReset
{
    public int Id { get; set; }

    public string UserEmail { get; set; } = null!;

    public string ResetToken { get; set; } = null!;

    public DateTime ExpirationDate { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
