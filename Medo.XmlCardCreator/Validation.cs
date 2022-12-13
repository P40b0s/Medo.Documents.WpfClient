using medo2_5;
using medo2_7;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XmlCardCreator
{
    public class Validation : MedoDeserializer
    {
        public communication medo25 = null;
        public container medo27 = null;

        public async Task<bool> WriteErrorToBase(Guid headerGuid, string error)
        {
            try
            {
                return await Task<bool>.Factory.StartNew(() =>
                {
                    using (SqlConnection conn = new SqlConnection(Medo.Helpers.ConnectionsStrings.ReserveServerAutoReportsConnectionString))
                    {
                        try
                        {
                            conn.Open();
                        }
                        catch (SqlException se)
                        {
                            logger.Fatal(se);
                        }
                        SqlCommand command = new SqlCommand(@"INSERT INTO ValidationErrors (HeaderGuid, ValidationError) VALUES (@HG, @VE)");
                        command.Parameters.Add("@HG", SqlDbType.UniqueIdentifier, 38, "HeaderGuid").Value = headerGuid;
                        command.Parameters.Add("@VE", SqlDbType.NVarChar, 2000, "ValidationError").Value = error;
                        command.Connection = conn;
                        command.ExecuteNonQuery();
                    }


                    return true;
                });

            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
                return false;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return false;
            }
        }

        readonly Logger logger = LogManager.GetCurrentClassLogger();



        public void tryValidate(List<DirectoryInfo> DirList)
        {
            List<FileInfo> files = DirList.Where(x => x.Extension.ToLower() == ".xml").Cast<FileInfo>().ToList();
            foreach (FileInfo f in files)
            {
                try
                {
                    XmlSchemaSet schema = new XmlSchemaSet();
                    try
                    {
                        schema.Add("http://www.infpres.com/IEDMS", "Medo2.5xsd\\Medo2.5.xsd");
                        medo25 = LoadMedo2_5(f);
                    }
                    catch (System.Exception ex)
                    {
                        schema.Add("http://minsvyaz.ru/container", "Medo2.7xsd\\Medo2.7.xsd");
                        medo27 = LoadMedo2_7(f);
                    }
                    bool errors = false;
                    XDocument doc = new XDocument(XElement.Load(f.FullName));

                    doc.Validate(schema, async (o, error) =>
                    {
                        Guid g = Guid.Empty;
                        errors = true;
                        logger.Info(string.Format("Ошибка {0} файл: {1}", error.Message, f.FullName));
                        if (medo25 != null)
                        {
                            g = new Guid(medo25.header.uid);
                        }
                        if (medo27 != null)
                        {
                            g = new Guid(medo27.uid);
                        }

                        await WriteErrorToBase(g, error.Message + Environment.NewLine);
                    }
                          );
                   
                }
                catch (System.Exception ex)
                {
                    logger.Fatal(ex);
                }
            }
        }

        public void tryValidate(FileInfo file)
        {
            try
            {
                XmlSchemaSet schema = new XmlSchemaSet();
                try
                {
                    schema.Add("http://www.infpres.com/IEDMS", "Medo2.5xsd\\Medo2.5.xsd");
                    medo25 = LoadMedo2_5(file);
                }
                catch (System.Exception ex)
                {
                    schema.Add("http://minsvyaz.ru/container", "Medo2.7xsd\\Medo2.7.xsd");
                    medo27 = LoadMedo2_7(file);
                }
                bool errors = false;
                XDocument doc = new XDocument(XElement.Load(file.FullName));

                doc.Validate(schema, async (o, error) =>
                {
                    Guid g = Guid.Empty;
                    errors = true;
                    logger.Info(string.Format("Ошибка {0} файл: {1}", error.Message, file.FullName));
                    if (medo25 != null)
                    {
                        g = new Guid(medo25.header.uid);
                    }
                    if (medo27 != null)
                    {
                        g = new Guid(medo27.uid);
                    }

                    await WriteErrorToBase(g, error.Message + Environment.NewLine);
                }
                      );

                if (errors)
                {
                    logger.Info(string.Format("Валидация с ошибками"));
                }
                else
                {
                    logger.Info(string.Format("Валидация успешна!"));
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

    }
}
