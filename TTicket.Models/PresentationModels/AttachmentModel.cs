namespace TTicket.Models.PresentationModels
{
    public class AttachmentModel
    {
        public AttachmentModel() { }
        public AttachmentModel(Attachment attachment)
        {
            Id = attachment.Id;
            AttachedToId = attachment.AttachedToId;
            FileName = attachment.FileName;
            Attacher = attachment.Attacher;
        }

        public Guid Id { get; set; }
        public Guid AttachedToId { get; set; } //Either a ticketId or a commentId
        public string FileName { get; set; } = string.Empty;
        public AttacherType Attacher { get; set; }
    }
}
