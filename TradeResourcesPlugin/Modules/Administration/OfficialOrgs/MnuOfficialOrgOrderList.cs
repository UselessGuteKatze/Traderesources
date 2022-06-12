using CommonSource.QueryTables;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.YodaHelpers.OrderHelpers;
using YodaHelpers.OrderHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.Administration.OfficialOrgs {
    public class MnuOfficialOrgOrderList : FrmMenu {
        public const string MnuName = "MnuOfficialOrgOrderList";
        public MnuOfficialOrgOrderList(string moduleName, OrderStandartPermissions perms) : base(MnuName, "Приказы по компетентным органам") {
            PageTitle("Приказы по компетентным органам");
            Access(perms.ViewOrder.Name);
            OnRendering(re => {
                var tbOfficialOrgsRevs = new TbOfficialOrgRevisions();
                var tbOrders = new TbOfficialOrgOrderResult();
                var query = tbOfficialOrgsRevs.JoinT("tbOrgs", tbOrders, "tbOrders").On((t1, t2) => new Join(t1.flRevisionId, t2.flSubjectId));
                query.Order(t => t.L.flRevisionId);
                query
                    .ToSearchWidget(re)
                    .AddToolbarItem(new Link(re.T("Добавить"), moduleName, MnuOfficialOrgOrder.MnuName, new OfficialOrgOrderQueryArgs { MenuAction = MnuOfficialOrgOrder.Actions.Blank }))
                    .AddReturnFields(tbOrders.flRegDate, tbOrders.flStatus, tbOrders.flExecDate)
                    .AddReturnFields(tbOfficialOrgsRevs.flBin, tbOfficialOrgsRevs.flNameRu, tbOfficialOrgsRevs.flAdrObl)
                    .AddFilter(tbOrders.flStatus, new[] { RefOrderResultStatus.Values.Running, RefOrderResultStatus.Values.None })
                    .AddFilters(tbOfficialOrgsRevs.flBin, tbOfficialOrgsRevs.flNameRu, tbOfficialOrgsRevs.flAdrObl)
                    .AddHiddenFields(tbOfficialOrgsRevs.flRevisionId.ToAlias("flRevisionIdHidden"))
                    .AddRowActions(r => new Link(re.T("Открыть"), moduleName, MnuOfficialOrgOrder.MnuName, new OfficialOrgOrderQueryArgs { RevisionId = tbOfficialOrgsRevs.flRevisionId.GetRowVal(r, "flRevisionIdHidden"), MenuAction = MnuOfficialOrgOrder.Actions.ViewOrder }))
                    .AutoExecuteQuery(true)
                    .HideSearchButton(false)
                    .CanConfigureFilterFields(true)
                    .CanConfigureOutputFields(true)
                    .CanExportToExcel(true)
                    .Print(re.Form, MnuName);
            });
        }
    }
}
