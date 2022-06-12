

ALTER TABLE agreements.tbpaymentmatches
ADD COLUMN flGuaranteeAmount numeric (18, 2) NOT NULL DEFAULT 0;


ALTER TABLE agreements.tbpaymentmatches
ADD COLUMN flHasSendAmount boolean NOT NULL DEFAULT true;


ALTER TABLE agreements.tbpaymentmatches
ADD COLUMN flSendOverpayment boolean NOT NULL DEFAULT false;


ALTER TABLE agreements.tbpaymentmatches
ADD COLUMN flOverpaymentSendAmount numeric (18, 2) ;


ALTER TABLE agreements.z_history_tbpaymentmatches
ADD COLUMN flGuaranteeAmount numeric (18, 2) NOT NULL DEFAULT 0;


ALTER TABLE agreements.z_history_tbpaymentmatches
ADD COLUMN flHasSendAmount boolean NOT NULL DEFAULT true;


ALTER TABLE agreements.z_history_tbpaymentmatches
ADD COLUMN flSendOverpayment boolean NOT NULL DEFAULT false;


ALTER TABLE agreements.z_history_tbpaymentmatches
ADD COLUMN flOverpaymentSendAmount numeric (18, 2) ;

