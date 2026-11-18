namespace Edubase.AcceptanceTests.Users
{
    public interface IUsers
    {
        Task<User> CreateBackOfficeUser();
        Task DeleteUser(User user);
    }
}
