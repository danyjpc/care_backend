using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using care_core.model;
using care_core.repository;
using care_core.repository.interfaces;
using care_core.security.services;
using care_core.util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace care_core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*CONFIG CORS*/
            services.AddMvc();
            services.AddCors(o => o.AddPolicy("AllowAnyOrigin", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            /*Register Repositories and interfaces */
            services.AddTransient<IAdmGame, AdmGameRepository>();
            services.AddTransient<IAdmUser, AdmUserRepository>();
            services.AddTransient<IAdmTypology, AdmTypologyRepository>();
            services.AddTransient<IAdmOrganization, AdmOrganizationRepository>();
            services.AddTransient<IAdmOrganizationMember, AdmOrganizationMRepository>();
            services.AddTransient<IAdmPerson, AdmPersonRepository>();
            services.AddTransient<IAdmCase, AdmCaseRepository>();
            services.AddTransient<IAdmProject, AdmProjectRepository>();
            services.AddTransient<IAdmCaseTracing, AdmCaseTracingRepository>();
            services.AddTransient<IAdmQuestion, AdmQuestionRepository>();
            services.AddTransient<IAdmOption, AdmOptionRepository>();
            services.AddTransient<IAdmModule, AdmModuleRepository>();
            services.AddTransient<IAdmCategory, AdmCategoryRepository>();
            services.AddTransient<IAdmModuleCategory, AdmModuleCategoryRepository>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IAdmAnswer, AdmAnswerRepository>();
            services.AddTransient<IAdmPermission, AdmPermissionRepository>();
            services.AddTransient<IAdmSurvey, AdmSurveyRepository>();
            services.AddTransient<IAdmForm, AdmFormRepository>();
            services.AddTransient<IAdmGeneralConfig, AdmGeneralConfigRepository>();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);


            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.secret);


            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.AllowTrailingCommas = true;
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Care Core",
                        Description = "Api Documentation with security",
                        Contact = new OpenApiContact
                        {
                            Name = "PEOPLE APPS",
                            Email = "devs@mypeopleapps.com",
                            //Url = new Uri("www.peopleapps.dev"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Licence GPL-2",
                            //Url = new Uri("www.gnu.org/licenses/old-licenses/lgpl-2.1.html"),
                        }
                    }
                );

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "Token",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            services.AddDbContext<EntityDbContext>(options =>
                options.UseNpgsql(
                    CareConstants.CONNECTION_STRING
                )
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors("AllowAnyOrigin");
            // app.UseStaticFiles();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CARE API V1");
                //c.RoutePrefix = string.Empty;
            });
        }
    }
}