using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using static ContactsControllerTests;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace TestPhoneContactMicroservices
{
    public class MockDbSet<T> : DbSet<T>, IQueryable<T>, IAsyncEnumerable<T> where T : class
    {
        private readonly List<T> _data;
        private readonly IQueryable<T> _query;

        public MockDbSet()
        {
            _data = new List<T>();
            _query = _data.AsQueryable();
        }

        public override EntityEntry<T> Add(T item)
        {
            _data.Add(item);
            return null;
        }

        public override EntityEntry<T> Remove(T item)
        {
            _data.Remove(item);
            return null;
        }

public override T Find(params object[] keyValues)
{
    var entityType = typeof(T);
    var keyProperties = entityType.GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Any()).ToList();

    if (keyValues.Length != keyProperties.Count)
    {
        throw new ArgumentException($"The number of key values provided does not match the primary key properties for entity {entityType.Name}");
    }

    var entity = _data.FirstOrDefault(item =>
    {
        for (var i = 0; i < keyProperties.Count; i++)
        {
            var keyValue = keyValues[i];
            var propertyValue = keyProperties[i].GetValue(item);

            if (!keyValue.Equals(propertyValue))
            {
                return false;
            }
        }

        return true;
    });

    return entity;
}


        public override ValueTask<T> FindAsync(params object[] keyValues)
        {
            var entity = _data.FirstOrDefault(item =>
            {
                var entityType = typeof(T);
                var keyProperties = entityType.GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Any()).ToList();

                if (keyValues.Length != keyProperties.Count)
                {
                    throw new ArgumentException($"The number of key values provided does not match the primary key properties for entity {entityType.Name}");
                }

                for (var i = 0; i < keyProperties.Count; i++)
                {
                    var keyValue = keyValues[i];
                    var propertyValue = keyProperties[i].GetValue(item);

                    if (!keyValue.Equals(propertyValue))
                    {
                        return false;
                    }
                }

                return true;
            });

            return new ValueTask<T>(entity);
        }


        public override ValueTask<T> FindAsync(object[] keyValues, System.Threading.CancellationToken cancellationToken)
        {
            return new ValueTask<T>(Find(keyValues));
        }


        Type IQueryable.ElementType => _query.ElementType;
        Expression IQueryable.Expression => _query.Expression;
        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(_query.Provider);

        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _data.GetEnumerator();
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(_data.GetEnumerator());
        }
        public override IEntityType EntityType => null;
    }

}
