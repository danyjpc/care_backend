



--https://dev.azure.com/People-Apps/CARE/_workitems/edit/1763
--adds status column for changing to inactive
ALTER TABLE adm_answer
    ADD COLUMN status_id INTEGER NOT NULL DEFAULT 160445
        REFERENCES adm_typology(typology_id);