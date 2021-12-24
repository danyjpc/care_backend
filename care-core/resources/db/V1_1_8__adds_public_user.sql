

--Creates public user
--Insert for adm_person
--Inserting new person and storing the generated ID
WITH valuesPerson AS
    ( INSERT INTO adm_person( first_name, last_name,phone_number,cui,email, state_id, city_id, status_id )
      VALUES('Guest', 'User', 00000000, 0000000000000,'guest@gmail.com', 160061, 160083, 160445 )
      RETURNING person_id
    ),
valuesOrganization AS
    (
      SELECT organization_id FROM adm_organization WHERE name_organization = 'CARE'
    )

INSERT INTO adm_user( password, organization_id, person_id, role_id,status_id)
VALUES('Guest1234', (SELECT organization_id FROM valuesOrganization),
       (SELECT person_id FROM valuesPerson),160529,160445);