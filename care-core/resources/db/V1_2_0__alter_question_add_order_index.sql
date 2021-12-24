
--https://dev.azure.com/People-Apps/CARE/_backlogs/backlog/CARE%20Team/Backlog%20items/?workitem=1955
--Adds order_index column
ALTER TABLE adm_question
ADD COLUMN IF NOT EXISTS order_index INTEGER NOT NULL DEFAULT 0;