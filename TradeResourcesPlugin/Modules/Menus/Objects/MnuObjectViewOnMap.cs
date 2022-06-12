using LandSource.References.Objects;
using TradeResourcesPlugin.Modules.FishingMenus.Objects;
using TradeResourcesPlugin.Modules.ForestMenus.ForestryPieces;
using TradeResourcesPlugin.Modules.HuntingMenus.Objects;
using TradeResourcesPlugin.Modules.HydrocarbonMenus.Objects;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Object;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Menu;

namespace TradeResourcesPlugin.Modules.Menus.Objects {
    public class MnuObjectViewToMap: FrmMenu<MnuObjectViewMapQueryArg> {
        public static string mnuName = nameof(MnuObjectViewToMap);
        public MnuObjectViewToMap(string moduleName) : base(mnuName, "Карта недропользования (Демо)") {
            OnRendering(re => {

                re.Redirect.SetRedirectToUrl(GetObjectOnMapUrl(re.Args.ObjectId, re.Args.ObjectType));

            });
        }

        public static string GetObjectOnMapUrl(int ObjectId, string ObjectType)
        {
            return $"https://lands.qoldau.kz/ru/lands-map/subsoils?zoomToObject={ObjectId}&objectType={ObjectType}";
        }

    }

    public class MnuSubsoilsViewMap: FrmMenu {
        public MnuSubsoilsViewMap(string prefix, string subsoilType) : base($"{prefix}-{nameof(MnuSubsoilsViewMap)}-{subsoilType}", "Карта") {
            OnRendering(re => {

                re.Redirect.SetRedirectToUrl($"https://lands.qoldau.kz/ru/lands-map/subsoils?objectType={subsoilType}");
                
            });
        }
    }

    public class MnuObjectViewFromMap: FrmMenu<MnuObjectViewMapQueryArg> {
        public static string mnuName = nameof(MnuObjectViewFromMap);
        public MnuObjectViewFromMap(string moduleName) : base(mnuName, "Объекты") {
            OnRendering(re => {

                var id = re.Args.ObjectId;
                var type = re.Args.ObjectType;

                switch (type)
                {
                    case RefTradeObjectTypes.Values.HYDROCARBON:
                        {
                            re.Redirect.SetRedirect(moduleName, nameof(MnuHydrocarbonObjectView), new HydrocarbonObjectViewArgs() { Id = id, MenuAction = MnuHydrocarbonObjectView.Actions.View });
                            break;
                        }
                    case RefTradeObjectTypes.Values.HUNTINGOBJECT:
                        {
                            re.Redirect.SetRedirect(moduleName, nameof(MnuHuntingObjectView), new HuntingObjectViewArgs() { Id = id, MenuAction = MnuHuntingObjectView.Actions.View });
                            break;
                        }
                    case RefTradeObjectTypes.Values.FISHINGOBJECT:
                        {
                            re.Redirect.SetRedirect(moduleName, nameof(MnuFishingObjectView), new FishingObjectViewArgs() { Id = id, MenuAction = MnuFishingObjectView.Actions.View });
                            break;
                        }
                    case RefTradeObjectTypes.Values.LANDOBJECT:
                        {
                            re.Redirect.SetRedirect(moduleName, nameof(MnuLandObjectView), new LandObjectViewActionQueryArgs() { Id = id, MenuAction = MnuLandObjectView.Actions.View });
                            break;
                        }
                    case RefTradeObjectTypes.Values.FORESTOBJECT:
                        {
                            re.Redirect.SetRedirect(moduleName, nameof(MnuForestryPieceView), new ForestryPieceViewArgs() { Id = id, MenuAction = MnuForestryPieceView.Actions.View });
                            break;
                        }
                    default:
                        {
                            re.Form.AddHtml($@"
                                <div class=""alert alert-warning"" role=""alert"" style=""
                                    font-size: large;
                                    font-weight: bold;
                                "">
                                    {re.T("Тип")} ""{type}"" {re.T("не известен")}.
                                </div>
                            ");
                            break;
                        }
                }

            });
        }
    }

    public class MnuObjectViewMapQueryArg {
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
    }

}