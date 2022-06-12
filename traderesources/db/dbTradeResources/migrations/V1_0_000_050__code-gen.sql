
ALTER TABLE agreements.tbpaymentmatches
ALTER COLUMN flmatchresult DROP NOT NULL;


ALTER TABLE agreements.z_history_tbpaymentmatches
ALTER COLUMN flmatchresult DROP NOT NULL;