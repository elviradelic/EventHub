using EventHub.Application.Interfaces;
using EventHub.Domain.Entities;

namespace EventHub.Infrastructure.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    public User? GetById(Guid id)
    {
        return _users.FirstOrDefault(user => user.Id == id);
    }

    public User? GetByEmail(string email)
    {
        return _users.FirstOrDefault(user =>
            string.Equals(
                user.Email,
                email,
                StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyCollection<User> GetAll()
    {
        return _users.AsReadOnly();
    }

    public void Add(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        _users.Add(user);
    }
}