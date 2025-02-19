using System;
using System.Collections.Generic;

namespace TheExpertise_Emp.Entities;

public partial class Document
{
    public int? UserId { get; set; }

    public Guid DocumentId { get; set; }

    public Guid LeaveTypeId { get; set; }

    public DateTime? Createdate { get; set; }

    public string? Fullname { get; set; }

    public Guid Rolesid { get; set; }

    public string? Reason { get; set; }

    public DateTime? Startdate { get; set; }

    public DateTime? Enddate { get; set; }

    public int? Totalleave { get; set; }

    public Guid LeavedType { get; set; }

    public DateTime? LeavedStartdate { get; set; }

    public DateTime? LeavedEnddate { get; set; }

    public int? Totalleaved { get; set; }

    public string? FriendeContact { get; set; }

    public string? Contact { get; set; }

    public DateTime? Workingstart { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public DateTime? HrApprovedDate { get; set; }

    public DateTime? SentToHrdate { get; set; }

    public string? HrSignature { get; set; }

    public string? ManagerName { get; set; }

    public string? ManagerComment { get; set; }

    public string? Status { get; set; } // ✅ ใช้ติดตามสถานะเอกสาร (pending_manager, pending_hr, approved)

}

