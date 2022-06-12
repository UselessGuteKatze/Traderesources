using TradeResourcesPlugin.Modules.Administration;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers.Agreements {
    public class MnuPaymentMatchOrderList : FrmMenu {
        public const string MnuName = nameof(MnuPaymentMatchOrderList);

        public MnuPaymentMatchOrderList(string moduleName) : base(MnuName, "Приказы по привязкам платежей") {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Enabled((rc) => {
                if (rc.User.IsGuest()) {
                    return false;
                }
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                // IAC
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || (rc.User.HasPermission(moduleName, AccountActivityTypesProvider.PaymentOrders.CreateOrder.Name) && rc.User.HasPermission(moduleName, AccountActivityTypesProvider.PaymentOrders.EditOrder.Name))) {
                    return true;
                }

                return false;
            });
            OnRendering(re => {

                var isInternal = !re.User.IsExternalUser() && !re.User.IsGuest();

                var xin = re.User.GetUserXin(re.QueryExecuter);
                var tbForestriesRev = new TbPaymentMatchesRevisions();
                var tbAgreements = new TbAgreements();
                if (!isInternal) {
                    tbAgreements.AddFilter(t => t.flAgreementCreatorBin, xin);
                }
                var tbForestriesOrderResult = new TbPaymentMatchesOrderResult();
                var join = tbForestriesRev
                .JoinT(tbForestriesRev.Name, tbForestriesOrderResult, tbForestriesOrderResult.Name)
                .On((t1, t2) => new Join(t1.flId, t2.flSubjectId))
                .JoinT(tbForestriesRev.Name, tbAgreements, tbAgreements.Name)
                .On((t1, t2) => new Join(t1.L.flAgreementId, t2.flAgreementId));
                join.OrderBy = new OrderField[] { new OrderField(tbForestriesRev.flId, OrderType.Desc) };
                join
                .Search(search => search
                        .Filtering(filter => filter
                            .AddField(t => t.L.L.flId)
                            .AddField(t => t.L.R.flStatus)
                            .AddField(t => t.R.flAgreementNumber)
                            .AddField(t => t.R.flAuctionId)
                        )
                        .TablePresentation(
                            t => new FieldAlias[] {
                                t.L.L.flId,
                                t.R.flAgreementNumber,
                                t.R.flAuctionId,
                                t.L.R.flStatus,
                                t.L.L.flAmount,
                                t.L.L.flOverpaymentAmount,
                                t.L.L.flOverpaymentSendAmount,
                            },
                            t => new[] {
                                t.Column("Действия", (env, r) =>
                                    new Link
                                    {
                                        Text = re.T("Открыть"),
                                        Controller = moduleName,
                                        Action = nameof(MnuPaymentMatchOrder),
                                        RouteValues = new PaymentMatchesOrderQueryArgs { MenuAction = "view-order", RevisionId = r.GetVal(t => t.L.L.flId) },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column("Статус приказа", (env, r) =>  {
                                    var value = r.GetVal(tr => tr.L.R.flStatus);
                                    var text = t.L.R.flStatus.GetDisplayText(value.ToString(), env.RequestContext);
                                    return new HtmlText(text);
                                }),
                                t.Column(t => t.R.flAgreementNumber),
                                t.Column(t => t.R.flAuctionId),
                                t.Column(t => t.L.R.flStatus),
                                t.Column(t => t.L.L.flAmount),
                                t.Column(t => t.L.L.flOverpaymentAmount),
                                t.Column(t => t.L.L.flOverpaymentSendAmount),
                            }
                        )
                    )
                    .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }

    }
}
