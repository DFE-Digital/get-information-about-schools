namespace Edubase.AcceptanceTests.Users
{
    public interface IUsers
    {
        Task<User> GetBackOfficeUser();
        Task RemoveUser(User user);
    }
}
