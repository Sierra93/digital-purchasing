using System;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class PRCounterService : IPRCounterService
    {
        private readonly ApplicationDbContext _db;

        public PRCounterService(ApplicationDbContext db) => _db = db;

        public int GetNextId()
        {
            var counter = _db.PRCounters.FirstOrDefault();
            if (counter == null)
            {
                var counterEntry = _db.PRCounters.Add(new PRCounter
                {
                    CurrentId = 0
                });
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
                        if (entry.Entity is PRCounter)
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
    }
}
