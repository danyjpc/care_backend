--Insert User Admin para el personal que probará el sistema

--Insert for adm_person
INSERT INTO adm_person( first_name, last_name,phone_number,cui,email, state_id, city_id, status_id )
VALUES('Admin', 'Plus', 24404908, 3399242441615,'admin@gmail.com', 160061, 160083, 160445 );

--Insert for Organization
INSERT INTO adm_organization (name_organization,email,status_id, created_by)
VALUES ('Care', 'care@gmail.com', 160445, 1);

--Insert for adm_user
INSERT INTO adm_user( password, organization_id, person_id, role_id,status_id)
VALUES('Admin1234', 2,2,160523,160445);

--Inser permissions for user
insert into adm_user_permission(user_id, module_id, permission_id)
values (2, 1, 1),
       (2, 1, 2),
       (2, 1, 3),
       (2, 2, 1),
       (2, 2, 2),
       (2, 2, 3),
       (2, 3, 1),
       (2, 3, 2),
       (2, 3, 3),
       (2, 4, 1),
       (2, 4, 2),
       (2, 4, 3);