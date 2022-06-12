using ForestSource.QueryTables.Common;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using YodaApp.UiSearch;
using YodaApp.Yoda.Interfaces.Forms.Components;
using YodaHelpers.ActionMenus;
using YodaHelpers.Fields;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.ForestMenus.Objects {
    public class MnuSellerCreatorsArgs : ActionQueryArgsBase {
        public int Id { get; set; }
    }
    public class MnuSellerCreators : MnuActionsExt<MnuSellerCreatorsArgs> {
        public MnuSellerCreators(string moduleName, string mnuName, string mnuTitle) : base(moduleName, mnuName, mnuTitle)
        {
            Enabled(rc => {

                var isInternal = (!rc.User.IsExternalUser() && !rc.User.IsGuest());
                var isUserRegistrator = rc.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", rc.QueryExecuter)/*rc.User.HasCustomRole("forestobjects", "dataEdit", rc.QueryExecuter)*/;

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

        public override void Configure(ActionConfig<MnuSellerCreatorsArgs> config)
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
                        var curXin = re.User.GetUserXin(re.QueryExecuter);
                        var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", re.QueryExecuter)/*re.User.HasCustomRole("forestobjects", "dataEdit", re.QueryExecuter)*/;
                        var tbSellerCreators = new TbSellerCreators();
                        
                        if (!isInternal)
                        {
                            var or = new LogicGrouper(GroupOperator.Or)
                            .AddFilter(tbSellerCreators.flSellerBin, ConditionOperator.Equal, curXin)
                            .AddFilter(tbSellerCreators.flCreatorBin, ConditionOperator.Equal, curXin);
                            tbSellerCreators.AddLogicGrouper(or);
                        } 

                        tbSellerCreators.OrderBy = new[] { new OrderField(tbSellerCreators.flId, OrderType.Desc) };
                        tbSellerCreators
                            .Search(search => search
                            .Toolbar(toolbar => toolbar.Add(new Link {
                                Controller = ModuleName,
                                Action = MenuName,
                                RouteValues = new MnuSellerCreatorsArgs { Id = -1, MenuAction = Actions.Create },
                                Text = re.T("Добавить"),
                                CssClass = "btn btn-success"
                            }))
                            .Filtering(filter => filter
                                .AddField(t => t.flSellerBin)
                                .AddField(t => t.flCreatorBin)
                            )
                            .TablePresentation(
                                t => new FieldAlias[] {
                                    t.flId,
                                    t.flSellerBin,
                                    t.flCreatorBin,
                                },
                                t => new[] {
                                    t.Column("Действия", (env, r) =>
                                        new Link
                                        {
                                            Text = re.T("Изменить"),
                                            Controller = ModuleName,
                                            Action = MenuName,
                                            RouteValues = new MnuSellerCreatorsArgs { Id = r.GetVal(t => t.flId), MenuAction = Actions.Edit },
                                            CssClass = "btn btn-secondary"
                                        },
                                        width: new WidthAttr(80, WidthMeasure.Px)
                                    ),
                                    t.Column(t => t.flSellerBin),
                                    t.Column(t => t.flCreatorBin),
                                }
                            )
                        )
                        .Print(re.Form, re, re.Form);
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
                        var tbSellerCreators = new TbSellerCreators();

                        tbSellerCreators.flSellerBin.RenderCustomT(re.Form, re, !isInternal ? re.User.GetUserXin(re.QueryExecuter) : null, readOnly: !isInternal);
                        tbSellerCreators.flCreatorBin.RenderCustomT(re.Form, re, null);
                        re.Form.AddSubmitButton("Создать");

                    })
                    .OnValidating(re =>
                    {
                        var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());

                        var tbSellerCreators = new TbSellerCreators();
                        if (isInternal)
                        {
                            tbSellerCreators.flSellerBin.Validate(re);
                        }
                        tbSellerCreators.flCreatorBin.Validate(re);

                    })
                    .OnProcessing(re =>
                    {
                        var tbSellerCreators = new TbSellerCreators();

                        var sellerBin = "";
                        var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                        if (isInternal)
                        {
                            sellerBin = tbSellerCreators.flSellerBin.GetVal(re);
                        } else
                        {
                            sellerBin = re.User.GetUserXin(re.QueryExecuter);
                        }

                        var newId = tbSellerCreators.flId.GetNextId(re.QueryExecuter);
                        tbSellerCreators
                            .Insert()
                            .Set(t => t.flId, newId)
                            .Set(t => t.flSellerBin, sellerBin)
                            .Set(t => t.flCreatorBin, tbSellerCreators.flCreatorBin.GetVal(re))
                            .Execute(re.QueryExecuter);
                        re.Redirect.SetRedirect(ModuleName, MenuName, new MnuSellerCreatorsArgs { Id = newId, MenuAction = Actions.View });
                    })
                )
                .OnAction(Actions.Edit, action => action
                    .IsValid(env =>
                    {
                        return new OkResult();
                    })
                    .OnRendering(re =>
                    {
                        var tbSellerCreators = new TbSellerCreators();
                        tbSellerCreators.GetPair(re.Args.Id, re.QueryExecuter, out var data);
                        tbSellerCreators.flSellerBin.RenderCustomT(re.Form, re, data.flSellerBin, readOnly: true);
                        tbSellerCreators.flCreatorBin.RenderCustomT(re.Form, re, data.flCreatorBin);
                        re.Form.AddSubmitButton("Принять");

                    })
                    .OnValidating(re => {
                        var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());

                        var tbSellerCreators = new TbSellerCreators();
                        if (isInternal)
                        {
                            tbSellerCreators.flSellerBin.Validate(re);
                        }
                        tbSellerCreators.flCreatorBin.Validate(re);

                    })
                    .OnProcessing(re =>
                    {
                        var tbSellerCreators = new TbSellerCreators();
                        tbSellerCreators.AddFilter(t => t.flId, re.Args.Id);

                        var sellerBin = "";
                        var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                        if (isInternal) {
                            sellerBin = tbSellerCreators.flSellerBin.GetVal(re);
                        }
                        else {
                            sellerBin = re.User.GetUserXin(re.QueryExecuter);
                        }

                        tbSellerCreators
                            .Update()
                            .Set(t => t.flSellerBin, sellerBin)
                            .Set(t => t.flCreatorBin, tbSellerCreators.flCreatorBin.GetVal(re))
                            .Execute(re.QueryExecuter);
                        re.Redirect.SetRedirect(ModuleName, MenuName, new MnuSellerCreatorsArgs { Id = re.Args.Id, MenuAction = Actions.View });
                    })
                )
                ;
        }
    }
}
