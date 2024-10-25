using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melecs.OracleDataBase.FIS
{
    public static class FisSimulator
    {
        #region Deklaration

        static bool isConnected = false;

        static ProcessAvailable currentProcess;

        static string processID;

        #endregion

        public static string DBUserName { get; set; }

        public static string DBPassword { get; set; }

        /// <summary>
        /// Enthält die Identifikationsnummer die bei den Events erzeugt und benötigt wird.
        /// </summary>
        public static Int64 AGID { private set; get; }

        static AssignData currentAssignData = null;

        /// <summary>
        /// Enthält alle Daten die bei einer Abfrage der Materialnummer oder des Auftrags zurückgegeben werden.
        /// </summary>
        public static AssignData CurrentAssignData 
        { 
            private set { currentAssignData = value;}
            get
            {
                if (currentAssignData == null)
                    currentAssignData = new AssignData();

                return currentAssignData;
            }
        }

        static Unit currentUnitData = null;

        /// <summary>
        /// Enthält alle Daten die bei einer Überprüfung des Ident zurückgegeben werden.
        /// </summary>
        public static Unit CurrentUnitData 
        {
            private set { currentUnitData = value; }
            get
            {
                if (currentUnitData == null)
                    currentUnitData = new Unit();

                return currentUnitData;
            }
        }

        static Operator currentOperatorData = null;

        /// <summary>
        /// Enthält die Prüferdaten.
        /// </summary>
        public static Operator CurrentOperatorData
        {
            private set { currentOperatorData = value; }
            get
            {
                if (currentOperatorData == null)
                    currentOperatorData = new Operator();

                return currentOperatorData;
            }
        }

        /// <summary>
        /// Hier stellt man den abfragenden Prozess ein. 
        /// </summary>
        public static ProcessAvailable CurrentProcess
        {
            set
            {
                currentProcess = value;
                switch (currentProcess)
                {
                    case ProcessAvailable.ICT:
                        processID = "ICT";
                        break;
                    case ProcessAvailable.EPE:
                        processID = "EPE";
                        break;
                    case ProcessAvailable.EPE2:
                        processID = "EPE2";
                        break;
                    case ProcessAvailable.VIS1:
                        processID = "VISUEL1";
                        break;
                    case ProcessAvailable.VIS2:
                        processID = "VISUEL2";
                        break;
                    case ProcessAvailable.MONT:
                        processID = "MONT";
                        break;
                    case ProcessAvailable.VERP:
                        processID = "VERP";
                        break;
                    case ProcessAvailable.BEST1:
                        processID = "BEST1";
                        break;
                    case ProcessAvailable.BEST2:
                        processID = "BEST2";
                        break;
                    default:
                        break;
                }
            }

            get { return currentProcess; }
        }

        /// <summary>
        /// Beinhaltet Fehlermeldungen der Datenbank.
        /// z.B. Vorprozessfehler, ...
        /// </summary>
        public static string ErrorMessage { set; get; }

        /// <summary>
        /// Hiermit kann man Daten der Zuordnungseinträge und der Ident-Einträge setzen.
        /// </summary>
        /// <param name="assignData">Die neuen Zuordnungseinträge</param>
        /// <param name="sIdentData">Die neuen Ident-Einträge</param>
        public static void UpdateData(AssignData assignData, Operator operatorData)
        {
            CurrentAssignData = assignData;
            CurrentOperatorData = operatorData;
        }

        /// <summary>
        /// Öffnet die Verbindung zur FIS-Datenbank.
        /// </summary>
        public static void DBConnect()
        {
            DBConnect(false);
        }

        /// <summary>
        /// Öffnet die Verbindung zur FIS-Datenbank.
        /// </summary>
        /// <param name="useTestDB">Mit true wird die TestDatenbank verbunden.</param>
        public static void DBConnect(bool useTestDB)
        {
            try
            {
                if (!IsConnected)
                {
                    Random tmpRnd = new Random(DateTime.Now.Millisecond);
                    if (tmpRnd.Next(20) == 1)
                        throw new Exception("Generated TestException");

                    IsConnected = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("DBConnect: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Schließt die Verbindung zur FIS-Datenbank.
        /// </summary>
        public static void DBDisconnect()
        {
            try
            {
                if (IsConnected)
                {
                    Random tmpRnd = new Random(DateTime.Now.Millisecond);
                    if (tmpRnd.Next(20) == 1)
                        throw new Exception("Generated TestException");

                    IsConnected = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("DBDisconnect: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Zeigt an ob die Verbindung zur FIS-Datenbank geöffnet ist oder nicht.
        /// </summary>
        public static bool IsConnected
        {
            private set
            {
                isConnected = value;
            }

            get
            {
                return isConnected;
            }
        }

        /// <summary>
        /// Mit dieser Funktion werden Prüferdaten zu der angegebenen Prüfernummer abgefragt.
        /// </summary>
        /// <param name="operatorNumber">Die mit der FIS-Datenbank zu überprüfende Prüfernummer.</param>
        /// <returns></returns>
        public static bool CheckPruefer(string operatorNumber)
        {
            try
            {
                List<Operator> oplist = new List<Operator>();
                oplist.Add(new Operator() { PrueferNr = "0010", Vorname = "Maximilian", Nachname = "Schiefer", Schicht = "A" });
                oplist.Add(new Operator() { PrueferNr = "0011", Vorname = "Markus", Nachname = "Hirter", Schicht = "C" });
                oplist.Add(new Operator() { PrueferNr = "0012", Vorname = "Hilde", Nachname = "Jäger", Schicht = "A" });
                oplist.Add(new Operator() { PrueferNr = "0013", Vorname = "Claudia", Nachname = "Schuster", Schicht = "B" });
                oplist.Add(new Operator() { PrueferNr = "0014", Vorname = "Nils", Nachname = "Müller", Schicht = "B" });
                oplist.Add(new Operator() { PrueferNr = "0015", Vorname = "Josef", Nachname = "Fischer", Schicht = "B" });
                oplist.Add(new Operator() { PrueferNr = "0016", Vorname = "Wolfgang", Nachname = "Jagschits", Schicht = "A" });
                oplist.Add(new Operator() { PrueferNr = "0017", Vorname = "Herbert", Nachname = "Erdinger", Schicht = "C" });
                oplist.Add(new Operator() { PrueferNr = "0018", Vorname = "Oliver", Nachname = "Grill", Schicht = "A" });
                oplist.Add(new Operator() { PrueferNr = "0019", Vorname = "Helena", Nachname = "Woodland", Schicht = "B" });

                if (!IsConnected) { throw new Exception(string.Format("{0}", Language.NoConnection)); }

                Operator tInspector = null;
                foreach (Operator listitem in oplist)
                {
                    if (listitem.PrueferNr == operatorNumber)
                        tInspector = listitem;
                }

                if (tInspector == null)
                    throw new Exception(string.Format("The Operator ID {0} is not valid", operatorNumber));

                CurrentOperatorData = tInspector;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CheckPruefer: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Mit dieser Funktion werden die Ident Daten zurück gesetzt.
        /// </summary>
        public static void ResetIdentData()
        {
            CurrentUnitData = new Unit();
        }

        /// <summary>
        /// Mit dieser Funktion wird der Vorprozess der Baugruppe überprüft.
        /// Bei vorhandenem Vorprozess werden die Baugruppenspezifischen Daten und Zuordnungsdaten der Baugruppe verfügbar.
        /// </summary>
        /// <param name="ident">Der mit der FIS-Datenbank zu überprüfende Ident.</param>"
        /// <returns></returns>
        public static bool CheckIdent(string ident)
        {
            try
            {
                if (!IsConnected) { throw new Exception(string.Format("{0}", Language.NoConnection)); }

                Random tmpRnd = new Random(DateTime.Now.Millisecond);

                if (tmpRnd.Next(20) == 1)
                    throw new Exception("Generated TestException");

                CurrentUnitData = new Unit();
                CurrentUnitData.Ident = ident;

                Unit tIdentData = new Unit();

                tIdentData.Ident = ident;
                tIdentData.G_Materialnummer = "0010161656";
                tIdentData.MLFB = "01254FFD";
                tIdentData.MLFB_Index = "01";
                tIdentData.BSH_Materialnummer = "900100200";
                tIdentData.BSH_Materialnummer_Kunde = "900212514";
                tIdentData.Verpackungseinheit = "SE20";
                tIdentData.SW_SYC = "asfsf.syc";
                tIdentData.EEP_File = "fasdf.eep";
                tIdentData.SW_VER = "01";
                tIdentData.SW_Index = "2";
                tIdentData.Benennung = "adfs asfdj salfd ";
                tIdentData.Typ = "Minix";
                tIdentData.F_Materialnummer = "0010161658";
                tIdentData.Approbationstype = "???";
                tIdentData.Kommentar = "???";
                tIdentData.Verfahrenstechnik = "???";
                tIdentData.Eingabetext = "???";

                switch (CurrentProcess)
                {
                    case ProcessAvailable.ICT:
                        tIdentData.ICT_PRG = "asfs.v01";
                        tIdentData.ICT_PRG_VAR = "v01";
                        tIdentData.ICT_ADAP = "001";
                        break;
                    case ProcessAvailable.EPE:
                        tIdentData.EPE_PRG = "???";
                        tIdentData.EPE_PRG_VAR = "asdfasfds.xml";
                        tIdentData.EPE_MDATA = "???";
                        tIdentData.EPE_ADAP = "9090";
                        break;
                    case ProcessAvailable.EPE2:
                        tIdentData.EPE_PRG = "???";
                        tIdentData.EPE_PRG_VAR = "adfffffde.xml";
                        tIdentData.EPE_MDATA = "???";
                        tIdentData.EPE_ADAP = "1250";
                        break;
                    case ProcessAvailable.VIS1:
                        break;
                    case ProcessAvailable.VIS2:
                        break;
                    case ProcessAvailable.MONT:
                        break;
                    case ProcessAvailable.VERP:
                        break;
                    case ProcessAvailable.BEST1:
                        break;
                    case ProcessAvailable.BEST2:
                        break;
                    default:
                        break;
                }

                tIdentData.Durchlauf = "01";
                tIdentData.Auftrag = "78001542";
                tIdentData.Stueckzahl = "40";
                tIdentData.Barcodetyp = "EINZEL";
                tIdentData.Sachnummer = "asdffsfdffd6546";

                CurrentUnitData = tIdentData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CheckIdent: {0}", ex.Message));
            }

        }

        /// <summary>
        /// Mit dieser Funktion werden die Zuordnungsdaten zu der angegebenen Materialnummer abgefragt.
        /// </summary>
        /// <param name="sIdent">Die mit der FIS-Datenbank zu überprüfende Materialnummer.</param>"
        /// <returns></returns>
        public static bool GetMaterialnummer(string materialNumber)
        {
            try
            {
                if (!IsConnected) { throw new Exception(string.Format("{0}", Language.NoConnection)); }

                Random tmpRnd = new Random(DateTime.Now.Millisecond);

                if (tmpRnd.Next(20) == 1)
                    throw new Exception("Generated TestException");

                CurrentAssignData = new AssignData();
                AssignData tAssignData = new AssignData();

                tAssignData.Auftrag = "";
                tAssignData.G_Materialnummer = materialNumber;
                tAssignData.MLFB = "01254FFD";
                tAssignData.MLFB_Index = "01";
                tAssignData.BSH_Materialnummer = "900100200";
                tAssignData.BSH_Materialnummer_Kunde = "900212514";
                tAssignData.Verpackungseinheit = "SE20";
                tAssignData.SW_SYC = "asfsf.syc";
                tAssignData.EEP_File = "fasdf.eep";
                tAssignData.SW_VER = "01";
                tAssignData.SW_Index = "2";
                tAssignData.Benennung = "adfs asfdj salfd ";
                tAssignData.Typ = "Minix";
                tAssignData.F_Materialnummer = "0010161658";
                tAssignData.Approbationstype = "???";
                tAssignData.Kommentar = "???";
                tAssignData.Verfahrenstechnik = "???";
                tAssignData.Eingabetext = "???";

                switch (CurrentProcess)
                {
                    case ProcessAvailable.ICT:
                        tAssignData.ICT_PRG = "asfs.v01";
                        tAssignData.ICT_PRG_VAR = "v01";
                        tAssignData.ICT_ADAP = "001";
                        break;
                    case ProcessAvailable.EPE:
                        tAssignData.EPE_PRG = "???";
                        tAssignData.EPE_PRG_VAR = "asdfasfds.xml";
                        tAssignData.EPE_MDATA = "???";
                        tAssignData.EPE_ADAP = "9090";
                        break;
                    case ProcessAvailable.EPE2:
                        tAssignData.EPE_PRG = "???";
                        tAssignData.EPE_PRG_VAR = "adfffffde.xml";
                        tAssignData.EPE_MDATA = "???";
                        tAssignData.EPE_ADAP = "1250";
                        break;
                    case ProcessAvailable.VIS1:
                        break;
                    case ProcessAvailable.VIS2:
                        break;
                    case ProcessAvailable.MONT:
                        break;
                    case ProcessAvailable.VERP:
                        break;
                    case ProcessAvailable.BEST1:
                        break;
                    case ProcessAvailable.BEST2:
                        break;
                    default:
                        break;
                }

                CurrentAssignData = tAssignData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetMaterialnummer: {0}", ex.Message));
            }

        }

        /// <summary>
        /// Mit dieser Funktion werden die Zuordnungsdaten zu dem angegebenen Auftrag abgefragt.
        /// </summary>
        /// <param name="sIdent">Der mit der FIS-Datenbank zu überprüfende Auftrag.</param>"
        /// <returns></returns>
        public static bool GetAuftrag(string orderNumber)
        {
            try
            {
                if (!IsConnected) { throw new Exception(string.Format("{0}", Language.NoConnection)); }

                Random tmpRnd = new Random(DateTime.Now.Millisecond);

                if (tmpRnd.Next(20) == 1)
                    throw new Exception("Generated TestException");

                CurrentAssignData = new AssignData();
                AssignData tAssignData = new AssignData();

                //string tRetVal = null;

                tAssignData.Auftrag = orderNumber;
                tAssignData.G_Materialnummer = "0010161656";
                tAssignData.MLFB = "01254FFD";
                tAssignData.MLFB_Index = "01";
                tAssignData.BSH_Materialnummer = "900100200";
                tAssignData.BSH_Materialnummer_Kunde = "900212514";
                tAssignData.Verpackungseinheit = "SE20";
                tAssignData.SW_SYC = "asfsf.syc";
                tAssignData.EEP_File = "fasdf.eep";
                tAssignData.SW_VER = "01";
                tAssignData.SW_Index = "2";
                tAssignData.Benennung = "adfs asfdj salfd ";
                tAssignData.Typ = "Minix";
                tAssignData.F_Materialnummer = "0010161658";
                tAssignData.Approbationstype = "???";
                tAssignData.Kommentar = "???";
                tAssignData.Verfahrenstechnik = "???";
                tAssignData.Eingabetext = "???";

                switch (CurrentProcess)
                {
                    case ProcessAvailable.ICT:
                        tAssignData.ICT_PRG = "asfs.v01";
                        tAssignData.ICT_PRG_VAR = "v01";
                        tAssignData.ICT_ADAP = "001";
                        break;
                    case ProcessAvailable.EPE:
                        tAssignData.EPE_PRG = "???";
                        tAssignData.EPE_PRG_VAR = "asdfasfds.xml";
                        tAssignData.EPE_MDATA = "???";
                        tAssignData.EPE_ADAP = "9090";
                        break;
                    case ProcessAvailable.EPE2:
                        tAssignData.EPE_PRG = "???";
                        tAssignData.EPE_PRG_VAR = "adfffffde.xml";
                        tAssignData.EPE_MDATA = "???";
                        tAssignData.EPE_ADAP = "1250";
                        break;
                    case ProcessAvailable.VIS1:
                        break;
                    case ProcessAvailable.VIS2:
                        break;
                    case ProcessAvailable.MONT:
                        break;
                    case ProcessAvailable.VERP:
                        break;
                    case ProcessAvailable.BEST1:
                        break;
                    case ProcessAvailable.BEST2:
                        break;
                    default:
                        break;
                }

                tAssignData.Sachnummer = "asdf54746df";

                CurrentAssignData = tAssignData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetAuftrag: {0}", ex.Message));
            }

        }

        /// <summary>
        /// Mit dieser Funktion wird eine Seriennummer von ein Nummernstreck von der FIS Datenbank angefordert.
        /// </summary>
        /// <param name="fnvID">Ist die Bezeichnung der Nummernstrecke.</param>
        /// <param name="formatID">Ist die Bezeichnung für der Formatierung.</param>
        /// <param name="result">Gibt die Seriennummer zurück.</param>
        /// <returns></returns>
        public static bool GetSeriennummer(string fnvID, string formatID, out string result)
        {
            try
            {
                if (!IsConnected) { throw new Exception(string.Format("{0}", Language.NoConnection)); }

                Random tmpRnd = new Random(DateTime.Now.Millisecond);

                if (tmpRnd.Next(20) == 1)
                    throw new Exception("Generated TestException");

                result = tmpRnd.Next(1000).ToString().PadLeft(10, '0');

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetSeriennummer: {0}", ex.Message));
            }

        }

        /// <summary>
        /// Mit dieser Funktion wird der Anfangseventeintrag in die FIS-Datenbank geschrieben.
        /// Wird am Anfang einer Prüfung geschrieben.
        /// </summary>
        /// <param name="ident">Der in die FIS-Datenbank einzutragende Ident.</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <returns></returns>
        public static bool EventStart(string ident, string mid)
        {
            try
            {
                if (!IsConnected) { throw new Exception(string.Format("{0}", Language.NoConnection)); }

                if (ident == "" | ident == null) { throw new Exception(string.Format("{0}", Language.NoIdent)); }
                
                Random tmpRnd = new Random(DateTime.Now.Millisecond);

                if (tmpRnd.Next(20) == 1)
                    throw new Exception("Generated TestException");

                Int64 tAGID = (Int64)tmpRnd.Next(1000000);

                if (tmpRnd.Next(50) == 1)
                    return false;

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventStart: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Mit dieser Funktion wird der Fehleventeintrag in die FIS-Datenbank geschrieben.
        /// Dies sollte nur gemacht werden wenn ein Fehler bei einer Prüfung aufgetreten ist.
        /// </summary>
        /// <param name="ident">Der in die FIS-Datenbank einzutragende Ident.</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <param name="errorText">Der Fehler wird in Form eines Textes hinterlegt.</param>
        /// <param name="errorNumber">Die Nummer des Testschrittes bei welchem die Baugruppe ausgefallen ist bzw. wo ein Fehler erkannt wurde.</param>
        /// <returns></returns>
        public static bool EventError(string ident, string mid, string errorText, string errorNumber)
        {
            try
            {
                if (!IsConnected) { throw new Exception(string.Format("{0}", Language.NoConnection)); }

                if (ident == "" | ident == null) { throw new Exception(string.Format("{0}", Language.NoIdent)); }

                Random tmpRnd = new Random(DateTime.Now.Millisecond);

                if (tmpRnd.Next(50) == 1)
                    throw new Exception("Generated TestException");

                Int64 tAGID = (Int64)tmpRnd.Next(1000000);

                if (tmpRnd.Next(50) == 1)
                    return false;

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventError: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Mit dieser Funktion wird der Endeventeintrag in die FIS-Datenbank geschrieben.
        /// Wird am Ende eine Prüfung geschrieben um das Event zu schließen.
        /// </summary>
        /// <param name="ident">Der in die FIS-Datenbank einzutragende Ident.</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <returns></returns>
        public static bool EventEnd(string ident, string mid)
        {
            try
            {
                if (!IsConnected) { throw new Exception(string.Format("{0}", Language.NoConnection)); }

                if (ident == "" | ident == null) { throw new Exception(string.Format("{0}", Language.NoIdent)); }

                Random tmpRnd = new Random(DateTime.Now.Millisecond);

                if (tmpRnd.Next(50) == 1)
                    throw new Exception("Generated TestException");

                Int64 tAGID = (Int64)tmpRnd.Next(1000000);

                if (tmpRnd.Next(50) == 1)
                    return false;

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventEnd: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Diese Funktion gibt die Version der Oracle.dll an.
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            Random tmpRnd = new Random(DateTime.Now.Millisecond);

            return string.Format("1.{0}.{1}", tmpRnd.Next(99).ToString(), tmpRnd.Next(99).ToString());
        }
    }
}
