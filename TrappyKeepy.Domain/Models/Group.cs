﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrappyKeepy.Domain.Models
{
    /// <summary>
    /// Table to store group records.
    /// </summary>
    [Table("groups", Schema = "tk")]
    [Index(nameof(Name), Name = "groups_name_key", IsUnique = true)]
    public partial class Group
    {
        public Group()
        {
            Memberships = new HashSet<Membership>();
            Permits = new HashSet<Permit>();
        }

        /// <summary>
        /// UUID primary key.
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Unique group name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; } = null!;
        /// <summary>
        /// Description of the group.
        /// </summary>
        [Column("description")]
        public string? Description { get; set; }
        /// <summary>
        /// Datetime the group was created in the database.
        /// </summary>
        [Column("date_created")]
        public DateTime DateCreated { get; set; }

        [InverseProperty(nameof(Membership.Group))]
        public virtual ICollection<Membership> Memberships { get; set; }
        [InverseProperty(nameof(Permit.Group))]
        public virtual ICollection<Permit> Permits { get; set; }
    }
}
