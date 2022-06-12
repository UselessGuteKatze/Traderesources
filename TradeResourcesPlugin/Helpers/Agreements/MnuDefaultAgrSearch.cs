using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers {
    public class MnuDefaultAgrSearch : FrmMenu {
        public MnuDefaultAgrSearch(string moduleName) : base(nameof(MnuDefaultAgrSearch), "Реестр")
        {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Access();
            Enabled((rc) => {
                return rc.User.IsAuthentificated;
            });
            OnRendering(re => {

                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());

                var tbAgreements = new TbAgreements();

                var xin = re.User.GetUserXin(re.QueryExecuter);
                if (!isInternal)
                {
                    var or = new LogicGrouper(GroupOperator.Or)
                        .AddFilter(tbAgreements.flSellerBin, ConditionOperator.Equal, xin)
                        .AddFilter(tbAgreements.flWinnerXin, ConditionOperator.Equal, xin)
                        .AddFilter(tbAgreements.flAgreementCreatorBin, ConditionOperator.Equal, xin);
                    tbAgreements.AddLogicGrouper(or);
                }
                tbAgreements.OrderBy = new OrderField[] { new OrderField(tbAgreements.flAgreementId, OrderType.Desc) };

                tbAgreements
                .Search(search => search
                    .Filtering(filter => filter
                        .AddField(t => t.flAgreementId)
                        .AddField(t => t.flAgreementType)
                        .AddField(t => t.flAgreementStatus)
                        .AddField(t => t.flObjectId)
                        .AddField(t => t.flObjectType)
                        .AddField(t => t.flTradeId)
                        .AddField(t => t.flTradeType)
                        .AddField(t => t.flAuctionId)
                    )
                    .TablePresentation(
                        t => new FieldAlias[] {
                            t.flAgreementId,
                            t.flAuctionId,
                            t.flAgreementNumber,
                            t.flAgreementType,
                            t.flObjectId,
                            t.flObjectType,
                            t.flTradeId,
                            t.flTradeType,
                            t.flAgreementStatus,
                            t.flAgreementCreateDate,
                            t.flAgreementSignDate,
                        },
                        t => new[] {
                            t.Column("Действия", (env, r) =>
                                new Link
                                {
                                    Text = re.T("Открыть"),
                                    Controller = nameof(RegistersModule),
                                    Action = nameof(MnuDefaultAgrWizard),
                                    RouteValues = new DefaultAgrTemplateArgs {
                                        MenuAction = MnuDefaultAgrWizard.Actions.View,
                                        AgreementId = r.GetVal(t => t.flAgreementId),
                                        AgreementType = r.GetVal(t => t.flAgreementType),
                                        ObjectId = r.GetVal(t => t.flObjectId),
                                        ObjectType = r.GetVal(t => t.flObjectType),
                                        TradeId = r.GetVal(t => t.flTradeId),
                                        TradeType = r.GetVal(t => t.flTradeType),
                                    },
                                    CssClass = "btn btn-secondary"
                                },
                                width: new WidthAttr(80, WidthMeasure.Px)
                            ),
                            t.Column(t => t.flAgreementNumber),
                            t.Column(t => t.flAgreementType),
                            t.Column(t => t.flAgreementStatus),
                            t.Column(t => t.flAgreementCreateDate),
                            t.Column(t => t.flAgreementSignDate),
                            t.Column(t => t.flObjectId),
                            t.Column(t => t.flObjectType),
                            t.Column(t => t.flTradeId),
                            t.Column(t => t.flTradeType)
                        }
                    )
                )
                .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }
    }
}
