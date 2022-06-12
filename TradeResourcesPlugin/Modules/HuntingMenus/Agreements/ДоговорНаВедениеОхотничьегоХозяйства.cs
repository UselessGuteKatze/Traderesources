using CommonSource.FieldEditors;
using CommonSource.References.Object;
using CommonSource.SearchCollections.Object;
using HuntingSource.FieldEditors.Object;
using HuntingSource.Helpers.Object;
using HuntingSource.Helpers.Trade;
using HuntingSource.QueryTables.Common;
using HuntingSource.QueryTables.Object;
using HuntingSource.QueryTables.Trade;
using HuntingSource.References.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.HuntingMenus.Objects;
using TradeResourcesPlugin.Modules.HuntingMenus.Trades;
using UsersResources;
using Yoda.Interfaces;
using YodaApp.YodaHelpers.Scriban;
using YodaApp.YodaHelpers.SearchCollections;
using YodaCommonReferences;
using YodaHelpers.ActionMenus;
using YodaHelpers.HtmlDocumentBuilder;
using YodaQuery;
using PaymentItemModel = TradeResourcesPlugin.Helpers.Agreements.PaymentItemModel;

namespace TradeResourcesPlugin.Modules.HuntingMenus.Agreements {
    public class ДоговорНаВедениеОхотничьегоХозяйства : DefaultAgrTemplate<ДоговорНаВедениеОхотничьегоХозяйства> {
        public override void SetAgreementNumber(int agreementId)
        {
            DocNumber = $"{agreementId}-ТПР";
        }
        public override string SetDefaultLanguage()
        {
            return "ru";
        }
        public override string[] SetLanguages()
        {
            return new[] { "kz", "ru" };
        }

        public string ГородПосёлокСело { get; set; }
        public ДоговорМодель Договор { get; set; }
        public ОхотничьиУгодьяМодель ОхотничьиУгодья { get; set; }
        public ПраваИОбязанностиМодель ПраваИОбязанности { get; set; }
        public РеквизитыМодель Реквизиты { get; set; }
        public ПаспортМодель Паспорт { get; set; }
        public class ДоговорМодель {
            public АкиматМодель Акимат { get; set; }
            public ПокупательМодель Покупатель { get; set; }
            public int СрокЗаключенияНаЛет { get; set; }
            public string СрокЗаключенияДоДаты { get; set; }
        }
        public class АкиматМодель {
            public string АкиматОбласти { get; set; }
            public string МестныйИсполнительныйОрган { get; set; }
            public string ПостановлениеАкиматаОтДаты { get; set; }
            public string ПостановлениеАкиматаНомер { get; set; }
        }
        public class ПокупательМодель {
            public string Название { get; set; }
            public string ВЛице { get; set; }
            public string НаОсновании { get; set; }
        }
        public class ОхотничьиУгодьяМодель {
            public string ВОбласти { get; set; }
            public string ВРайоне { get; set; }
            public ОхотничьиУгодьяПлощадиМодель Площади { get; set; }
            public МежеваяТочкаМодель[] МежевыеТочки { get; set; }
        }
        public class ОхотничьиУгодьяПлощадиМодель {
            public decimal ОбщаяПлощадь { get; set; }
            public decimal ПлощадьУчастковСельхозНазначения { get; set; }
            public decimal ПлощадьУчастковГосударственногоЛесногоФонда { get; set; }
            public decimal ПлощадьУчастковГосударственногоЗемельногоЗапаса { get; set; }
            public decimal ПлощадьВодоёмов { get; set; }
            public decimal ПлощадьПрочихУчастков { get; set; }
        }
        public class МежеваяТочкаМодель {
            public string X { get; set; }
            public string Y { get; set; }
        }
        public class ПраваИОбязанностиМодель {
            public ОбязанностиПользователяМодель ОбязанностиПользователя { get; set; }
        }
        public class ОбязанностиПользователяМодель {
            public int СоздатьЕгерскуюСлужбуВКоличествеЧеловек { get; set; }
            public string ПровестиИОбеспечитьВыполнениеВнутрихозяйственногоОхотоустройстваДоДаты { get; set; }
        }
        public class РеквизитыМодель {
            public string МестныйИсполнительныйОрган { get; set; }
            public string Пользователь { get; set; }
        }
        public class ПаспортМодель {
            public string Наименование { get; set; }
            public string Пользователь { get; set; }
            public ПаспортОснованиеМодель ПаспортОснование { get; set; }
            public string НаходитсяНаТерриторииРайона { get; set; }
            public string НаходитсяНаТерриторииОбласти { get; set; }
            public int КоличествоЕгерскихУчастков { get; set; }
            public int ВнутрихозяйственноеОхотоустройствоВыполненоВГоду { get; set; }
            public string ИсполнительВнутрихозяйственногоОхотоустройства { get; set; }
            public string КатегорияОхотничьегоХозяйства { get; set; }
            public ТаблицаМодель ПропускнаяСпособность { get; set; }
            public ТаблицаМодель ВнутрихозяйственноеОхотоустройство { get; set; }
            public ТаблицаМодель Штат { get; set; }
            public ТаблицаМодель ОграничениеВредныхЖивотных { get; set; }
            public ТаблицаМодель ВыпускЖивотных { get; set; }
            public ТаблицаМодель БиотехническиеМероприятия { get; set; }
            public ТаблицаМодель ОхотничьиЖивотные { get; set; }
            public ТаблицаМодель Кормы { get; set; }
            public ТаблицаМодель Труд { get; set; }
            public ТаблицаМодель БорьбаСБраконьерством { get; set; }
            public ТаблицаМодель СобакиПодсадныеУтки { get; set; }
            public ТаблицаМодель Строения { get; set; }
            public ТаблицаМодель Транспорт { get; set; }
        }
        public class ПаспортОснованиеМодель {
            public string АкиматОбласти { get; set; }
            public string ПостановлениеАкиматаНомер { get; set; }
            public string ПостановлениеАкиматаОтДаты { get; set; }
            public string ДоговорНомер { get; set; }
            public string ДоговорОтДаты { get; set; }
            public string ДоговорМежду { get; set; }
            public int СрокЗакрепленияЛет { get; set; }
            public string СрокЗакрепленияС { get; set; }
            public string СрокЗакрепленияПо { get; set; }
        }
        public class ТаблицаМодель {
            public Dictionary<string, object>[] Данные { get; set; }
            public Dictionary<string, RowFieldParams> Столбцы { get; set; }
        }
        public override string[] SetReadOnlyProprties() {
            return new[] { "МежевыеТочки", "ПропускнаяСпособностьПоВидамЖивотных", "ПропускнаяСпособность", "ВнутрихозяйственноеОхотоустройство", "Штат", "ОграничениеВредныхЖивотных", "ВыпускЖивотных", "БиотехническиеМероприятия", "ОхотничьиЖивотные", "Кормы", "Труд", "БорьбаСБраконьерством", "СобакиПодсадныеУтки", "Строения", "Транспорт", "Площади" };
        }

        public class paspTableValue {
            public string row { get; set; }
            public string col { get; set; }
            public string val { get; set; }
        }

        public override decimal? GetSellPrice(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            var tradeId = env.Args.TradeId;
            if (tradeId == 0)
            {
                tradeId = new TbAgreements().AddFilter(t => t.flAgreementId, env.Args.AgreementId).SelectScalar(t => t.flTradeId, env.QueryExecuter).Value;
            }
            var flcost = new TbTrades().AddFilter(t => t.flId, tradeId).SelectScalar(t => t.flCost, env.QueryExecuter);
            return flcost;
        }

        public string GetListEditorsXYTable(ТаблицаМодель tableModel, string zeroHeader, string[] colFields, string rowField)
        {
            var tableCols = new string[] { "№", zeroHeader };
            foreach (var colField in colFields)
            {
                var colEditorField = tableModel.Столбцы[colField];
                tableCols = tableCols.Append(colEditorField.text).ToArray();
            }

            var tableRows = new string[] { };
            var rowEditorField = tableModel.Столбцы[rowField];

            var rowValues = tableModel.Данные.Select(x => x[rowField]);
            if (rowEditorField.type == "date")
                rowValues = rowValues.OrderBy(x => Convert.ToDateTime(x));
            else if (rowEditorField.type == "datetime")
                rowValues = rowValues.OrderBy(x => Convert.ToDateTime(x));
            else if (rowEditorField.type == "decimal")
                rowValues = rowValues.OrderBy(x => Convert.ToDecimal(x));
            else if (rowEditorField.type == "int")
                rowValues = rowValues.OrderBy(x => Convert.ToInt32(x));
            else if (rowEditorField.type == "reference")
                rowValues = rowValues.Select(x => SearchInKeyValue(rowEditorField.reference, x.ToString())).OrderBy(x => x);
            else
                rowValues = rowValues.OrderBy(x => x);

            tableRows = tableRows.Union(rowValues.Select(x => x.ToString())).Distinct().ToArray();

            string[][] EmptyTable(string[] cols, string[] rows)
            {
                var table = new string[][] { };
                var dataRows = new List<string[]>() { };
                dataRows.Add(cols);
                var index = 0;
                rows.Each(row => {
                    index++;
                    var dataRow = new string[cols.Length];
                    dataRow[0] = index.ToString();
                    dataRow[1] = row;
                    dataRows.Add(dataRow);
                });
                table = dataRows.ToArray();
                return table;
            }

            string[][] FillTable(string[][] table, ТаблицаМодель tableModel, string[] tableCols, string[] tableRows, string[] colFields, string rowField)
            {
                var paspTableValues = new paspTableValue[] { };

                foreach (var editorRow in tableModel.Данные)
                {
                    foreach (var colField in colFields)
                    {
                        var paspTableValue = new paspTableValue();

                        var colEditorField = tableModel.Столбцы[colField];
                        paspTableValue.col = colEditorField.text;

                        var rowEditorField = tableModel.Столбцы[rowField];
                        paspTableValue.row = editorRow[rowField].ToString();
                        if (rowEditorField.type == "reference")
                        {
                            paspTableValue.row = SearchInKeyValue(rowEditorField.reference, paspTableValue.row);
                        }

                        paspTableValue.val = editorRow[colField].ToString();

                        paspTableValues = paspTableValues.Append(paspTableValue).ToArray();
                    }
                }

                var rowIndex = 1;
                foreach (var row in tableRows)
                {
                    var colIndex = 0;
                    foreach (var col in tableCols)
                    {
                        foreach(var paspTableValue in paspTableValues)
                        {
                            if (paspTableValue.row == row && paspTableValue.col == col)
                                table[rowIndex][colIndex] = paspTableValue.val;
                        }
                        colIndex++;
                    }
                    rowIndex++;
                }

                return table;
            }

            var table = FillTable(EmptyTable(tableCols, tableRows), tableModel, tableCols, tableRows, colFields, rowField);

            return GetListEditorsTableHtml(table);
        }
        public string GetListEditorsYXTable(ТаблицаМодель tableModel, string zeroHeader, string colField, string[] rowFields)
        {
            var tableCols = new string[] { "№", zeroHeader };
            var colEditorField = tableModel.Столбцы[colField];

            var colValues = tableModel.Данные.Select(x => x[colField]);
            if (colEditorField.type == "date")
                colValues = colValues.OrderBy(x => Convert.ToDateTime(x));
            else if (colEditorField.type == "datetime")
                colValues = colValues.OrderBy(x => Convert.ToDateTime(x));
            else if (colEditorField.type == "decimal")
                colValues = colValues.OrderBy(x => Convert.ToDecimal(x));
            else if (colEditorField.type == "int")
                colValues = colValues.OrderBy(x => Convert.ToInt32(x));
            else if (colEditorField.type == "reference")
                colValues = colValues.Select(x => SearchInKeyValue(colEditorField.reference, x.ToString())).OrderBy(x => x);
            else 
                colValues = colValues.OrderBy(x => x);

            tableCols = tableCols.Union(colValues.Select(x => x.ToString())).Distinct().ToArray();


            var tableRows = new string[] {};
            foreach (var rowField in rowFields)
            {
                var rowEditorField = tableModel.Столбцы[rowField];
                tableRows = tableRows.Append(rowEditorField.text).ToArray();
            }



            string[][] EmptyTable(string[] cols, string[] rows)
            {
                var table = new string[][] { };
                var dataRows = new List<string[]>() { };
                dataRows.Add(cols);
                var index = 0;
                rows.Each(row => {
                    index++;
                    var dataRow = new string[cols.Length];
                    dataRow[0] = index.ToString();
                    dataRow[1] = row;
                    dataRows.Add(dataRow);
                });
                table = dataRows.ToArray();
                return table;
            }

            string[][] FillTable(string[][] table, ТаблицаМодель tableModel, string[] tableCols, string[] tableRows, string colField, string[] rowFields)
            {
                var paspTableValues = new paspTableValue[] { };

                foreach (var editorRow in tableModel.Данные)
                {
                    foreach (var rowField in rowFields)
                    {
                        var paspTableValue = new paspTableValue();

                        var rowEditorField = tableModel.Столбцы[rowField];
                        paspTableValue.row = rowEditorField.text;

                        var colEditorField = tableModel.Столбцы[colField];
                        paspTableValue.col = editorRow[colField].ToString();
                        if (colEditorField.type == "reference")
                        {
                            paspTableValue.col = SearchInKeyValue(colEditorField.reference, paspTableValue.col);
                        }

                        paspTableValue.val = editorRow[rowField].ToString();

                        paspTableValues = paspTableValues.Append(paspTableValue).ToArray();
                    }
                }

                var rowIndex = 1;
                foreach (var row in tableRows)
                {
                    var colIndex = 0;
                    foreach (var col in tableCols)
                    {
                        foreach(var paspTableValue in paspTableValues)
                        {
                            if (paspTableValue.row == row && paspTableValue.col == col)
                                table[rowIndex][colIndex] = paspTableValue.val;
                        }
                        colIndex++;
                    }
                    rowIndex++;
                }

                return table;
            }

            var table = FillTable(EmptyTable(tableCols, tableRows), tableModel, tableCols, tableRows, colField, rowFields);

            return GetListEditorsTableHtml(table);
        }
        public string GetListEditorsXYZTable(ТаблицаМодель tableModel, string zeroHeader, string colField, string rowField, string[] valueFields, string cellValuesFormat)
        {
            var tableCols = new string[] { "№", zeroHeader };
            var colEditorField = tableModel.Столбцы[colField];

            var colValues = tableModel.Данные.Select(x => x[colField]);
            if (colEditorField.type == "date")
                colValues = colValues.OrderBy(x => Convert.ToDateTime(x));
            else if (colEditorField.type == "datetime")
                colValues = colValues.OrderBy(x => Convert.ToDateTime(x));
            else if (colEditorField.type == "decimal")
                colValues = colValues.OrderBy(x => Convert.ToDecimal(x));
            else if (colEditorField.type == "int")
                colValues = colValues.OrderBy(x => Convert.ToInt32(x));
            else if (colEditorField.type == "reference")
                colValues = colValues.Select(x => SearchInKeyValue(colEditorField.reference, x.ToString())).OrderBy(x => x);
            else
                colValues = colValues.OrderBy(x => x);

            tableCols = tableCols.Union(colValues.Select(x => x.ToString())).Distinct().ToArray();


            var tableRows = new string[] { };
            var rowEditorField = tableModel.Столбцы[rowField];

            var rowValues = tableModel.Данные.Select(x => x[rowField]);
            if (rowEditorField.type == "date")
                rowValues = rowValues.OrderBy(x => Convert.ToDateTime(x));
            else if (rowEditorField.type == "datetime")
                rowValues = rowValues.OrderBy(x => Convert.ToDateTime(x));
            else if (rowEditorField.type == "decimal")
                rowValues = rowValues.OrderBy(x => Convert.ToDecimal(x));
            else if (rowEditorField.type == "int")
                rowValues = rowValues.OrderBy(x => Convert.ToInt32(x));
            else if (rowEditorField.type == "reference")
                rowValues = rowValues.Select(x => SearchInKeyValue(rowEditorField.reference, x.ToString())).OrderBy(x => x);
            else
                rowValues = rowValues.OrderBy(x => x);

            tableRows = tableRows.Union(rowValues.Select(x => x.ToString())).Distinct().ToArray();

            string[][] EmptyTable(string[] cols, string[] rows)
            {
                var table = new string[][] { };
                var dataRows = new List<string[]>() { };
                dataRows.Add(cols);
                var index = 0;
                rows.Each(row => {
                    index++;
                    var dataRow = new string[cols.Length];
                    dataRow[0] = index.ToString();
                    dataRow[1] = row;
                    dataRows.Add(dataRow);
                });
                table = dataRows.ToArray();
                return table;
            }

            string[][] FillTable(string[][] table, ТаблицаМодель tableModel, string[] tableCols, string[] tableRows, string colField, string rowField, string[] valueFields, string cellValuesFormat)
            {
                var paspTableValues = new paspTableValue[] { };

                foreach (var editorRow in tableModel.Данные)
                {

                    var paspTableValue = new paspTableValue();

                    var colEditorField = tableModel.Столбцы[colField];
                    paspTableValue.col = editorRow[colField].ToString();
                    if (colEditorField.type == "reference")
                    {
                        paspTableValue.col = SearchInKeyValue(colEditorField.reference, paspTableValue.col);
                    }

                    var rowEditorField = tableModel.Столбцы[rowField];
                    paspTableValue.row = editorRow[rowField].ToString();
                    if (rowEditorField.type == "reference")
                    {
                        paspTableValue.row = SearchInKeyValue(rowEditorField.reference, paspTableValue.row);
                    }

                    paspTableValue.val = cellValuesFormat;

                    foreach (var valueField in valueFields)
                    {
                        var valEditorField = tableModel.Столбцы[valueField];
                        var val = editorRow[valueField].ToString();
                        if (valEditorField.type == "reference")
                        {
                            val = SearchInKeyValue(valEditorField.reference, val);
                        }
                        paspTableValue.val = paspTableValue.val.Replace(valueField, val);
                    }
                    paspTableValues = paspTableValues.Append(paspTableValue).ToArray();
                }

                var rowIndex = 1;
                foreach (var row in tableRows)
                {
                    var colIndex = 0;
                    foreach (var col in tableCols)
                    {
                        foreach(var paspTableValue in paspTableValues)
                        {
                            if (paspTableValue.row == row && paspTableValue.col == col)
                                table[rowIndex][colIndex] = paspTableValue.val;
                        }
                        colIndex++;
                    }
                    rowIndex++;
                }

                return table;
            }

            var table = FillTable(EmptyTable(tableCols, tableRows), tableModel, tableCols, tableRows, colField, rowField, valueFields, cellValuesFormat);

            return GetListEditorsTableHtml(table);
        }
        public string GetListEditorsXYZTable(ТаблицаМодель tableModel, string zeroHeader, string colField, string rowField, string valueField)
        {
            return GetListEditorsXYZTable(tableModel, zeroHeader, colField, rowField, new[] { valueField }, valueField);
        }
        public string GetListEditorsTableHtml(string[][] table)
        {
            var tableHtml = "<table class=\"wide-table\"><tbody>";
            foreach(var tableRow in table)
            {
                tableHtml += "<tr>";
                foreach (var tableCol in tableRow)
                {
                    tableHtml += $"<td class=\"td-border-solid-gray\">{tableCol}</td>";
                }
                tableHtml += "</tr>";
            }
            tableHtml += "</tbody></table>";
            return tableHtml;
        }
        public override string GetContent()
        {
            var template = string.Empty;

            switch(Language)
            {
                case "ru":
                    {
                        template += new DocumentBuilder()
                            .Doc(doc => {
                            doc.AddSection(s => s.Body(b => b
                                .Div("ДОГОВОР № {{ DocNumber }} НА ВЕДЕНИЕ ОХОТНИЧЬЕГО ХОЗЯЙСТВА", new CssClass("text-center font-weight-bold"))
                                .BrTag()
                                .Div("{{ ГородПосёлокСело }}")
                                .BrTag()
                                .Paragraph("На основании постановления акимата {{ Договор.Акимат.АкиматОбласти }} от {{ Договор.Акимат.ПостановлениеАкиматаОтДаты }} № {{ Договор.Акимат.ПостановлениеАкиматаНомер }} {{ Договор.Акимат.МестныйИсполнительныйОрган }}, действующий на основании Положения, в дальнейшем именуемый «Местный исполнительный орган», с одной стороны, и {{ Договор.Покупатель.Название }}, в лице {{ Договор.Покупатель.ВЛице }} именуемый в дальнейшем «Покупатель», действующий на основании {{ Договор.Покупатель.НаОсновании }}, заключили настоящий договор (далее – договор) о нижеследующем:")
                                .BrTag()

                                .Div("1. Предмет договора", new CssClass("text-center font-weight-bold"))
                                .Paragraph("1. Местный исполнительный орган предоставляет право ведения охотничьего хозяйства на закрепленных за Пользователем охотничьих угодий, расположенных в {{ ОхотничьиУгодья.ВОбласти }} {{ ОхотничьиУгодья.ВРайоне }}, общей площадью {{ ОхотничьиУгодья.Площади.ОбщаяПлощадь }} гектар, из них земельные участки:")
                                .Paragraph("сельскохозяйственного назначения {{ ОхотничьиУгодья.Площади.ПлощадьУчастковСельхозНазначения }} гектар,")
                                .Paragraph("государственного лесного фонда {{ ОхотничьиУгодья.Площади.ПлощадьУчастковГосударственногоЛесногоФонда }} гектар,")
                                .Paragraph("государственного земельного запаса {{ ОхотничьиУгодья.Площади.ПлощадьУчастковГосударственногоЗемельногоЗапаса }} гектар;")
                                .Paragraph("водоемы {{ ОхотничьиУгодья.Площади.ПлощадьВодоёмов }} гектар;")
                                .Paragraph("прочие: {{ ОхотничьиУгодья.Площади.ПлощадьПрочихУчастков }} гектар, в границах:")
                                .Table(new [] { new DocTableCol("X", "Широта", 50), new DocTableCol("Y", "Долгота", 50) }, ОхотничьиУгодья.МежевыеТочки, tableCssClass: "wide-table")
                                .BrTag()

                                .Div("2. Права и обязанности сторон", new CssClass("text-center font-weight-bold"))
                                .Paragraph("2. Пользователь имеет право:")
                                .Paragraph("1) выдавать и устанавливать срок действия путевки в установленном законодательством Республики Казахстан порядке;")
                                .Paragraph("2) осуществлять только те виды пользования животным миром, которые им разрешены;")
                                .Paragraph("3) пользоваться объектами животного мира в соответствии с условиями их предоставления;")
                                .Paragraph("4) собственности на добытые объекты животного мира, в том числе охотничьи трофеи и полученную при этом продукцию, а также на их перевозку и реализацию;")
                                .Paragraph("5) заключать договоры с физическими и юридическими лицами на пользование животным миром, в том числе в электронной форме на веб-портале реестра государственного имущества (далее – Портал);")
                                .Paragraph("6) строительства временных сооружений для нужд охотничьего хозяйства в соответствии с установленным сервитутом;")
                                .Paragraph("7) обеспечить егерей служебным оружием в соответствии с нормами и правилами, установленными законодательством Республики Казахстан;")
                                .Paragraph("8) осуществлять дичеразведение и проводить любительскую (спортивную) охоту на территории, отведенной для дичеразведения (в неволе и (или) полувольных условиях), а также самостоятельно использовать воспроизведенных в результате дичеразведения животных.")
                                .Paragraph("3. Пользователь обязан:")
                                .Paragraph("1) соблюдать требования законодательства Республики Казахстан в области охраны, воспроизводства и использования животного мира;")
                                .Paragraph("2) своевременно вносить плату за пользование животным миром по месту получения разрешения в порядке, установленном налоговым законодательством Республики Казахстан;")
                                .Paragraph("3) не допускать ухудшения среды обитания животных;")
                                .Paragraph("4) соблюдать требования пожарной безопасности;")
                                .Paragraph("5) пользоваться животным миром способами, безопасными для населения и окружающей среды, не допускающими нарушения целостности естественных сообществ и жестокого обращения с животными;")
                                .Paragraph("6) проводить ежегодный учет численности используемых объектов животного мира и представлять отчетность в порядке, установленном законодательством Республики Казахстан;")
                                .Paragraph("7) обеспечивать охрану и воспроизводство объектов животного мира, в том числе редких и находящихся под угрозой исчезновения, и не допускать снижение их численности;")
                                .Paragraph("8) утверждать внутренний регламент охотничьего хозяйства;")
                                .Paragraph("9) выдавать путевки на проведение любительской (спортивной) охоты физическим лицам по их устному и письменному заявлению, в том числе в электронной форме;")
                                .Paragraph("10) проводить необходимые мероприятия, обеспечивающие воспроизводство объектов животного мира в соответствии с внутрихозяйственным охотоустройством;")
                                .Paragraph("11) устанавливать аншлаги;")
                                .Paragraph("12) создать егерскую службу в количестве {{ ПраваИОбязанности.ОбязанностиПользователя.СоздатьЕгерскуюСлужбуВКоличествеЧеловек }} (человек);")
                                .Paragraph("13) обеспечивать проведение ветеринарных мероприятий;")
                                .Paragraph("14) провести до {{ ПраваИОбязанности.ОбязанностиПользователя.ПровестиИОбеспечитьВыполнениеВнутрихозяйственногоОхотоустройстваДоДаты }} внутрихозяйственное охотоустройство и обеспечить его выполнение;")
                                .Paragraph("15) исполнять обязательства, заявленные Пользователем при участии в конкурсе на закрепление охотничьих угодий;")
                                .Paragraph("16) выполнять условия настоящего договора;")
                                .Paragraph("17) не препятствовать осуществлению проверок в целях государственного контроля и надзора за соблюдением требований законодательства Республики Казахстан об охране, воспроизводстве и использовании животного мира;")
                                .Paragraph("18) в порядке и сроки, установленные уполномоченным органом, направлять в территориальное подразделение ведомства информацию о заключенных договорах с физическими и юридическими лицами на пользование животным миром, в том числе об их расторжении;")
                                .Paragraph("19) обеспечить егерей средствами транспорта, связи, специальной одеждой со знаками различия, нагрудным знаком егеря, удостоверением егеря;")
                                .Paragraph("20) вести культурно-просветительскую работу в области охраны природы и использования животного мира;")
                                .Paragraph("21) уведомлять ведомство уполномоченного органа о создании зоологической коллекции. Подача уведомления в ведомство уполномоченного органа осуществляется не менее чем за десять рабочих дней до начала осуществления деятельности;")
                                .Paragraph("22) производить финансирование мероприятий по охране, воспроизводству и устойчивому использованию животного мира на закрепленных охотничьих угодьях за счет средств субъектов охотничьего хозяйства;")
                                .Paragraph("23) обеспечить производственный контроль по охране, воспроизводству и использованию животного мира на закрепленных охотничьих угодьях;")
                                .Paragraph("24) предоставить физическим лицам сервитут для осуществления любительской (спортивной) охоты;")
                                .Paragraph("25) при осуществлении эмиссий в окружающую среду получать экологическое разрешение в соответствии с Экологическим кодексом Республики Казахстан.")
                                .Paragraph("4. Местный исполнительный орган в пределах компетенции, установленной законодательством Республики Казахстан, имеет право расторгать в одностороннем порядке договор:")
                                .Paragraph("1) при систематическом нарушении условий договора на ведение охотничьего хозяйства;")
                                .Paragraph("2) при систематическом нарушении требований законодательства Республики Казахстан в области охраны, воспроизводства и использования животного мира.")
                                .BrTag()

                                .Div("3. Ответственность сторон", new CssClass("text-center font-weight-bold"))
                                .Paragraph("5. Пользователь ни полностью, ни частично не должен передавать кому-либо свои обязательства по настоящему договору.")
                                .Paragraph("6. Местный исполнительный орган выдает разрешения на пользование животным миром в порядке, установленном законодательством Республики Казахстан.")
                                .Paragraph("7. В случае невыполнения Пользователем обязательств по настоящему договору к нему могут быть применены меры воздействия в соответствии с законодательством Республики Казахстан.")
                                .Paragraph("8. В случае нарушения прав Пользователя, Местный исполнительный орган несет ответственность в соответствии с законодательством Республики Казахстан.")
                                .BrTag()

                                .Div("4. Обстоятельства непреодолимой силы", new CssClass("text-center font-weight-bold"))
                                .Paragraph("9. Ни одна из сторон не будет нести ответственности за неисполнение или ненадлежащее исполнение каких-либо обязательств договора, если такое неисполнение или ненадлежащее исполнение вызваны обстоятельствами непреодолимой силы.")
                                .Paragraph("10. Обстоятельством непреодолимой силы признается событие, препятствующее исполнению настоящего договора, неподвластное контролю Сторон, не связанное с их просчетом или небрежностью и имеющее непредвиденный характер.")
                                .Paragraph("11. В случае возникновения обстоятельств непреодолимой силы, Пользователь незамедлительно уведомляет об этом Местный исполнительный орган и Инспекцию путем вручения и (или) отправки письменного уведомления по почте либо факсимильной связью, уточняющего дату начала и описание обстоятельств непреодолимой силы.")
                                .Paragraph("12. При возникновении обстоятельств непреодолимой силы Стороны незамедлительно проводят совещание с участием представителей Инспекции, для поиска решения выхода из сложившейся ситуации и используют все не противоречащие законодательству средства, для сведения к минимуму последствий обстоятельств непреодолимой силы.")
                                .Paragraph("13. Обстоятельства непреодолимой силы, указанные в настоящей главе, признаются правомочными, если они подтверждены компетентными государственными органами и организациями.")
                                .BrTag()

                                .Div("5. Заключительные положения", new CssClass("text-center font-weight-bold"))
                                .Paragraph("14. Настоящий договор вступает в силу с момента подписания и заключен сроком на {{ Договор.СрокЗаключенияНаЛет }} лет до {{ Договор.СрокЗаключенияДоДаты }}.")
                                .Paragraph("15. Действие настоящего договора прекращается в случаях:")
                                .Paragraph("1) добровольного отказа от ведения охотничьего хозяйства;")
                                .Paragraph("2) истечения срока действия договора;")
                                .Paragraph("3) прекращения деятельности Пользователя;")
                                .Paragraph("4) изъятия земельных участков, на которых произведено закрепление охотничьих угодий и (или) участков, для государственных нужд в порядке, определенном законодательством Республики Казахстан.")
                                .Paragraph("16. При разрешении споров по ведению охотничьего хозяйства стороны руководствуются условиями настоящего договора, внутрихозяйственным охотоустройством и законодательством Республики Казахстан.")
                                .Paragraph("17. Паспорт установленной формы, с картами–схемами спроектированных охотничьих хозяйств с указанием учетных площадок и маршрутов учета животных, согласно приложению к настоящему договору, является неотъемлемой частью договора.")
                                .Paragraph("Паспорт охотничьего хозяйства заполняется Пользователем на Портале ежегодно, в первом квартале следующим за отчетным годом на основании документов статистического и бухгалтерского учетов.")
                                .Paragraph("18. Все изменения и дополнения к настоящему договору имеют юридическую силу и являются неотъемлемой его частью, если они совершены в электронной форме и подписаны уполномоченными представителями обеих сторон.")
                                .Paragraph("19. Стороны стремятся к разрешению споров, возникающих из настоящего договора, путем переговоров, а в случае не достижения сторонами соглашения, разрешаются в порядке, установленном законодательством Республики Казахстан.")
                                .Paragraph("20. Настоящий договор составлен в двух экземплярах на государственном и русском языках, имеющих одинаковую юридическую силу.")
                                .Paragraph("21. Настоящий Договор вступает в силу с момента его подписания последней из Сторон с использованием электронной цифровой подписи на Портале.")
                                .Paragraph("При этом, датой заключения настоящего договора определяется дата его подписания с ЭЦП последней из Сторон.")
                                .BrTag()

                                .Div("6. Юридические адреса и реквизиты сторон", new CssClass("text-center font-weight-bold"))
                                .Table(new[] { new DocTableCol("МестныйИсполнительныйОрган", "Местный исполнительный орган", 50, "vertical-align-top text-align-justify p-0-10-px"), new DocTableCol("Пользователь", "Пользователь", 50, "vertical-align-top text-align-justify p-0-10-px") }, new РеквизитыМодель[] { Реквизиты }, tableCssClass: "wide-table")
                            ));
                        }).Build();
                        
                        template += new DocumentBuilder()
                            .Doc(doc => {
                            doc.AddSection(s => s.Body(b => b
                                .Div("Паспорт охотничьего хозяйства", new CssClass("text-center font-weight-bold"))
                                .Div("Глава 1. Описание охотничьего хозяйства", new CssClass("text-center font-weight-bold"))
                                .BrTag()
                                .Paragraph("1. Наименование охотничьего хозяйства: {{ Паспорт.Наименование }}")
                                .Paragraph("2. Пользователь: {{ Паспорт.Пользователь }}")
                                .Paragraph("3. Основание - постановление акимата {{ Паспорт.ПаспортОснование.АкиматОбласти }} № {{ Паспорт.ПаспортОснование.ПостановлениеАкиматаНомер }} от {{ Паспорт.ПаспортОснование.ПостановлениеАкиматаОтДаты }} и договор на ведение охотничьего хозяйства № {{ Паспорт.ПаспортОснование.ДоговорНомер }} от {{ Паспорт.ПаспортОснование.ДоговорОтДаты }} , заключенный между {{ Паспорт.ПаспортОснование.ДоговорМежду }}.")
                                .Paragraph("Срок закрепления {{ Паспорт.ПаспортОснование.СрокЗакрепленияЛет }} лет, с {{ Паспорт.ПаспортОснование.СрокЗакрепленияС }} по {{ Паспорт.ПаспортОснование.СрокЗакрепленияПо }}.")
                                .Paragraph("4. Охотничье хозяйство находится на территории: {{ Паспорт.НаходитсяНаТерриторииРайона }} {{ Паспорт.НаходитсяНаТерриторииОбласти }}.")
                                .Paragraph("5. Границы охотничьего хозяйства:")
                                .Table(new[] { new DocTableCol("X", "Широта", 50), new DocTableCol("Y", "Долгота", 50) }, ОхотничьиУгодья.МежевыеТочки, tableCssClass: "wide-table")
                                .Paragraph("6. Площадь охотничьего хозяйства {{ ОхотничьиУгодья.Площади.ОбщаяПлощадь }} гектар, в том числе:")
                                .Paragraph("земли государственного лесного фонда {{ ОхотничьиУгодья.Площади.ПлощадьУчастковГосударственногоЛесногоФонда }} гектар,")
                                .Paragraph("закрепленные земли сельскохозяйственного назначения {{ ОхотничьиУгодья.Площади.ПлощадьУчастковСельхозНазначения }} гектар,")
                                .Paragraph("водоемы {{ ОхотничьиУгодья.Площади.ПлощадьВодоёмов }} гектар,")
                                .Paragraph("земли государственного земельного запаса {{ ОхотничьиУгодья.Площади.ПлощадьУчастковГосударственногоЗемельногоЗапаса }} гектар,")
                                .Paragraph("прочие {{ ОхотничьиУгодья.Площади.ПлощадьПрочихУчастков }} гектар.")
                                .Paragraph("7. Количество егерских участков (обходов) в охотничьем хозяйстве: {{ Паспорт.КоличествоЕгерскихУчастков }}")
                                .BrTag()

                                .Div("Глава 2. Показатели охотоустройства", new CssClass("text-center font-weight-bold"))
                                .Paragraph("8. Внутрихозяйственное охотоустройство выполнено в {{ Паспорт.ВнутрихозяйственноеОхотоустройствоВыполненоВГоду }} году {{ Паспорт.ИсполнительВнутрихозяйственногоОхотоустройства }}.")
                                .Paragraph("9. Категория охотничьего хозяйства {{ Паспорт.КатегорияОхотничьегоХозяйства }}, основные направления деятельности охотничьего хозяйства: сохранение видового разнообразия животного мира, их среды обитания, устойчивое использование, воспроизводство и охрана видов животных, в том числе не относящихся к объектам охоты, организация любительской (спортивной) охоты.")
                                .Paragraph("10. Бонитетная оценка по основным видам животных, являющихся объектами охоты, приводится в материалах внутрихозяйственного охотоустройства.")

                                .Paragraph("11. Пропускная способность охотничьих угодий по видам животных:")
                                .Html(string.Join("", Паспорт.ПропускнаяСпособность.Данные.Select(row => $"<p class=\"app-section-item\">{SearchInKeyValue(Паспорт.ПропускнаяСпособность.Столбцы["flWildfowlType"].reference, row["flWildfowlType"].ToString())}: {row["flCount"]};</p>")))
                                .Paragraph("12. Показатели внутрихозяйственного охотоустройства:")
                                .Html(GetListEditorsYXTable(
                                    Паспорт.ВнутрихозяйственноеОхотоустройство,
                                    "Показатели внутрихозяйственного охотоустройства",
                                    "flYear",
                                    new[] {
                                        "flReproductionObjectsCount",
                                        "flReproductionObjectsArea",
                                        "flRestObjectsCount",
                                        "flRestObjectsArea",
                                        "flSoldOutsCount",
                                        "flTowersCount",
                                        "flHutsCount",
                                        "flAnother"
                                    }
                                ))
                                .Paragraph("13. Штат охотничьего хозяйства:")
                                .Html(GetListEditorsXYTable(
                                    Паспорт.Штат,
                                    "Наименование штатных должностей",
                                    new[] {
                                        "flWorkerTypeCount",
                                        "flWorkerTypeSalary",
                                        "flNote"
                                    },
                                    "flWorkerType"
                                ))
                                .BrTag()

                                .Div("Глава 3. Показатели воспроизводства", new CssClass("text-center font-weight-bold"))
                                .Paragraph("14. Ограничение численности вредных для охотничьего хозяйства животных:")
                                .Html(GetListEditorsXYZTable(
                                    Паспорт.ОграничениеВредныхЖивотных,
                                    "Добыто (голов)",
                                    "flYear",
                                    "flHarmfulAnimalType",
                                    "flCount"
                                ))
                                .Paragraph("15. Выпуск животных в охотничье хозяйство:")
                                .Html(GetListEditorsXYZTable(
                                    Паспорт.ВыпускЖивотных,
                                    "Вид животного",
                                    "flYear",
                                    "flAnimalType",
                                    "flCount"
                                ))
                                .Paragraph("16. Проведение биотехнических мероприятий в охотничьем хозяйстве (количество единиц):")
                                .Html(GetListEditorsXYZTable(
                                    Паспорт.БиотехническиеМероприятия,
                                    "Устроено",
                                    "flYear",
                                    "flActivities",
                                    "flCount"
                                ))
                                .Paragraph("17. Учтено охотничьих животных на территории охотничьего хозяйства (особей):")
                                .Html(GetListEditorsXYZTable(
                                    Паспорт.ОхотничьиЖивотные,
                                    "Виды животных",
                                    "flYear",
                                    "flAnimalType",
                                    "flCount"
                                ))
                                .Paragraph("18. Заготовлено и выложено кормов и подкормок для диких животных:")
                                .Html(GetListEditorsXYZTable(
                                    Паспорт.Кормы,
                                    "Виды кормов",
                                    "flYear",
                                    "flWildAnimalsFeedTypes",
                                    new[] { "flHarvestedWeight", "flLaidOutWeight", "flUnit" },
                                    "flHarvestedWeight/flLaidOutWeight flUnit."
                                ))
                                .Paragraph("19. Количество труда, вложенного в деятельность охотничьего хозяйства:")
                                .Html(GetListEditorsXYZTable(
                                    Паспорт.Труд,
                                    "Наименование работ",
                                    "flYear",
                                    "flWorkType",
                                    new[] { "flValue", "flUnit" },
                                    "flValue flUnit."
                                ))
                                .BrTag()

                                .Div("Глава 4. Экономические показатели", new CssClass("text-center font-weight-bold"))
                                .Paragraph("20. Борьба с браконьерством:")
                                .Html(GetListEditorsXYZTable(
                                    Паспорт.БорьбаСБраконьерством,
                                    "Выявлено фактов нарушений правил:",
                                    "flYear",
                                    "flRuleType",
                                    "flValue"
                                ))
                                .Paragraph("21. Наличие в охотничьем хозяйстве охотничьих собак, подсадных уток:")
                                .Html(GetListEditorsXYZTable(
                                    Паспорт.СобакиПодсадныеУтки,
                                    "Виды",
                                    "flYear",
                                    "flHunterAnimalType",
                                    "flCount"
                                ))
                                .Paragraph("22. Наличие строений в охотничьем хозяйстве:")
                                .Html(GetListEditorsXYTable(
                                    Паспорт.Строения,
                                    "Вид строения",
                                    new[] { "flBuildYear", "flCount", "flArea", "flBerthsCount", "flNote" },
                                    "flBuildingType"
                                ))
                                .Paragraph("23. Наличие транспорта в охотничьем хозяйстве:")
                                .Html(GetListEditorsXYZTable(
                                    Паспорт.Транспорт,
                                    "Виды транспорта",
                                    "flYear",
                                    "flTransportType",
                                    "flCount"
                                ))

                                .Paragraph("Паспорт охотничьего хозяйства заполняется Пользователем на веб-портале реестра государственного имущества, ежегодно в первом квартале следующим за отчетным годом на основании документов статистического и бухгалтерского учетов. К паспорту прилагается карта-схема охотничьего хозяйства с нанесенными границами и межевыми точками, и карта-схема с указанием учетных площадок и маршрутов учета животных.")
                            ));
                        }).Build();

                        break;
                    }
                case "kz":
                    {
                        template += new DocumentBuilder()
                            .Doc(doc => {
                            doc.AddSection(s => s.Body(b => b
                                .Div("№ {{ DocNumber }} ҮЛГІЛІК НЫСАН АҢШЫЛЫҚ ШАРУАШЫЛЫҒЫН ЖҮРГІЗУГЕ АРНАЛҒАН ШАРТ", new CssClass("text-center font-weight-bold"))
                                .BrTag()
                                .Div("{{ ГородПосёлокСело }}")
                                .BrTag()
                                .Paragraph("{{ Договор.Акимат.АкиматОбласти }} әкiмдігінiң {{ Договор.Акимат.ПостановлениеАкиматаОтДаты }} № {{ Договор.Акимат.ПостановлениеАкиматаНомер }} қаулысы негізiнде, бұдан әрi «Жергілікті атқарушы орган» деп аталатын, Ереже негізінде әрекет ететін {{ Договор.Акимат.МестныйИсполнительныйОрган }}, бiр тараптан және бұдан әрі «Пайдаланушы» деп аталатын, {{ Договор.Покупатель.НаОсновании }} негізінде әрекет ететін {{ Договор.Покупатель.ВЛице }} атынан {{ Договор.Покупатель.Название }} екінші тараптан, төмендегiлер туралы осы шартты (бұдан әрі – шарт) жасасты:")
                                .BrTag()

                                .Div("1. Шарттың нысанасы", new CssClass("text-center font-weight-bold"))
                                .Paragraph("1. Жергілікті атқарушы орган {{ ОхотничьиУгодья.ВОбласти }} {{ ОхотничьиУгодья.ВРайоне }} шекараларында:")
                                .Table(new[] { new DocTableCol("X", "Ендік", 50), new DocTableCol("Y", "Бойлық", 50) }, ОхотничьиУгодья.МежевыеТочки, tableCssClass: "wide-table")
                                .Paragraph("орналасқан, жалпы алаңы {{ ОхотничьиУгодья.Площади.ОбщаяПлощадь }} гектар, оның iшiнде:")
                                .Paragraph("ауыл шаруашылығы мақсатындағы жер учаскелерi {{ ОхотничьиУгодья.Площади.ПлощадьУчастковСельхозНазначения }} гектар,")
                                .Paragraph("мемлекеттiк орман қорының жер учаскелерi {{ ОхотничьиУгодья.Площади.ПлощадьУчастковГосударственногоЛесногоФонда }} гектар,")
                                .Paragraph("мемлекеттiк жер қорының жер учаскелерi {{ ОхотничьиУгодья.Площади.ПлощадьУчастковГосударственногоЗемельногоЗапаса }} гектар;")
                                .Paragraph("су айдындары {{ ОхотничьиУгодья.Площади.ПлощадьВодоёмов }} гектар;")
                                .Paragraph("басқалары {{ ОхотничьиУгодья.Площади.ПлощадьПрочихУчастков }} гектар болатын Пайдаланушыға бекiтiп берiлген аңшылық алқаптарда аңшылық шаруашылығын жүргiзу құқығын бередi.")
                                .BrTag()

                                .Div("2. Тараптардың құқықтары мен мiндеттерi", new CssClass("text-center font-weight-bold"))
                                .Paragraph("2. Пайдаланушының:")
                                .Paragraph("1) Қазақстан Республикасының заңнамасында белгiленген тәртiппен жолдамалар беруге және олардың пайдалану мерзімін белгiлеуге;")
                                .Paragraph("2) жануарлар дүниесін пайдаланудың тек рұқсат берiлген түрлерін ғана жүзеге асыруға;")
                                .Paragraph("3) жануарлар дүниесi объектiлерiн оларды беру шарттарына сәйкес пайдалануға;")
                                .Paragraph("4) ауланған жануарлар дүниесi объектiлерiн, оның ішінде аңшылық олжаларын және бұл ретте алынған өнiмдi меншiктенуге, сондай-ақ оларды тасымалдауға және сатуға;")
                                .Paragraph("5) жануарлар дүниесін пайдалануға жеке және заңды тұлғалармен шарттар жасасуға, соның ішінде электрондық нысанда мемлекеттік мүлік тізілімінің веб-порталында (бұдан әрі - Портал);")
                                .Paragraph("6) белгіленген сервитутқа сәйкес аңшылық шаруашылығының мұқтаждықтары үшін уақытша құрылысжайлар салуға;")
                                .Paragraph("7) Қазақстан Республикасының заңнамасында белгіленген нормалар мен қағидаларға сәйкес қорықшыларды қызметтік қарумен қамтамасыз етуге;")
                                .Paragraph("8) аң-құс өсіруді жүзеге асыруға және аң-құс өсіруге (еріксіз және (немесе) жартылай ерікті жағдайларда) арналған аумақта әуесқойлық (спорттық) аң аулауды жүргізуге, сондай-ақ аң-құс өсіру нәтижесінде өсімі молайған жануарларды өз бетінше пайдалануға құқығы бар.")
                                .Paragraph("3. Пайдаланушы:")
                                .Paragraph("1) жануарлар дүниесiн қорғау, өсiмiн молайту және пайдалану саласындағы Қазақстан Республикасы заңнамасының талаптарын сақтауға;")
                                .Paragraph("2) Қазақстан Республикасының салық заңнамасында белгiленген тәртiппен рұқсат алынған жер бойынша жануарлар дүниесiн пайдаланғаны үшiн төлемақыны дер кезiнде төлеуге;")
                                .Paragraph("3) жануарлар мекендейтiн ортаның нашарлауына жол бермеуге;")
                                .Paragraph("4) өрт қауіпсіздігі талаптарын сақтауға;")
                                .Paragraph("5) жануарлар дүниесін табиғи қауымдастықтар тұтастығының бұзылуына және жануарларға қатыгез қарауға жол бермейтiн халық пен қоршаған орта үшiн қауiпсiз тәсiлдермен пайдалануға;")
                                .Paragraph("6) пайдаланылатын жануарлар дүниесi объектiлерi санының жыл сайынғы есебін жүргізіп, Қазақстан Республикасының заңнамасында белгіленген тәртіппен есеп беріп тұруға;")
                                .Paragraph("7) жануарлар дүниесi объектiлерiн, оның iшiнде сирек кездесетiн және құрып кету қаупі төнгендерiн қорғау мен өсiмiн молайтуды қамтамасыз етуге және олардың санының азайып кетуіне жол бермеуге;")
                                .Paragraph("8) аңшылық шаруашылығының ішкі регламентін бекітуге;")
                                .Paragraph("9) жеке тұлғаларға олардың жазбаша және ауызша өтініші бойынша әуесқойлық (спорттық) аң аулауды жүргізуге жолдама беруге, соның ішінде электрондық нысанда;")
                                .Paragraph("10) шаруашылық ішіндегі аңшылық ісін ұйымдастыруға сәйкес жануарлар дүниесi объектiлерiнің өсімін молайтуды қамтамасыз ететiн қажеттi iс-шаралар жүргiзуге;")
                                .Paragraph("11) аншлагтар орнатуға;")
                                .Paragraph("12) {{ ПраваИОбязанности.ОбязанностиПользователя.СоздатьЕгерскуюСлужбуВКоличествеЧеловек }} адам мөлшерінде қорықшылық қызметiн құруға;")
                                .Paragraph("13) ветеринариялық iс-шаралардың өткiзiлуiн қамтамасыз етуге;")
                                .Paragraph("14) {{ ПраваИОбязанности.ОбязанностиПользователя.ПровестиИОбеспечитьВыполнениеВнутрихозяйственногоОхотоустройстваДоДаты }} дейін шаруашылық ішіндегі аңшылық ісін ұйымдастыруға және оның орындалуын қамтамасыз етуге;")
                                .Paragraph("15) аңшылық алқаптарды бекiтiп беруге арналған конкурсқа қатысу кезінде Пайдаланушы мәлiмдеген мiндеттемелердi орындауға;")
                                .Paragraph("16) осы шарттың талаптарын орындауға;")
                                .Paragraph("17) жануарлар дүниесiн қорғау, өсiмiн молайту және пайдалану саласындағы Қазақстан Республикасы заңнамасы талаптарының сақталуын мемлекеттiк бақылау және қадағалау мақсатында тексерістерді жүзеге асыруға кедергi жасамауға;")
                                .Paragraph("18) уәкілетті орган белгілеген тәртіппен және мерзімдерде ведомствоның аумақтық бөлімшесіне жеке және заңды тұлғалармен жануарлар дүниесін пайдалануға жасалған шарттар туралы, оның ішінде оларды бұзу туралы ақпарат жіберуге;")
                                .Paragraph("19) қорықшыларды көлік, байланыс құралдарымен, айырым белгілері бар арнайы киіммен, қорықшының төсбелгісімен, қорықшының куәлігімен қамтамасыз етуге;")
                                .Paragraph("20) табиғатты қорғау және жануарлар дүниесін пайдалану саласында мәдени-ағарту жұмыстарын жүргізуге;")
                                .Paragraph("21) зоологиялық коллекцияның жасалғаны туралы уәкілетті органның ведомствосын хабардар етуге міндетті. Уәкілетті органның ведомствосына хабарламалар беру қызметтің жүзеге асырылуы басталғанға дейін кемінде он жұмыс күні бұрын жүзеге асырылады;")
                                .Paragraph("22) бекiтiп берiлген аңшылық алқаптарда жануарлар дүниесiн қорғау, өсiмiн молайту және пайдалану жөніндегі іс-шараларды аңшылық шаруашылығы субъектілері есебінен қаржыландыруға;")
                                .Paragraph("23) бекiтiп берiлген аңшылық алқаптарда жануарлар дүниесiн қорғау, өсiмiн молайту және пайдалану бойынша өндірістік бақылауды қамтамасыз етуге;")
                                .Paragraph("24) жеке тұлғаларға әуесқойлық (спорттық) аң аулауды жүзеге асыру үшін сервитут беруге;")
                                .Paragraph("25) қоршаған ортаға эмиссияларды жүзеге асырғанда Қазақстан Республикасының Экологиялық кодексіне сәйкес экологиялық рұқсат алуға міндетті.")
                                .Paragraph("4. Жергілікті атқарушы органның Қазақстан Республикасының заңнамасымен белгіленген құзырет шегінде:")
                                .Paragraph("1) аңшылық шаруашылығын жүргізу шартының талаптары жүйелі түрде бұзылғанда;")
                                .Paragraph("2) жануарлар дүниесiн қорғау, өсiмiн молайту және пайдалану саласындағы Қазақстан Республикасы заңнамасының талаптары жүйелі түрде бұзылғанда шартты бір жақты тәртіппен бұзуға құқығы бар.")
                                .BrTag()

                                .Div("3. Тараптардың жауапкершiлiгi", new CssClass("text-center font-weight-bold"))
                                .Paragraph("5. Пайдаланушы осы шарт бойынша міндеттемелерін ешкімге толықтай да, ішінара да бермеуі тиіс.")
                                .Paragraph("6. Жергілікті атқарушы орган Қазақстан Республикасының заңнамасымен белгіленген тәртіппен жануарлар дүниесін пайдалануға рұқсат береді.")
                                .Paragraph("7. Пайдаланушы осы шарт бойынша міндеттемелерін орындамаған жағдайда, Қазақстан Республикасының заңнамасына сәйкес оған ықпал ету шаралары қолданылуы мүмкін.")
                                .Paragraph("8. Пайдаланушының құқықтары бұзылған жағдайда, Жергілікті атқарушы орган Қазақстан Республикасының заңнамасына сәйкес жауапты болады.")
                                .BrTag()

                                .Div("4. Еңсерілмейтін күштің мән-жайлары", new CssClass("text-center font-weight-bold"))
                                .Paragraph("9. Егер шарттың қандай да бір міндеттемелерін орындамау немесе тиісінше орындамау еңсерілмейтін күштің мән-жайларынан туындаса, Тараптардың ешқайсысы осылай орындамағаны немесе тиісінше орындамағаны үшін жауапты болмайды.")
                                .Paragraph("10. Еңсерілмейтін күштің мән-жайлары деп Тараптардың бақылауына көнбейтін, олардың қателігімен немесе салақтығымен байланысты емес және тосын сипатқа ие осы шартты орындауға кедергі келтіретін оқиға танылады.")
                                .Paragraph("11. Еңсерілмейтін күштің мән-жайлары туындаған жағдайда, Пайдаланушы бұл жайында почтамен немесе факсимильді байланыспен еңсерілмейтін күштің мән-жайлары басталған уақытты және сипаттамасын нақтылайтын жазбаша хабарлама тапсыру және (немесе) жөнелту арқылы Жергілікті атқарушы органды және Инспекцияны дереу хабардар етеді.")
                                .Paragraph("12. Еңсерілмейтін күштің мән-жайлары туындаған кезде, Тараптар орын алып отырған жағдайдан шығудың амалын іздестіру үшін Инспекция өкілдерінің қатысуымен дереу кеңес өткізеді және еңсерілмейтін күштің мән-жайларының зардаптарын барынша азайту үшін заңнамаға қайшы келмейтін барлық құралдарды пайдаланады.")
                                .Paragraph("13. Осы тарауда көрсетілген еңсерілмейтін күштің мән-жайларын құзыретті мемлекеттік органдар мен ұйымдар растаған болса, олар заңды болып танылады.")
                                .BrTag()

                                .Div("5. Қорытынды ережелер", new CssClass("text-center font-weight-bold"))
                                .Paragraph("14. Осы шарт қол қойылған сәттен бастап күшiне енедi және {{ Договор.СрокЗаключенияДоДаты }} дейін {{ Договор.СрокЗаключенияНаЛет }} жыл мерзiмге жасалды.")
                                .Paragraph("15. Осы шарттың қолданылуы:")
                                .Paragraph("1) аңшылық шаруашылығын жүргізуден ерікті түрде бас тартқан;")
                                .Paragraph("2) шарттың қолданылу мерзімі аяқталған;")
                                .Paragraph("3) Пайдаланушының қызметі тоқтатылған;")
                                .Paragraph("4) аңшылық алқаптар және (немесе) учаскелер бекітіп берілген жер учаскелері Қазақстан Республикасының заңнамасында айқындалған тәртіппен мемлекет мұқтаждығы үшін алып қойылған жағдайларда тоқтатылады.")
                                .Paragraph("16. Аңшылық шаруашылығын жүргізу жөніндегі дауларды шешу кезінде тараптар осы шарттың талаптарын, шаруашылық ішіндегі аңшылық ісін ұйымдастыруды және Қазақстан Республикасының заңнамасын басшылыққа алады.")
                                .Paragraph("17. Есепке алу алаңдары мен жануарлардың маршруты көрсетіле отырып жоспарланған аңшылық шаруашылықтардың карта-схемалары бар осы шартқа қосымшаға сәйкес бекітілген нысандағы паспорт осы шарттың ажырамас бөлігі болып табылады.")
                                .Paragraph("Пайдаланушы аңшылық шаруашылығы паспортын статистикалық және бухгалтерлік есепке алу құжаттарының негізінде жыл сайын, есепті жылдан кейінгі бірінші тоқсанда Порталда толтырады.")
                                .Paragraph("18. Осы Шартқа енгізілетін барлық өзгерістер мен толықтырулардың заңды күші бар және егер олар электрондық нысанда жасалса және екі Тараптың уәкілетті өкілдері қол қойса, оның ажырамас бөлігі болып табылады.")
                                .Paragraph("19. Тараптар осы шарттан туындайтын дауларды келіссөздер арқылы шешуге тырысады, ал тараптар келісімге қол жеткізбеген жағдайда, Қазақстан Республикасының заңнамасында белгіленген тәртіппен шешіледі.")
                                .Paragraph("20. Осы шарт заңдық күші бірдей екі данада мемлекеттік және орыс тілдерінде жасалды.")
                                .Paragraph("21. Осы Шарт Порталда электрондық цифрлық қолтаңбаны пайдалана отырып, Тараптардың соңғысы қол қойған сәттен бастап күшіне енеді.")
                                .Paragraph("Бұл ретте, осы шарттың жасалған күні Тараптардың соңғысының ЭЦҚ-мен қол қойылған күні айқындалады")
                                .BrTag()

                                .Div("6. Тараптардың заңды мекенжайлары және деректемелері", new CssClass("text-center font-weight-bold"))
                                .Table(new[] { new DocTableCol("МестныйИсполнительныйОрган", "Жергілікті атқарушы орган", 50, "vertical-align-top text-align-justify p-0-10-px"), new DocTableCol("Пользователь", "Пайдаланушы", 50, "vertical-align-top text-align-justify p-0-10-px") }, new РеквизитыМодель[] { Реквизиты }, tableCssClass: "wide-table")
                            ));
                            }).Build();

                        template += new DocumentBuilder()
                            .Doc(doc => {
                                doc.AddSection(s => s.Body(b => b
                                    .Div("Аңшылық шаруашылығының паспорты", new CssClass("text-center font-weight-bold"))
                                    .Div("1-тарау. Аңшылық шаруашылығының сипаттамасы", new CssClass("text-center font-weight-bold"))
                                    .BrTag()
                                    .Paragraph("1. Аңшылық шаруашылығының атауы: {{ Паспорт.Наименование }}")
                                    .Paragraph("2. Пайдаланушы: {{ Паспорт.Пользователь }}")
                                    .Paragraph("3. Негiздеме - {{ Паспорт.ПаспортОснование.АкиматОбласти }} әкiмдiгiнiң {{ Паспорт.ПаспортОснование.ПостановлениеАкиматаОтДаты }} № {{ Паспорт.ПаспортОснование.ПостановлениеАкиматаНомер }} қаулысы және {{ Паспорт.ПаспортОснование.ДоговорОтДаты }} № {{ Паспорт.ПаспортОснование.ДоговорНомер }} {{ Паспорт.ПаспортОснование.ДоговорМежду }} арасындағы аңшылық шаруашылығын жүргізуге арналған шарт.")
                                    .Paragraph("Бекіту мерзімі {{ Паспорт.ПаспортОснование.СрокЗакрепленияЛет }} жыл, {{ Паспорт.ПаспортОснование.СрокЗакрепленияС }} {{ Паспорт.ПаспортОснование.СрокЗакрепленияПо }} дейін.")
                                    .Paragraph("4. Аңшылық шаруашылығы {{ Паспорт.НаходитсяНаТерриторииОбласти }} {{ Паспорт.НаходитсяНаТерриторииРайона }} аумағында орналасқан.")
                                    .Paragraph("5. Аңшылық шаруашылығының шекаралары:")
                                    .Table(new[] { new DocTableCol("X", "Ендік", 50), new DocTableCol("Y", "Бойлық", 50) }, ОхотничьиУгодья.МежевыеТочки, tableCssClass: "wide-table")
                                    .Paragraph("6. Аңшылық шаруашылығының алаңы: {{ ОхотничьиУгодья.Площади.ОбщаяПлощадь }} гектар, оның iшiнде:")
                                    .Paragraph("мемлекеттiк орман қорының жер учаскелерi {{ ОхотничьиУгодья.Площади.ПлощадьУчастковГосударственногоЛесногоФонда }} гектар,")
                                    .Paragraph("ауыл шаруашылығы мақсатындағы жер учаскелерi {{ ОхотничьиУгодья.Площади.ПлощадьУчастковСельхозНазначения }} гектар,")
                                    .Paragraph("су айдындары {{ ОхотничьиУгодья.Площади.ПлощадьВодоёмов }} гектар;")
                                    .Paragraph("мемлекеттiк жер қорының жер учаскелерi {{ ОхотничьиУгодья.Площади.ПлощадьУчастковГосударственногоЗемельногоЗапаса }} гектар;")
                                    .Paragraph("өзгелері {{ ОхотничьиУгодья.Площади.ПлощадьПрочихУчастков }} гектар .")
                                    .Paragraph("7. Аңшылық шаруашылығындағы қорықшы учаскелердің (айналмалардың) саны: {{ Паспорт.КоличествоЕгерскихУчастков }}")
                                    .BrTag()

                                    .Div("2-тарау. Аңшылық iсiнің көрсеткiштерi", new CssClass("text-center font-weight-bold"))
                                    .Paragraph("8. Шаруашылық iшiндегі аңшылық iсiн {{ Паспорт.ВнутрихозяйственноеОхотоустройствоВыполненоВГоду }} жылы {{ Паспорт.ИсполнительВнутрихозяйственногоОхотоустройства }} орындады.")
                                    .Paragraph("9. {{ Паспорт.КатегорияОхотничьегоХозяйства }} аңшылық шаруашылығының санаты, аңшылық шаруашылығы қызметiнiң негiзгi бағыты: жануарлар дүниесі түрлерінің әр алуандылығын, олардың мекендеу ортасын, жануарларды, оның ішінде аң аулау объектісіне жатпайтындарды, тұрақты пайдалануды, өсімін молайтуды және қорғауды сақтау, әуесқой (спорттық) аң аулауды ұйымдастыру.")
                                    .Paragraph("10. Аң аулау объектілері болып табылатын жануарлардың негізгі түрлері бойынша бонитеттік бағалау шаруашылық ішіндегі аңшылық ісін ұйымдастыру материалдарында келтіріледі.")

                                    .Paragraph("11. Аңшылық алқаптардың жануарлар түрлерi бойынша өткiзу қабiлетi:")
                                    .Html(string.Join("", Паспорт.ПропускнаяСпособность.Данные.Select(row => $"<p class=\"app-section-item\">{SearchInKeyValue(Паспорт.ПропускнаяСпособность.Столбцы["flWildfowlType"].reference, row["flWildfowlType"].ToString())}: {row["flCount"]};</p>")))
                                    .Paragraph("12. Шаруашылық iшiндегі аңшылық iсiн ұйымдастыру көрсеткiштерi:")
                                    .Html(GetListEditorsYXTable(
                                        Паспорт.ВнутрихозяйственноеОхотоустройство,
                                        "Шаруашылық iшiндегі аңшылық iсiнің көрсеткiштерi",
                                        "flYear",
                                        new[] {
                                        "flReproductionObjectsCount",
                                        "flReproductionObjectsArea",
                                        "flRestObjectsCount",
                                        "flRestObjectsArea",
                                        "flSoldOutsCount",
                                        "flTowersCount",
                                        "flHutsCount",
                                        "flAnother"
                                        }
                                    ))
                                    .Paragraph("13. Аңшылық шаруашылығының штаты:")
                                    .Html(GetListEditorsXYTable(
                                        Паспорт.Штат,
                                        "Штаттық лауазымдардың атауы",
                                        new[] {
                                        "flWorkerTypeCount",
                                        "flWorkerTypeSalary",
                                        "flNote"
                                        },
                                        "flWorkerType"
                                    ))
                                    .BrTag()

                                    .Div("3-тарау. Өсiмді молайту көрсеткіштері", new CssClass("text-center font-weight-bold"))
                                    .Paragraph("14. Аңшылық шаруашылығы үшiн зиянды жануарлардың санын шектеу:")
                                    .Html(GetListEditorsXYZTable(
                                        Паспорт.ОграничениеВредныхЖивотных,
                                        "Ауланғаны (бас)",
                                        "flYear",
                                        "flHarmfulAnimalType",
                                        "flCount"
                                    ))
                                    .Paragraph("15. Жануарларды аңшылық шаруашылығына шығару:")
                                    .Html(GetListEditorsXYZTable(
                                        Паспорт.ВыпускЖивотных,
                                        "Жануар түрi",
                                        "flYear",
                                        "flAnimalType",
                                        "flCount"
                                    ))
                                    .Paragraph("16. Аңшылық шаруашылығында биотехникалық iс-шаралар жүргізу (бiрлiктер саны):")
                                    .Html(GetListEditorsXYZTable(
                                        Паспорт.БиотехническиеМероприятия,
                                        "Жасалды",
                                        "flYear",
                                        "flActivities",
                                        "flCount"
                                    ))
                                    .Paragraph("17. Аңшылық шаруашылығының аумағында есепке алынған аңшылық жануарлары (дарақ):")
                                    .Html(GetListEditorsXYZTable(
                                        Паспорт.ОхотничьиЖивотные,
                                        "Жануарлар түрлері",
                                        "flYear",
                                        "flAnimalType",
                                        "flCount"
                                    ))
                                    .Paragraph("18. Жабайы жануарлар үшiн дайындалған және салынған азық және үстеме азық:")
                                    .Html(GetListEditorsXYZTable(
                                        Паспорт.Кормы,
                                        "Азық түрлерi",
                                        "flYear",
                                        "flWildAnimalsFeedTypes",
                                        new[] { "flHarvestedWeight", "flLaidOutWeight", "flUnit" },
                                        "flHarvestedWeight/flLaidOutWeight flUnit."
                                    ))
                                    .Paragraph("19. Аңшылық шаруашылығы қызметiне жұмсалған еңбек үлесi:")
                                    .Html(GetListEditorsXYZTable(
                                        Паспорт.Труд,
                                        "Жұмыстар атауы",
                                        "flYear",
                                        "flWorkType",
                                        new[] { "flValue", "flUnit" },
                                        "flValue flUnit."
                                    ))
                                    .BrTag()

                                    .Div("4-тарау. Экономикалық көрсеткіштер", new CssClass("text-center font-weight-bold"))
                                    .Paragraph("20. Браконьерлiкпен күрес:")
                                    .Html(GetListEditorsXYZTable(
                                        Паспорт.БорьбаСБраконьерством,
                                        "Анықталған қағидаларды бұзу фактілері",
                                        "flYear",
                                        "flRuleType",
                                        "flValue"
                                    ))
                                    .Paragraph("21. Аңшылық шаруашылығында аң аулайтын иттердiң, елiктiрушi үйректердiң болуы:")
                                    .Html(GetListEditorsXYZTable(
                                        Паспорт.СобакиПодсадныеУтки,
                                        "Түрлерi",
                                        "flYear",
                                        "flHunterAnimalType",
                                        "flCount"
                                    ))
                                    .Paragraph("22. Аңшылық шаруашылығындағы құрылыстардың болуы:")
                                    .Html(GetListEditorsXYTable(
                                        Паспорт.Строения,
                                        "Құрылыс түрі",
                                        new[] { "flBuildYear", "flCount", "flArea", "flBerthsCount", "flNote" },
                                        "flBuildingType"
                                    ))
                                    .Paragraph("23. Аңшылық шаруашылығындағы көлiктердің болуы:")
                                    .Html(GetListEditorsXYZTable(
                                        Паспорт.Транспорт,
                                        "Көлiк түрлерi",
                                        "flYear",
                                        "flTransportType",
                                        "flCount"
                                    ))

                                    .Paragraph("Аңшылық шаруашылығының паспортын Пайдаланушы жыл сайын есепті жылдан кейінгі жылдың бірінші тоқсанында статистикалық және бухгалтерлiк есепке алу құжаттарының негiзiнде мемлекеттік мүлік тізілімінің веб-порталында толтырады. Паспортқа аңшылық шаруашылығының шекаралары мен межелі нүктелері көрсетілген карта-схема және жануарлардың есепке алу алаңдары мен есепке алу маршруттары көрсетілген карта-схема қоса беріледі.")
                                ));
                            }).Build();

                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"Неописанный язык: {Language}");
                    }
            }

            var html = new ScribanRenderBuilder()
                .SetTemplate(template)
                .UseDefaultContext(this)
                .Render();

            return html;
        }
        public override void FillModel(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            var obj = ObjectHelper.GetObjectModel(env.Args.ObjectId, env.QueryExecuter);
            var trd = TradeHelper.GetTradeModel(env.Args.TradeId, env.QueryExecuter);

            var seller = env.User.GetAccountData(env.QueryExecuter);
            var winner = trd.flWinnerData;

            var refAr = RefAr.Instance;

            ГородПосёлокСело = "_____";
            Договор = new ДоговорМодель() 
            {
                Акимат = new АкиматМодель()
                {
                    АкиматОбласти = refAr.Search(obj.flRegion).Text,
                    МестныйИсполнительныйОрган = seller.Account.NameRu,
                    ПостановлениеАкиматаОтДаты = $"«__» ____ 20__ года",
                    ПостановлениеАкиматаНомер = "__"
                },
                Покупатель = new ПокупательМодель()
                {
                    ВЛице = $"(должность) {winner.FullName}",
                    Название = winner.CorpName,
                    НаОсновании = "Устава"
                },
                СрокЗаключенияНаЛет = 0,
                СрокЗаключенияДоДаты = $"«__» ____ 20__ года"
            };
            ОхотничьиУгодья = new ОхотничьиУгодьяМодель()
            {
                ВОбласти = refAr.Search(obj.flRegion).Text,
                ВРайоне = refAr.Search(obj.flDistrict).Text,
                МежевыеТочки = obj.flCoords.Select(x => new МежеваяТочкаМодель() { X = x.appropriateX, Y = x.appropriateY, }).ToArray(),
                Площади = new ОхотничьиУгодьяПлощадиМодель()
                {
                    ОбщаяПлощадь = obj.flHuntArea,
                    ПлощадьВодоёмов = obj.flWaterArea,
                    ПлощадьПрочихУчастков = obj.flOtherArea,
                    ПлощадьУчастковГосударственногоЗемельногоЗапаса = obj.flLandReserveArea,
                    ПлощадьУчастковГосударственногоЛесногоФонда = obj.flForestArea,
                    ПлощадьУчастковСельхозНазначения = obj.flAgriArea
                }
            };
            ПраваИОбязанности = new ПраваИОбязанностиМодель
            {
                ОбязанностиПользователя = new ОбязанностиПользователяМодель
                {
                    ПровестиИОбеспечитьВыполнениеВнутрихозяйственногоОхотоустройстваДоДаты = $"«__» ____ 20__ года",
                    СоздатьЕгерскуюСлужбуВКоличествеЧеловек = 0
                }
            };
            Реквизиты = new РеквизитыМодель
            {
                МестныйИсполнительныйОрган = seller.BankAccount != null ? $"{seller.Account.NameRu}, БИН {seller.CorpData.Bin}, {new BankSearchCollection().GetItem(seller.BankAccount.Bic, env.RequestContext).Name}, БИК {seller.BankAccount.Bic}, ИИК {seller.BankAccount.Iban}, {string.Join(", ", seller.AccountAddresses.Select(x => YodaUserHelpers.ToAddressString(x)))}" : $"Не найдены банковские реквизиты для {seller.Account.NameRu}. Укажите их в профиле, чтобы избежать подобных ошибок",
                Пользователь = $"{winner.FullOrgXinName}, {winner.ParticipiantBankDetails.BankName}, БИК {winner.ParticipiantBankDetails.BIK}, ИИК {winner.ParticipiantBankDetails.IIK}, {winner.AddressInfo}"
            };
            Паспорт = new ПаспортМодель
            {
                ПаспортОснование = new ПаспортОснованиеМодель
                {
                    АкиматОбласти = refAr.Search(obj.flRegion).Text,
                    ДоговорМежду = "_______ и _______",
                    ДоговорНомер = "__",
                    ДоговорОтДаты = "«__» ____ 20__ года",
                    ПостановлениеАкиматаНомер = "__",
                    ПостановлениеАкиматаОтДаты = "«__» ____ 20__ года",
                    СрокЗакрепленияЛет = 0,
                    СрокЗакрепленияС = "«__» ____ 20__ года",
                    СрокЗакрепленияПо = "«__» ____ 20__ года"
                },
                ПропускнаяСпособность = new ТаблицаМодель()
                {
                    Данные = obj.flWildfowlThroughput,
                    Столбцы = new WildfowlThroughputEditor(null, null).Fields
                },
                ВнутрихозяйственноеОхотоустройство = new ТаблицаМодель()
                {
                    Данные = obj.flInsideOrganization,
                    Столбцы = new InsideOrganizationEditor(null, null).Fields
                },
                Штат = new ТаблицаМодель()
                {
                    Данные = obj.flHuntingState,
                    Столбцы = new HuntingStateEditor(null, null).Fields
                },
                ОграничениеВредныхЖивотных = new ТаблицаМодель()
                {
                    Данные = obj.flHarmfulAnimals,
                    Столбцы = new HarmfulAnimalsEditor(null, null).Fields
                },
                ВыпускЖивотных = new ТаблицаМодель()
                {
                    Данные = obj.flAnimalsAdmission,
                    Столбцы = new AnimalsAdmissionEditor(null, null).Fields
                },
                БиотехническиеМероприятия = new ТаблицаМодель()
                {
                    Данные = obj.flBiotechnicalActivities,
                    Столбцы = new BiotechnicalActivitiesEditor(null, null).Fields
                },
                ОхотничьиЖивотные = new ТаблицаМодель()
                {
                    Данные = obj.flConsideredAnimals,
                    Столбцы = new AnimalsAdmissionEditor(null, null).Fields
                },
                Кормы = new ТаблицаМодель()
                {
                    Данные = obj.flWildAnimalsFeed,
                    Столбцы = new WildAnimalsFeedEditor(null, null).Fields
                },
                Труд = new ТаблицаМодель()
                {
                    Данные = obj.flInvestedWork,
                    Столбцы = new InvestedWorkEditor(null, null).Fields
                },
                БорьбаСБраконьерством = new ТаблицаМодель()
                {
                    Данные = obj.flAntipoaching,
                    Столбцы = new AntipoachingEditor(null, null).Fields
                },
                СобакиПодсадныеУтки = new ТаблицаМодель()
                {
                    Данные = obj.flHuntingDogsAndDecoyDucks,
                    Столбцы = new HuntingDogsAndDecoyDucksEditor(null, null).Fields
                },
                Строения = new ТаблицаМодель()
                {
                    Данные = obj.flBuildings,
                    Столбцы = new BuildingsEditor(null, null).Fields
                },
                Транспорт = new ТаблицаМодель()
                {
                    Данные = obj.flTransport,
                    Столбцы = new TransportEditor(null, null).Fields
                },
                ВнутрихозяйственноеОхотоустройствоВыполненоВГоду = obj.flYear,
                ИсполнительВнутрихозяйственногоОхотоустройства = "____________",
                КатегорияОхотничьегоХозяйства = obj.flCategory,
                КоличествоЕгерскихУчастков = obj.flRangerSites,
                Наименование = obj.flName,
                НаходитсяНаТерриторииОбласти = refAr.Search(obj.flRegion).Text,
                НаходитсяНаТерриторииРайона = refAr.Search(obj.flDistrict).Text,
                Пользователь = seller.Account.NameRu
            };
        }
        private Dictionary<string, RowFieldParams> TranslateRefFields(Dictionary<string, RowFieldParams> fieldsDict, IYodaRequestContext requestContext)
        {
            foreach (var field in fieldsDict)
            {
                fieldsDict[field.Key].text = Translate(requestContext, Language, fieldsDict[field.Key].text);
                if (field.Value.type == "reference")
                {
                    fieldsDict[field.Key].reference = field.Value.reference.Select(x => TranslateKeyValue(x, requestContext)).ToArray();
                }
            }
            return fieldsDict;
        }
        private KeyValueItem TranslateKeyValue(KeyValueItem item, IYodaRequestContext requestContext)
        {
            item.Text = Translate(requestContext, Language, item.Text);
            if (item.Items != null && item.Items.Length > 0)
                item.Items = item.Items.Select(x => TranslateKeyValue(x, requestContext)).ToArray();
            return item;
        }
        private string SearchInKeyValue(KeyValueItem[] reference, string value)
        {
            string result = string.Empty;
            foreach(var item in reference)
            {
                if (item.Value != value && item.Items != null && item.Items.Length > 0)
                {
                    result = SearchInKeyValue(item.Items, value);
                } else if (item.Value == value)
                {
                    result = item.Text;
                }
                if (!string.IsNullOrEmpty(result))
                    break;
            }
            return result;
        }
        private string[] GetKeyValueLastChilds(KeyValueItem reference)
        {
            var result = new string[] { };

            if (reference.Items != null && reference.Items.Length > 0)
            {
                foreach (var item in reference.Items)
                {
                    result = new[] { result, GetKeyValueLastChilds(item) }.SelectMany(x => x).ToArray();
                }
            } else
            {
                result = new string[] { reference.Text };
            }
            return result;
        }
        public override void TranslateModel(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            switch(Language)
            {
                case "kz": {
                        Договор.Акимат.ПостановлениеАкиматаОтДаты = "20__ жылғы «__» ____";
                        Договор.Акимат.АкиматОбласти = Translate(env.RequestContext, Language, Договор.Акимат.АкиматОбласти);
                        Договор.СрокЗаключенияДоДаты = "20__ жылғы «__» ____";
                        ПраваИОбязанности.ОбязанностиПользователя.ПровестиИОбеспечитьВыполнениеВнутрихозяйственногоОхотоустройстваДоДаты = "20__ жылғы «__» ____";

                        ОхотничьиУгодья.ВОбласти = Translate(env.RequestContext, Language, ОхотничьиУгодья.ВОбласти);
                        ОхотничьиУгодья.ВРайоне = Translate(env.RequestContext, Language, ОхотничьиУгодья.ВРайоне);

                        Паспорт.ПаспортОснование.ДоговорОтДаты = "20__ жылғы «__» ____";
                        Паспорт.ПаспортОснование.ПостановлениеАкиматаОтДаты = "20__ жылғы «__» ____";
                        Паспорт.ПаспортОснование.СрокЗакрепленияС = "20__ жылғы «__» ____";
                        Паспорт.ПаспортОснование.СрокЗакрепленияПо = "20__ жылғы «__» ____";
                        Паспорт.ПаспортОснование.АкиматОбласти = Translate(env.RequestContext, Language, Паспорт.ПаспортОснование.АкиматОбласти);
                        Паспорт.НаходитсяНаТерриторииОбласти = Translate(env.RequestContext, Language, Паспорт.НаходитсяНаТерриторииОбласти);

                        Паспорт.ПропускнаяСпособность.Столбцы = TranslateRefFields(Паспорт.ПропускнаяСпособность.Столбцы, env.RequestContext);
                        Паспорт.ВнутрихозяйственноеОхотоустройство.Столбцы = TranslateRefFields(Паспорт.ВнутрихозяйственноеОхотоустройство.Столбцы, env.RequestContext);
                        Паспорт.Штат.Столбцы = TranslateRefFields(Паспорт.Штат.Столбцы, env.RequestContext);
                        Паспорт.ОграничениеВредныхЖивотных.Столбцы = TranslateRefFields(Паспорт.ОграничениеВредныхЖивотных.Столбцы, env.RequestContext);
                        Паспорт.ВыпускЖивотных.Столбцы = TranslateRefFields(Паспорт.ВыпускЖивотных.Столбцы, env.RequestContext);
                        Паспорт.БиотехническиеМероприятия.Столбцы = TranslateRefFields(Паспорт.БиотехническиеМероприятия.Столбцы, env.RequestContext);
                        Паспорт.ОхотничьиЖивотные.Столбцы = TranslateRefFields(Паспорт.ОхотничьиЖивотные.Столбцы, env.RequestContext);
                        Паспорт.Кормы.Столбцы = TranslateRefFields(Паспорт.Кормы.Столбцы, env.RequestContext);
                        Паспорт.Труд.Столбцы = TranslateRefFields(Паспорт.Труд.Столбцы, env.RequestContext);
                        Паспорт.БорьбаСБраконьерством.Столбцы = TranslateRefFields(Паспорт.БорьбаСБраконьерством.Столбцы, env.RequestContext);
                        Паспорт.СобакиПодсадныеУтки.Столбцы = TranslateRefFields(Паспорт.СобакиПодсадныеУтки.Столбцы, env.RequestContext);
                        Паспорт.Строения.Столбцы = TranslateRefFields(Паспорт.Строения.Столбцы, env.RequestContext);
                        Паспорт.Транспорт.Столбцы = TranslateRefFields(Паспорт.Транспорт.Столбцы, env.RequestContext);
                        break;
                }
                default:
                    {
                        throw new NotImplementedException($"Unknown language: {Language}");
                    }
            }
        }
        public override bool HasAccessToCreate(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            var objectId = env.Args.ObjectId;
            if (objectId == 0)
            {
                objectId = new TbAgreements().AddFilter(t => t.flAgreementId, env.Args.AgreementId).SelectScalar(t => t.flObjectId, env.QueryExecuter).Value;
            }
            var objectSeller = new TbObjects().AddFilter(t => t.flId, objectId).SelectScalar(t => t.flSallerBin, env.QueryExecuter);
            var hasPair = new TbSellerSigners().GetPair(objectSeller, env.QueryExecuter, out var data);
            var agreementSigner = hasPair ? data.flSignerBin : string.Empty;
            var currentUser = env.User.GetUserXin(env.QueryExecuter);

            var isObjectSeller = currentUser == objectSeller && env.User.HasRole("TRADERESOURCES-Охотничьи угодья-Создание приказов", env.QueryExecuter)/*env.User.HasCustomRole("huntingobjects", "dataEdit", env.QueryExecuter)*/;
            var isAgreementSigner = currentUser == agreementSigner;
            return isAgreementSigner;
        }
        public override bool HasAccessToSign(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            var objectId = env.Args.ObjectId;
            if (objectId == 0)
            {
                objectId = new TbAgreements().AddFilter(t => t.flAgreementId, env.Args.AgreementId).SelectScalar(t => t.flObjectId, env.QueryExecuter).Value;
            }
            var objectSeller = new TbObjects().AddFilter(t => t.flId, objectId).SelectScalar(t => t.flSallerBin, env.QueryExecuter);
            var hasPair = new TbSellerSigners().GetPair(objectSeller, env.QueryExecuter, out var data);
            var agreementSigner = hasPair ? data.flSignerBin : string.Empty;
            var currentUser = env.User.GetUserXin(env.QueryExecuter);

            var isObjectSeller = currentUser == objectSeller && env.User.HasRole("TRADERESOURCES-Охотничьи угодья-Создание приказов", env.QueryExecuter)/*env.User.HasCustomRole("huntingobjects", "dataEdit", env.QueryExecuter)*/;
            var isAgreementSigner = currentUser == agreementSigner;
            return isAgreementSigner;
        }
        public override void OnSignEnd(ActionEnv<DefaultAgrTemplateArgs> env, ITransaction transaction)
        {
            ObjectHelper.SetBlock(AgreementHelper.GetAgreementObjectId(env.Args.AgreementId, env.QueryExecuter), HuntingObjectBlocks.SaledAgr, env.QueryExecuter, transaction);
        }

        public override PaymentAndOverpaymentRequisitesModel GetPaymentAndOverpaymentRequisites(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            if (env.Args.TradeId == 0)
            {
                env.Args.TradeId = AgreementHelper.GetAgreementTradeId(env.Args.AgreementId, env.QueryExecuter);
            }
            var trd = TradeHelper.GetTradeModel(env.Args.TradeId, env.QueryExecuter);
            return new PaymentAndOverpaymentRequisitesModel()
            {
                flPayment = new RequisitesModel()
                {
                    flName = new GrObjectSearchCollection().GetItem(trd.flTaxAuthorityBin, env.RequestContext).ObjectData.NameRu,
                    flXin = trd.flTaxAuthorityBin,
                    flBik = trd.flBik,
                    flIban = trd.flIik,
                    flKbe = trd.flKbe,
                    flKnp = trd.flKnp,
                    flKbk = trd.flKbk
                },
                flOverPayment = new RequisitesModel()
                {
                    flName = $"{trd.flWinnerData.CorpName} {trd.flWinnerData.LastName} {trd.flWinnerData.FirstName} {trd.flWinnerData.MiddleName}",
                    flXin = trd.flWinnerData.Xin,
                    flBik = trd.flWinnerData.ParticipiantBankDetails.BIK,
                    flIban = trd.flWinnerData.ParticipiantBankDetails.IIK,
                    flKbe = int.TryParse(trd.flWinnerData.ParticipiantBankDetails.KBE, out var flKbe) ? (int?)flKbe : null,
                    flKnp = null,
                    flKbk = null,
                    flContacts = trd.flWinnerData.ContactInfo
                }
            };
        }

        public override SidesAccountsData GetSidesAccountData(ActionEnv<DefaultAgrTemplateArgs> env) {
            if (env.Args.TradeId == 0) {
                env.Args.TradeId = AgreementHelper.GetAgreementTradeId(env.Args.AgreementId, env.QueryExecuter);
            }
            var trd = TradeHelper.GetTradeModel(env.Args.TradeId, env.QueryExecuter);
            return new SidesAccountsData() {
                flWinner = new SideAccountData() {
                    flXin = trd.flWinnerData.Xin,
                    flName = trd.flWinnerData.FullOrgXinName,
                    flAccountType = trd.flWinnerData.UserType
                },
                flSeller = new SideAccountData() {
                    flXin = trd.flCompetentOrgBin,
                    flName = new GrObjectSearchCollection().GetItem(trd.flCompetentOrgBin, env.RequestContext).ObjectData.NameRu,
                    flAccountType = "Corporate"
                }
            };
        }

        public override PaymentItemModel[] GetGuaranteePayments(ActionEnv<DefaultAgrTemplateArgs> env) {
            if (env.Args.TradeId == 0) {
                env.Args.TradeId = AgreementHelper.GetAgreementTradeId(env.Args.AgreementId, env.QueryExecuter);
            }
            var trd = TradeHelper.GetTradeModel(env.Args.TradeId, env.QueryExecuter);
            return JsonConvert.DeserializeObject<PaymentItemModel[]>(JsonConvert.SerializeObject(trd.flWinnerData.GuaranteePayments)).ToArray();
        }

        public override DateTime SignAvailableDate(ActionEnv<DefaultAgrTemplateArgs> env) {
            var currentDateTime = env.QueryExecuter.GetDateTime("dbAgreements");
            return currentDateTime;
        }

        public override bool IsSignAvailableDate(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            return true;
        }

        public override (string module, string action, object routeValues, string project) GetLinkToObject(ActionEnv<DefaultAgrTemplateArgs> env) {
            var objectId = env.Args.ObjectId;
            if (objectId == 0) {
                objectId = new TbAgreements().AddFilter(t => t.flAgreementId, env.Args.AgreementId).SelectScalar(t => t.flObjectId, env.QueryExecuter).Value;
            }
            return (nameof(RegistersModule), MnuHuntingObjectView.MnuName, new HuntingObjectViewArgs() { Id = objectId, MenuAction = MnuHuntingObjectView.Actions.View }, null);
        }
        public override (string module, string action, object routeValues, string project) GetLinkToTrade(ActionEnv<DefaultAgrTemplateArgs> env) {
            var tradeId = env.Args.TradeId;
            if (tradeId == 0) {
                tradeId = new TbAgreements().AddFilter(t => t.flAgreementId, env.Args.AgreementId).SelectScalar(t => t.flTradeId, env.QueryExecuter).Value;
            }
            return (nameof(RegistersModule), nameof(MnuHuntingTradeView), new MnuHuntingTradeViewArgs() { tradeId = tradeId }, null);
        }
        public override string GetLinkToEtp(ActionEnv<DefaultAgrTemplateArgs> env) {
            var auctionId = new TbAgreements().AddFilter(t => t.flAgreementId, env.Args.AgreementId).SelectScalar(t => t.flAuctionId, env.QueryExecuter).Value;
            return $"https://e-auction.gosreestr.kz/p/ru/auctions/{auctionId}/view";
        }
    }
}
