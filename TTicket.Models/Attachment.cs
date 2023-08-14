namespace TTicket.Models
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public Guid AttachedToId { get; set; } //Either a ticketId or a commentId
        public string FileName { get; set; } = string.Empty;
        public AttacherType Attacher { get; set; }
    }

    public enum AttacherType : byte
    {
        Ticket = 1,
        Comment = 2
    }
}
