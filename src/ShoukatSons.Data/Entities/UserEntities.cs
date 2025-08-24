using System;
using System.ComponentModel.DataAnnotations;

namespace ShoukatSons.Data.Entities
{
    public enum UserRole { CASHIER, MANAGER, OWNER }

    public class User
    {
        [Key] public int UserId { get; set; }
        [Required, MaxLength(50)] public string Username { get; set; } = "";
        [Required] public UserRole Role { get; set; }
        [Required] public string PinHash { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public class AuditLog
    {
        [Key] public int AuditId { get; set; }
        [MaxLength(100)] public string Entity { get; set; } = "";
        [MaxLength(100)] public string EntityId { get; set; } = "";
        [MaxLength(50)] public string Action { get; set; } = "";
        public string? DataBefore { get; set; }
        public string? DataAfter { get; set; }
        public int? PerformedByUserId { get; set; }
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    }
}