namespace TTicket.Models.ResponseModels 
{ 
    public enum ErrorCode
    {
        NoError = 0,

        LoggedError = 1,

        InvalidModelState = 2,

        //User errors
        UserNotFound = 1000,
        UsersNotFound = 1001,

        UsernameAlreadyUsed = 1101,
        UserEmailAlreadyUsed = 1102,
        UserPhoneAlreadyUsed = 1103,

        InvalidUsername = 1201,
        InvalidEmailAddress = 1202,
        InvalidPhoneNumber = 1203,
        InvalidPassword = 1204,

        UserAlreadyActive = 1301,
        UserAlreadyInactive = 1302,

        AuthenticationFailed = 1401,


        //Attachments errors
        AttachmentNotFound = 2000,
        AttachmentsNotFound = 2001,
        
        AttachmentFileNameAlreadyUsed = 2101,

        InvalidAttachedToId = 2201,
        InvalidFileName = 2202,

        AttachmentRequired = 2501,


        //Comments errors
        CommentNotFound = 3000,
        CommentsNotFound = 3001,

        InvalidCommentContent = 3201,



        //Products errors
        ProductNotFound = 4000,
        ProductsNotFound = 4001,

        ProductNameAlreadyUsed = 4101,

        InvalidProductName = 4201,


        //Tickets errors
        TicketNotFound = 5000,
        TicketsNotFound = 5001,

        TicketAlreadyAssigned = 5101,
        TicketAlreadyClosed = 5102,

        InvalidTicketContent = 5201,







    }
}
