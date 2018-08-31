using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;

namespace DigitalPurchasing.Services
{
    public class ColumnNameService : IColumnNameService
    {
        private const string Separator = "|=|";

        private readonly ApplicationDbContext _db;

        public ColumnNameService(ApplicationDbContext db) => _db = db;

        public string[] GetNames(TableColumnType type)
        {
            var entity = _db.ColumnNames.AsQueryable().FirstOrDefault(q => q.Type == type);
            if (entity == null) return new string[0];
            return entity.Names.Split(Separator);
        }

        public void SaveName(TableColumnType type, string name)
        {
            var entity = _db.ColumnNames.AsQueryable().FirstOrDefault(q => q.Type == type);
            if (entity == null)
            {
                _db.ColumnNames.Add(new ColumnName { Type = type, Names = name });
                _db.SaveChanges();
                return;
            }

            var names = entity.Names.Split(Separator).ToList();
            if (names.Contains(name, StringComparer.InvariantCultureIgnoreCase)) return;
            names.Add(name);
            entity.Names = string.Join(Separator, names);
            _db.SaveChanges();
        }

        public ColumnResponse GetAllNames()
        {
            var result = new ColumnResponse();
            var columnNames = _db.ColumnNames.AsQueryable().ToList();
            foreach (var columnName in columnNames)
            {
                result.AddColumn(DefaultName(columnName.Type), columnName.Names.Split(Separator).ToArray());
            }
            return result;
        }

        public string DefaultName(TableColumnType type)
        {
            switch (type)
            {
                case TableColumnType.Code: return "Код";
                case TableColumnType.Name: return "Наименование";
                case TableColumnType.Qty: return "Количество";
                case TableColumnType.Uom: return "ЕИ";
                default:
                    return "";
            }
        }
    }
}
