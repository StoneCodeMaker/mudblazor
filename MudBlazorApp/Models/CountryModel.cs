using System;

namespace MudBlazorApp.Models
{
    public class CountryModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Verified { get; set; }
        public string Mode { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime LastModified { get; set; }
    }
}
