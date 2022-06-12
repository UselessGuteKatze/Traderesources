
ALTER TABLE fishing.tbtradesorderresult
ALTER COLUMN fltype TYPE character varying(8);


ALTER TABLE fishing.z_history_tbtradesorderresult
ALTER COLUMN fltype TYPE character varying(8);


ALTER TABLE trades.tblandobjectstradesorderresult
ALTER COLUMN fltype TYPE character varying(8);


ALTER TABLE trades.z_history_tblandobjectstradesorderresult
ALTER COLUMN fltype TYPE character varying(8);


ALTER TABLE agreements.tbagreements
ALTER COLUMN fltradetype TYPE character varying(100);


ALTER TABLE agreements.tbagreements
ALTER COLUMN flobjecttype TYPE character varying(100);


ALTER TABLE agreements.tbagreements
ALTER COLUMN flagreementtype TYPE character varying(100);


ALTER TABLE agreements.z_history_tbagreements
ALTER COLUMN flagreementtype TYPE character varying(100);


ALTER TABLE agreements.z_history_tbagreements
ALTER COLUMN fltradetype TYPE character varying(100);


ALTER TABLE agreements.z_history_tbagreements
ALTER COLUMN flobjecttype TYPE character varying(100);


ALTER TABLE yoda_system.tbmrp
ALTER COLUMN flmrp SET NOT NULL;


ALTER TABLE yoda_system.tbmrp
ADD COLUMN flDate date;

with mrp as(
	SELECT * FROM (VALUES
		(2017, '2017-01-01'::date, 2269),
		(2018, '2018-01-01'::date, 2405),
		(2019, '2019-01-01'::date, 2525),
		(2020, '2020-01-01'::date, 2651),
		(2020, '2020-04-01'::date, 2778),
		(2021, '2021-01-01'::date, 2917),
		(2022, '2022-01-01'::date, 3063)
	) as t(flyear, fldate, flmrp)
)
UPDATE yoda_system.tbmrp
SET flDate = (SELECT fldate FROM mrp WHERE mrp.flyear = tbmrp.flyear AND mrp.flmrp = tbmrp.flmrp);

ALTER TABLE yoda_system.tbmrp
ALTER COLUMN flDate SET NOT NULL;


ALTER TABLE yoda_system.tbMrp DROP CONSTRAINT "pk_tbmrp";
ALTER TABLE yoda_system.tbMrp ADD CONSTRAINT PK_tbMrp PRIMARY KEY (flDate);
ALTER TABLE yoda_system.tbMrp ADD CONSTRAINT ch_tbmrp_opt_lock CHECK (flyear = DATE_PART('year', fldate)::integer);


ALTER TABLE yoda_system.z_history_tbmrp
ADD COLUMN flDate date;

with mrp as(
	SELECT * FROM (VALUES
		(2017, '2017-01-01'::date, 2269),
		(2018, '2018-01-01'::date, 2405),
		(2019, '2019-01-01'::date, 2525),
		(2020, '2020-01-01'::date, 2651),
		(2020, '2020-04-01'::date, 2778),
		(2021, '2021-01-01'::date, 2917),
		(2022, '2022-01-01'::date, 3063)
	) as t(flyear, fldate, flmrp)
)
UPDATE yoda_system.z_history_tbmrp
SET flDate = (SELECT fldate FROM mrp WHERE mrp.flyear = z_history_tbmrp.flyear AND mrp.flmrp = z_history_tbmrp.flmrp);

ALTER TABLE yoda_system.z_history_tbmrp
ALTER COLUMN flDate SET NOT NULL;


ALTER TABLE hunting.tbsellersigners
ALTER COLUMN flsignerbin SET NOT NULL;


ALTER TABLE hunting.tbtradesorderresult
ALTER COLUMN fltype TYPE character varying(8);


ALTER TABLE hunting.z_history_tbtradesorderresult
ALTER COLUMN fltype TYPE character varying(8);


ALTER TABLE users.tbaccounts
ADD COLUMN flTags character varying ;


ALTER TABLE users.tbroles
ADD COLUMN flRoleAssignType character varying(14);


ALTER TABLE users.z_history_tbroles
ADD COLUMN flRoleAssignType character varying(14);

update users.tbroles set flRoleAssignType = case
    when flautoassigned = true then 'Auto' else 'BySysAdmin'
end;

update users.z_history_tbroles set flRoleAssignType = case
    when flautoassigned = true then 'Auto' else 'BySysAdmin'
end;

ALTER TABLE users.tbroles
ALTER COLUMN flRoleAssignType SET NOT NULL;


ALTER TABLE users.z_history_tbroles
ALTER COLUMN flRoleAssignType SET NOT NULL;


ALTER TABLE users.tbroles 
RENAME flautoassigned TO __rm__flautoassigned; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE users.tbroles
ADD COLUMN flTags character varying ;


ALTER TABLE users.z_history_tbaccounts
ADD COLUMN flTags character varying ;


ALTER TABLE users.z_history_tbroles 
RENAME flautoassigned TO __rm__flautoassigned; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE users.z_history_tbroles
ADD COLUMN flTags character varying ;
