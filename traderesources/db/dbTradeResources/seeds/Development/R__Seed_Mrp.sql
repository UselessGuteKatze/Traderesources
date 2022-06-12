DELETE FROM yoda_system.tbmrp;

INSERT INTO yoda_system.tbmrp VALUES (1278616, 258723521, false, 0, ((EXTRACT(year from now())-1)::text || '-01-01 01:00:01.000')::timestamp without time zone, 1, EXTRACT(year from now())-1, 2778.00, ((EXTRACT(year from now())-1)::text || '-01-01')::date);
INSERT INTO yoda_system.tbmrp VALUES (1278617, 258723522, false, 0, (EXTRACT(year from now())::text || '-01-01 01:00:01.000')::timestamp without time zone, 1, EXTRACT(year from now()), 2917.00, (EXTRACT(year from now())::text || '-01-01')::date);
INSERT INTO yoda_system.tbmrp VALUES (1278618, 258723523, false, 0, ((EXTRACT(year from now())+1)::text || '-01-01 01:00:01.000')::timestamp without time zone, 1, EXTRACT(year from now())+1, 3063.00, ((EXTRACT(year from now())+1)::text || '-01-01')::date);