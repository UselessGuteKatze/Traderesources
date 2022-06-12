using HydrocarbonSource.QueryTables.Object;
using HydrocarbonSource.QueryTables.Trade;
using HydrocarbonSource.References.Trade;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Forms.Components.Tabs;
using Yoda.Interfaces.Menu;
using YodaApp.Yoda.Interfaces.Forms.Components;
using YodaApp.Yoda.Interfaces.Forms.Components.Charts;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.Menus {
    public class MnuHydrocarbonStatistics : FrmMenu {

        private static MemoryCache _memCache = new MemoryCache(new MemoryCacheOptions());

        public MnuHydrocarbonStatistics(string moduleName) : base(nameof(MnuHydrocarbonStatistics), "Статистика")
        {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Access();
            Enabled((rc) => {
                return true;
            });
            OnRendering(re => {

                var firstDayHalf = DateTime.Now.Hour - (DateTime.Now.Hour % 12) == 0;
                new Panel("mb-2 text-muted").Append(new HtmlText($"Данные на момент {DateTime.Now:dd.MM.yyyy} ~{(firstDayHalf ? "00:00" : "12:00")} (обновление статистики происходит 2 раза в сутки)")).AppendTo(re.Form);

                var rows = _memCache.GetOrCreate(DateTime.Now.ToString("dd.MM.yyyy.") + (firstDayHalf ? "1" : "2"), (entry) => {
                    return new TbTrades().Self(out var tbLandObjectsTrades)
                    .JoinT(nameof(TbTrades), new TbObjects().Self(out var tbLandObjects), nameof(TbObjects), JoinType.Left)
                    .On((t1, t2) => new Join(t1.flObjectId, t2.flId))
                    .Select(t => new FieldAlias[] { t.L.flStatus, t.L.flDateTime, t.L.flCost }, re.QueryExecuter);
                });

                tradeCountGraphs(rows).AppendTo(re.Form);

                var row = new GridRow().AppendTo(re.Form);
                var col1 = new GridCol("col-lg-5").AppendTo(row);
                var col2 = new GridCol("col-lg-7").AppendTo(row);

                tradeSellCost(rows).AppendTo(col1);
                tradeWaitings(rows).AppendTo(col1);
                tradeSuccessPie(rows).AppendTo(col2);

            });
        }

        private Card tradeCountGraphs(SelectResultProxy<QueryJoin<TbTrades, TbObjects>> rows)
        {
            var dateGroupedValues = rows
                    .Where(r => new[] { HydrocarbonTradeStatuses.Held, HydrocarbonTradeStatuses.CancelledAfter }.Contains(r.GetVal(t => t.L.flStatus)))
                    .GroupBy(r => r.GetVal(t => t.L.flDateTime).Date)
                    .Select(r => {
                        return new
                        {
                            Date = r.Key,
                            Count = r.Count()
                        };
                    })
                    .OrderByDescending(r => r.Date);

            var tabPanel = new HyperTabs();
            void addChart(string title, DateTime fromDate)
            {
                var searchingRows = dateGroupedValues.TakeWhile(x => x.Date > fromDate).OrderBy(r => r.Date);

                var seriesHour = new[] {
                            new Apex.Chart.SeriesXIntYDateTime() {
                                name = "Количество",
                                data = searchingRows.OrderBy(x=>x.Date).Select(r => new Apex.Chart.SeriesXIntYDateTimeDataUnit {
                                    x = r.Date,
                                    y = r.Count
                                }).ToArray()
                            },
                        };
                var chartHour = new Apex.Chart(seriesHour)
                    .SetResponsive(new[] {
                        new Apex.Chart.Responsive() {
                            breakpoint = 800,
                            options = new Apex.Chart.ChartModel() {
                                chart = new Apex.Chart.ChartData()
                                {
                                    height = 800
                                },
                                plotOptions = new Apex.Chart.PlotOptions() {
                                    bar = new Apex.Chart.Bar() {
                                        horizontal = true
                                    }
                                },
                                dataLabels = new Apex.Chart.DataLabels()
                                {
                                    offsetY = 20,
                                    offsetX = 20
                                },
                                tooltip = new Apex.Chart.ToolTip()
                                {
                                    x = new Apex.Chart.ToolTipX()
                                    {
                                        format = "d MMM"
                                    }
                                }
                            }
                        }
                    })
                    .SetToolTipXFormat("d MMM")
                    .SetChartType(Apex.Chart.ChartTypes.bar)
                    .SetColors(new[] { "#727cf5" })
                    .SetLabelsColors(new[] { "#727cf5" })
                    .HideDataLabelsOnSeries()
                    .HideDataLabels();
                if (searchingRows.Count() >= 2)
                {
                    var lastDate = searchingRows.Last().Date;
                    var firstDate = searchingRows.First().Date;
                    if ((lastDate - firstDate).TotalDays <= 100)
                    {
                        chartHour.ShowDataLabels();
                    }
                }
                tabPanel.AddNewTab(title, chartHour);
            }

            addChart("За последние 6 месяцев", DateTime.Now.AddMonths(-6));
            addChart("За последний год", DateTime.Now.AddYears(-1));
            addChart("За все время", DateTime.Now.AddYears(-20));

            return new Card("Состоявшиеся торги").Append(tabPanel);
        }
        private Card tradeSuccessPie(SelectResultProxy<QueryJoin<TbTrades, TbObjects>> rows)
        {
            var statusGroupedValues = rows
                    .GroupBy(r => r.GetVal(t => t.L.flStatus))
                    .Select(r =>
                    {
                        return new
                        {
                            Status = r.Key,
                            Count = r.Count()
                        };
                    });

            var refSt = new RefTradeStatuses();
            var seriesHour = statusGroupedValues.Select(r =>
                new Apex.PieDonut.SeriesInt()
                {
                    x = refSt.Search(r.Status.ToString()).Text.Text,
                    y = r.Count
                }
            ).ToArray();
            var pie = new Apex.PieDonut(seriesHour)
                .SetHeight(300)
                .SetResponsive(new[] {
                    new Apex.PieDonut.Responsive() {
                        breakpoint = 800,
                        options = new Apex.PieDonut.ChartModel() {
                            legend = new Apex.PieDonut.Legend() {
                                position = Apex.PieDonut.LegendPositions.bottom
                            }
                        }
                    }
                })
                .SetColors(new[] {
                    "#39afd1",
                    "#02a8b5",
                    "#0acf97",
                    "#ffbc00",
                    "#fd7e14",
                    "#fa5c7c",
                    "#ff679b",
                    "#6b5eae",
                    "#2c8ef8",
                    "#727cf5"
                })
                .SetLabelsColors(new[] {
                    "#39afd1",
                    "#02a8b5",
                    "#0acf97",
                    "#ffbc00",
                    "#fd7e14",
                    "#fa5c7c",
                    "#ff679b",
                    "#6b5eae",
                    "#2c8ef8",
                    "#727cf5"
                })
                ;


            return new Card().Append(pie);
        }
        private Card tradeWaitings(SelectResultProxy<QueryJoin<TbTrades, TbObjects>> rows)
        {
            var statusGroupedValues = rows
                    .Where(r => r.GetVal(t => t.L.flStatus) == HydrocarbonTradeStatuses.Wait).Count();

            var card = new Card(bodyCssClass: "text-center");

            card.AddComponent(new Label(null, "mdi mdi-clock-outline text-muted font-24"));
            card.AddComponent(new Heading(HeadingLevel.h3, statusGroupedValues.ToString()));
            card.AddComponent(new Label("Торги ожидаются", "text-muted font-15 mb-0"));

            return card;
        }
        private Card tradeSellCost(SelectResultProxy<QueryJoin<TbTrades, TbObjects>> rows)
        {
            var statusGroupedValues = rows.Where(r => r.GetVal(t => t.L.flStatus) == HydrocarbonTradeStatuses.Held && r.GetValOrNull(t => t.L.flCost).HasValue).Sum(r => r.GetVal(t => t.L.flCost));

            var card = new Card(bodyCssClass: "text-center");

            card.AddComponent(new Label(null, "uil uil-coins text-muted font-24"));
            card.AddComponent(new Heading(HeadingLevel.h3, $"{statusGroupedValues:N} тг."));
            card.AddComponent(new Label("Реализовано на торгах", "text-muted font-15 mb-0"));

            return card;
        }

    }
}
