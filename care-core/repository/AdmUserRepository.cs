using System;
using System.Collections.Generic;
using System.Linq;
using care_core.dto.AdmOrganization;
using care_core.dto.AdmPerson;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.EntityFrameworkCore;
using Serilog;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;

namespace care_core.repository
{
    public class AdmUserRepository : IAdmUser
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmPerson _admPerson;

        public AdmUserRepository(EntityDbContext dbContext, IAdmPerson admPerson)
        {
            _dbContext = dbContext;
            _admPerson = admPerson;
        }

        public IEnumerable<Object> getAll()
        {
            var users = _dbContext.admUsers
                .Where(x => x.user_id > 1 && x.status.typology_id == CareConstants.STATUS_ACTIVE
                 && x.person.email!="guest@gmail.com")
                .Select(
                    user => new
                    {
                        user_id = user.user_id,
                        password = user.password,
                        organization = new
                        {
                            organization_id = user.organization.organization_id,
                            name_organization = user.organization.name_organization
                        },
                        person = new
                        {
                            person_id = user.person.person_id,
                            first_name = user.person.first_name,
                            last_name = user.person.last_name,
                            cui = user.person.cui,
                            birthday = user.person.birthday,
                            email = user.person.email,
                            state = new
                            {
                                typology_id = user.person.state.typology_id,
                                description = user.person.state.description
                            },
                            city = new
                            {
                                typology_id = user.person.city.typology_id,
                                description = user.person.city.description
                            }
                        },

                        role = new
                        {
                            typology_id = user.role.typology_id,
                            description = user.role.description
                        },
                        status = new
                        {
                            typology_id = user.status.typology_id,
                            description = user.status.description
                        }
                    }
                ).OrderByDescending(id => id.user_id).ToList();

            return users;
        }

        public AdmUser findByCredentials(String email, String pwd)
        {
            return (from user in _dbContext.admUsers
                    join person in _dbContext.admPersons on user.person.person_id equals person.person_id
                    join status in _dbContext.admTypologies on user.status.typology_id equals status.typology_id
                    join role in _dbContext.admTypologies on user.role.typology_id equals role.typology_id
                    where user.person.email.Equals(email) && user.password.Equals(pwd)
                                                          && user.status.typology_id == CareConstants.STATUS_ACTIVE
                    select new AdmUser
                    {
                        user_id = user.user_id,
                        person = new AdmPerson
                        {
                            person_id = person.person_id,
                            first_name = person.first_name,
                            last_name = person.last_name,
                            cui = person.cui,
                        },
                        status = new AdmTypology()
                        {
                            typology_id = user.status.typology_id,
                            description = user.status.description
                        },
                        role = new AdmTypology()
                        {
                            typology_id = user.role.typology_id,
                            description = user.role.description
                        }
                    }
                ).FirstOrDefault();
        }

        //Method that finds user by email, returns null if not found
        public AdmUser findByEmail(string email)
        {
            AdmPerson person = _admPerson.findByEmail(email, true);

            AdmUser user = _dbContext.admUsers.SingleOrDefault(x => x.person.person_id == person.person_id);

            return user;
        }

        public Object getById(long id)
        {
            var user = _dbContext.admUsers.Where(x => x.user_id > 1 && x.user_id == id).Select(
                user => new
                {
                    user_id = user.user_id,
                    password = user.password,
                    user.date_create,
                    organization = new
                    {
                        organization_id = user.organization.organization_id,
                        name_organization = user.organization.name_organization
                    },
                    person = new
                    {
                        person_id = user.person.person_id,
                        first_name = user.person.first_name,
                        last_name = user.person.last_name,
                        cui = user.person.cui,
                        birthday = user.person.birthday,
                        email = user.person.email,
                        state = new
                        {
                            typology_id = user.person.state.typology_id,
                            description = user.person.state.description
                        },
                        city = new
                        {
                            typology_id = user.person.city.typology_id,
                            description = user.person.city.description
                        }
                    },
                    role = new
                    {
                        typology_id = user.role.typology_id,
                        description = user.role.description
                    },
                    status = new
                    {
                        typology_id = user.status.typology_id,
                        description = user.status.description
                    }
                }
            ).SingleOrDefault();

            return user;
        }

        public int persist(AdmUser admUser)
        {
            try
            {
                _dbContext.Add(admUser);
            }
            catch (Exception ex)
            {
                Log.Error("Error: " + ex.Message);
            }

            save();

            return admUser.user_id;
        }

        public void upd(AdmUser admUser)
        {
            var per_id = _dbContext.admUsers.Where(x => x.user_id == admUser.user_id)
                .Select(
                    p => new AdmPerson
                    {
                        person_id = p.person.person_id
                    }
                ).SingleOrDefault();

            //Update Person
            AdmPerson person = updPerson(admUser, per_id.person_id);

            //Update User
            AdmUser updUser = _dbContext.admUsers.Find(admUser.user_id);
            AdmOrganization organization = _dbContext.admOrganizations.Find(admUser.organization.organization_id);
            AdmTypology role = _dbContext.admTypologies.Find(admUser.role.typology_id);
            AdmTypology status = _dbContext.admTypologies.Find(admUser.status.typology_id);


            updUser.password = admUser.password;
            updUser.organization = organization;
            updUser.person = person;
            updUser.role = role;
            updUser.status = status;

            _dbContext.Entry(updUser).State = EntityState.Modified;
            save();
        }

        public AdmPerson updPerson(AdmUser admUser, int person_id)
        {
            AdmPerson updPerson = _dbContext.admPersons.Find(person_id);

            updPerson.first_name = admUser.person.first_name;
            updPerson.last_name = admUser.person.last_name;
            updPerson.birthday = admUser.person.birthday;
            updPerson.cui = admUser.person.cui;
            updPerson.email = admUser.person.email;
            AdmTypology state = _dbContext.admTypologies.Find(admUser.person.state.typology_id);
            AdmTypology city = _dbContext.admTypologies.Find(admUser.person.city.typology_id);
            updPerson.state = state;
            updPerson.city = city;

            _dbContext.Entry(updPerson).State = EntityState.Modified;
            save();
            return updPerson;
        }

        public void updPassword(AdmUser admUser)
        {
            AdmUser updUser = _dbContext.admUsers.Find(admUser.user_id);

            updUser.password = admUser.password;

            _dbContext.Entry(updUser).State = EntityState.Modified;
            save();
        }

        public void passwordReset(AdmPerson admPerson){
            AdmUser resetPassUser = _dbContext.admUsers.Where(x=> x.person.person_id==admPerson.person_id).Single();
            //se genera la contraseña nueva del usuario
            string newpass = generarcontraseña();

            resetPassUser.password = newpass;
            //se actualiza los datos del usuario
            _dbContext.Entry(resetPassUser).State = EntityState.Modified;
            save();
            //se envía el email al usuario, con su nueva contraseña
            sendMail(resetPassUser.password, admPerson.email);
            //se envía la notificacion a los administradores 
            var adminsEmail = GetAdminEmails();
            foreach (var emails in adminsEmail)
            {
                sendMailToAdms(admPerson.email, emails.email);
            }
        }

        public string generarcontraseña()
        {
            Random rdn = new Random();
            string caracteres = "abcdefghi-jklmnopqrstuvwxyz_-ABCDEFGHIJKLM_NOPQRSTUVWXYZ1234567890%$#@";
            int longitud = caracteres.Length;
            char letra;
            int longitudContrasenia = 7;
            string contraseniaAleatoria = string.Empty;
            for (int i = 0; i < longitudContrasenia; i++)
            {
                letra = caracteres[rdn.Next(longitud)];
                contraseniaAleatoria += letra.ToString();
            }
            return contraseniaAleatoria;
        }

        public void sendMail(string newPass, string toEmail)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(CareConstants.SMTP_NAME_FROM));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Restablecer contraseña";

            message.Body = new TextPart("plain")
            {
                Text = @"Hola,

                Hemos recibido una solicitud para recuperar tu contraseña,
                de tu cuenta asociada al correo: "+ toEmail+" \n"

                +"Tu nueva contraseña es: "+ newPass+ " \n\n"

                +"--Saludos"
            };

            using (var client = new SmtpClient())
            {
                client.Connect(CareConstants.SMTP_HOST, CareConstants.SMTP_PORT, SecureSocketOptions.Auto);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(CareConstants.SMTP_USER, CareConstants.SMTP_PASS);

                client.Send(message);
                client.Disconnect(true);
            }
        }
        //Metodo que obtiene a los administradores
        public AdmUserEmailDto[] GetAdminEmails(){
            AdmUserEmailDto [] emails = _dbContext.admUsers
                .Where(x => x.user_id > 1 && x.status.typology_id == CareConstants.STATUS_ACTIVE
                 && x.person.email!="guest@gmail.com" && x.role.typology_id ==160523)
                .Select(
                    user => new AdmUserEmailDto
                    {
                        user_id = user.user_id,
                        email = user.person.email,
                        status_id = user.status.typology_id,
                        role_id = user.role.typology_id,
                    }
                ).ToArray();

            return emails;
        }
        //Metodo que envia notificaciones a los administradores debiado a un cambio de contraseña por un usuario
        public void sendMailToAdms(string userEmail, string AdmEmail)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(CareConstants.SMTP_NAME_FROM));
            message.To.Add(MailboxAddress.Parse(AdmEmail));
            message.Subject = "Notificación de recuperación de contraseña Sistema CARE-MMITZ";

            message.Body = new TextPart("plain")
            {
                Text = @"Hola,

                Este correo es para notificarle que el usuario con cuenta asociada 
                al correo: "+userEmail+" ha realizado la recuperación de su contraseña \n\n"

                +"--Saludos cordiales."
            };

            using (var client = new SmtpClient())
            {
                client.Connect(CareConstants.SMTP_HOST, CareConstants.SMTP_PORT, SecureSocketOptions.Auto);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(CareConstants.SMTP_USER, CareConstants.SMTP_PASS);

                client.Send(message);
                client.Disconnect(true);
            }
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }
        
         public IEnumerable<AdmUserDto> getAllDto()
        {
            var users = _dbContext.admUsers
                .Where(x => x.status.typology_id == CareConstants.STATUS_ACTIVE)
                .Select(
                    user => new AdmUserDto()
                    {
                        user_id = user.user_id,
                        password = user.password,
                        email = user.person.email,
                        organization = new AdmOrganizationDto()
                        {
                            organization_id = user.organization.organization_id,
                            name_organization = user.organization.name_organization
                        },
                        person = new AdmPersonDto()
                        {
                            person_id = user.person.person_id,
                            first_name = user.person.first_name,
                            last_name = user.person.last_name,
                            cui = user.person.cui,
                            birthday = user.person.birthday,
                            email = user.person.email,
                            state = new AdmTypologyDto()
                            {
                                typology_id = user.person.state.typology_id,
                                description = user.person.state.description
                            },
                            city = new AdmTypologyDto()
                            {
                                typology_id = user.person.city.typology_id,
                                description = user.person.city.description
                            }
                        },

                        role = new AdmTypologyDto()
                        {
                            typology_id = user.role.typology_id,
                            description = user.role.description
                        },
                        status = new AdmTypologyDto()
                        {
                            typology_id = user.status.typology_id,
                            description = user.status.description
                        }
                    }
                ).OrderByDescending(id => id.user_id).ToList();

            return users;
        }
    }
}