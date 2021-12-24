
CREATE SEQUENCE IF NOT EXISTS adm_typology_sequence START WITH 170000 INCREMENT BY 1;
create table adm_typology
(
    typology_id        bigint      DEFAULT NEXTVAL('adm_typology_sequence') not null
        constraint adm_typology_pk
            primary key,
    parent_typology_id bigint      default 0,
    internal_id        bigint      default 0                        not null,
    description        varchar(75) default 'S/D'::character varying not null,
    value_1            varchar(75) default 'S/D'::character varying not null,
    value_2            varchar(75) default 'S/D'::character varying not null,
    is_editable        boolean     default false                    not null,
    show_survey        boolean     default false                    not null
);
--comments
COMMENT ON COLUMN adm_typology.show_survey IS 'Este campo se utilizara para darle la opción al usuario 
si quiere usar sus propias opciones o las definidas por el sistema';

CREATE SEQUENCE IF NOT EXISTS adm_organization_sequence START WITH 1 INCREMENT BY 1;
create table IF NOT EXISTS adm_organization
(
    organization_id          bigint   DEFAULT NEXTVAL('adm_organization_sequence')   not null
    constraint adm_organization_pk
    primary key,
    name_organization        varchar(100) DEFAULT 'S/D'     NOT NULL,
    type_organization_id        bigint    default 160000       not null
    constraint type_organization    references adm_typology,
    responsible_organization varchar(150) default 'S/D'     not null,
    phone_number             bigint DEFAULT  0              NOT NULL,
    email                    varchar(150)   DEFAULT  '@'    NOT NULL,
    state_id                    bigint    default 160000       not null
    constraint state    references adm_typology,
    city_id                     bigint    default 160000       not null
    constraint city     references adm_typology,
    address                  varchar(300)   DEFAULT  'S/D'  NOT NULL,
    date                     timestamp default '1900-01-01 00:00:00'::timestamp without time zone    NOT NULL,
    frequency_meeting_id        bigint    default 160000       not null
    constraint frequency_meeting    references adm_typology,
    participation_space_id      bigint    default 160000       not null
    constraint participation_space  references adm_typology,
    main_problem             text   DEFAULT ''  NOT NULL,
    action_perform           text   DEFAULT ''  NOT NULL,
    status_id                   bigint    default 160445       not null
    constraint adm_person_status_fkey
    references adm_typology,
    created_by               bigint    default 0                                                  not null,
    date_created             timestamp default '1900-01-01 00:00:00'::timestamp without time zone not null
    );

--comments
COMMENT ON COLUMN adm_organization.organization_id IS 'Id interno de registros';
COMMENT ON COLUMN adm_organization.name_organization IS 'Nombre de la organizacion';
COMMENT ON COLUMN adm_organization.responsible_organization IS 'El encargado de la organizacion';
COMMENT ON COLUMN adm_organization.phone_number IS 'Numero telefonico de la organizacion';
COMMENT ON COLUMN adm_organization.email IS 'Direccion de correo electronico de la organizacion';
COMMENT ON COLUMN adm_organization.state_id IS 'Departamento en el que se ubica la organizacion';
COMMENT ON COLUMN adm_organization.city_id IS 'Municipio en el que se ubica la organizacion';
COMMENT ON COLUMN adm_organization.address IS 'Direccion de la organizacion';
COMMENT ON COLUMN adm_organization.date IS 'Fecha de constitucion de la organizacion';
COMMENT ON COLUMN adm_organization.frequency_meeting_id IS 'Frecuencia de reunion de la organizacion';
COMMENT ON COLUMN adm_organization.participation_space_id IS 'Espacio de participacion de la organizacion';
COMMENT ON COLUMN adm_organization.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_organization.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_organization.date_created IS 'Fecha en que se creo el registro';


CREATE SEQUENCE IF NOT EXISTS adm_project_sequence START WITH 1 INCREMENT BY 1;
create table if not exists adm_project
(
    project_id      bigint  default nextval('adm_project_sequence')  not null constraint adm_project_pk  primary key,
    name_project    varchar(100) default 'S/D'              not null,
    date            timestamp default '1900-01-01 00:00:00'::timestamp without time zone not null,
    organization_id bigint    default 160000                not null
    constraint organization_id
    references adm_organization,
    status_id          bigint    default 160445                not null
    constraint adm_person_status_fkey
    references adm_typology,
    created_by      bigint    default 0                     not null,
    date_created    timestamp default '1900-01-01 00:00:00'::timestamp without time zone not null
    );
--Comments
COMMENT ON COLUMN adm_project.project_id IS 'Id interno de registros';
COMMENT ON COLUMN adm_project.name_project IS 'Nombre del proyecto';
COMMENT ON COLUMN adm_project.date IS 'Fecha de incio del proyecto';
COMMENT ON COLUMN adm_project.organization_id IS 'Id de la organizacion al que pertenece el proyecto';
COMMENT ON COLUMN adm_project.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_project.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_project.date_created IS 'Fecha en que se creo el registro';


CREATE SEQUENCE IF NOT EXISTS adm_organization_member_sequence START WITH 1 INCREMENT BY 1;
create table if not exists adm_organization_member
(
    organization_member_id   bigint     default nextval('adm_organization_member_sequence')       not null
    constraint adm_organization_member_pk
    primary key,
    name_organization_member varchar(50)    default ('S/D')                                     not null,
    phone_number             bigint         default (0)                                         not null,
    email                    varchar(200)   default ('@')                                       not null,
    organization_id          bigint         default (160000)                                    not null
    constraint organization_id
    references adm_organization,
    status_id                   bigint    default 160445                                             not null
    constraint adm_person_status_fkey
    references adm_typology,
    created_by               bigint    default 0                                                  not null,
    date_created             timestamp default '1900-01-01 00:00:00'::timestamp without time zone not null
    );

--Commments
COMMENT ON COLUMN adm_organization_member.organization_member_id IS 'Id interno de registros';
COMMENT ON COLUMN adm_organization_member.name_organization_member IS 'Nombre de un miembro de la organizacion';
COMMENT ON COLUMN adm_organization_member.phone_number IS 'Numero telefonico de cada miembro de la organizacion';
COMMENT ON COLUMN adm_organization_member.email IS 'Direccion de correo de un miembro de la organizacion';
COMMENT ON COLUMN adm_organization_member.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_organization_member.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_organization_member.date_created IS 'Fecha en que se creo el registro';



create sequence if not exists adm_project_activity_sequence start  with 1 increment by 1;
create table if not exists adm_project_activity
(
    project_activity_id      bigint     DEFAULT NEXTVAL('adm_project_activity_sequence')          not null
    constraint adm_project_activity_pk
    primary key,
    activity_address         varchar(250)   default 'S/D'                                         not null,
    state_id                    bigint    default 160000                                             not null
    constraint state
    references adm_typology,
    city_id                     bigint    default 160000                                             not null
    constraint city
    references adm_typology,
    date                     timestamp default '1900-01-01 00:00:00'::timestamp without time zone   not null,
    activity_type_id            bigint    default 160000                                             not null
    constraint activity_type
    references adm_typology,
    number_participant       integer        default 0                                               not null ,
    activity_cost            integer        default 0                                               not null ,
    time_duration            integer        default 0                                               not null ,
    main_contribution        text           default 'S/D'                                           not null ,
    limit_challenge_solution text           default 'S/D'                                           not null ,
    project_id               bigint                                                               not null
    constraint project_id
    references adm_project,
    status_id                   bigint    default 160445                                             not null
    constraint adm_person_status_fkey
    references adm_typology,
    created_by               bigint    default 0                                                  not null,
    date_created             timestamp default '1900-01-01 00:00:00'::timestamp without time zone not null
    );
--Comments
COMMENT ON COLUMN adm_project_activity.project_activity_id IS 'Id interno de registros';
COMMENT ON COLUMN adm_project_activity.activity_address IS 'Direccion en la que se realizara la actividad';
COMMENT ON COLUMN adm_project_activity.state_id IS 'Deparatamento en que se realizara la actividad';
COMMENT ON COLUMN adm_project_activity.city_id IS 'Municipio en el que se realizara la actidada';
COMMENT ON COLUMN adm_project_activity.date IS 'Fecha de la actidad';
COMMENT ON COLUMN adm_project_activity.activity_type_id IS 'Tipo de actidad que se realizara';
COMMENT ON COLUMN adm_project_activity.number_participant IS 'Numeros de participantes en la actidad';
COMMENT ON COLUMN adm_project_activity.activity_cost IS 'Costo de la catidad en Q';
COMMENT ON COLUMN adm_project_activity.time_duration IS 'Tiempo de duracion de la actividad';
COMMENT ON COLUMN adm_project_activity.main_contribution IS 'Principales aportes de la actidad';
COMMENT ON COLUMN adm_project_activity.limit_challenge_solution IS 'Limitantes, retos y soluciones de la actidad';


----
---
CREATE SEQUENCE IF NOT EXISTS adm_person_sequence START WITH 1 INCREMENT BY 1;
CREATE TABLE IF NOT EXISTS adm_person
(
    person_id               BIGINT       DEFAULT NEXTVAL('adm_person_sequence') PRIMARY KEY,
    first_name              VARCHAR(100) DEFAULT 'S/D'                 NOT NULL,
    last_name               VARCHAR(100) DEFAULT 'S/D'                 NOT NULL,
    birthday                DATE         DEFAULT '1900-01-01'          NOT NULL,
    phone_number            BIGINT       DEFAULT 0                     NOT NULL,
    cui                     BIGINT       DEFAULT 0                     NOT NULL UNIQUE,
    cultural_identity_id       BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_person_cultural_identity_fkey REFERENCES adm_typology,
    state_id                   BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_person_state_fkey REFERENCES adm_typology,
    city_id                    BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_person_city_fkey REFERENCES adm_typology,
    occupation_id               BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_person_occupation_fkey REFERENCES adm_typology,
    marital_status_id          BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_person_marital_status_fkey REFERENCES adm_typology,
    education_id               BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_person_education_fkey REFERENCES adm_typology,
    spoken_language_id         BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_person_spoken_language_fkey REFERENCES adm_typology,
    address_line            VARCHAR(500) DEFAULT 'S/D'                 NOT NULL,
    email                   VARCHAR(500) DEFAULT '@'                   NOT NULL UNIQUE,
    daughters_no            BIGINT       DEFAULT 0                     NOT NULL,
    sons_no                 BIGINT       DEFAULT 0                     NOT NULL,
    genre_id                   BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_person_genre_fkey REFERENCES adm_typology,
    status_id                  BIGINT       DEFAULT 160445                NOT NULL CONSTRAINT adm_person_status_fkey REFERENCES adm_typology,
    created_by              BIGINT       DEFAULT 0                     NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL
    );

--comments
COMMENT ON COLUMN adm_person.person_id IS 'Id interno de registro';
COMMENT ON COLUMN adm_person.first_name IS 'Primer nombre de la persona';
COMMENT ON COLUMN adm_person.last_name IS 'Primer apellido de la persona';
COMMENT ON COLUMN adm_person.birthday IS 'Fecha de nacimiento de la persona';
COMMENT ON COLUMN adm_person.phone_number IS 'Telefono de la persona';
COMMENT ON COLUMN adm_person.cui IS 'Codigo Unico de Identificacion de la persona';
COMMENT ON COLUMN adm_person.cultural_identity_id IS 'Identidad cultural de la persona';
COMMENT ON COLUMN adm_person.state_id IS 'Departamento de residencia de la persona';
COMMENT ON COLUMN adm_person.city_id IS 'Municipio de residencia';
COMMENT ON COLUMN adm_person.occupation_id IS 'Ocupacion de la persona';
COMMENT ON COLUMN adm_person.marital_status_id IS 'Estado marital de la persona';
COMMENT ON COLUMN adm_person.education_id IS 'Escolaridad de la persona';
COMMENT ON COLUMN adm_person.spoken_language_id IS 'Lenguaje hablado por la persona';
COMMENT ON COLUMN adm_person.address_line IS 'Direccion de la persona';
COMMENT ON COLUMN adm_person.email IS 'Correo electronico de la persona';
COMMENT ON COLUMN adm_person.daughters_no IS 'Numero de hijas';
COMMENT ON COLUMN adm_person.sons_no IS 'Numero de hijos';
COMMENT ON COLUMN adm_person.genre_id IS 'Genero de la persona';
COMMENT ON COLUMN adm_person.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_person.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_person.date_created IS 'Fecha en que se creo el registro';


CREATE SEQUENCE IF NOT EXISTS adm_case_sequence START WITH 1 INCREMENT BY 1;
CREATE TABLE IF NOT EXISTS adm_case
(
    case_id                           BIGINT       DEFAULT NEXTVAL('adm_case_sequence') PRIMARY KEY,
    aggressor_first_name              VARCHAR(100) DEFAULT 'S/D'                 NOT NULL,
    aggressor_last_name               VARCHAR(100) DEFAULT 'S/D'                 NOT NULL,
    aggressor_birthday                DATE         DEFAULT '1900-01-01'          NOT NULL,
    aggressor_phone_number            BIGINT       DEFAULT 0                     NOT NULL,
    aggressor_cui                     BIGINT       DEFAULT 0                     NOT NULL,
    aggressor_state_id                   BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_case_state_fkey REFERENCES adm_typology,
    aggressor_address_line            VARCHAR(500) DEFAULT 'S/D'                 NOT NULL,
    aggressor_city_id                    BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_case_city_fkey REFERENCES adm_typology,
    aggressor_ocupation_id               BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_case_ocupation_fkey REFERENCES adm_typology,
    aggressor_marital_status_id          BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_case_marital_status_fkey REFERENCES adm_typology,
    aggressor_work_place              VARCHAR(500) DEFAULT 'S/D'                 NOT NULL,
    act_date                DATE         DEFAULT '1900-01-01'          NOT NULL,
    act_place               VARCHAR(500) DEFAULT 'S/D'                 NOT NULL,
    victim_relationship_id     BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_case_victim_relationship_fkey REFERENCES adm_typology,
    act_description         TEXT         DEFAULT 'S/D'                 NOT NULL,
    identified_violence_type_id BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_case_identified_violence_type_fkey REFERENCES adm_typology,
    lives_with_aggressor    BOOLEAN      DEFAULT FALSE                  NOT NULL,
    accompaniment_type_id      BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_case_accompaniment_type_fkey REFERENCES adm_typology,
    institution_id             BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_case_institution_fkey REFERENCES adm_typology,
    accompaniment_route_id     BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_case_accompaniment_route_fkey REFERENCES adm_typology,
    annotation              TEXT         DEFAULT 'S/D'                 NOT NULL,
    recording_organization_id  BIGINT       DEFAULT 1                     NOT NULL CONSTRAINT adm_case_recording_organization_fkey REFERENCES adm_organization,
    recorded_by              BIGINT       DEFAULT 0                     NOT NULL,
    victim_id                BIGINT                                     NOT NULL CONSTRAINT adm_case_victim_fkey REFERENCES adm_person,
    case_category_id            BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_case_case_category_fkey REFERENCES adm_typology,
    attached_victim_dpi                    BOOLEAN      DEFAULT FALSE                  NOT NULL,
    attached_victim_birth_certificate      BOOLEAN      DEFAULT FALSE                  NOT NULL,
    attached_children_birth_certificate    BOOLEAN      DEFAULT FALSE                  NOT NULL,
    attached_marriage_certificate          BOOLEAN      DEFAULT FALSE                  NOT NULL,
    attached_aggressor_dpi                 BOOLEAN      DEFAULT FALSE                  NOT NULL,
    status_id                  BIGINT       DEFAULT 160445                NOT NULL CONSTRAINT adm_person_status_fkey REFERENCES adm_typology,
    created_by              BIGINT       DEFAULT 0                     NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL
    );

--comments
COMMENT ON COLUMN adm_case.case_id IS 'Id interno del registro';
COMMENT ON COLUMN adm_case.aggressor_first_name IS 'Primer nombre del agresor';
COMMENT ON COLUMN adm_case.aggressor_last_name IS 'Apellido del agresor';
COMMENT ON COLUMN adm_case.aggressor_birthday IS 'Fecha de nacimiento del agresor';
COMMENT ON COLUMN adm_case.aggressor_phone_number IS 'Telefono del agresor';
COMMENT ON COLUMN adm_case.aggressor_cui IS 'Codigo Unico de Identificacion del agresor';
COMMENT ON COLUMN adm_case.aggressor_state_id  IS 'Departamento de residencia del agresor';
COMMENT ON COLUMN adm_case.aggressor_address_line IS 'Direccion del agresor';
COMMENT ON COLUMN adm_case.aggressor_city_id  IS 'Municipio de residencia del agresor';
COMMENT ON COLUMN adm_case.aggressor_ocupation_id  IS 'Ocupacion del agresor';
COMMENT ON COLUMN adm_case.aggressor_marital_status_id  IS 'Estado civil del agresor';
COMMENT ON COLUMN adm_case.aggressor_work_place IS 'Lugar de trabajo del agresor';
COMMENT ON COLUMN adm_case.act_date IS 'Fecha de lo ocurrido';
COMMENT ON COLUMN adm_case.act_place IS 'Lugar del hecho';
COMMENT ON COLUMN adm_case.victim_relationship_id IS 'Parentezco con la vicima';
COMMENT ON COLUMN adm_case.act_description IS 'Descripcion del hecho';
COMMENT ON COLUMN adm_case.identified_violence_type_id  IS 'Tipo de violencia identificado';
COMMENT ON COLUMN adm_case.lives_with_aggressor IS 'Si la victima convive con el agresor';
COMMENT ON COLUMN adm_case.accompaniment_type_id IS 'Tipo de acompanamiento';
COMMENT ON COLUMN adm_case.institution_id IS 'Institucion donde se presento el caso';
COMMENT ON COLUMN adm_case.accompaniment_route_id IS 'Ruta de acompanamiento a seguir';
COMMENT ON COLUMN adm_case.annotation IS 'Anotaciones del caso';
COMMENT ON COLUMN adm_case.recording_organization_id IS 'Id de la organizacion que registra el caso';
COMMENT ON COLUMN adm_case.recorded_by IS 'Usuario que registra el caso (lider / lidereza)';
COMMENT ON COLUMN adm_case.victim_id IS 'Id de la victima';
COMMENT ON COLUMN adm_case.case_category_id IS 'Categoria del caso';
COMMENT ON COLUMN adm_case.attached_victim_dpi IS 'Si se adjunto DPI de la victima';
COMMENT ON COLUMN adm_case.attached_victim_birth_certificate IS 'Si se adjunto certificado de nacimiento de la victima';
COMMENT ON COLUMN adm_case.attached_children_birth_certificate IS 'Si se adjunto certificado de nacimiento de los hijos';
COMMENT ON COLUMN adm_case.attached_marriage_certificate IS 'Si se adjunto certificado de nacimiento';
COMMENT ON COLUMN adm_case.attached_aggressor_dpi IS 'Si se adjunto el DPI del agresor';
COMMENT ON COLUMN adm_case.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_case.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_case.date_created IS 'Fecha en que se creo el registro';


CREATE SEQUENCE IF NOT EXISTS adm_tracing_sequence START WITH 1 INCREMENT BY 1;
CREATE TABLE IF NOT EXISTS adm_tracing
(
    tracing_id                  BIGINT       DEFAULT NEXTVAL('adm_tracing_sequence') PRIMARY KEY,
    tracing_status_id           BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_tracing_tracing_status_fkey REFERENCES adm_typology,
    partner_relationship        VARCHAR(150) DEFAULT 'S/D'                 NOT NULL,
    children_relationship       VARCHAR(150) DEFAULT 'S/D'                 NOT NULL,
    support_reason              VARCHAR(150) DEFAULT 'S/D'                 NOT NULL,
    tracing_diagnosis           VARCHAR(150) DEFAULT 'S/D'                 NOT NULL,
    tracing_description         VARCHAR(150) DEFAULT 'S/D'                 NOT NULL,
    observations                VARCHAR(150) DEFAULT 'S/D'                 NOT NULL,
    case_id                 BIGINT                                         NOT NULL CONSTRAINT adm_tracing_case_fkey REFERENCES adm_case,
    status_id                  BIGINT       DEFAULT 160445                NOT NULL CONSTRAINT adm_tracing_status_fkey REFERENCES adm_typology,
    created_by              BIGINT       DEFAULT 0                     NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL
    );

COMMENT ON COLUMN adm_tracing.tracing_id IS 'Id interno del registro';
COMMENT ON COLUMN adm_tracing.tracing_status_id IS 'Estado del caso';
COMMENT ON COLUMN adm_tracing.partner_relationship IS 'Relacio con el conviviente';
COMMENT ON COLUMN adm_tracing.children_relationship IS 'Relacion con los hijos';
COMMENT ON COLUMN adm_tracing.support_reason IS 'Motivo de apoyo';
COMMENT ON COLUMN adm_tracing.tracing_diagnosis IS 'Diagnostico';
COMMENT ON COLUMN adm_tracing.tracing_description IS 'Descripcion del seguimiento';
COMMENT ON COLUMN adm_tracing.observations IS 'Observaciones del seguimiento';
COMMENT ON COLUMN adm_tracing.case_id IS 'Caso al que pertenece el seguimiento';
COMMENT ON COLUMN adm_tracing.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_tracing.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_tracing.date_created IS 'Fecha en que se creo el registro';


CREATE SEQUENCE IF NOT EXISTS adm_user_sequence START WITH 1 INCREMENT BY 1;
CREATE TABLE IF NOT EXISTS adm_user
(
    user_id                     BIGINT        DEFAULT NEXTVAL('adm_user_sequence') PRIMARY KEY,
    password                    VARCHAR(150) DEFAULT 'S/D'                 NOT NULL,
    organization_id             BIGINT                                     NOT NULL CONSTRAINT adm_user_organization_fkey REFERENCES adm_organization,
    person_id                BIGINT                                     NOT NULL CONSTRAINT adm_user_person_fkey REFERENCES adm_person CONSTRAINT adm_user_person_unique UNIQUE,
    role_id                    BIGINT       DEFAULT 160000                NOT NULL CONSTRAINT adm_user_role_fkey REFERENCES adm_typology,
    status_id                  BIGINT       DEFAULT 160445                NOT NULL CONSTRAINT adm_user_status_fkey REFERENCES adm_typology,
    created_by              BIGINT       DEFAULT 0                     NOT NULL,
    date_created            TIMESTAMP    DEFAULT '1900-01-01 00:00:00' NOT NULL
    );

COMMENT ON COLUMN adm_user.user_id IS 'Id interno del registro';
COMMENT ON COLUMN adm_user.password IS 'Password del usuario';
COMMENT ON COLUMN adm_user.organization_id IS 'Organizacion del usuario';
COMMENT ON COLUMN adm_user.person_id IS 'Registro de persona';
COMMENT ON COLUMN adm_user.role_id IS 'Rol del usuario';
COMMENT ON COLUMN adm_user.status_id IS 'Estado interno del registro';
COMMENT ON COLUMN adm_user.created_by IS 'Usuario que creo el registro';
COMMENT ON COLUMN adm_user.date_created IS 'Fecha en que se creo el registro';