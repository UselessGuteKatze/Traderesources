

ALTER TABLE agreements.tbpaymentmatches 
RENAME fliban TO __rm__fliban; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE agreements.tbpaymentmatches 
RENAME flbik TO __rm__flbik; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE agreements.tbpaymentmatches 
RENAME flkbe TO __rm__flkbe; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE agreements.tbpaymentmatches 
RENAME flknp TO __rm__flknp; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE agreements.tbpaymentmatches 
RENAME flkbk TO __rm__flkbk; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE agreements.tbpaymentmatches
ADD COLUMN flRequisites character varying ;


ALTER TABLE agreements.tbpaymentmatches
ADD COLUMN flOverpaymentRequisites character varying ;


ALTER TABLE agreements.z_history_tbpaymentmatches 
RENAME flkbk TO __rm__flkbk; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE agreements.z_history_tbpaymentmatches 
RENAME fliban TO __rm__fliban; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE agreements.z_history_tbpaymentmatches 
RENAME flkbe TO __rm__flkbe; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE agreements.z_history_tbpaymentmatches 
RENAME flbik TO __rm__flbik; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE agreements.z_history_tbpaymentmatches 
RENAME flknp TO __rm__flknp; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE agreements.z_history_tbpaymentmatches
ADD COLUMN flRequisites character varying ;


ALTER TABLE agreements.z_history_tbpaymentmatches
ADD COLUMN flOverpaymentRequisites character varying ;

