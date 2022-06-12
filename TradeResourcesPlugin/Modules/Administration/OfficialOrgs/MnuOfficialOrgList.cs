using CommonSource.QueryTables;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaHelpers.OrderHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.Administration.OfficialOrgs {
    public class MnuOfficialOrgList : FrmMenu {
        public const string MnuName = "MnuOfficialOrgList";
        public MnuOfficialOrgList(string moduleName, ViewWithOrderStandartPermissions perms) : base(MnuName, "Компетентные органы") {
            Access(perms.ViewObject.Name);
            OnRendering(re => {
                var tbOfficialOrgs = new TbOfficialOrg().Order(t => t.flNameRu);
                tbOfficialOrgs
                   .ToSearchWidget(re)
                   .AddToolbarItem(new Link(re.T("Добавить"), moduleName, MnuOfficialOrgOrder.MnuName, new OfficialOrgOrderQueryArgs { MenuAction = MnuOfficialOrgOrder.Actions.Blank }))
                   .AddReturnFields(tbOfficialOrgs.flBin, tbOfficialOrgs.flNameRu, tbOfficialOrgs.flAdrObl)
                   .AddFilters(tbOfficialOrgs.flBin, tbOfficialOrgs.flNameRu, tbOfficialOrgs.flAdrObl)
                   .AddHiddenFields(tbOfficialOrgs.flOrgId.ToAlias("flOrgIdHidden"))
                   .AddRowActions(r => new Link(re.T("Открыть"), moduleName, MnuOfficialOrg.MnuName, new OfficialOrgQueryArgs { OrgId = tbOfficialOrgs.flOrgId.GetRowVal(r, "flOrgIdHidden"), MenuAction = MnuOfficialOrg.Actions.View }))
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
