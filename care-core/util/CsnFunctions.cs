using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using care_core.dto.AdmForm;
using care_core.dto.AdmQuestionGroup;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Serilog;

namespace care_core.util
{
    public class CsnFunctions
    {
        public static DateTime now()
        {
            return DateTime.UtcNow.AddHours(CareConstants.UTC_CONFIG);
        }

        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long) (datetime - sTime).TotalSeconds;
        }


        //Creates report for form questions according to https://dev.azure.com/People-Apps/CARE/_workitems/edit/1608/
        public static byte[] createAnswerReportExcel(
            //contains both headers and answers
            List<AdmReportAnswerDto> pivote,
            string name_file,
            List<AdmQuestionDto> preguntas
        )
        {
            var newFile = @"resources/documents/" + name_file + ".xlsx";

            using (var fs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();

                ISheet sheet1 = workbook.CreateSheet("Hoja1");

                var rowHeaderIndex = 0;
                //Headers
                //Object foo = new ReportTransaction();
                IRow row = sheet1.CreateRow(0);

                var neededRows = 0;
                //creating headers

                row.CreateCell(rowHeaderIndex).SetCellValue("Survey number");
                rowHeaderIndex++;
                row.CreateCell(rowHeaderIndex).SetCellValue("Username");
                rowHeaderIndex++;
                row.CreateCell(rowHeaderIndex).SetCellValue("Fecha");
                rowHeaderIndex++;
                foreach (var pregunta in preguntas)
                {
                    row.CreateCell(rowHeaderIndex).SetCellValue(pregunta.name_question);
                    rowHeaderIndex++;
                }


                //Defining rows needed for body
                //a row equals to one survey
                neededRows = pivote.Count;

                var rowBodyIndex = 1;
                //create rows
                for (int i = 1; i <= neededRows; i++)
                {
                    IRow rowBody = sheet1.CreateRow(rowBodyIndex++);
                }
                
                //Iterate over each answer array and give value to the cells
                try
                {
                    var rowBodyColumn = 0;
                    rowBodyIndex = 1;
                    foreach (var reportAnswerDto in pivote)
                    {
                        rowBodyColumn = 0;
                        foreach (var questionAnswerDto in reportAnswerDto.elementos)
                        {

                            IRow rowBody = sheet1.GetRow(rowBodyIndex);
                            
                            if (rowBodyColumn == 0)
                            {
                                rowBody.CreateCell(rowBodyColumn).SetCellValue(reportAnswerDto.surveyId);
                                rowBodyColumn++;
                            }

                            if (rowBodyColumn == 1)
                            {
                                rowBody.CreateCell(rowBodyColumn).SetCellValue(reportAnswerDto.userName);
                                rowBodyColumn++;
                            }

                            if (rowBodyColumn == 2)
                            {
                                rowBody.CreateCell(rowBodyColumn).SetCellValue(reportAnswerDto.dateCreated.ToString());
                                rowBodyColumn++;
                            }

                            if (rowBodyColumn > 2)
                            {
                                rowBody.CreateCell(rowBodyColumn).SetCellValue(questionAnswerDto.respuesta);
                                rowBodyColumn++;
                            }
                        }

                        rowBodyIndex++;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                workbook.Write(fs);

                byte[] fileContents = null;
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    fileContents = memoryStream.ToArray();
                }

                return fileContents;
            }
        }


        //Creates report for form questions according to https://dev.azure.com/People-Apps/CARE/_workitems/edit/1608/
        public static byte[] createAnswerReportCsv(
            //contains both questions and surveys that contain answers
            List<AdmReportAnswerDto> pivote,
            string name_file,
            List<AdmQuestionDto> preguntas
        )
        {
            var newFile = @"resources/documents/" + name_file + ".csv";

            using (var fs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
            {
                string delimiterChar = ";";
                StringBuilder stringBuilder = new StringBuilder();
                //Header string
                StringBuilder preguntasString = new StringBuilder();
                for (int i = 0; i < preguntas.Count; i++)
                {
                    //Adding survey number header
                    if (i == 0)
                    {
                        preguntasString.Append("Survey number").Append(delimiterChar)
                            .Append("Usuario").Append(delimiterChar)
                            .Append("Fecha").Append(delimiterChar);
                    }

                    preguntasString.Append(preguntas[i].name_question);
                    //append delimiter if not on last object
                    if (preguntas.Count - i != 1)
                    {
                        preguntasString.Append(delimiterChar);
                    }
                }

                stringBuilder.Append(preguntasString);
                //jumping line after headers
                stringBuilder.AppendLine();

                StringBuilder filas = new StringBuilder();
                //Iterating on surveys
                foreach (var survey in pivote)
                {
                    //Adding surveyId to every line
                    filas.Append(survey.surveyId).Append(delimiterChar);
                    //Append user
                    filas.Append(survey.userName).Append(delimiterChar);
                    //Append date
                    filas.Append(survey.dateCreated).Append(delimiterChar);
                    //will be used to count elements and avoid adding a ',' after last element
                    int posPregunta = 0;

                    //Iterating through questions
                    for (int i = 0; i < preguntas.Count; i++)
                    {
                        //Iterating on survey elements (answers)
                        for (int j = 0; j < survey.elementos.Count; j++)
                        {
                            //we use try and catch because we try to acces to every question, if survey.elemento has no available
                            //element for such index, a out of bounds exception is produced
                            try
                            {
                                //trying to access to every element using preguntas index
                                if (survey.elementos[i].preguntaId == preguntas[i].question_id)
                                {
                                    //adds answer to output result
                                    filas.Append(survey.elementos[i].respuesta);
                                    
                                    if ((preguntas.Count - posPregunta != 1))
                                    {
                                        //append only if not on last element
                                        filas.Append(delimiterChar);
                                    }

                                    //moving on the elements
                                    posPregunta++;
                                    //skip current cycle
                                    break;
                                }

                                //if no match, just append a ','
                                if ((preguntas.Count - posPregunta != 1))
                                {
                                    filas.Append(delimiterChar);
                                    posPregunta++;
                                }
                            }
                            catch (Exception ex)
                            {
                                //if an exception happens, just append ','
                                if ((preguntas.Count - posPregunta != 1))
                                {
                                    filas.Append(delimiterChar);
                                    posPregunta++;
                                }

                                //skip current cycle
                                break;
                            }
                        }
                    }

                    //if survey has no answers
                    if (survey.elementos.Count == 0)
                    {
                        for (int a = 0; a < (preguntas.Count - 1); a++)
                        {
                            filas.Append(delimiterChar);
                        }
                    }

                    filas.AppendLine();
                }

                stringBuilder.Append(filas.ToString());
                //writing to file
                byte[] fileContents = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                fs.Write(fileContents, 0, fileContents.Length);

                //Delete the created file, as it has no further use
                File.Delete(newFile);

                return fileContents;
            }
        }
    }
}