using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryEvents;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AiGeneratedApi.Tests.Repositories
{
    public class MockRepositoryEvents : IRepositoryEvents
    {
        private readonly List<Event> _events;

        public MockRepositoryEvents()
        {
            _events = new List<Event>();
        }

        public Task AddAsync(Event entity)
        {
            _events.Add(entity);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Event>> FindAsync(Expression<Func<Event, bool>> predicate)
        {
            return Task.FromResult<IEnumerable<Event>>(_events.Where(predicate.Compile()));
        }

        public Task<IEnumerable<Event>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Event>>(_events);
        }

        public Task<Event?> GetByIdAsync(object id)
        {
            return Task.FromResult(_events.FirstOrDefault(e => e.Id == id.ToString()));
        }

        public void Remove(Event entity)
        {
            _events.Remove(entity);
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }

        public void Update(Event entity)
        {
            var existingEvent = _events.FirstOrDefault(e => e.Id == entity.Id);
            if (existingEvent != null)
            {
                _events.Remove(existingEvent);
                _events.Add(entity);
            }
        }
    }
}
