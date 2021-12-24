

create table adm_module
(
    module_id   serial                                     not null
        constraint adm_module_pk    primary key,
    name_module varchar(100) default ''::character varying not null,
    icon        varchar(100) default ''::character varying not null,
    picture     varchar(350) default ''::character varying not null,
    status_id                  BIGINT       DEFAULT 160445             NOT NULL
        CONSTRAINT adm_module_status_fk REFERENCES adm_typology,
    created_by              BIGINT                                     NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL
);
--Commets
COMMENT ON COLUMN adm_module.module_id is 'Id interno de registros';
COMMENT ON COLUMN adm_module.name_module is 'Nombre del modulo';
COMMENT ON COLUMN adm_module.icon is 'Icono que representa el modulo';
COMMENT ON COLUMN adm_module.picture is 'Imagen del fondo para el modulo';
COMMENT ON COLUMN adm_module.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_module.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_module.date_created IS 'Fecha en que se creo el registro';


create table adm_category
(
    category_id   serial                                     not null
        constraint adm_category_form_pk primary key,
    name_category varchar(100) default 'S/D'::character varying not null,
    icon          varchar(100) default 'S/D'::character varying not null,
    color         varchar(100) default 'blue'::character varying not null,
    status_id                  BIGINT       DEFAULT 160445             NOT NULL
        CONSTRAINT adm_category_status_fk REFERENCES adm_typology,
    created_by              BIGINT                                      NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL
);
--Commets
COMMENT ON COLUMN adm_category.category_id is 'Id interno de registros';
COMMENT ON COLUMN adm_category.name_category is 'Nombre de la categoria';
COMMENT ON COLUMN adm_category.icon is 'Icono de la categoria';
COMMENT ON COLUMN adm_category.color is 'Color de la categoria';
COMMENT ON COLUMN adm_category.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_category.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_category.date_created IS 'Fecha en que se creo el registro';

create table adm_module_category
(
    module_category_id serial  not null
        constraint adm_module_category_pk
            primary key,
    module_id          integer not null
        constraint adm_module_category_adm_module_fk references adm_module,
    category_id        integer not null
        constraint adm_module_category_adm_category_fk references adm_category
);
--Commets
COMMENT ON COLUMN adm_module_category.module_category_id is 'Id interno de registros';
COMMENT ON COLUMN adm_module_category.module_id is 'LLave foranea para la relacion con module ';
COMMENT ON COLUMN adm_module_category.category_id is 'LLave foranea para la relacion con category';

create table adm_form
(
    form_id            serial            not null
        constraint admForm_pk   primary key,
    name_form          varchar(250) default 'S/D'   not null ,
    module_category_id integer           not null
        constraint adm_form_adm_module_category_fk references adm_module_category,
    status_id                  BIGINT       DEFAULT 160445             NOT NULL
        CONSTRAINT adm_form_status_fk REFERENCES adm_typology,
    created_by              BIGINT                                     NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL
);
--Commets
COMMENT ON COLUMN adm_form.form_id is 'Id interno de registros';
COMMENT ON COLUMN adm_form.name_form is 'Nombre del registro';
COMMENT ON COLUMN adm_form.module_category_id is 'Identifica a que module_category pertenece el registro';
COMMENT ON COLUMN adm_form.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_form.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_form.date_created IS 'Fecha en que se creo el registro';

create table adm_group
(
    group_id serial                                     not null
        constraint adm_group_pk primary key,
    name_group        varchar(150) default 'S/D'::character varying not null,
    status_id                  BIGINT       DEFAULT 160445             NOT NULL
        CONSTRAINT adm_goup_status_fk REFERENCES adm_typology,
    created_by              BIGINT                                     NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL,
    form_id     BIGINT
        CONSTRAINT adm_group_form_fk REFERENCES adm_form
);
--Commets
COMMENT ON COLUMN adm_group.group_id is 'Id interno de registros';
COMMENT ON COLUMN adm_group.name_group is 'Nomnre del registro';
COMMENT ON COLUMN adm_group.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_group.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_group.date_created IS 'Fecha en que se creo el registro';
COMMENT ON COLUMN adm_group.form_id IS 'Formulario al que pertenece el grupo';

create table adm_question
(
    question_id          serial                                         not null
        constraint admQuestion_pk   primary key,
    name_question        varchar(200)   default 'S/D'   not null,
    type                 varchar(100)   default 'text'  not null,
    use_custom_option        boolean        DEFAULT TRUE    not null,
    use_for_counter          boolean        DEFAULT FALSE    not null,
    group_id integer                        not null
        constraint adm_question_group_fk references adm_group,
    typology_id         bigint       default 160000 not null
        constraint adm_question_typology_fk references adm_typology,
    status_id               BIGINT       DEFAULT 160445             NOT NULL
        CONSTRAINT adm_question_status_fk REFERENCES adm_typology,
    created_by              BIGINT                                     NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL
);
--Commets
COMMENT ON COLUMN adm_question.question_id is 'Id interno de registros';
COMMENT ON COLUMN adm_question.name_question is 'Nombre del registro';
COMMENT ON COLUMN adm_question.type is 'Identifica de que tipo es el registro, text, boolean, etc..';
COMMENT ON COLUMN adm_question.use_custom_option is 'Se utlizado para definir si el usuario quiere utlizar opciones definidas 
        por el o por el sistema cuando la pregunta es de tipo select. 
        Si es false usar las del sistema y si es true las opciones propias. 
        Valor por default es true.
        Si es true la tipologia es 160000
        Si es false la tipologia que escoja.';
COMMENT ON COLUMN adm_question.use_for_counter is 'Identifica que pregunta tiene un counter';
COMMENT ON COLUMN adm_question.group_id is 'LLave foranea para saber a que grupo pertenece la pregunta';
COMMENT ON COLUMN adm_question.typology_id is 'Este campo se utlizara cuando el usuario quiere usar las tipologias';
COMMENT ON COLUMN adm_question.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_question.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_question.date_created IS 'Fecha en que se creo el registro';


create  table adm_option
(
    option_id serial    not null,
    value   varchar(200)    default 'S/D',
    question_id  integer    not null
        constraint adm_option_question_fk  references adm_question,
    status_id               BIGINT       DEFAULT 160445             NOT NULL
        CONSTRAINT adm_option_status_fk REFERENCES adm_typology,
    created_by              BIGINT                                     NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL
);
COMMENT ON COLUMN adm_option.option_id is 'Id interno de registros';
COMMENT ON COLUMN adm_option.value is 'Valor que tentra el regitro';
COMMENT ON COLUMN adm_option.question_id is 'Identifica a que pregunta pertenece el registro';
COMMENT ON COLUMN adm_option.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_option.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_option.date_created IS 'Fecha en que se creo el registro';

create table adm_answer
(
    answer_id    serial not null
        constraint adm_answer_pk
            primary key,
    form_id      integer    not null
        constraint adm_answer_form_id_fk    references adm_form,
    question_id  integer    not null
        constraint adm_answer_question_id_fk  references adm_question,
    answer       text,
    created_by              BIGINT                                     NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL
);
COMMENT ON COLUMN adm_answer.answer_id is 'Id interno de registros';
COMMENT ON COLUMN adm_answer.form_id is 'Identifica a que formulario pertenece el registro';
COMMENT ON COLUMN adm_answer.question_id is 'Identifica a que pregunta pertenece el registro';
COMMENT ON COLUMN adm_answer.answer is 'Guarda la respuesta';
COMMENT ON COLUMN adm_answer.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_answer.date_created IS 'Fecha en que se creo el registro';

INSERT INTO adm_module (name_module, icon, picture, status_id, created_by, date_created)
VALUES ('spotlight', 'assignment', 'http://www.care.org.gt/images/web/quienessomos.jpg', 160445, 1, '2021/07/15'),
       ('Observatorios', 'file_copy', 'http://www.care.org.gt/images/noticias/tejiendo_alianzas.jpg', 160445, 1, '2021/07/15'),
       ('MMITZ', 'groups', 'http://www.care.org.gt/images/01.jpg', 160445, 1, '2021/07/15'),
       ('Indicadores', 'poll', 'http://www.care.org.gt/images/noticias/Learningtour_01.jpg', 160445, 1, '2021/07/15');

INSERT INTO adm_category (name_category, icon, color, status_id, created_by, date_created)
VALUES ('Fortalecimiento Lideresas', 'people', '#f396b8', 160445, 1, '2021/07/15'),
       ('Ambito Educativo','auto_stories', '#a8c6fa', 160445, 1, '2021/07/15'),
       ('Autoridades Ancestrales','face', '#ffc3ae', 160445, 1, '2021/07/15'),
       ('Visibilidad e incidencia','pageview', '#dacafb', 160445, 1, '2021/07/15'),
       ('Categoria default','visibility', '#8ccddc', 160445, 1, '2021/07/15' ),
       ('Categoria default','visibility', '#8ccddc', 160445, 1, '2021/07/15' ),
       ('Categoria default','visibility', '#8ccddc', 160445, 1, '2021/07/15' );
       

INSERT INTO adm_module_category ( module_id, category_id) VALUES (1, 1),
                                                                 (1, 2),
                                                                 (1, 3),
                                                                 (1, 4),
                                                                 (2, 5),
                                                                 (3, 6),
                                                                 (4, 7);