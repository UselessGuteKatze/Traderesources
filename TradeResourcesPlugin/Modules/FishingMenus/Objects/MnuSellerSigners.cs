using FishingSource.QueryTables.Common;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using YodaApp.Yoda.Interfaces.Forms.Components;
using YodaHelpers.ActionMenus;
using YodaHelpers.Fields;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.FishingMenus.Objects {
    public class MnuSellerSignersArgs : ActionQueryArgsBase {
        public int Id { get; set; }
    }
    public class MnuSellerSigners : MnuActionsExt<MnuSellerSignersArgs> {
        public MnuSellerSigners(string moduleName, string mnuName, string mnuTitle) : base(moduleName, mnuName, mnuTitle)
        {
            Enabled(rc => {

                var isInternal = (!rc.User.IsExternalUser() && !rc.User.IsGuest());
                var isUserRegistrator = rc.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("fishingobjects", "dataEdit", rc.QueryExecuter)*/;

                if (isInternal || isUserRegistrator)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }
        public class Actions {
            public const string
                View = nameof(View),
                Create = nameof(Create),
                Edit = nameof(Edit);
        }
        protected override string GetDefaultActionName()
        {
            return Actions.View;
        }

        public override void Configure(ActionConfig<MnuSellerSignersArgs> config)
        {
            config
                .OnAction(Actions.View, action => action
                    .IsValid(env =>
                    {
                        return new OkResult();
                    })
                    .OnRendering(re =>
                    {
                        var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                        var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", re.QueryExecuter)/*re.User.HasCustomRole("fishingobjects", "dataEdit", re.QueryExecuter)*/;
                        var tbSellerSigners = new TbSellerSigners();

                        if (!isInternal)
                        {
                            var xin = re.User.GetUserXin(re.QueryExecuter);
                            var or = new LogicGrouper(GroupOperator.Or)
                                .AddFilter(tbSellerSigners.flSellerBin, ConditionOperator.Equal, xin)
                                .AddFilter(tbSellerSigners.flSignerBin, ConditionOperator.Equal, xin);
                            tbSellerSigners.AddLogicGrouper(or);
                        }

                        tbSellerSigners.OrderBy = new[] { new OrderField(tbSellerSigners.flId, OrderType.Desc) };
                        tbSellerSigners
                            .ToSearchWidget(re)
                            .AddFilters(
                                tbSellerSigners.flSellerBin,
                                tbSellerSigners.flSignerBin
                            )
                            .AddReturnFields(
                                tbSellerSigners.flSellerBin,
                                tbSellerSigners.flSignerBin
                            )
                            .AddHiddenFields(tbSellerSigners.flId)
                            .HideSearchButton(false)
                            .AutoExecuteQuery(true)
                            .AddRowActions(r => new Link
                            {
                                Text = re.T("Изменить"),
                                Controller = ModuleName,
                                Action = MenuName,
                                RouteValues = new MnuSellerSignersArgs { Id = tbSellerSigners.flId.GetVal(r), MenuAction = Actions.Edit }
                            })
                            .AddToolbarItem(new Link
                            {
                                Controller = ModuleName,
                                Action = MenuName,
                                RouteValues = new MnuSellerSignersArgs { Id = -1, MenuAction = Actions.Create },
                                Text = re.T("Добавить")
                            })
                            .Print(re.Form, "tbSellerSigners");
                    })
                )
                .OnAction(Actions.Create, action => action
                    .IsValid(env =>
                    {
                        return new OkResult();
                    })
                    .OnRendering(re =>
                    {
                        var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                        var tbSellerSigners = new TbSellerSigners();

                        tbSellerSigners.flSellerBin.RenderCustomT(re.Form, re, !isInternal ? re.User.GetUserXin(re.QueryExecuter) : null, readOnly: !isInternal);
                        tbSellerSigners.flSignerBin.RenderCustomT(re.Form, re, null);
                        re.Form.AddSubmitButton("Создать");

                    })
                    .OnValidating(re =>
                    {
                        var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());

                        var tbSellerSigners = new TbSellerSigners();
                        if (isInternal)
                        {
                            tbSellerSigners.flSellerBin.Validate(re);
                        }
                        tbSellerSigners.flSignerBin.Validate(re);

                    })
                    .OnProcessing(re =>
                    {
                        var tbSellerSigners = new TbSellerSigners();

                        var sellerBin = "";
                        var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                        if (isInternal)
                        {
                            sellerBin = tbSellerSigners.flSellerBin.GetVal(re);
                        } else
                        {
                            sellerBin = re.User.GetUserXin(re.QueryExecuter);
                        }

                        var newId = tbSellerSigners.flId.GetNextId(re.QueryExecuter);
                        tbSellerSigners
                            .Insert()
                            .Set(t => t.flId, newId)
                            .Set(t => t.flSellerBin, sellerBin)
                            .Set(t => t.flSignerBin, tbSellerSigners.flSignerBin.GetVal(re))
                            .Execute(re.QueryExecuter);
                        re.Redirect.SetRedirect(ModuleName, MenuName, new MnuSellerSignersArgs { Id = newId, MenuAction = Actions.View });
                    })
                )
                .OnAction(Actions.Edit, action => action
                    .IsValid(env =>
                    {
                        return new OkResult();
                    })
                    .OnRendering(re =>
                    {
                        var tbSellerSigners = new TbSellerSigners();
                        tbSellerSigners.GetPair(re.Args.Id, re.QueryExecuter, out var data);
                        tbSellerSigners.flSellerBin.RenderCustomT(re.Form, re, data.flSellerBin, readOnly: true);
                        tbSellerSigners.flSignerBin.RenderCustomT(re.Form, re, data.flSignerBin);
                        re.Form.AddSubmitButton("Принять");

                    })
                    .OnValidating(re =>
                    {
                        var tbSellerSigners = new TbSellerSigners();
                        tbSellerSigners.flSellerBin.Validate(re);
                        tbSellerSigners.flSignerBin.Validate(re);

                    })
                    .OnProcessing(re =>
                    {
                        var tbSellerSigners = new TbSellerSigners();
                        tbSellerSigners.AddFilter(t => t.flId, re.Args.Id);
                        tbSellerSigners
                            .Update()
                            .Set(t => t.flSellerBin, tbSellerSigners.flSellerBin.GetVal(re))
                            .Set(t => t.flSignerBin, tbSellerSigners.flSignerBin.GetVal(re))
                            .Execute(re.QueryExecuter);
                        re.Redirect.SetRedirect(ModuleName, MenuName, new MnuSellerSignersArgs { Id = re.Args.Id, MenuAction = Actions.View });
                    })
                )
                ;
        }
    }
}
