using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Helpers
{
    public static class Methods
    {
        public static string getRequisiteHash(DateTime? SignDate, string ActType, string OrganName, string DocumentNumber)
        {
            try
            {
                string returnString = null;
                if (SignDate != null && ActType != null && OrganName != null && DocumentNumber != null)
                {
                    string date = SignDate.Value.ToString("dd.MM.yyyy");
                    var str = new System.Text.StringBuilder();
                    str.Append(date);

                    str.Append(DocumentNumber.Replace(" ", "").ToUpper());
                    //изменить название органа как в издании, чтобы получить такой же хеш как в издании
                    str.Append(OrganName.Replace(" ", "").ToUpper());
                    str.Append(ActType.Replace(" ", "").ToUpper());
                    string input = str.ToString();

                    using (var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider())
                    {
                        byte[] hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                        var sb = new System.Text.StringBuilder();
                        foreach (byte b in hash)
                        {
                            sb.AppendFormat("{0:x2}", b);
                        }
                        returnString = sb.ToString();
                    }
                }
                return returnString;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
    }
}
