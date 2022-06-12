
ALTER TABLE trades.tbtrades
ALTER COLUMN flstatus TYPE character varying(26);


ALTER TABLE trades.tbtradesrevisions
ALTER COLUMN flstatus TYPE character varying(26);


ALTER TABLE trades.z_history_tbtradesrevisions
ALTER COLUMN flstatus TYPE character varying(26);


ALTER TABLE trades.z_history_tbtrades
ALTER COLUMN flstatus TYPE character varying(26);

ALTER TABLE settings.tbSearchDefaultFieldsOrderSettings DROP CONSTRAINT "pk_ed064f58871724e7f1ee8516a4f4b2cf02cdfa07";
ALTER TABLE settings.tbSearchDefaultFieldsWidthSettings DROP CONSTRAINT "pk_f2c25c8bca80278b459e947524c8f6b486ae5fb7";
ALTER TABLE settings.tbSearchDefaultFilterFieldsSettings DROP CONSTRAINT "pk_df9a8b498a9626f3f67e4fd4c2e8dc95e1416f2f";
ALTER TABLE settings.tbSearchDefaultOutputFieldsSettings DROP CONSTRAINT "pk_af24cfe2c2dbb3e02c11b24b847af14897236a9d";
ALTER TABLE settings.tbSearchFieldsOrderSettings DROP CONSTRAINT "pk_476448a060b868428340e1e04aff0b21ce64ce71";
ALTER TABLE settings.tbSearchFieldsWidthSettings DROP CONSTRAINT "pk_6024a27e3c441ba9b6d59de9b9cc99ba60aefd9f";
ALTER TABLE settings.tbSearchFilterFieldsSettings DROP CONSTRAINT "pk_231fd5babba87e3f0f62aca8eb78d55ae73e1320";
ALTER TABLE settings.tbSearchOutputFieldsSettings DROP CONSTRAINT "pk_df994cc4be528aa825c46238b7860d326985ca5f";
ALTER TABLE yoda_system.tbAccessToReferencesRoles DROP CONSTRAINT "pk_3c9ab0d701d98f34ee054a602724e48edb72f0fa";

ALTER TABLE yoda_system.tbbanks 
RENAME TO __rm__tbbanks;

ALTER TABLE yoda_system.tbBlock DROP CONSTRAINT "pk_b01bdce7d64d18543432a1e0c9626926ff71d1b8";
ALTER TABLE yoda_system.TbCsSavedQueriesSchemas DROP CONSTRAINT "pk_8cb48a156d5298799ac25553a3ccfb58a4748389";

ALTER TABLE yoda_system.tbmrp
ALTER COLUMN flyear SET NOT NULL;

ALTER TABLE yoda_system.tbMrp DROP CONSTRAINT "pk_c61f0e29f7fa1b1767aa818b6e7d95c44b9aa3b4";
ALTER TABLE yoda_system.tbMrp ADD CONSTRAINT PK_tbMrp PRIMARY KEY (flYear);
ALTER TABLE yoda_system.tbNotWorkingDaysCalendar DROP CONSTRAINT "pk_a31c3275886a974b5ec56ef24b5233bff36ea22d";
ALTER TABLE yoda_system.tbTextsIgnore DROP CONSTRAINT "pk_17315d296aeb903a991fa5b380dc8305ede27ba3";
ALTER TABLE yoda_system.TbTextSourceCategories DROP CONSTRAINT "pk_65d7352094427eed99e57d2252ccaee461757c82";
ALTER TABLE yoda_system.TbTextSources2 DROP CONSTRAINT "pk_a6b381db47ef9b21099f873b665b2ba363fc278a";

ALTER TABLE yoda_system.tbtexttranslations2
ALTER COLUMN fllang SET NOT NULL;


ALTER TABLE yoda_system.tbtexttranslations2
ALTER COLUMN fltextid SET NOT NULL;

ALTER TABLE yoda_system.tbTextTranslations2 DROP CONSTRAINT "pk_f15dc8c427f36b7861644155fdcd1851ee7fd24a";
ALTER TABLE yoda_system.tbTextTranslations2 ADD CONSTRAINT PK_tbTextTranslations2 PRIMARY KEY (flTextId, flLang);
ALTER TABLE yoda_system.TbTextTranslationsCategories DROP CONSTRAINT "pk_5f1a4e8be8b8c205448d5520c3b7b641fc20ad29";
ALTER TABLE yoda_system.tbTranslationsAllRegistredTexts DROP CONSTRAINT "pk_1a4ce3a2a295dc958454c57305dad39a372577fc";

ALTER TABLE files.tbfilestorefiles
ALTER COLUMN flfileid TYPE character varying(60),
ALTER COLUMN flfileid SET NOT NULL;

ALTER TABLE files.tbFileStoreFiles ADD CONSTRAINT IX_tbFileStoreFiles PRIMARY KEY (flFileId);

CREATE SCHEMA dbreferences;

CREATE TABLE dbreferences.tbbanks (
flNodeId integer ,
flRevisionId integer ,
flBik character varying(8) ,
flName character varying(2048) ,
flNote character varying ,
flStatus character varying 
);