using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using care_core.util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace care_core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        
        static Program()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            MigrateDatabase();

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
        
        private static void MigrateDatabase()
        {
            try
            {
                if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SERVER_DB")))
                {
                    if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SERVER_DB_PORT")))
                    {
                        if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SERVER_DB_USER")))
                        {
                            if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SERVER_DB_PASS")))
                            {
                                if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SERVER_DB_NAME")))
                                {
                                    var cnx = new Npgsql.NpgsqlConnection(CareConstants.CONNECTION_STRING);
                                    
                                    string routeMigration = (CareConstants.TERMINAL_MODE.Equals("PROD"))
                                        ? System.AppDomain.CurrentDomain.BaseDirectory + "resources/db"
                                        : "resources/db";

                                    var envolve = new Evolve.Evolve(cnx, msg => Log.Information(msg))
                                    {
                                        Locations = new[] {routeMigration},
                                        IsEraseDisabled = true,
                                        MetadataTableName = "schema_version_care"
                                    };
                                    envolve.Migrate();
                                    envolve.Info();
                                }
                                else
                                {
                                    Log.Error("Database Migration Fail ENVIRONMNENT SERVER_DB_NAME doesnt exist ");
                                }
                            }
                            else
                            {
                                Log.Error("Database Migration Fail ENVIRONMNENT SERVER_DB_PASS doesnt exist ");
                            }
                        }
                        else
                        {
                            Log.Error("Database Migration Fail ENVIRONMNENT SERVER_DB_USER doesnt exist ");
                        }
                    }
                    else
                    {
                        Log.Error("Database Migration Fail ENVIRONMNENT SERVER_DB_PORT doesnt exist ");
                    }
                }
                else
                {
                    Log.Error("Database Migration Fail ENVIRONMNENT SERVER_DB doesnt exist ");
                }
            }
            catch (Evolve.EvolveSqlException ex)
            {
                Log.Error(ex.ToString());
                Log.Error("Sql  Fail", ex.Message);
                //throw;
            }
            catch (Evolve.EvolveConfigurationException ex)
            {
                Log.Error(ex.ToString());
                Log.Error("Configure Migration Fail", ex.Message);
                //throw;
            }
            catch (Evolve.EvolveValidationException ex)
            {
                Log.Error(ex.ToString());
                Log.Error("Validacion  Fail", ex.Message);
                //throw;
            }
            catch (Evolve.EvolveException ex)
            {
                Log.Error(ex.ToString());
                Log.Error("Excetion General  Fail", ex.Message);

                //throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Error("Database Migration Fail", ex.Message);
                //throw;
            }
        }

    }
}