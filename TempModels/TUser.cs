using System;
using System.Collections.Generic;

namespace Minimart_Api.TempModels
{
    public partial class TUser
    {
        public TUser()
        {
            TCarts = new HashSet<TCart>();
            TOrders = new HashSet<TOrder>();
            TReviews = new HashSet<TReview>();
            Addresses = new HashSet<Address>();
        }

        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsLoggedIn { get; set; }
        public string? Password { get; set; }
        public bool? IsAdmin { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? PasswordChangesOn { get; set; }
        public int? FailedAttempts { get; set; }
        public string? RoleId { get; set; }
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? Salt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public virtual ICollection<TCart> TCarts { get; set; }
        public virtual ICollection<TOrder> TOrders { get; set; }
        public virtual ICollection<TReview> TReviews { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}
