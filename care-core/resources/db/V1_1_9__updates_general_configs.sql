
--removing previous records and restarting sequence
TRUNCATE adm_general_config RESTART IDENTITY;

--Adds new attribute description
--https://dev.azure.com/People-Apps/CARE/_workitems/edit/1809/
ALTER TABLE adm_general_config
    ADD COLUMN description TEXT NOT NULL DEFAULT '%';

--Inserting initial values
INSERT INTO adm_general_config(config_name, config_value,
                               created_by, date_created,
                               status_id, description)
VALUES('URL de imagen de portada',
       'https://sgc-mmitz.mypeopleapps.com/assets/images/backgrounds/login-bg.jpg',
       1, LOCALTIMESTAMP,
       160445, 'Imagen que aparece en la pantalla principal del sitio web.');

INSERT INTO adm_general_config(config_name, config_value,
                               created_by, date_created,
                               status_id, description)
VALUES('URL de imagenes de los logos',
       'https://care-beta.mypeopleapps.com/assets/images/backgrounds/logos-care-sgc.png',
       1, LOCALTIMESTAMP,
       160445, 'Imagen de los logos que aparecen en la pantalla principal del sitio web.');

INSERT INTO adm_general_config(config_name, config_value,
                               created_by, date_created,
                               status_id, description)
VALUES('Texto portada','Sistema Gestión de conocimiento',
       1, LOCALTIMESTAMP, 160445, 'Texto que aparece en la pantalla principal del sitio web');


--Adding disclaimer
--https://dev.azure.com/People-Apps/CARE/_workitems/edit/1808/
INSERT INTO adm_general_config(config_name, config_value,
                               created_by, date_created,
                               status_id, description)
VALUES('Descargo de responsabilidad',
       'Esta publicación se ha elaborado con financiación del Fondo Fiduciario de la ONU ' ||
       'para Eliminar la Violencia contra la Mujer; no obstante, las opiniones expresadas y el ' ||
       'contenido incluido en ella no aplican su adhesión o aceptación oficial por parte de las Naciones Unidas.',
       1, LOCALTIMESTAMP, 160445,
       'El descargo de responsabilidad aparece en la pantalla de inicio de sesion del ' ||
       'sitio web e incluye información sobre la organización, su función es proteger ' ||
       'de acciones legales contra la organización');

INSERT INTO adm_general_config(config_name, config_value,
                               created_by, date_created,
                               status_id, description)
VALUES('URL de imagen de confirmación de cambio de clave',
       'https://sgc-mmitz.mypeopleapps.com/assets/images/backgrounds/login-bg.jpg',
       1, LOCALTIMESTAMP,
       160445, 'Imagen que aparece en la pantalla al confirmar el cambio de clave de un usuario.');