

ALTER TABLE agreements.tbpaymentmatches
ADD COLUMN flSendAmount numeric (18, 2) NOT NULL DEFAULT 0;


ALTER TABLE agreements.z_history_tbpaymentmatches
ADD COLUMN flSendAmount numeric (18, 2) NOT NULL DEFAULT 0;

