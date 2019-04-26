using System;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using DigitalPurchasing.Models.Counters;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class CounterService : ICounterService
    {
        private readonly ApplicationDbContext _db;

        public CounterService(ApplicationDbContext db) => _db = db;

        private int GetNextId<TEntity>(Guid? ownerId) where TEntity : Counter, new()
        {
            var dbSet = _db.Set<TEntity>();

            var counter = ownerId.HasValue
                ? dbSet.IgnoreQueryFilters().FirstOrDefault(q => q.OwnerId == ownerId.Value)
                : dbSet.FirstOrDefault();

            if (counter == null)
            {
                var entity = new TEntity {CurrentId = 0};
                if (ownerId.HasValue)
                {
                    entity.OwnerId = ownerId.Value;
                }
                var counterEntry = dbSet.Add(entity);
                _db.SaveChanges();
                counter = counterEntry.Entity;
            }

            var nextId = ++counter.CurrentId;

            var isDone = false;
            while (!isDone)
            {
                try
                {
                    _db.SaveChanges();
                    isDone = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is TEntity)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                if (property.Name == "CurrentId")
                                {
                                    var proposedValue = (int)proposedValues[property];
                                    var databaseValue = (int)databaseValues[property];

                                    nextId = ++databaseValue;
                                    proposedValues[property] = nextId;
                                }
                            }
                            // Refresh original values to bypass next concurrency check
                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException($"Don't know how to handle concurrency conflicts for {entry.Metadata.Name}");
                        }
                    }
                }
            }
            return nextId;
        }

        public int GetQRNextId(Guid? ownerId = null) => GetNextId<QRCounter>(ownerId);

        public int GetPRNextId(Guid? ownerId = null) => GetNextId<PRCounter>(ownerId);

        public int GetCLNextId(Guid? ownerId = null) => GetNextId<CLCounter>(ownerId);

        public int GetSONextId(Guid? ownerId = null) => GetNextId<SOCounter>(ownerId);

        public int GetCustomerNextId(Guid ownerId) => GetNextId<CustomerCounter>(ownerId);
    }
}
