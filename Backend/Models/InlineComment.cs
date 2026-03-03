using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class InlineComment
    {
        [Key]
        public int CommentId { get; set; }

        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        [Required]
        public string CommentText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int VersionId { get; set; }

        [ForeignKey(nameof(VersionId))]
        public DocumentVersion DocumentVersion { get; set; } = null!;

        public int AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public User Author { get; set; } = null!;
    }
}