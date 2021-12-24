--Se agregaron las siguientes campos a la tabla adm_organization
Alter TABLE adm_organization ADD abbreviation varchar(50) NOT NULL  DEFAULT  'S/D';
Alter TABLE adm_organization ADD url varchar(300) NOT NULL DEFAULT  'S/D';