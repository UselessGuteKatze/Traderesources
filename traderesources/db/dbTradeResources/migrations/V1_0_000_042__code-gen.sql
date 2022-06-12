

ALTER TABLE agreements.tbpaymentmatches 
RENAME florder TO __rm__florder;


ALTER TABLE agreements.tbpaymentmatches 
RENAME floverpaymentorder TO __rm__floverpaymentorder;


ALTER TABLE agreements.z_history_tbpaymentmatches 
RENAME florder TO __rm__florder;


ALTER TABLE agreements.z_history_tbpaymentmatches 
RENAME floverpaymentorder TO __rm__floverpaymentorder;

