



--https://dev.azure.com/People-Apps/CARE/_workitems/edit/1781/
--Creates general configuration
CREATE TABLE adm_general_config
(
    config_id    SERIAL       NOT NULL PRIMARY KEY,
    config_name  VARCHAR(200) NOT NULL DEFAULT '%',
    config_value TEXT         NOT NULL DEFAULT '%',
    created_by   BIGINT       NOT NULL REFERENCES adm_user(user_id),
    date_created TIMESTAMP    NOT NULL DEFAULT '1900-01-01 00:00:00'::timestamp without time zone,
    status_id    INTEGER      NOT NULL DEFAULT 160445  REFERENCES adm_typology(typology_id)
);

--Inserting initial values
INSERT INTO adm_general_config(config_name, config_value, created_by, date_created, status_id)
VALUES('url_de_imagen_de_portada','https://sgc-mmitz.mypeopleapps.com/assets/images/backgrounds/login-bg.jpg', 1, LOCALTIMESTAMP, 160445);

INSERT INTO adm_general_config(config_name, config_value, created_by, date_created, status_id)
VALUES('url_logos','https://care-beta.mypeopleapps.com/assets/images/backgrounds/logos-care-sgc.png',1, LOCALTIMESTAMP, 160445);

INSERT INTO adm_general_config(config_name, config_value, created_by, date_created, status_id)
VALUES('texto_portada','Sistema Gestión de conocimiento',1, LOCALTIMESTAMP, 160445);


--https://dev.azure.com/people-apps/CARE/_workitems/edit/1786
--Adds new field to adm_form
ALTER TABLE adm_form ADD is_public BOOLEAN  NOT NULL DEFAULT FALSE;