

CREATE TABLE IF NOT EXISTS adm_survey
(
    survey_id       SERIAL    NOT NULL PRIMARY KEY,
    form_id         BIGINT    NOT NULL REFERENCES adm_form(form_id),
    status_id       BIGINT    DEFAULT 160445 NOT NULL REFERENCES adm_typology(typology_id),
    created_by      BIGINT    DEFAULT 0 NOT NULL,
    date_created    TIMESTAMP DEFAULT '1900-01-01 00:00:00'::TIMESTAMP WITHOUT TIME ZONE
    );

ALTER TABLE adm_answer DROP COLUMN form_id;
ALTER TABLE adm_answer ADD COLUMN survey_id INT REFERENCES adm_survey(survey_id);
