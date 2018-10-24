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

        private int GetNextId<TEntity>() where TEntity : Counter, new()
        {
            var dbSet = _db.Set<TEntity>();

            var counter = dbSet.FirstOrDefault();
            if (counter == null)
            {
                var counterEntry = dbSet.Add(new TEntity { CurrentId = 0 });
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

        public int GetQRNextId() => GetNextId<QRCounter>();

        public int GetPRNextId() => GetNextId<PRCounter>();

        public int GetCLNextId() => GetNextId<CLCounter>();
    }
}
