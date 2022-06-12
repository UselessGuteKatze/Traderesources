using HydrocarbonSource.QueryTables.Application;
using HydrocarbonSource.References.Application;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.HydrocarbonMenus.Applications {
    public class MnuHydrocarbonAppsSearch : FrmMenu {
        public const string MnuName = nameof(MnuHydrocarbonAppsSearch);

        public MnuHydrocarbonAppsSearch(string moduleName) : base(MnuName, "Заявления на проведение") {
            AccessCheck((args, env) => {
                if (env.User.IsGuest()) {
                    return new AccessCheckResult {
                        HasAccess = false,
                        ExceptionMessage = "Доступ запрещен"
                    };
                }
                return new AccessCheckResult {
                    HasAccess = true
                };
            });
            OnRendering(re => {

                var isDep = re.User.GetUserXin(re.QueryExecuter).In("140940023346", "050540004455", "050540000002");
                var isIntern = !re.User.IsExternalUser() && !re.User.IsGuest();

                if (!isDep && !isIntern) {
                    renderInfo(re);
                }

                var tbApps = new TbTradePreApplication();

                if (isDep || isIntern) {
                    tbApps.AddFilterIn(tbApps.flStatus,
                            new[] {
                        PreApplicationStatus.Registered.ToString(),
                        PreApplicationStatus.Accepted.ToString(),
                        PreApplicationStatus.Declined.ToString()
                            });
                } else {
                    tbApps.AddFilter(tbApps.flApplicantXin, re.User.GetUserXin(re.QueryExecuter));
                }

                tbApps.Order(t => t.flRegDate, OrderType.Asc);
                tbApps
                .Search(search => search
                        .Toolbar(toolbar => toolbar.AddIf(!isDep && !isIntern, new Link {
                            Controller = moduleName,
                            Action = MnuHydrocarbonApp.MnuName,
                            RouteValues = new SubsoilsApplicationArgs { AppId = 0, ObjectId = 0, MenuAction = MnuHydrocarbonApp.Actions.Create },
                            Text = re.T("Подать заявление на проведение"),
                            CssClass = "btn btn-success"
                        }))
                        .Filtering(filter => filter
                            .AddField(t => t.flAppId)
                            .AddField(t => t.flAppNumber)
                            .AddField(t => t.flStatus)
                        )
                        .TablePresentation(
                            t => new FieldAlias[] {
                                t.flAppId,
                                t.flAppNumber,
                                t.flStatus,
                                t.flApplicantName,
                                t.flApplicantXin,
                                t.flSubsoilsName,
                                t.flRegDate,
                                t.flAcceptDate,
                                t.flRejectDate,
                                t.flSubsoilsObjectId,
                            },
                            t => new[] {
                                t.Column("Действия", (env, r) => new Link
                                    {
                                        Text = re.T("Открыть"),
                                        Controller = moduleName,
                                        Action = MnuHydrocarbonApp.MnuName,
                                        RouteValues = isDep || isIntern ?
                                            new SubsoilsApplicationArgs { AppId = r.GetVal(t => t.flAppId), ObjectId = r.GetVal(t => t.flSubsoilsObjectId), MenuAction = MnuHydrocarbonApp.Actions.OfficialOrgView }
                                            :
                                            new SubsoilsApplicationArgs { AppId = r.GetVal(t => t.flAppId), ObjectId = r.GetVal(t => t.flSubsoilsObjectId), MenuAction = MnuHydrocarbonApp.Actions.ViewApplicant },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column(t => t.flAppNumber),
                                t.Column(t => t.flStatus),
                                t.Column(t => t.flApplicantName),
                                t.Column(t => t.flApplicantXin),
                                t.Column(t => t.flSubsoilsName),
                                t.Column(t => t.flRegDate),
                                t.Column(t => t.flAcceptDate),
                                t.Column(t => t.flRejectDate),
                                t.Column(t => t.flSubsoilsObjectId),
                            }
                        )
                        .ExcelPresentation(
                            t => new FieldAlias[] {
                                t.flAppNumber,
                                t.flStatus,
                                t.flApplicantName,
                                t.flApplicantXin,
                                t.flSubsoilsName,
                                t.flRegDate,
                                t.flAcceptDate,
                                t.flRejectDate,
                                t.flSubsoilsObjectId,
                            },
                            t => new[] {
                                t.ExcelColumn(t => t.flAppNumber),
                                t.ExcelColumn(t => t.flStatus),
                                t.ExcelColumn(t => t.flApplicantName),
                                t.ExcelColumn(t => t.flApplicantXin),
                                t.ExcelColumn(t => t.flSubsoilsName),
                                t.ExcelColumn(t => t.flRegDate),
                                t.ExcelColumn(t => t.flAcceptDate),
                                t.ExcelColumn(t => t.flRejectDate),
                                t.ExcelColumn(t => t.flSubsoilsObjectId)
                            }
                        )
                    )
                    .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }
        private void renderInfo(FrmRenderEnvironment<EmptyQueryArgs> re) {
            re.Form.AddComponent(new HtmlText(re.T(@"
<p>
    Лицо, заинтересованное в получении права недропользования по углеводородам,
    может подать в компетентный орган Заявление на проведение аукциона (далее –
    Заявление) посредством торговой системы оператора электронных аукционов,
    которым определено акционерное общество «Информационно-учетный центр» в
	соответствии с <a target='_blank' href='https://traderesources.gosreestr.kz/ru/traderesources/prikaz-270'>приказом</a> и.о. Министра энергетики Республики Казахстан от 30
    июля 2020 года № 270.
</p>
<p>
    В соответствии с пунктом 7 Правил проведения аукциона с использованием
    интернет-ресурса оператора электронных аукционов на предоставление права
    недропользования по углеводородам в электронной форме, утвержденных
    <a target='_blank' href='http://adilet.zan.kz/rus/docs/V2000021038'>приказом</a> и.о. Министра энергетики Республики Казахстан от 30 июля 2020 года
    № 269, формирование, прием и обработка Заявлений состоит из трех этапов:
</p>
<p>
    1) формирование Заявления в торговой системе и отправка его в компетентный
    орган;
</p>
<p>
    2) прием и обработка компетентным органом Заявления;
</p>
<p>
    3) отображение статуса обработки Заявления в торговой системе.
</p>
<p>
    Заявление подлежит рассмотрению в течение 20 (двадцати) рабочих дней со дня
    его поступления в компетентный орган и по результатам его рассмотрения
    компетентный орган уведомляет заявителя о принятии либо об отказе
    рассмотрения заявления.
</p>
<p>
    После отправки Заявления в компетентный орган для заявителя в торговой
    системе отображается текущий статус обработки заявления компетентным
    органом (зарегистрировано, принято в работу, отказано в рассмотрении).
</p>
<p>
    По результатам рассмотрения заявления компетентный орган публикует
    извещение о проведении аукциона либо отказывает в рассмотрении заявления по
    следующим основаниям:
</p>
<p>
    1) если запрашиваемая территория участка недр не указана в Программе
    управления государственным фондом недр в качестве территории, в пределах
    которой участок недр может быть предоставлен для разведки и добычи или
    добычи углеводородов на основании аукциона, либо не соответствует такой
    территории;
</p>
<p>
    2) если в течение 3 (трех) лет до подачи заявления заявитель подавал другое
    заявление на проведение аукциона, но не зарегистрировался в качестве
    участника аукциона;
</p>
<p>
3) по основаниям, предусмотренным подпунктами <a target='_blank' href='http://adilet.zan.kz/rus/docs/K1700000125#z1549'>2)</a>,<a target='_blank' href='http://adilet.zan.kz/rus/docs/K1700000125#z1549'>3)</a>,<a target='_blank' href='http://adilet.zan.kz/rus/docs/K1700000125#z1550'>4)</a>,<a target='_blank' href='http://adilet.zan.kz/rus/docs/K1700000125#z1551'>5)</a>,<a href='http://adilet.zan.kz/rus/docs/K1700000125#z1552'>6)</a>,<a href='http://adilet.zan.kz/rus/docs/K1700000125#z1554'>8)</a> и    <a href='http://adilet.zan.kz/rus/docs/K1700000125#z1555'>9)</a> пункта 3
    статьи 97 <a target='_blank' href='http://adilet.zan.kz/rus/docs/K1700000125#z1555'>Кодекса</a> Республики Казахстан от 27 декабря 2017 года «О недрах и
    недропользовании».
</p><br/>
")));
        }
    }
}
