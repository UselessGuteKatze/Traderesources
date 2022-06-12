
ALTER TABLE fishing.TbSellerSigners DROP CONSTRAINT "pk_tbsellersigners";
ALTER TABLE fishing.TbSellerSigners ADD CONSTRAINT PK_TbSellerSigners PRIMARY KEY (flSellerBin, flSignerBin);

ALTER TABLE trades.tblandapplications
ALTER COLUMN flstatus TYPE character varying(9);


ALTER TABLE trades.tbtradesorderresult
ALTER COLUMN fltype TYPE character varying(6);

ALTER TABLE trades.z_history_tbtradesorderresult
ALTER COLUMN fltype TYPE character varying(6);


ALTER TABLE trades.z_history_tblandapplications
ALTER COLUMN flstatus TYPE character varying(9);


CREATE TABLE files.TbMinioFiles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flFileSrcId character varying(36) NOT NULL,
flFileHashSha256 character varying(64) NOT NULL,
flFileSize bigint NOT NULL, 

CONSTRAINT PK_TbMinioFiles PRIMARY KEY (
    flFileSrcId
)

);

CREATE UNIQUE INDEX IX_TbMinioFiles_flHash ON files.TbMinioFiles USING btree (flFileHashSha256);
