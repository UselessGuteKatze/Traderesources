
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

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,2,'TRADERESOURCES-Недропользование-Создание приказов','TRADERESOURCES-Недропользование-Создание приказов','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-Subsoil"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,2,'RegistersModule','SubsoilObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,2,'RegistersModule','SubsoilObjectsOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,2,'RegistersModule','SubsoilObjectsOrders-edit-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,2,'RegistersModule','SubsoilTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,2,'RegistersModule','SubsoilTradesOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,2,'RegistersModule','SubsoilTradesOrders-edit-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,2,'RegistersModule','SubsoilAppView');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,3,'TRADERESOURCES-Недропользование-Исполнение приказов','TRADERESOURCES-Недропользование-Исполнение приказов','traderesources','internal','BySysAdmin',NULL);
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,3,'RegistersModule','SubsoilObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,3,'RegistersModule','SubsoilObjectsOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,3,'RegistersModule','SubsoilTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,3,'RegistersModule','SubsoilTradesOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,3,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,3,'RegistersModule','PaymentOrders-exec-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,4,'TRADERESOURCES-Недропользование-Согласование заявлений','TRADERESOURCES-Недропользование-Согласование заявлений','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-Subsoil"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,4,'RegistersModule','SubsoilAppView'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,4,'RegistersModule','SubsoilAppNegotiation');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,5,'TRADERESOURCES-Недропользование-Принятие заявлений','TRADERESOURCES-Недропользование-Принятие заявлений','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-Subsoil"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,5,'RegistersModule','SubsoilAppView'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,5,'RegistersModule','SubsoilAppExecute');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,6,'TRADERESOURCES-Недропользование-Создание договора','TRADERESOURCES-Недропользование-Создание договора','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-Subsoil"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,6,'RegistersModule','SubsoilAgreementCreate'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,6,'RegistersModule','SubsoilAgreementSign'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,6,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,6,'RegistersModule','PaymentOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,6,'RegistersModule','PaymentOrders-edit-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,7,'TRADERESOURCES-Земельные ресурсы-Создание приказов','TRADERESOURCES-Земельные ресурсы-Создание приказов','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-Land"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,7,'RegistersModule','LandObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,7,'RegistersModule','LandObjectsOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,7,'RegistersModule','LandObjectsOrders-edit-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,7,'RegistersModule','LandTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,7,'RegistersModule','LandTradesOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,7,'RegistersModule','LandTradesOrders-edit-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,8,'TRADERESOURCES-Земельные ресурсы-Исполнение приказов','TRADERESOURCES-Земельные ресурсы-Исполнение приказов','traderesources','internal','BySysAdmin',NULL);
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,8,'RegistersModule','LandObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,8,'RegistersModule','LandObjectsOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,8,'RegistersModule','LandObjectsOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,8,'RegistersModule','LandTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,8,'RegistersModule','LandTradesOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,8,'RegistersModule','LandTradesOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,8,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,8,'RegistersModule','PaymentOrders-exec-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,9,'TRADERESOURCES-Земельные ресурсы-Создание договора','TRADERESOURCES-Земельные ресурсы-Создание договора','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-Land"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,9,'RegistersModule','LandAgreementCreate'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,9,'RegistersModule','LandAgreementSign'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,9,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,9,'RegistersModule','PaymentOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,9,'RegistersModule','PaymentOrders-edit-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,10,'TRADERESOURCES-Охотничьи угодья-Создание приказов','TRADERESOURCES-Охотничьи угодья-Создание приказов','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-Hunt"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,10,'RegistersModule','HuntingObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,10,'RegistersModule','HuntingObjectsOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,10,'RegistersModule','HuntingObjectsOrders-edit-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,10,'RegistersModule','HuntingTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,10,'RegistersModule','HuntingTradesOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,10,'RegistersModule','HuntingTradesOrders-edit-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,11,'TRADERESOURCES-Охотничьи угодья-Исполнение приказов','TRADERESOURCES-Охотничьи угодья-Исполнение приказов','traderesources','internal','BySysAdmin',NULL);
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,11,'RegistersModule','HuntingObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,11,'RegistersModule','HuntingObjectsOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,11,'RegistersModule','HuntingTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,11,'RegistersModule','HuntingTradesOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,11,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,11,'RegistersModule','PaymentOrders-exec-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,12,'TRADERESOURCES-Охотничьи угодья-Создание договора','TRADERESOURCES-Охотничьи угодья-Создание договора','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-HuntAgreementer"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,12,'RegistersModule','HuntingAgreementCreate'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,12,'RegistersModule','HuntingAgreementSign'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,12,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,12,'RegistersModule','PaymentOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,12,'RegistersModule','PaymentOrders-edit-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,13,'TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов','TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-Fish"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,13,'RegistersModule','FishingObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,13,'RegistersModule','FishingObjectsOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,13,'RegistersModule','FishingObjectsOrders-edit-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,13,'RegistersModule','FishingTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,13,'RegistersModule','FishingTradesOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,13,'RegistersModule','FishingTradesOrders-edit-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,14,'TRADERESOURCES-Рыбохозяйственные водоёмы-Исполнение приказов','TRADERESOURCES-Рыбохозяйственные водоёмы-Исполнение приказов','traderesources','internal','BySysAdmin',NULL);
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,14,'RegistersModule','FishingObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,14,'RegistersModule','FishingObjectsOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,14,'RegistersModule','FishingTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,14,'RegistersModule','FishingTradesOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,14,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,14,'RegistersModule','PaymentOrders-exec-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,15,'TRADERESOURCES-Рыбохозяйственные водоёмы-Создание договора','TRADERESOURCES-Рыбохозяйственные водоёмы-Создание договора','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-FishAgreementer"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,15,'RegistersModule','FishingAgreementCreate'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,15,'RegistersModule','FishingAgreementSign'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,15,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,15,'RegistersModule','PaymentOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,15,'RegistersModule','PaymentOrders-edit-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,16,'TRADERESOURCES-Лесные ресурсы-Создание объектов','TRADERESOURCES-Лесные ресурсы-Создание объектов','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-ForestCreator"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,16,'RegistersModule','ForestOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,16,'RegistersModule','ForestOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,16,'RegistersModule','ForestOrders-edit-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,17,'TRADERESOURCES-Лесные ресурсы-Выставление на торги','TRADERESOURCES-Лесные ресурсы-Выставление на торги','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-Forest"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,17,'RegistersModule','ForestObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,17,'RegistersModule','ForestObjectsOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,17,'RegistersModule','ForestObjectsOrders-edit-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,17,'RegistersModule','ForestTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,17,'RegistersModule','ForestTradesOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,17,'RegistersModule','ForestTradesOrders-edit-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,18,'TRADERESOURCES-Лесные ресурсы-Исполнение приказов','TRADERESOURCES-Лесные ресурсы-Исполнение приказов','traderesources','internal','BySysAdmin',NULL);
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,18,'RegistersModule','ForestOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,18,'RegistersModule','ForestOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,18,'RegistersModule','ForestObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,18,'RegistersModule','ForestObjectsOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,18,'RegistersModule','ForestTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,18,'RegistersModule','ForestTradesOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,18,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,18,'RegistersModule','PaymentOrders-exec-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,19,'TRADERESOURCES-Лесные ресурсы-Создание договора','TRADERESOURCES-Лесные ресурсы-Создание договора','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-ForestCreator"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,19,'RegistersModule','ForestAgreementCreate'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,19,'RegistersModule','ForestAgreementSign'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,19,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,19,'RegistersModule','PaymentOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,19,'RegistersModule','PaymentOrders-edit-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,20,'TRADERESOURCES-Компетентные органы-Добавление и изменения в ресстр компетентных органов','TRADERESOURCES-Компетентные органы-Добавление и изменения в ресстр компетентных органов','traderesources','internal','BySysAdmin',NULL);
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,20,'RegistersModule','OfficialOrgOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,20,'RegistersModule','OfficialOrgOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,20,'RegistersModule','OfficialOrgOrders-edit-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,20,'RegistersModule','OfficialOrgOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,20,'RegistersModule','OfficialOrgOrders-view-object');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,21,'TRADERESOURCES-Ресурсы связи-Для операторов связи-Создание приказов','TRADERESOURCES-Ресурсы связи-Для операторов связи-Создание приказов','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-TelecomOperators"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,21,'RegistersModule','TelecomOperatorsObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,21,'RegistersModule','TelecomOperatorsObjectsOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,21,'RegistersModule','TelecomOperatorsObjectsOrders-edit-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,21,'RegistersModule','TelecomOperatorsTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,21,'RegistersModule','TelecomOperatorsTradesOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,21,'RegistersModule','TelecomOperatorsTradesOrders-edit-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,22,'TRADERESOURCES-Ресурсы связи-Для операторов связи-Исполнение приказов','TRADERESOURCES-Ресурсы связи-Для операторов связи-Исполнение приказов','traderesources','internal','BySysAdmin',NULL);
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,22,'RegistersModule','TelecomOperatorsObjectsOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,22,'RegistersModule','TelecomOperatorsObjectsOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,22,'RegistersModule','TelecomOperatorsTradesOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,22,'RegistersModule','TelecomOperatorsTradesOrders-exec-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,22,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,22,'RegistersModule','PaymentOrders-exec-order');

INSERT INTO users.tbroles (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flrolename,flroledescription,flproject,fluserprofile,flroleassigntype,fltags) VALUES
    (pg_catalog.nextval('public.seq_entity_id'), pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,23,'TRADERESOURCES-Ресурсы связи-Для операторов связи-Создание договора','TRADERESOURCES-Ресурсы связи-Для операторов связи-Создание договора','cabinetResourceSeller','external','ByAccountAdmin','["cabinetResourceSeller-TelecomOperators"]');
INSERT INTO users.tbrolepermissions (flentityid,flrequestid,flishidden,flrequesttype,flrequestdate,flrequestuserid,flroleid,flcontrollername,flpermission) VALUES
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,23,'RegistersModule','TelecomOperatorsAgreementCreate'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,23,'RegistersModule','TelecomOperatorsAgreementSign'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,23,'RegistersModule','PaymentOrders-view-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,23,'RegistersModule','PaymentOrders-create-order'),
    (pg_catalog.nextval('public.seq_entity_id'),pg_catalog.nextval('public.seq_request_id'),false,0,now(),1,23,'RegistersModule','PaymentOrders-edit-order');
