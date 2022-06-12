using CommonSource.QueryTables;
using FileStoreInterfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Yoda.Interfaces;
using Yoda.Interfaces.Menu;
using YodaQuery;

namespace TradeResourcesPlugin.Modules {
    public class MnuTablesExcel : FrmMenu {
        public MnuTablesExcel(string moduleName) : base(nameof(MnuTablesExcel), "Выгрузка таблиц")
        {
            Path("MnuTablesExcel");
            ProjectsConfig(ProjectsList.All);
            OnRendering(re => {
                var tables = new List<QueryTable>();
                foreach (var module in re.RequestContext.ModulesProvider.Modules)
                {
                    var schemas = ((YodaModule)module).DataSchema().Where(s => new[] { 
                        "dbFiles",
                        "dbTradeResources",
                        "dbHunting",
                        "dbFishing",
                        "dbAgreements",
                    }.Contains(s.DbKey)).ToArray();
                    tables.AddRange(schemas);
                }

                var dataTable = new DataTable();
                dataTable.Columns.Add("TableName", typeof(string));
                dataTable.Columns.Add("TableDescription", typeof(string));
                dataTable.Columns.Add("ColumnName", typeof(string));
                dataTable.Columns.Add("ColumnDescription", typeof(string));
                dataTable.Columns.Add("ColumnType", typeof(string));

                foreach (var table in tables)
                {
                    var fields = table.SystemFields.ToList();
                    fields.AddRange(table.Fields);
                    foreach (var field in fields)
                    {
                        var row = dataTable.NewRow();
                        row["TableName"] = table.Name.ToLower();
                        row["TableDescription"] = table.Text;
                        row["ColumnName"] = field.FieldName.ToLower();
                        row["ColumnDescription"] = field.Text.Text;
                        string columnType;
                        if (field.GetType() == typeof(IntField))
                        {
                            columnType = "integer";
                        }
                        else if (field.GetType() == typeof(BooleanField))
                        {
                            columnType = "boolean";
                        }
                        else if (field.GetType() == typeof(TextField))
                        {
                            var length = ((TextField)field).Length;
                            columnType = length > 0 ? string.Format("character varying({0})", length.ToString()) : "character varying";
                        }
                        else if (field.GetType() == typeof(LongField))
                        {
                            columnType = "bigint";
                        }
                        else if (field.GetType() == typeof(MoneyField))
                        {
                            columnType = string.Format("numeric({0}, {1})", ((MoneyField)field).Precision.ToString(), ((MoneyField)field).DecimalPlaces.ToString());
                        }
                        else if (field.GetType() == typeof(DateField))
                        {
                            columnType = "datetime";
                        }
                        else if (field.GetType() == typeof(DateTimeField))
                        {
                            columnType = "datetime";
                        }
                        else if (field.GetType() == typeof(BinaryField))
                        {
                            columnType = "bytea";
                        }
                        else if (field.GetType() == typeof(ReferenceIntField))
                        {
                            columnType = "int";
                        }
                        else if (field.GetType() == typeof(ReferenceTextField))
                        {
                            var length = ((ReferenceTextField)field).Length;
                            columnType = length > 0 ? string.Format("character varying({0})", length.ToString()) : "character varying";
                        }
                        else if (field.GetType().Name.Contains("RefField"))
                        {
                            columnType = "character varying";
                        }
                        else if (field.GetType().Name.Contains("JsonField"))
                        {
                            columnType = "character varying";
                        }
                        else if (field.GetType() == typeof(FilesField))
                        {
                            columnType = "character varying";
                        }
                        else if (field.GetType() == typeof(GeomField))
                        {
                            columnType = "geometry";
                        }
                        else if (field.GetType() == typeof(ActivityTypesField))
                        {
                            columnType = "character varying";
                        }
                        else
                        {
                            throw new Exception();
                        }
                        row["ColumnType"] = columnType;
                        dataTable.Rows.Add(row);
                    }
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new System.IO.FileInfo("C:\\Users\\gaan_v\\Desktop\\" + DateTime.Now.Ticks + ".xlsx")))
                {
                    var worksheet = package.Workbook.Worksheets.Add(tables.First(t => !string.IsNullOrEmpty(t.DbKey)).DbKey);
                    worksheet.Cells[1, 1].LoadFromDataTable(dataTable, true);
                    package.Save();
                }

            });
        }
    }
}
