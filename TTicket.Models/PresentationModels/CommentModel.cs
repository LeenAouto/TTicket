namespace TTicket.Models.PresentationModels
{
    public class CommentModel
    {
        public CommentModel() { }
        public CommentModel(Comment comment)
        {
            Id = comment.Id;
            TicketId = comment.TicketId;
            UserId = comment.UserId;
            Content = comment.Content;
            CreatedDate = comment.CreatedDate;
        }

        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
