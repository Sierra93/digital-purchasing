using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Microsoft.IdentityModel.Tokens;

namespace DigitalPurchasing.Services
{
    public class ColumnNameService : IColumnNameService
    {
        private const string Separator = "|=|";

        private const string ColumnCode = "Код";
        private const string ColumnName = "Наименование";
        private const string ColumnQty = "Количество";
        private const string ColumnUom = "ЕИ";

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
            if (string.IsNullOrEmpty(name)) return;
            name = name.Trim();
            var defaultName = DefaultName(type);
            var entity = _db.ColumnNames.AsQueryable().FirstOrDefault(q => q.Type == type);
            if (entity == null)
            {
                if (!defaultName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    _db.ColumnNames.Add(new ColumnName { Type = type, Names = name });
                    _db.SaveChanges();
                }                
                return;
            }

            var names = entity.Names.Split(Separator, StringSplitOptions.RemoveEmptyEntries).Union(new [] { defaultName }).ToList();
            if (names.Contains(name, StringComparer.InvariantCultureIgnoreCase)) return;
            names.Add(name);
            entity.Names = string.Join(Separator, names);
            _db.SaveChanges();
        }

        public ColumnResponse GetAllNames()
        {
            var result = new ColumnResponse();
            var columnNames = _db.ColumnNames.AsQueryable().ToList();
            foreach (var tableColumnType in (TableColumnType[]) Enum.GetValues(typeof(TableColumnType)))
            {
                if (tableColumnType == TableColumnType.Unknown) continue;
                var columnName = columnNames.FirstOrDefault(q => q.Type == tableColumnType);
                result.AddColumn(DefaultName(tableColumnType), !string.IsNullOrEmpty(columnName?.Names)
                    ? columnName.Names.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                    : new string[0]);
            }
            return result;
        }

        public string DefaultName(TableColumnType type)
        {
            switch (type)
            {
                case TableColumnType.Code: return ColumnCode;
                case TableColumnType.Name: return ColumnName;
                case TableColumnType.Qty: return ColumnQty;
                case TableColumnType.Uom: return ColumnUom;
                default:
                    return "";
            }
        }

        public TableColumnType ToTableColumnType(string name)
        {
            switch (name)
            {
                case ColumnCode: return TableColumnType.Code;
                case ColumnName: return TableColumnType.Name;
                case ColumnQty: return TableColumnType.Qty;
                case ColumnUom: return TableColumnType.Uom;
                default: return TableColumnType.Unknown;
            }
        }

        public void SaveAllNames(ColumnResponse model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            foreach (var column in model.Columns)
            {
                var type = ToTableColumnType(column.Name);
                var names = string.Join(Separator, column.AltNames);
                var entity = _db.ColumnNames.FirstOrDefault(q => q.Type == type);
                if (entity == null)
                {
                    _db.ColumnNames.Add(new ColumnName { Type = type, Names = names });
                }
                else
                {
                    entity.Names = names;
                }
                _db.SaveChanges();
            }
        }
    }
}
