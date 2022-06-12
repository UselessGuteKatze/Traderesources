using LandSource.QueryTables.Applications;
using System;
using System.Collections.Generic;
using System.Text;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.LandObjectsMenus.Applications {
    public class MnuLandApplicationsSearch : FrmMenu {
        public const string MnuName = nameof(MnuLandApplicationsSearch);
        public MnuLandApplicationsSearch(string moduleName) : base(MnuName, "Реестр")
        {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Enabled((rc) => {
                return true;
            });
            OnRendering(re => {

                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", re.QueryExecuter)/*re.User.HasCustomRole("landobjects", "appLandEdit", re.QueryExecuter) || re.User.HasCustomRole("landobjects", "appLandView", re.QueryExecuter)*/;

                var tbLandApplications = new TbLandApplications();

                if (!isInternal) {
                    var xin = re.User.GetUserXin(re.QueryExecuter);
                    var or = new LogicGrouper(GroupOperator.Or)
                        .AddFilter(tbLandApplications.flApplicantXin, ConditionOperator.Equal, xin)
                        .AddFilter(tbLandApplications.flCompetentOrgBin, ConditionOperator.Equal, xin);
                    tbLandApplications.AddLogicGrouper(or);
                }

                if (re.User.IsGuest()) {
                    tbLandApplications.AddFilter(t => t.flApplicantXin, "NULL");
                }

                tbLandApplications
                .Search(search => search
                    .Toolbar(toolbar => toolbar.Add(new Link {
                        Controller = moduleName,
                        Action = nameof(MnuLandApplications),
                        RouteValues = new LandApplicationsArgs { AppId = -1, MenuAction = "create" },
                        Text = re.T("Подать заявление"),
                        CssClass = "btn btn-success"
                    }))
                    .Filtering(filter => filter
                        .AddField(t => t.flDistrict)
                        .AddField(t => t.flId)
                        .AddField(t => t.flStatus)
                    )
                    .TablePresentation(
                        t => new FieldAlias[] {
                            t.flId,
                            t.flRegion,
                            t.flDistrict,
                            t.flStatus,
                            t.flApplicantName,
                            t.flApplicantXin,
                            t.flRegDate,
                            t.flSignSendDate,
                            t.flAcceptDate,
                            t.flRejectDate,
                            t.flRejectReason,
                        },
                        t => new[] {
                            t.Column("Действия", (env, r) =>
                                new Link
                                {
                                    Text = re.T("Открыть"),
                                    Controller = moduleName,
                                    Action = nameof(MnuLandApplications),
                                    RouteValues = new LandApplicationsArgs { MenuAction = "view", AppId = r.GetVal(t => t.flId) },
                                    CssClass = "btn btn-secondary"
                                },
                                width: new WidthAttr(80, WidthMeasure.Px)
                            ),
                            t.Column(t => t.flId),
                            t.Column(t => t.flRegion),
                            t.Column(t => t.flDistrict),
                            t.Column(t => t.flStatus),
                            t.Column(t => t.flApplicantName),
                            t.Column(t => t.flApplicantXin),
                            t.Column(t => t.flRegDate),
                            t.Column(t => t.flSignSendDate),
                            t.Column(t => t.flAcceptDate),
                            t.Column(t => t.flRejectDate),
                            t.Column(t => t.flRejectReason),
                        }
                    )
                )
                .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }

    }
}
