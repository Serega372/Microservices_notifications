using System.ComponentModel.DataAnnotations;


namespace EmailService.Core.Entities
{
    public class EmailMessage
    {
        public Guid Id { get; set; }

        public string? Subject { get; set; }

        public string? Body { get; set; }

        [Required]
        public required IEnumerable<string> Recipients { get; set; }

        [Required]
        public required string Sender { get; set; }

        public IEnumerable<string> Attachments { get; set; } = [];
    }
}
