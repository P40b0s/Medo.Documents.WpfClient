using medo2_5;
using medo2_7;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XmlCardCreator
{
  public class MedoDeserializer
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public communication LoadMedo2_5(FileInfo file)
        {
            try
            {
                communication com = null;                
                communicationDocument doc = null;
                Notification not = null;
                XmlSerializer serializer = new XmlSerializer(typeof(communication));
                using (StreamReader sr = new StreamReader(file.FullName, Encoding.Default))
                {
                    com = (communication)serializer.Deserialize(sr);
                }
                try
                {
                    doc = (communicationDocument)com.Item;                    
                }
                catch (System.InvalidCastException ex)
                {
                    not = (Notification)com.Item;                    
                }
                return com;
            }
            catch (Exception ex) { logger.Fatal(ex); return null; }
        }


        public container LoadMedo2_7(FileInfo file)
        {
            try
            {                
                container con = null;                
                XmlSerializer serializer = new XmlSerializer(typeof(container));
                using (StreamReader sr = new StreamReader(file.FullName, Encoding.Default))
                {
                    con = (container)serializer.Deserialize(sr);
                }              
                return con;
            }
            catch (Exception ex) { logger.Fatal(ex); return null; }
        }
    }
}
