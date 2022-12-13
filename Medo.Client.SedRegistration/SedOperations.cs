using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Client.SedRegistration
{
    /// <summary>
    /// Операции с документом в СЕДе
    /// </summary>
    public class SedOperations : CreateSEDTransportPacket
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
       
        private string PakConnection { get; set; }
       
        public SedOperations() {
           
            PakConnection = Helpers.ConnectionsStrings.PAKConnectionString;         
        }

        #region Регистрация в седе

        /// <summary>
        /// Получение ID документа кторый отображается в СЭДе (document_pending)
        /// </summary>
        /// <param name="sourceGuid">ID принявшего органа документа</param>
        /// <param name="number">ПАКовский номер документа</param>
        /// <returns>Id документа который отображается в СЭДе</returns>
        private List<Guid> GetDocPendingId(Guid sourceGuid, string number)
        {
            List<Guid> dguid = new List<Guid>();
            try
            {
                using (SqlConnection conn = new SqlConnection(PakConnection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand("SELECT trans_id FROM dbo.document_pending WHERE docpend_number = @number AND s_dep_id = @sguid");
                    cmd.Parameters.Add("@number", SqlDbType.VarChar, 64).Value = number;
                    cmd.Parameters.Add("@sguid", SqlDbType.UniqueIdentifier, 38).Value = sourceGuid;
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            if (dr.HasRows)
                            {
                                dguid.Add(dr.GetGuid(0));
                            }
                        }
                    }
                }
                return dguid;
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
                return dguid;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return dguid;
            }
        }


        /// <summary>
        /// Проверка, зарегистрирован ли данный документ в системе
        /// </summary>
        /// <param name="DocGuid">GUID документа</param>
        /// <returns></returns>
        private bool DocAlreadyRegistred(Guid DocGuid)
        {
            bool exists = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(PakConnection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand("SELECT CAST(CASE WHEN ((SELECT COUNT(*) FROM document WHERE doc_id = @docGuid)> 0)THEN 1 ELSE 0 END AS BIT)");
                    cmd.Parameters.Add("@docGuid", SqlDbType.UniqueIdentifier).Value = DocGuid;
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            if (dr.HasRows)
                            {
                                exists = dr.GetBoolean(0);
                            }
                        }
                    }
                }
                return exists;
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
                return exists;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return exists;
            }
        }
        /// <summary>
        /// Очистка таблицы document_pending от ненужных документов
        /// </summary>
        public void ClearPendingDocuments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(PakConnection))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(@"dbo.DeletePendingDocuments", conn);                                     
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = conn;
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);	
            }
        }
        /// <summary>
        /// Изменение статуса видимости документа с 0  на 1
        /// </summary>
        public void DonePendingDocument(Guid source, string number)
        {
            try
            {
                List<Guid> transId = GetDocPendingId(source, number);
                using (SqlConnection conn = new SqlConnection(PakConnection))
                {
                    conn.Open();
                    foreach(Guid g in transId)
                    {
                        SqlCommand command = new SqlCommand(@"dbo.DonePendingDocument", conn);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@DocPendID", SqlDbType.UniqueIdentifier).Value = g;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        /// <summary>
        /// Регистрация, удаление или отказ в регистрации в СЕДе.
        /// </summary>
        /// <param name="doc">Модель для регистрации документа</param>
        /// <param name="eonumber">Номер электронного опубликования (используется только при регистрации документа совмесно с отправкой уведомлений об упубликовании)</param>
        /// <param name="eodate">Дата электронного опубликования (используется только при регистрации документа совмесно с отправкой уведомлений об упубликовании)</param>
        /// <returns>Результат выполнения операции</returns>
        public async Task<bool> Register(SedModel doc, string eonumber = null, DateTime? eodate = null)
        {
            switch (doc.Operation)
            {
                default: return false;
                case SedOperationEnum.Register:
                    {
                        try
                        {
                            if (!DocAlreadyRegistred(doc.DocGuid))
                            {
                                using (SqlConnection conn = new SqlConnection(PakConnection))
                                {
                                    conn.Open();
                                    SqlCommand command = new SqlCommand(@"dbo.AddNewDocument", conn);
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.Add("@DocNumber", SqlDbType.VarChar, 64).Value = string.IsNullOrEmpty(eonumber) ? doc.Number : eonumber;
                                    command.Parameters.Add("@DocDesc", SqlDbType.VarChar, 512).Value = string.IsNullOrEmpty(doc.DocumentName) ? (object)DBNull.Value : doc.DocumentName;
                                    command.Parameters.Add("@DocMLNum", SqlDbType.Int).Value = doc.PagesCount;
                                    command.Parameters.Add("@DocCopyNum", SqlDbType.Int).Value = 1;
                                    command.Parameters.Add("@DocCopyStr", SqlDbType.VarChar, 128).Value = "";
                                    command.Parameters.Add("@DocRegDate", SqlDbType.DateTime).Value = eodate.HasValue ? eodate : DateTime.Now;
                                    command.Parameters.Add("@DocRAccessID", SqlDbType.UniqueIdentifier).Value = new Guid("00000000-0000-0000-0000-000000000004");
                                    command.Parameters.Add("@DocKindID", SqlDbType.UniqueIdentifier).Value = new Guid("00000000-0000-0000-0000-000000000001");
                                    command.Parameters.Add("@DocTypeID", SqlDbType.UniqueIdentifier).Value = new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
                                    command.Parameters.Add("@DocStatID", SqlDbType.UniqueIdentifier).Value = new Guid(((int)doc.Operation).ToString().PadLeft(32, '0'));
                                    command.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier).Value = new Guid("25E3E6DE-04EB-4BE0-9291-8DFAB3F25DD9");
                                    command.Parameters.Add("@DocID", SqlDbType.UniqueIdentifier).Value = doc.DocGuid;
                                    command.Parameters.Add("@ParentDocID", SqlDbType.UniqueIdentifier).Value = (object)DBNull.Value;
                                    command.Parameters.Add("@DopDocCopyNum", SqlDbType.Int).Value = 0;
                                    command.Parameters.Add("@DopDocCopyStr", SqlDbType.VarChar, 128).Value = "";
                                    command.Parameters.Add("@DopDocCopyDate", SqlDbType.DateTime).Value = (object)DBNull.Value;
                                    command.Parameters.Add("@DocSignStatus", SqlDbType.VarChar, 512).Value = (object)DBNull.Value;
                                    command.Parameters.Add("@DocCreator", SqlDbType.UniqueIdentifier).Value = Guid.Empty;
                                    command.Connection = conn;
                                    command.ExecuteNonQuery();
                                    DonePendingDocument(doc.SGuid, doc.Number);
                                }
                                logger.Info(string.Format("Документ организации {0} номер {1} успешно зарегистрирован в СЭДе", doc.SGuid.ToString("D"), doc.Number));
                                if (await CreateRegisterationPacket(doc, eonumber, eodate))
                                {
                                    logger.Info(string.Format("Транспортный пакет по организации {0} номеру {1} успешно создан", doc.SGuid.ToString("D"), doc.Number));
                                    return true;
                                }
                                else
                                {
                                    logger.Fatal(string.Format("Ошибка создания транспортного пакета по организации {0} номеру {1}", doc.SGuid.ToString("D"), doc.Number));
                                    return false;
                                }

                            }
                            else
                            {
                                logger.Info(string.Format("Документ органа {0} номер {1} уже зарегистрирован в СЭДе, документ удаляется...", doc.SGuid.ToString("D"), doc.Number));
                                goto case SedOperationEnum.Delete;
                            }
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
                case SedOperationEnum.Delete:
                    {
                        try
                        {
                            List<Guid> trans_id = GetDocPendingId(doc.SGuid, doc.Number);
                            if (trans_id.Count > 0)
                            {
                                using (SqlConnection conn = new SqlConnection(PakConnection))
                                {
                                    conn.Open();
                                    foreach(Guid g in trans_id)
                                    {
                                        SqlCommand command = new SqlCommand(@"dbo.DonePendingDocument", conn);
                                        command.CommandType = CommandType.StoredProcedure;
                                        command.Parameters.Add("@DocPendID", SqlDbType.UniqueIdentifier).Value = g;
                                        command.Connection = conn;
                                        command.ExecuteNonQuery();
                                    }                                   
                                }
                                ClearPendingDocuments();
                                logger.Info(string.Format("Документ организации {0} номер {1} успешно удален из СЭДа", doc.SGuid.ToString("D"), doc.Number));
                                return true;

                            }
                            else
                            {
                                logger.Info(string.Format("Ошибка удаления документа организации {0} номер {1}, в таблице document_pending не найден trans_id ", doc.SGuid.ToString("D"), doc.Number));
                                return false;
                            }

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
                case SedOperationEnum.Refuse:
                    {
                        try
                        {
                            using (SqlConnection conn = new SqlConnection(PakConnection))
                            {
                                List<Guid> trans_id = GetDocPendingId(doc.SGuid, doc.Number);
                                conn.Open();
                                foreach(Guid g in trans_id)
                                {
                                    SqlCommand command = new SqlCommand(@"dbo.RefusePendingDocument", conn);
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.Add("@PendID", SqlDbType.UniqueIdentifier).Value = g;
                                    command.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier).Value = new Guid("25E3E6DE-04EB-4BE0-9291-8DFAB3F25DD9"); //гыгы GUID Никитина ))
                                    command.Parameters.Add("@RefuseReason", SqlDbType.VarChar).Value = string.IsNullOrEmpty(doc.RefuseStatus) ? (object)DBNull.Value : doc.RefuseStatus;
                                    command.Parameters.Add("@RefuseComment", SqlDbType.VarChar).Value = string.IsNullOrEmpty(doc.RefuseComment) ? (object)DBNull.Value : doc.RefuseComment;
                                    command.Connection = conn;
                                    command.ExecuteNonQuery();
                                }
                               
                            }
                            logger.Info(string.Format("Документ организации {0} номер {1} отклонен по причине {2} с комментарием {3}", doc.SGuid.ToString("D"), doc.Number, doc.RefuseStatus, doc.RefuseStatus));
                            if (await CreateRefusionPacket(doc))
                            {
                                logger.Info(string.Format("Транспортный пакет по организации {0} номеру {1} успешно создан", doc.SGuid.ToString("D"), doc.Number));
                                return true;
                            }
                            else
                            {
                                logger.Fatal(string.Format("Ошибка создания транспортного пакета по организации {0} номеру {1}", doc.SGuid.ToString("D"), doc.Number));
                                return false;
                            }
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
            }
        }
        #endregion

        /// <summary>
        /// Изменение коментариев и иконок в СЕДе
        /// </summary>
        /// <param name="sourceGuid">Орган приславший документ</param>
        /// <param name="Number">Номер документа в СЕДе</param>
        /// <param name="comment">Коментарий к документу</param>
        /// <param name="sedIcon">Иконка документа</param>
        /// <returns></returns>
        public bool ChangeSedComment(Guid sourceGuid, string Number, string comment = null, SedCommentIcon sedIcon = SedCommentIcon.NoIcon)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(PakConnection))
                {
                    conn.Open();
                    List<Guid> trans_id = GetDocPendingId(sourceGuid, Number);
                    foreach (Guid g in trans_id)
                    {
                        SqlCommand command = new SqlCommand(@"dbo.UpdateDocPendComments", conn);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@PendID", SqlDbType.UniqueIdentifier).Value = g;
                        command.Parameters.Add("@Comment", SqlDbType.VarChar).Value = string.IsNullOrEmpty(comment) ? (object)DBNull.Value : comment;
                        command.Parameters.Add("@IconIndex", SqlDbType.Int).Value = sedIcon;
                        command.Connection = conn;
                        int commandsok = command.ExecuteNonQuery();
                    }
                    return true;
                }              
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
    }
}

