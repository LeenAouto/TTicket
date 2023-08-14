namespace TTicket.Security.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string hashedPassword, string inputPassword);
    }
}
