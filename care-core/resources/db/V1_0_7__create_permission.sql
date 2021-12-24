create table adm_permission
(
    permission_id            serial            not null
        constraint adm_permission_pk   primary key,
    name_permission          varchar(100)   default 'S/D'              not null ,
    status_id                  BIGINT       DEFAULT 160445             NOT NULL
        CONSTRAINT adm_permission_status_fk REFERENCES adm_typology,
    created_by              BIGINT                                     NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL
);
--Commets
COMMENT ON COLUMN adm_permission.permission_id is 'Id interno de registros';
COMMENT ON COLUMN adm_permission.name_permission is 'Nombre del registro';
COMMENT ON COLUMN adm_permission.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_permission.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_permission.date_created IS 'Fecha en que se creo el registro';

create table adm_user_permission
(
    user_permission_id serial  not null
        constraint adm_user_permission_pk   primary key,
    has_permissions             boolean not null     DEFAULT 'true',
    user_id            integer not null
        constraint adm_user_permission_adm_user_fk references adm_user,
    module_id          integer not null
        constraint adm_user_permission_adm_module_fk references adm_module,
    permission_id         integer not null
        constraint adm_user_permission_adm_permission_fk references adm_permission
);
--Commets
COMMENT ON COLUMN adm_user_permission.user_permission_id is 'Id interno de registros';
COMMENT ON COLUMN adm_user_permission.has_permissions is 'Campo que nos informa si el usuario tiene el permiso ativo o no';
COMMENT ON COLUMN adm_user_permission.user_id is 'LLave foranea para la relacion con user';
COMMENT ON COLUMN adm_user_permission.module_id is 'LLave foranea para la relacion con module';
COMMENT ON COLUMN adm_user_permission.permission_id is 'LLave foranea para la relacion con permission';

insert into adm_permission(name_permission, created_by)
values('fill_survey', 1),
       ('download_csv', 1),
       ('view_stats', 1);

insert into adm_user_permission(user_id, module_id, permission_id)
values (1, 1, 1),
       (1, 1, 2),
       (1, 1, 3),
       (1, 2, 1),
       (1, 2, 2),
       (1, 2, 3),
       (1, 3, 1),
       (1, 3, 2),
       (1, 3, 3),
       (1, 4, 1),
       (1, 4, 2),
       (1, 4, 3);