
ALTER TABLE trades.tblandobjectstrades 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE trades.tblandobjectstradesrevisions 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE trades.z_history_tblandobjectstradesrevisions 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE trades.z_history_tblandobjectstrades 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE fishing.tbtrades 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE fishing.tbtrades
ADD COLUMN flWinnerData character varying ;


ALTER TABLE fishing.tbtradesrevisions 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE fishing.tbtradesrevisions
ADD COLUMN flWinnerData character varying ;


ALTER TABLE fishing.z_history_tbtrades 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE fishing.z_history_tbtrades
ADD COLUMN flWinnerData character varying ;


ALTER TABLE fishing.z_history_tbtradesrevisions 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE fishing.z_history_tbtradesrevisions
ADD COLUMN flWinnerData character varying ;


ALTER TABLE hunting.tbtrades 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE hunting.tbtradesrevisions 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE hunting.z_history_tbtradesrevisions 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE hunting.z_history_tbtrades 
RENAME flpostcontrol TO __rm__flpostcontrol; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.
