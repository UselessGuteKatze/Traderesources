using CommonSource.QueryTables;
using System;
using System.Linq;
using Yoda.Interfaces.Permissions;
using YodaApp.UsersResources;
using YodaApp.UsersResources.QueryTables;
using YodaHelpers.OrderHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.Administration {
    public class AccountActivityTypesProvider : IAccountActivityTypesProvider {
        public AccountActivityTypes[] GetAccountsActivityTypes(IQueryExecuter queryExecuter) {

            var mainOrgs = new TbOfficialOrg().Select(t => new FieldAlias[] { t.flBin, t.flActivityTypes }, queryExecuter).Select(x => {
                var bin = x.GetVal(t => t.flBin);
                var activeActivityTypes = x.Query.flActivityTypes.GetGoodVal(x.FirstRow)?.Where(y => y.From <= DateTime.Now && (DateTime.Now <= y.To || y.To == null));
                var activityTypes = activeActivityTypes.Select(y => y.Value).ToArray();
                return new AccountActivityTypes(bin, AccountType.Corporate, "cabinetResourceSeller", activityTypes);
            }).ToList();

            mainOrgs.AddRange(new FishingSource.QueryTables.Common.TbSellerSigners().Select(t => new FieldAlias[] { t.flSignerBin }, queryExecuter).Select(x => {
                var bin = x.GetVal(t => t.flSignerBin);
                return new AccountActivityTypes(bin, AccountType.Corporate, "cabinetResourceSeller", new[] { "FishAgreementer" });
            }));

            mainOrgs.AddRange(new HuntingSource.QueryTables.Common.TbSellerSigners().Select(t => new FieldAlias[] { t.flSignerBin }, queryExecuter).Select(x => {
                var bin = x.GetVal(t => t.flSignerBin);
                return new AccountActivityTypes(bin, AccountType.Corporate, "cabinetResourceSeller", new[] { "HuntAgreementer" });
            }));

            mainOrgs.AddRange(new ForestSource.QueryTables.Common.TbSellerCreators().Select(t => new FieldAlias[] { t.flCreatorBin }, queryExecuter).Select(x => {
                var bin = x.GetVal(t => t.flCreatorBin);
                return new AccountActivityTypes(bin, AccountType.Corporate, "cabinetResourceSeller", new[] { "ForestCreator" });
            }));

            var grouppedMainOrgs = mainOrgs.GroupBy(x => new { x.Xin, x.AccountType, x.Project }).Select(x => new AccountActivityTypes(x.Key.Xin, x.Key.AccountType, x.Key.Project, x.SelectMany(y => y.ActitvityTypes).Distinct().ToArray())).ToArray();

            return grouppedMainOrgs;
        }

        #region Администрирование

        public static ViewWithOrderStandartPermissions OfficialOrgOrders { get; } = new ViewWithOrderStandartPermissions("Компетентные органы", "OfficialOrgOrders");

        #endregion

        #region Недра

        public static ViewWithOrderStandartPermissions SubsoilObjectsOrders { get; } = new ViewWithOrderStandartPermissions("Недра - объекты", "SubsoilObjectsOrders");
        public static ViewWithOrderStandartPermissions SubsoilTradesOrders { get; } = new ViewWithOrderStandartPermissions("Недра - торги", "SubsoilTradesOrders");

        public static Permission SubsoilAppView { get; } = new Permission("SubsoilAppView", "Недра - просмотр заявлений");
        public static Permission SubsoilAppNegotiation { get; } = new Permission("SubsoilAppNegotiation", "Недра - согласование заявлений");
        public static Permission SubsoilAppExecute { get; } = new Permission("SubsoilAppExecute", "Недра - принятие заявлений");

        public static Permission SubsoilAgreementCreate { get; } = new Permission("SubsoilAgreementCreate", "Недра - создание договора");
        public static Permission SubsoilAgreementSign { get; } = new Permission("SubsoilAgreementSign", "Недра - подписание договора");

        #endregion

        #region Земельные ресурсы

        public static ViewWithOrderStandartPermissions LandObjectsOrders { get; } = new ViewWithOrderStandartPermissions("Земельные ресурсы - объекты", "LandObjectsOrders");
        public static ViewWithOrderStandartPermissions LandTradesOrders { get; } = new ViewWithOrderStandartPermissions("Земельные ресурсы - торги", "LandTradesOrders");

        public static Permission LandAgreementCreate { get; } = new Permission("LandAgreementCreate", "Земельные ресурсы - создание договора");
        public static Permission LandAgreementSign { get; } = new Permission("LandAgreementSign", "Земельные ресурсы - подписание договора");

        #endregion

        #region Охота

        public static ViewWithOrderStandartPermissions HuntingObjectsOrders { get; } = new ViewWithOrderStandartPermissions("Охотничьи угодья - объекты", "HuntingObjectsOrders");
        public static ViewWithOrderStandartPermissions HuntingTradesOrders { get; } = new ViewWithOrderStandartPermissions("Охотничьи угодья - торги", "HuntingTradesOrders");

        public static Permission HuntingAgreementCreate { get; } = new Permission("HuntingAgreementCreate", "Охотничьи угодья - создание договора");
        public static Permission HuntingAgreementSign { get; } = new Permission("HuntingAgreementSign", "Охотничьи угодья - подписание договора");

        #endregion

        #region Водоёмы

        public static ViewWithOrderStandartPermissions FishingObjectsOrders { get; } = new ViewWithOrderStandartPermissions("Водоёмы - объекты", "FishingObjectsOrders");
        public static ViewWithOrderStandartPermissions FishingTradesOrders { get; } = new ViewWithOrderStandartPermissions("Водоёмы - торги", "FishingTradesOrders");

        public static Permission FishingAgreementCreate { get; } = new Permission("FishingAgreementCreate", "Водоёмы - создание договора");
        public static Permission FishingAgreementSign { get; } = new Permission("FishingAgreementSign", "Водоёмы - подписание договора");

        #endregion

        #region Лес

        public static ViewWithOrderStandartPermissions ForestOrders { get; } = new ViewWithOrderStandartPermissions("Лес - хозяйства, кварталы, выделы", "ForestOrders");
        public static ViewWithOrderStandartPermissions ForestObjectsOrders { get; } = new ViewWithOrderStandartPermissions("Лес - объекты", "ForestObjectsOrders");
        public static ViewWithOrderStandartPermissions ForestTradesOrders { get; } = new ViewWithOrderStandartPermissions("Лес - торги", "ForestTradesOrders");

        public static Permission ForestAgreementCreate { get; } = new Permission("ForestAgreementCreate", "Лес - создание договора");
        public static Permission ForestAgreementSign { get; } = new Permission("ForestAgreementSign", "Лес - подписание договора");

        #endregion

        #region Операторы

        public static ViewWithOrderStandartPermissions TelecomOperatorsObjectsOrders { get; } = new ViewWithOrderStandartPermissions("РЧС Операторы - объекты", "TelecomOperatorsObjectsOrders");
        public static ViewWithOrderStandartPermissions TelecomOperatorsTradesOrders { get; } = new ViewWithOrderStandartPermissions("РЧС Операторы - торги", "TelecomOperatorsTradesOrders");

        public static Permission TelecomOperatorsAgreementCreate { get; } = new Permission("TelecomOperatorsAgreementCreate", "РЧС Операторы - создание договора");
        public static Permission TelecomOperatorsAgreementSign { get; } = new Permission("TelecomOperatorsAgreementSign", "РЧС Операторы - подписание договора");

        #endregion

        #region Платежи

        public static ViewWithOrderStandartPermissions PaymentOrders { get; } = new ViewWithOrderStandartPermissions("Привязка платежей", "PaymentOrders");

        #endregion


        public static string Roles() {

            var result = @"
DELETE FROM users.tbroles;
DELETE FROM users.tbrolepermissions;

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,0,'Внешний пользователь', 'Автоматически назначаемая роль всем внешним пользователям.', 'cabinet', 'external', 'Auto', NULL),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,1,'Кабинет пользователя','Кабинет пользователя','cabinetResourceUser','external','Auto',NULL);
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,0,'CabinetUserBankDetails','user-bank-details'),
	(pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,0,'CabinetUserDataModule','access-to-self-data-edit'),
	(pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,0,'CabinetStartPageModule','cabinet'),
	(pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,1,'CabinetUserBankDetails','user-bank-details'),
	(pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,1,'TradePreApplication','trade-resources-user'),
	(pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,1,'CabinetUserDataModule','access-to-self-data-edit'),
	(pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,1,'CabinetStartPageModule','cabinet');
";

            var roleSeq = 1;

            #region Недра

            result += new RolePerms(++roleSeq, "Недропользование", "Создание приказов", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-Subsoil" }, new[] {
                SubsoilObjectsOrders.ViewOrder,
                SubsoilObjectsOrders.CreateOrder,
                SubsoilObjectsOrders.EditOrder,

                SubsoilTradesOrders.ViewOrder,
                SubsoilTradesOrders.CreateOrder,
                SubsoilTradesOrders.EditOrder,

                SubsoilAppView,
            });

            result += new RolePerms(++roleSeq, "Недропользование", "Исполнение приказов", "traderesources", "internal", "BySysAdmin", null, new[] {
                SubsoilObjectsOrders.ViewOrder,
                SubsoilObjectsOrders.ExecOrder,

                SubsoilTradesOrders.ViewOrder,
                SubsoilTradesOrders.ExecOrder,

                PaymentOrders.ViewOrder,
                PaymentOrders.ExecOrder,
            });

            result += new RolePerms(++roleSeq, "Недропользование", "Согласование заявлений", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-Subsoil" }, new[] {
                SubsoilAppView,
                SubsoilAppNegotiation
            });

            result += new RolePerms(++roleSeq, "Недропользование", "Принятие заявлений", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-Subsoil" }, new[] {
                SubsoilAppView,
                SubsoilAppExecute
            });

            result += new RolePerms(++roleSeq, "Недропользование", "Создание договора", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-Subsoil" }, new[] {
                SubsoilAgreementCreate,
                SubsoilAgreementSign,

                PaymentOrders.ViewOrder,
                PaymentOrders.CreateOrder,
                PaymentOrders.EditOrder,
            });

            #endregion

            #region Земельные ресурсы
            
            result += new RolePerms(++roleSeq, "Земельные ресурсы", "Создание приказов", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-Land" }, new[] {
                LandObjectsOrders.ViewOrder,
                LandObjectsOrders.CreateOrder,
                LandObjectsOrders.EditOrder,

                LandTradesOrders.ViewOrder,
                LandTradesOrders.CreateOrder,
                LandTradesOrders.EditOrder,
            });

            result += new RolePerms(++roleSeq, "Земельные ресурсы", "Исполнение приказов", "traderesources", "internal", "BySysAdmin", null, new[] {
                LandObjectsOrders.ViewOrder,
                LandObjectsOrders.CreateOrder,
                LandObjectsOrders.ExecOrder,

                LandTradesOrders.ViewOrder,
                LandTradesOrders.CreateOrder,
                LandTradesOrders.ExecOrder,

                PaymentOrders.ViewOrder,
                PaymentOrders.ExecOrder,
            });

            result += new RolePerms(++roleSeq, "Земельные ресурсы", "Создание договора", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-Land" }, new[] {
                LandAgreementCreate,
                LandAgreementSign,

                PaymentOrders.ViewOrder,
                PaymentOrders.CreateOrder,
                PaymentOrders.EditOrder,
            });

            #endregion

            #region Охота

            result += new RolePerms(++roleSeq, "Охотничьи угодья", "Создание приказов", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-Hunt" }, new[] {
                HuntingObjectsOrders.ViewOrder,
                HuntingObjectsOrders.CreateOrder,
                HuntingObjectsOrders.EditOrder,

                HuntingTradesOrders.ViewOrder,
                HuntingTradesOrders.CreateOrder,
                HuntingTradesOrders.EditOrder,
            });

            result += new RolePerms(++roleSeq, "Охотничьи угодья", "Исполнение приказов", "traderesources", "internal", "BySysAdmin", null, new[] {
                HuntingObjectsOrders.ViewOrder,
                HuntingObjectsOrders.ExecOrder,

                HuntingTradesOrders.ViewOrder,
                HuntingTradesOrders.ExecOrder,

                PaymentOrders.ViewOrder,
                PaymentOrders.ExecOrder,
            });

            result += new RolePerms(++roleSeq, "Охотничьи угодья", "Создание договора", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-HuntAgreementer" }, new[] {
                HuntingAgreementCreate,
                HuntingAgreementSign,

                PaymentOrders.ViewOrder,
                PaymentOrders.CreateOrder,
                PaymentOrders.EditOrder,
            });

            #endregion

            #region Водоёмы

            result += new RolePerms(++roleSeq, "Рыбохозяйственные водоёмы", "Создание приказов", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-Fish" }, new[] {
                FishingObjectsOrders.ViewOrder,
                FishingObjectsOrders.CreateOrder,
                FishingObjectsOrders.EditOrder,

                FishingTradesOrders.ViewOrder,
                FishingTradesOrders.CreateOrder,
                FishingTradesOrders.EditOrder,
            });

            result += new RolePerms(++roleSeq, "Рыбохозяйственные водоёмы", "Исполнение приказов", "traderesources", "internal", "BySysAdmin", null, new[] {
                FishingObjectsOrders.ViewOrder,
                FishingObjectsOrders.ExecOrder,

                FishingTradesOrders.ViewOrder,
                FishingTradesOrders.ExecOrder,

                PaymentOrders.ViewOrder,
                PaymentOrders.ExecOrder,
            });

            result += new RolePerms(++roleSeq, "Рыбохозяйственные водоёмы", "Создание договора", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-FishAgreementer" }, new[] {
                FishingAgreementCreate,
                FishingAgreementSign,

                PaymentOrders.ViewOrder,
                PaymentOrders.CreateOrder,
                PaymentOrders.EditOrder,
            });

            #endregion

            #region Лес

            result += new RolePerms(++roleSeq, "Лесные ресурсы", "Создание объектов", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-ForestCreator" }, new[] {
                ForestOrders.ViewOrder,
                ForestOrders.CreateOrder,
                ForestOrders.EditOrder,
            });

            result += new RolePerms(++roleSeq, "Лесные ресурсы", "Выставление на торги", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-Forest" }, new[] {
                ForestObjectsOrders.ViewOrder,
                ForestObjectsOrders.CreateOrder,
                ForestObjectsOrders.EditOrder,

                ForestTradesOrders.ViewOrder,
                ForestTradesOrders.CreateOrder,
                ForestTradesOrders.EditOrder,
            });

            result += new RolePerms(++roleSeq, "Лесные ресурсы", "Исполнение приказов", "traderesources", "internal", "BySysAdmin", null, new[] {
                ForestOrders.ViewOrder,
                ForestOrders.ExecOrder,

                ForestObjectsOrders.ViewOrder,
                ForestObjectsOrders.ExecOrder,

                ForestTradesOrders.ViewOrder,
                ForestTradesOrders.ExecOrder,

                PaymentOrders.ViewOrder,
                PaymentOrders.ExecOrder,
            });

            result += new RolePerms(++roleSeq, "Лесные ресурсы", "Создание договора", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-ForestCreator" }, new[] {
                ForestAgreementCreate,
                ForestAgreementSign,

                PaymentOrders.ViewOrder,
                PaymentOrders.CreateOrder,
                PaymentOrders.EditOrder,
            });

            #endregion

            #region Администрирование

            result += new RolePerms(++roleSeq, "Компетентные органы", "Добавление и изменения в ресстр компетентных органов", "traderesources", "internal", "BySysAdmin", null, new[] {
                OfficialOrgOrders.ViewOrder,
                OfficialOrgOrders.CreateOrder,
                OfficialOrgOrders.EditOrder,
                OfficialOrgOrders.ExecOrder,
                OfficialOrgOrders.ViewObject,
            });

            #endregion

            #region Операторы

            result += new RolePerms(++roleSeq, "Ресурсы связи-Для операторов связи", "Создание приказов", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-TelecomOperators" }, new[] {
                TelecomOperatorsObjectsOrders.ViewOrder,
                TelecomOperatorsObjectsOrders.CreateOrder,
                TelecomOperatorsObjectsOrders.EditOrder,

                TelecomOperatorsTradesOrders.ViewOrder,
                TelecomOperatorsTradesOrders.CreateOrder,
                TelecomOperatorsTradesOrders.EditOrder,
            });

            result += new RolePerms(++roleSeq, "Ресурсы связи-Для операторов связи", "Исполнение приказов", "traderesources", "internal", "BySysAdmin", null, new[] {
                TelecomOperatorsObjectsOrders.ViewOrder,
                TelecomOperatorsObjectsOrders.ExecOrder,

                TelecomOperatorsTradesOrders.ViewOrder,
                TelecomOperatorsTradesOrders.ExecOrder,

                PaymentOrders.ViewOrder,
                PaymentOrders.ExecOrder,
            });

            result += new RolePerms(++roleSeq, "Ресурсы связи-Для операторов связи", "Создание договора", "cabinetResourceSeller", "external", "ByAccountAdmin", new[] { "cabinetResourceSeller-TelecomOperators" }, new[] {
                TelecomOperatorsAgreementCreate,
                TelecomOperatorsAgreementSign,

                PaymentOrders.ViewOrder,
                PaymentOrders.CreateOrder,
                PaymentOrders.EditOrder,
            });

            #endregion

            return result;
        }

        public class RolePerms {
            public RolePerms(int id, string project, string name, string subDomain, string userType, string assignType, string[] activityTypes, Permission[] permissions) {
                Id = id;
                Project = project;
                Name = name;
                SubDomain = subDomain;
                UserType = userType;
                AssignType = assignType;
                ActivityTypes = activityTypes;
                Permissions = permissions;
            }
            public override string ToString() {
                return $@"
INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,{Id},'TRADERESOURCES-{Project}-{Name}','TRADERESOURCES-{Project}-{Name}','{SubDomain}','{UserType}','{AssignType}',{(
    (ActivityTypes == null || ActivityTypes.Length == 0)
    ?
    "NULL"
    :
    $"'[{string.Join(",", ActivityTypes.Select(activityType => $"\"{activityType}\""))}]'")
    });{(
    !(Permissions != null && Permissions.Length > 0)
    ?
    ""
    :
    $@"
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    {string.Join(@",
    ", Permissions.Select(perm => $"(pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,{Id},'RegistersModule','{perm.Name}')"))};
"
)}";
            }
            public int Id { get; set; }
            public string Project { get; set; }
            public string Name { get; set; }
            public string SubDomain { get; set; }
            public string UserType { get; set; }
            public string AssignType { get; set; }
            public string[] ActivityTypes { get; set; }
            public Permission[] Permissions { get; set; }
        }
    }
}