using CommonSource.QueryTables;
using System;
using System.Collections.Generic;
using System.Text;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.Menus.Comission {
    public class MnuCommissionMembersSearch : FrmMenu{

        public MnuCommissionMembersSearch() : base(nameof(MnuCommissionMembersSearch), "Члены комиссии") {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Enabled(rc => {
                if (
                rc.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", rc.QueryExecuter)/*rc.User.HasPermission(nameof(RegistersModule), RegistersModule.LocalPermissions.Landlords)*/
                || rc.User.HasRole("TRADERESOURCES-Охотничьи угодья-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("huntingobjects", "dataEdit", rc.QueryExecuter)*/
                || rc.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("fishingobjects", "dataEdit", rc.QueryExecuter)*/
                || rc.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("landobjects", "appLandEdit", rc.QueryExecuter)*/
                || rc.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", rc.QueryExecuter)
                )
                {
                    return true;
                }
                return false;
            });
            OnRendering(re => {
                var xin = re.User.GetUserXin(re.QueryExecuter);
                var tbCommMembers = new TbComissionMembers();
                if (re.User.IsExternalUser() && xin != "050540004455") {
                    tbCommMembers.AddFilter(t => t.flCompetentOrgBin, xin);
                }

                tbCommMembers.OrderBy = new OrderField[] { new OrderField(tbCommMembers.flId, OrderType.Desc) };

                tbCommMembers
                    .ToSearchWidget(re)
                    .AddFilters(tbCommMembers.Fields)
                    .AddReturnFields(tbCommMembers.Fields.ToFieldsAliases())
                    .CanConfigureFilterFields(re.User.IsAuthentificated)
                    .CanConfigureOutputFields(re.User.IsAuthentificated)
                    .CanExportToExcel(re.User.IsAuthentificated)
                    .HideSearchButton(false)
                    .AutoExecuteQuery(false)
                    .AddToolbarItemIf(
                        (
                            re.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", re.QueryExecuter)
                            || re.User.HasRole("TRADERESOURCES-Охотничьи угодья-Создание приказов", re.QueryExecuter)
                            || re.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", re.QueryExecuter)
                            || re.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", re.QueryExecuter)
                            || re.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", re.QueryExecuter)
                        ),
                        new Link {
                        Controller = nameof(RegistersModule),
                        Action = nameof(MnuCommissionActions),
                        RouteValues = new CommissionQueryArgs {
                            MenuAction = MnuCommissionActions.Actions.Create,
                            Id = -1
                        },
                        Project = re.RequestContext.Project,
                        Text = re.T("Добавить члена комиссии")
                    })
                    .AddRowActions(r => new Link { 
                        Text = re.T("Открыть"),
                        Controller = nameof(RegistersModule),
                        Action = nameof(MnuCommissionActions),
                        RouteValues = new CommissionQueryArgs {
                            MenuAction = MnuCommissionActions.Actions.View,
                            Id = tbCommMembers.flId.GetRowVal(r)
                        },
                        Project = re.RequestContext.Project
                    })
                    .Print(re.Form, nameof(MnuCommissionMembersSearch) + "search_form");

            });
        }

    }
}
