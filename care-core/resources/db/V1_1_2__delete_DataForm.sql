--Product Backlog Item 1737: Remover Formularios con nombre CAMPOS DESAGREGADOS
--Se actualizo el estado del registro, debido a que se filtra por estado activo 160445 el usuario
--no vera los regitros con estado desactivo 160446
UPDATE adm_form SET status_id = 160446 WHERE form_id = 3;
UPDATE adm_form SET status_id = 160446 WHERE form_id = 7;
UPDATE adm_form SET status_id = 160446 WHERE form_id = 9;
UPDATE adm_form SET status_id = 160446 WHERE form_id = 11;