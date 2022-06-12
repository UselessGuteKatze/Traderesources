
ALTER TABLE trades.tblandapplications
ADD COLUMN flGuid character varying(36) ;


ALTER TABLE trades.tblandapplications
ADD COLUMN flArea numeric (18, 2) ;


ALTER TABLE trades.tblandapplications
ADD COLUMN flUsageAimText character varying ;


ALTER TABLE trades.z_history_tblandapplications
ADD COLUMN flGuid character varying(36) ;


ALTER TABLE trades.z_history_tblandapplications
ADD COLUMN flArea numeric (18, 2) ;


ALTER TABLE trades.z_history_tblandapplications
ADD COLUMN flUsageAimText character varying ;
