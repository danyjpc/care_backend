create sequence if not exists adm_game_sequence increment by 1 start with 1;

create table if NOT EXISTS adm_game
(
    id     bigint primary key default nextval('adm_game_sequence'::regclass),
    title  varchar(200)       default 'S/D',
    phrase varchar(200)       default 'S/D'
);

insert into adm_game(title, phrase)
values ('mass effect 2', 'Is simply awe-inspiring ');
insert into adm_game(title, phrase)
values ('the last of us', 'the most beautiful game seen on any console');