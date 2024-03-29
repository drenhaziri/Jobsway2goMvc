﻿using Jobsway2goMvc.Enums;

namespace Jobsway2goMvc.Models
{
    public class GroupMembership
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public Group Group { get; set; }
        public int GroupId { get; set; }
        public Approval Status { get; set; }
        public bool? IsMember { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsModerator { get; set; }
        public bool? IsBanned { get; set; }
        public bool? IsOwner { get; set; }
    }
}
