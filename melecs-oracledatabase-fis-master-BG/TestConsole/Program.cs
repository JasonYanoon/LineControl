using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Melecs.OracleDataBase.FIS;
using System.Threading;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            DBCom fis = null;

            try
            {

                fis = new DBCom("fisfema", "fis1fe2+ma", DataSource.EWW);
                fis.DBConnect();

                string tmpstr = string.Empty;

                DateTime dateTime = new DateTime();

                string str="";

                string smtIdentt = "W09HZFW";
                string gIdentt = "W09R5B4";
                string fMID = "LASER-22";
                string sMID = "EOL25EP1";

                fis.GetIdentInfo(gIdentt, ObjectTypes.All, ref tmpstr);

                Console.WriteLine(tmpstr);



                List<string> list = new List<string>();

                fis.CheckIdent(gIdentt, out list);

                var passedIdents = fis.GetPassedIdentsForLAP("LASER-22");

                string ss= fis.GetAlleVorprozesse(gIdentt, sMID);
                fis.GetAlleVorprozesse(gIdentt, "SMT44-B");

                fis.CheckVorprozess(gIdentt, fMID, ApTypeAvailable.LAPLIST, ref dateTime, ref str);

                fis.CheckVorprozess(gIdentt, "", ApTypeAvailable.LAPLIST);

                fis.GetIdentInfo(gIdentt);
                fis.GetIdent(gIdentt);
                fis.GetMetaData(gIdentt);

                string ident = "00IG91M";

                DBCom.EMFData eMF = new DBCom.EMFData();

                List<string> Process = new List<string>() { "MONTAGE", "ICT", "NUTZENTRENNER" };

                foreach (string process in Process)
                {
                    fis.EMFGetIntervall(gIdentt, process, out eMF);

                    string type = "";

                    if (eMF.IntervallCount > 0 && eMF.IntervallTime > 0)
                        type = "Time and Pcs";
                    else if (eMF.IntervallCount > 0)
                        type = "Pcs";
                    else if (eMF.IntervallTime > 0)
                        type = "Time";
                    else
                        type = "Error!";

                    Console.WriteLine(string.Format("Process {0} ist {1}", process, type));
                }

                var openOrder = fis.GetOpenOrdersForMaterialNumber("0010503987");
                var resultOfPackaged = fis.GetIdentsFromOrder("70180777", "T");
                var resultOfNotPackaged = fis.GetIdentsFromOrder("70180777", "F");
                var resultOfAllIdents = fis.GetIdentsFromOrder("70180777");
                //var passedIdents = fis.GetPassedIdentsForLAP("VSMT-TOP");

                fis.DBDisconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }

            Console.ReadLine();
        }
    }
}
