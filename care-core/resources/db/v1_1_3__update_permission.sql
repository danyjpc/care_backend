--Se agrego el atributo alias a la tabla de permisos task 1740
ALTER TABLE adm_permission ADD COLUMN alias varchar(100)    default 'S/D'  not null;

UPDATE adm_permission SET alias = 'Llenar Boletas' WHERE permission_id =1;
UPDATE adm_permission SET alias = 'Descargar Base de Datos' WHERE permission_id =2;
UPDATE adm_permission SET alias = 'Ver Estadísticas' WHERE permission_id =3;

