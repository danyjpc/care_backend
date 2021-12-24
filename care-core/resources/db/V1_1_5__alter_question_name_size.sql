



--https://dev.azure.com/People-Apps/CARE/_workitems/edit/1756/
--changes column type to avoid question name sizes constraint
ALTER TABLE adm_question
ALTER COLUMN name_question TYPE TEXT;