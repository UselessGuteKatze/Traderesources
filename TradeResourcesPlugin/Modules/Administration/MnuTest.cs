using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Menu;
using Yoda.YodaReferences;
using YodaApp.Yoda.Application.Helpers;
using YodaCommonReferences;

namespace TradeResourcesPlugin.Modules.Administration {
    public class MnuTest : FrmMenu {

        public const string MenuName = nameof(MnuTest);
        public MnuTest() : base(MenuName, "test") {
            ProjectsConfig(ProjectsList.All);
            Path("test");
            Enabled(c => c.User.GetUserIin(c.QueryExecuter) == "990922350945");
            OnRendering(re => {

                //var pms = new TbPaymentMatches()
                //    .AddFilter(t => t.flId, ConditionOperator.In, new[] {
                //        114,
                //        780,
                //        84,
                //        19,
                //        309,
                //        783,
                //        510,
                //        785,
                //        792,
                //        46,
                //        218,
                //        485,
                //        483,
                //        513,
                //        512,
                //        388,
                //        379,
                //        380,
                //        496,
                //        336,
                //        436,
                //        548,
                //        463,
                //        585,
                //        560,
                //        562,
                //        563
                //    })
                //    .GetPaymentMatches(re.QueryExecuter);
                //;

                //pms.Each(pm => {

                //    var payAmount = new TbPayments()
                //        .AddFilter(t => t.flPaymentId, pm.flPaymentId)
                //        .SelectScalar(t => t.flPayAmount, re.QueryExecuter).Value;

                //    pm.flAmount = pm.flPaymentItems.Sum(x => x.flAmount);
                //    pm.flGuaranteeAmount = pm.flPaymentItems.Where(x => x.flIsGuarantee).Sum(x => x.flAmount);

                //    var tbpms = new TbPaymentMatches()
                //        .AddFilter(t => t.flId, pm.flId)
                //        .Update()
                //        .Set(t => t.flAmount, pm.flAmount)
                //        .Set(t => t.flGuaranteeAmount, pm.flGuaranteeAmount);
                //    tbpms.Set(t => t.flOverpayment, pm.flAmount > payAmount);
                //    tbpms.Set(t => t.flOverpaymentAmount, pm.flAmount > payAmount ? pm.flAmount - payAmount : 0);
                //    //tbpms.Set(t => t.flSendOverpayment, pm.flAmount > payAmount && pm.flRealAmount > 0);
                //    //tbpms.Set(t => t.flOverpaymentSendAmount, pm.flAmount > payAmount && pm.flRealAmount > 0 ? pm.flGuaranteeAmount > payAmount ? pm.flRealAmount : pm.flRealAmount - (payAmount - pm.flGuaranteeAmount) : 0);
                //    tbpms.Execute(re.QueryExecuter);

                //    var paidAmount = pm.flAmount - (pm.flSendOverpayment ? pm.flOverpaymentSendAmount : 0);

                //    new TbPayments()
                //        .AddFilter(t => t.flPaymentId, pm.flPaymentId)
                //        .Update()
                //        .Set(t => t.flPaidAmount, paidAmount)
                //        .SetT(t => t.flPaymentStatus, paidAmount < payAmount ? PaymentStatus.Paying : PaymentStatus.Paid)
                //        .Execute(re.QueryExecuter);
                //});

                //await new FishingSource.QueryTables.Object.TbObjects()
                //.Map(map => map
                //    .Presentation(
                //        t => new FieldAlias[] {
                //            t.flId,
                //            t.flName,
                //            t.flRegion,
                //            t.flDistrict,
                //            t.flStatus,
                //            t.flGeometry
                //        },
                //        t => new[] {
                //            t.GeomAttribute(t=>t.flId),
                //            t.GeomAttribute(t=>t.flName),
                //            t.GeomAttribute(t=>t.flRegion),
                //            t.GeomAttribute(t=>t.flDistrict),
                //            t.GeomAttribute(t=>t.flStatus)
                //        },
                //        t => t.flGeometry,
                //        t => new[] {
                //            //t.StyleByAttribute(t => t.flLandReserveArea)
                //            //    .Reference(new HuntingSource.References.Object.RefObjectStatuses().Name)
                //            //    .Colors(
                //            //        new System.Collections.Generic.Dictionary<HuntingSource.References.Object.HuntingObjectStatuses, string> {
                //            //            [HuntingSource.References.Object.HuntingObjectStatuses.Active]="#FFFF99",
                //            //            [HuntingSource.References.Object.HuntingObjectStatuses.Deleted]="#FFFF66",
                //            //            [HuntingSource.References.Object.HuntingObjectStatuses.Saled]="#FFFF33",
                //            //        }
                //            //    ),
                //            t.StyleByRefAttribute(t=>t.flRegion)
                //            .RandomColors(),

                //            t.StyleByRefAttribute(t=>t.flDistrict)
                //            .RandomColors()
                //        },
                //        t => new[] {
                //            t.TopN("Топ 5 наименовании", t=>t.flName, t=>t.flId),
                //            t.GroupByWidget("Количество по районам", t=>t.flRegion, t=>(t.flId, AggregationFunc.Count))
                //        }
                //    )
                //)
                //.PrintAsync(re.Form, re.AsFormEnv(), re.Form, new MapDataCachingPolicy(DataToApiDeliveryMode.Push, null));

                //var emailSender = re.RequestContext.AppEnv.ServiceProvider.GetRequiredService<IEmailSender>();
                //try {
                //    await emailSender.SendAsync("uselessgutekatze@gmail.com", "ТЕСТ", "ТЕСТ");
                //    re.Form.Append(new HtmlText("SENDED"));
                //}
                //catch (Exception ex) {
                //    re.Form.Append(new HtmlText("NOT SENDED"));
                //    return;
                //}

                //var apps = new HydrocarbonSource.QueryTables.Application.TbTradePreApplication()
                //.JoinT("TbTradePreApplication", new HydrocarbonSource.QueryTables.Object.TbObjects(), "TbObjects")
                //.On((t1, t2) => new Join(t1.flSubsoilsObjectId, t2.flId))
                //.AddFilter(t => t.L.flAppContent, ConditionOperator.ContainsWord, "Широта</td></tr></table")
                //.Select(t => new FieldAlias[] { t.L.flAppId, t.L.flAppContent, t.R.flCoords }, re.QueryExecuter)
                //.Select(r => new {
                //    flAppId = r.GetVal(t => t.L.flAppId),
                //    flAppContent = r.GetVal(t => t.L.flAppContent),
                //    flSubsoilsCoordinates = r.GetVal(t => t.R.flCoords)
                //});
                //apps.Each(app => {
                //    var coordRows = "Широта</td></tr>";
                //    foreach (var coordinate in app.flSubsoilsCoordinates.MainRing) {
                //        coordRows += "<tr>"
                //            + $"<td>{coordinate.AppropriateX}</td>"
                //            + $"<td>{coordinate.AppropriateY}</td>"
                //            + "</tr>";
                //    }
                //    coordRows += "</table";
                //    new HydrocarbonSource.QueryTables.Application.TbTradePreApplication()
                //    .AddFilter(t => t.flAppId, app.flAppId)
                //    .Update()
                //    .Set(t => t.flAppContent, app.flAppContent.Replace("Широта</td></tr></table", coordRows))
                //    .Execute(re.QueryExecuter);
                //});

                //var qrCodeImageBase64 = Convert.ToBase64String(QrGenerator.GenerateQr("https://github.com/codebude/QRCoder"));
                //re.Form.AddComponent(new HtmlText($"<img src='data:image/bmp;base64,{qrCodeImageBase64}'/>"));

                //var seed = Convert.ToInt32(DateTime.Now.ToString("mmssfff"));
                //RandomColor.Seed(seed);
                //re.Form.AddLabel($"Seed: {seed}");
                //re.Form.Append(new Panel().Self(out var panel));
                ////var colors = RandomColor.GetColors(ColorScheme.Random, Luminosity.Light, 90);
                //var colors = new List<Color>();
                //var luminosityOrder = new[] {
                //    Luminosity.Dark,
                //    Luminosity.Bright,
                //    Luminosity.Light,
                //};
                //var colorSchemeOrder = new[] {
                //    ColorScheme.Green,
                //    ColorScheme.Blue,
                //    ColorScheme.Purple,
                //    ColorScheme.Red,
                //    ColorScheme.Pink,
                //    ColorScheme.Orange,
                //    ColorScheme.Yellow,
                //};
                //colors.AddRange(new int[10].SelectMany(x => {
                //    RandomColor.Seed(++seed);
                //    return RandomColor.GetColors(luminosityOrder.SelectMany(luminosity => colorSchemeOrder.Select(colorScheme => new Options(colorScheme, luminosity))).ToArray());
                //}));
                //colors.Each(color => {
                //    panel.Append(
                //        new Panel("rounded-circle d-inline-block") {
                //            Attributes = {
                //                {"style", $"background-color: {ColorTranslator.ToHtml(Color.FromArgb(color.ToArgb()))}; width: 75px; height: 75px;"}
                //            }
                //        }
                //    );
                //});

                //var subsNumber = 147;
                //var lines = File.ReadAllLines("C:/Users/gaan_v/Desktop/newHYDR.csv").Concat(File.ReadAllLines("C:/Users/gaan_v/Desktop/newHYDR2.csv"));
                //var subsoils = new List<subs>();
                //foreach (var line in lines) {
                //    var cols = Regex.Split(line, ";(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                //    var coordNumber = Convert.ToInt32(cols[(int)colNames.coordNum]);

                //    crd getCoord() {
                //        return new crd() {
                //            Number = coordNumber,
                //            longit = $"{Convert.ToInt32(cols[(int)colNames.lg])}° {Convert.ToInt32(cols[(int)colNames.lm])}′ {Convert.ToDecimal(cols[(int)colNames.ls])}″",
                //            latit = $"{Convert.ToInt32(cols[(int)colNames.rg])}° {Convert.ToInt32(cols[(int)colNames.rm])}′ {Convert.ToDecimal(cols[(int)colNames.rs])}″",
                //            X = Convert.ToDecimal(cols[(int)colNames.lg]) + (1m / 60m * Convert.ToDecimal(cols[(int)colNames.lm])) + ((1m / 60m) / 60m * Convert.ToDecimal(cols[(int)colNames.ls])),
                //            Y = Convert.ToDecimal(cols[(int)colNames.rg]) + (1m / 60m * Convert.ToDecimal(cols[(int)colNames.rm])) + ((1m / 60m) / 60m * Convert.ToDecimal(cols[(int)colNames.rs]))
                //        };
                //    }

                //    decimal? getArea() {
                //        var descr = cols[(int)colNames.info].Replace("\"", "").Replace("\"", "").Trim();
                //        if (descr.Contains("Площадь -")) {
                //            var areaDescr = descr.Split("Площадь -")[1];
                //            if (areaDescr.Contains("км2")) {
                //                return Convert.ToDecimal(areaDescr.Split("км2")[0].Trim().Replace(".", ","));
                //            }
                //        }
                //        return null;
                //    }

                //    int? getBlocks() {
                //        var descr = cols[(int)colNames.info].Replace("\"", "").Replace("\"", "").Trim();
                //        if (descr.Contains("блоков")) {
                //            var blockDescr = descr.Split("блоков")[0].Trim();
                //                return Convert.ToInt32(blockDescr);
                //        }
                //        return null;
                //    }

                //    if (coordNumber == 1) {
                //        subsoils.Add(
                //            new subs() {
                //                Name = cols[(int)colNames.name].Trim(),
                //                Region = cols[(int)colNames.region],
                //                Info = cols[(int)colNames.info].Replace("\"", "").Replace("\"", "").Trim(),
                //                Area = getArea(),
                //                Blocks = getBlocks(),
                //                Coords = new List<crd>() {
                //                    getCoord()
                //                }
                //            }
                //        );
                //    } else {
                //        subsoils[subsoils.Count - 1].Coords.Add(getCoord());
                //    }
                //}

                //var batch = new BatchQuery();

                //var updates = 0;

                //foreach(var subsoil in subsoils) {

                //    var polygonCoords = new List<crd>();
                //    polygonCoords.AddRange(subsoil.Coords);
                //    polygonCoords.Add(subsoil.Coords[0]);

                //    var objs = new TbSubsoilsObjects()
                //        .AddFilter(t => t.flName, ConditionOperator.Contain, subsoil.Name);

                //    var number = "";

                //    if (objs.Count(re.QueryExecuter) == 0) {
                //        number = subsNumber++.ToString();
                //        var objId = new TbSubsoilsObjects().flId.GetNextId(re.QueryExecuter);
                //        batch.Add(
                //            new TbSubsoilsObjects().Insert()
                //            .Set(t => t.flId, objId)
                //            .Set(t => t.flName, subsoil.Name)
                //            .Set(t => t.flNumber, number)
                //            .Set(t => t.flInfo, subsoil.Info)
                //            .Set(t => t.flType, "Hydrocarbon")
                //            .Set(t => t.flArea, (object)subsoil.Area ?? DBNull.Value)
                //            .Set(t => t.flBlockCount, (object)subsoil.Blocks ?? DBNull.Value)
                //            .Set(t => t.flRegion, subsoil.Region)
                //            .Set(t => t.flStatus, "Free")
                //            .Set(t => t.flWKT, $"POLYGON(({string.Join(",", polygonCoords.Select(coord => $"{coord.X:00.0000000000} {coord.Y:00.0000000000}".Replace(",", ".")))}))")
                //            .DataModifingQuery
                //        );

                //        var geomId = new TbSubsoilsObjects().flId.GetNextId(re.QueryExecuter);
                //        batch.Add(
                //            new TbSubsoilsPolygons().Insert()
                //            .Set(t => t.flId, geomId)
                //            .Set(t => t.flSubsoilNumber, number)
                //            .Set(t => t.flGeomNumber, "1")
                //            .Set(t => t.flGeomCode, $"{number}.1")
                //            .Set(t => t.flGeomType, "object")
                //            .DataModifingQuery
                //        );
                //    } else {
                //        number = objs.SelectScalar(t => t.flNumber, re.QueryExecuter);
                //        updates++;
                //        batch.Add(
                //            objs.Update()
                //            .Set(t => t.flInfo, subsoil.Info)
                //            .Set(t => t.flArea, (object)subsoil.Area ?? DBNull.Value)
                //            .Set(t => t.flBlockCount, (object)subsoil.Blocks ?? DBNull.Value)
                //            .Set(t => t.flRegion, subsoil.Region)
                //            .Set(t => t.flWKT, $"POLYGON(({string.Join(",", polygonCoords.Select(coord => $"{coord.X:00.0000000000} {coord.Y:00.0000000000}".Replace(",", ".")))}))")
                //            .DataModifingQuery
                //        );
                //        new TbSubsoilsPolygonsCoords()
                //        .AddFilter(t => t.flGeomCode, $"{number}.1")
                //        .Remove().Execute(re.QueryExecuter);
                //    }

                //    foreach (var coord in subsoil.Coords) {
                //        var coordId = new TbSubsoilsPolygonsCoords().flId.GetNextId(re.QueryExecuter);
                //        batch.Add(
                //            new TbSubsoilsPolygonsCoords().Insert()
                //            .Set(t => t.flId, coordId)
                //            .Set(t => t.flGeomCode, $"{number}.1")
                //            .Set(t => t.flCoordNumber, coord.Number)
                //            .Set(t => t.flLongitute, coord.longit)
                //            .Set(t => t.flLatitude, coord.latit)
                //            .Set(t => t.flxdecimal, coord.X)
                //            .Set(t => t.flydecimal, coord.Y)
                //            .Set(t => t.flWkt, $"POINT({coord.X:00.0000000000} {coord.Y:00.0000000000})".Replace(",", "."))
                //            .DataModifingQuery
                //        );
                //    }

                //}

                //batch.Execute(re.QueryExecuter);

            });
        }

    }
}