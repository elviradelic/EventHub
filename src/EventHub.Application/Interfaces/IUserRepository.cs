using EventHub.Domain.Entities;

namespace EventHub.Application.Interfaces;

public interface IUserRepository
{
    User? GetById(Guid id);

    User? GetByEmail(string email);

    IReadOnlyCollection<User> GetAll();

    void Add(User user);
}