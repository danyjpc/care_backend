


--Remove created_by from adm_user, causes a stack overflow on the model
ALTER TABLE adm_user DROP COLUMN created_by;
ALTER TABLE adm_person DROP COLUMN created_by;