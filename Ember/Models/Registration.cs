﻿namespace Ember.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? AccountType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
