using Analytics;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Menu;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.Administration {
    public class MnuAnalyticsMap : FrmMenu {

        public const string MenuName = nameof(MnuAnalyticsMap);
        public MnuAnalyticsMap() : base(MenuName, "Аналитическая карта") {
            ProjectsConfig(ProjectsList.All);
            Path("analytics-map");
            Enabled(c => c.User.GetUserIin(c.QueryExecuter) == "990922350945");
            OnRendering(async re => {

               await new FishingSource.QueryTables.Object.TbObjects()
               .Map(map => map
                   .Presentation(
                       t => new FieldAlias[] {
                           t.flId,
                           t.flName,
                           t.flRegion,
                           t.flDistrict,
                           t.flStatus,
                           t.flGeometry
                       },
                       t => new[] {
                           t.GeomAttribute(t=>t.flId),
                           t.GeomAttribute(t=>t.flName),
                           t.GeomAttribute(t=>t.flRegion),
                           t.GeomAttribute(t=>t.flDistrict),
                           t.GeomAttribute(t=>t.flStatus)
                       },
                       t => t.flGeometry,
                       t => new[] {
                           //t.StyleByAttribute(t => t.flLandReserveArea)
                           //    .Reference(new HuntingSource.References.Object.RefObjectStatuses().Name)
                           //    .Colors(
                           //        new System.Collections.Generic.Dictionary<HuntingSource.References.Object.HuntingObjectStatuses, string> {
                           //            [HuntingSource.References.Object.HuntingObjectStatuses.Active]="#FFFF99",
                           //            [HuntingSource.References.Object.HuntingObjectStatuses.Deleted]="#FFFF66",
                           //            [HuntingSource.References.Object.HuntingObjectStatuses.Saled]="#FFFF33",
                           //        }
                           //    ),
                           t.StyleByRefAttribute(t=>t.flRegion)
                           .RandomColors(),

                           t.StyleByRefAttribute(t=>t.flDistrict)
                           .RandomColors()
                       },
                       t => new[] {
                           t.TopN("Топ 5 наименовании", t=>t.flName, t=>t.flId),
                           t.GroupByWidget("Количество по районам", t=>t.flRegion, t=>(t.flId, AggregationFunc.Count))
                       }
                   )
               )
               .PrintAsync(re.Form, re.AsFormEnv(), re.Form, new MapDataCachingPolicy(DataToApiDeliveryMode.Push, null));

            });
        }

    }
}