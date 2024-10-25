using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Melecs.OracleDataBase.FIS
{
    using System.Globalization;

    using Oracle = OracleDB.Oracle;

    public class DBCom
    {
        #region Deklaration

        Oracle oracleDB = null;

        const string delimiter = "\r";

        const int errorValue = -99999;

        ProcessAvailable currentProcess;

        string processID;

        #endregion

        #region Properties

        public string DBUserName { get; set; }

        public string DBPassword { get; set; }

        public bool UseTestDB { get; set; }

        /// <summary>
        /// Fis Delimiter (Konstante)
        /// </summary>
        public string Delimiter
        {
            get
            {
                return delimiter;
            }
        }

        /// <summary>
        /// 事件生成标识符
        /// Enthält die Identifikationsnummer die bei den Events erzeugt und benötigt wird.
        /// </summary>
        public Int64 AGID { set; get; }

        /// <summary>
        /// 物料标号或者查询订单时返回的所有数据
        /// Enthält alle Daten die bei einer Abfrage der Materialnummer oder des Auftrags zurückgegeben werden.
        /// </summary>
        public AssignData CurrentAssignData { set; get; }

        /// <summary>
        /// 查询标识符时返回的所有数据
        /// Enthält alle Daten die bei einer Überprüfung des Ident zurückgegeben werden.
        /// </summary>
        public Unit CurrentUnitData { set; get; }

        /// <summary>
        /// 返回的操作员信息
        /// Enthält die Prüferdaten.
        /// </summary>
        public Operator CurrentOperatorData { set; get; }

        /// <summary>
        /// 当前要连接的数据库源
        /// The current data source to connect.
        /// </summary>
        public Oracle.DataSources CurrentDataSource { private set; get; }

        /// <summary>
        /// 设置的查询流程
        /// Hier stellt man den abfragenden Prozess ein. 
        /// </summary>
        public ProcessAvailable CurrentProcess
        {
            set
            {
                this.currentProcess = value;
                switch (this.currentProcess)
                {
                    case ProcessAvailable.ICT:
                        this.processID = "ICT";
                        break;
                    case ProcessAvailable.EPE:
                        this.processID = "EPE";
                        break;
                    case ProcessAvailable.EPE2:
                        this.processID = "EPE2";
                        break;
                    case ProcessAvailable.VIS1:
                        this.processID = "VISUELL1";
                        break;
                    case ProcessAvailable.VIS2:
                        this.processID = "VISUELL2";
                        break;
                    case ProcessAvailable.MONT:
                        this.processID = "MONT";
                        break;
                    case ProcessAvailable.VERP:
                        this.processID = "VERP";
                        break;
                    case ProcessAvailable.BEST1:
                        this.processID = "BEST1";
                        break;
                    case ProcessAvailable.BEST2:
                        this.processID = "BEST2";
                        break;
                    default:
                        break;
                }
            }

            get { return this.currentProcess; }
        }

        /// <summary>
        /// 查询返回的所有错误信息
        /// Beinhaltet Fehlermeldungen der Datenbank.
        /// z.B. Vorprozessfehler, ...
        /// </summary>
        public string ErrorMessage { set; get; }

        /// <summary>
        /// Zeigt an ob die Verbindung zur FIS-Datenbank geöffnet ist oder nicht.
        /// </summary>
        public bool IsConnected
        {
            set
            {

            }

            get
            {
                return true;
            }
        }

        #endregion

        #region UpdateData Function

        /// <summary>
        /// 更改订单数据
        /// Hiermit kann man Daten der Zuordnungseinträge und der Ident-Einträge setzen.
        /// </summary>
        /// <param name="assignData">Die neuen Zuordnungseinträge</param>
        /// <param name="operatorData">Die neuen Bedienereinträge</param>
        public void UpdateData(AssignData assignData, Operator operatorData)
        {
            this.CurrentAssignData = assignData;
            this.CurrentOperatorData = operatorData;
        }

        /// <summary>
        /// Hiermit kann man Daten der Zuordnungseinträge und der Ident-Einträge setzen.
        /// </summary>
        /// <param name="assignData">Die neuen Zuordnungseinträge</param>
        public void UpdateData(AssignData assignData)
        {
            this.CurrentAssignData = assignData;
        }

        /// <summary>
        /// Hiermit kann man Daten des Bedieners setzen.
        /// 可用于设置操作员数据。
        /// </summary>
        /// <param name="operatorData">Die neuen Bedienerdaten</param>
        public void UpdateData(Operator operatorData)
        {
            this.CurrentOperatorData = operatorData;
        }

        #endregion

        #region Connections

        /// <summary>
        /// Öffnet die Verbindung zur FIS-Datenbank.
        /// </summary>
        public void DBConnect()
        {
            this.DBConnect(false);
        }

        /// <summary>
        /// Öffnet die Verbindung zur FIS-Datenbank.
        /// </summary>
        /// <param name="useTestDB">Mit true wird die TestDatenbank verbunden.</param>
        public void DBConnect(bool useTestDB)
        {
            this.UseTestDB = useTestDB;
        }

        /// <summary>
        /// Schließt die Verbindung zur FIS-Datenbank.
        /// </summary>
        public void DBDisconnect()
        {
        }

        #endregion

        #region EMF

        /// <summary>
        /// 电磁场数据的结构体
        /// Struktur für die EMF Daten.
        /// </summary>
        public struct EMFData
        {
            /// <summary>
            /// Der Ident der abgefragt wurde.
            /// </summary>
            public string Ident { get; set; }

            /// <summary>
            /// Die Auftragsnummer des Idents.
            /// </summary>
            public string Ordernumber { get; set; }

            /// <summary>
            /// Die Materialnummer des Idents.
            /// </summary>
            public string Materialnumber { get; set; }

            /// <summary>
            /// Die Versionsnummer des EMF Fragebogens.
            /// EMF 问卷的版本号
            /// </summary>
            public string Versionnumber { get; set; }

            /// <summary>
            /// 显示 EMF 问卷的时间限制
            /// Das Zeitlimit nach dem der EMF Fragebogen angezeigt werden soll.
            /// </summary>
            public int IntervallTime { get; set; }

            /// <summary>
            /// 显示 EMF 问卷的时间间隔限制
            /// Das Intervall Limit bei dem der EMF Fragebogen angezeigt werden soll.
            /// </summary>
            public int IntervallCount { get; set; }

            /// <summary>
            /// 设置间隔类型
            /// Der eingestellte Intervall Typ.
            /// </summary>
            public IntervallType UsedIntervallType { get; set; }

            public enum IntervallType
            {
                Time,
                Count,
                Both
            }

            /// <summary>
            /// 问题目录类型
            /// Typ des Fragenkataloges
            /// </summary>
            public string Process { get; set; }
        }

        /// <summary>
        /// 查询初始样本释放的时间
        /// Diese Funktion gibt das Intervall zurück, in welchem das Erstmusterfreigabe Formular angezeigt werden soll.
        /// </summary>
        /// <param name="ident">Der Ident zu dem das Intervall abgefragt werden soll.</param>
        /// <param name="data">Die Daten, welche das Intervall beinhalten.</param>
        /// <returns></returns>
        public bool EMFGetIntervall(string ident, out EMFData data)
        {
            try
            {
                data = new EMFData();

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string tRetVal = null;
                tRetVal = oracleDB.GetIntervall(ident);

                if (tRetVal.Length <= 0)
                {
                    this.ErrorMessage = string.Format("{0}", Language.NoReturnValue);
                    return false;
                }

                if (tRetVal.Contains("ORA-"))
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                string outValue = string.Empty;
                int intervall = 0;

                data.Ident = oracleDB.SPLITTER(tRetVal, delimiter, "IDENT", out outValue);
                data.Ordernumber = oracleDB.SPLITTER(tRetVal, delimiter, "ORDERNUMBER", out outValue);
                data.Materialnumber = oracleDB.SPLITTER(tRetVal, delimiter, "MATERIALNUMBER", out outValue);
                data.Versionnumber = oracleDB.SPLITTER(tRetVal, delimiter, "VERSIONNUMBER", out outValue);

                int.TryParse(oracleDB.SPLITTER(tRetVal, delimiter, "INTERVALLTIME", out outValue), out intervall);
                data.IntervallTime = intervall;

                int.TryParse(oracleDB.SPLITTER(tRetVal, delimiter, "INTERVALLCOUNT", out outValue), out intervall);
                data.IntervallCount = intervall;

                if (data.IntervallTime > 0 && data.IntervallCount > 0)
                {
                    data.UsedIntervallType = EMFData.IntervallType.Both;
                }
                else if (data.IntervallCount <= 0 && data.IntervallTime > 0)
                {
                    data.UsedIntervallType = EMFData.IntervallType.Time;
                }
                else if (data.IntervallTime <= 0 && data.IntervallCount > 0)
                {
                    data.UsedIntervallType = EMFData.IntervallType.Count;
                }
                else
                {
                    this.ErrorMessage = string.Format("{0}", Language.WrongEMFType);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EMFGetIntervall: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 查询初始样本释放的时间
        /// Diese Funktion gibt das Intervall zurück, in welchem das Erstmusterfreigabe Formular angezeigt werden soll.
        /// </summary>
        /// <param name="ident">Der Ident zu dem das Intervall abgefragt werden soll.</param>
        /// <param name="data">Die Daten, welche das Intervall beinhalten.</param>
        /// <param name="process">Der Prozess der abgefragt werden soll.</param>
        /// <returns></returns>
        public bool EMFGetIntervall(string ident, string process, out EMFData data)
        {
            try
            {
                data = new EMFData();

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string tRetVal = null;
                tRetVal = oracleDB.GetIntervall(ident, process);

                if (tRetVal.Length <= 0)
                {
                    this.ErrorMessage = string.Format("{0}", Language.NoReturnValue);
                    return false;
                }

                if (tRetVal.Contains("ORA-"))
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                string outValue = string.Empty;
                int intervall = 0;

                data.Ident = oracleDB.SPLITTER(tRetVal, delimiter, "IDENT", out outValue);
                data.Ordernumber = oracleDB.SPLITTER(tRetVal, delimiter, "ORDERNUMBER", out outValue);
                data.Materialnumber = oracleDB.SPLITTER(tRetVal, delimiter, "MATERIALNUMBER", out outValue);
                data.Versionnumber = oracleDB.SPLITTER(tRetVal, delimiter, "VERSIONNUMBER", out outValue);

                int.TryParse(oracleDB.SPLITTER(tRetVal, delimiter, "INTERVALLTIME", out outValue), out intervall);
                data.IntervallTime = intervall;

                int.TryParse(oracleDB.SPLITTER(tRetVal, delimiter, "INTERVALLCOUNT", out outValue), out intervall);
                data.IntervallCount = intervall;

                if (data.IntervallTime > 0 && data.IntervallCount > 0)
                {
                    data.UsedIntervallType = EMFData.IntervallType.Both;
                }
                else if (data.IntervallCount <= 0 && data.IntervallTime > 0)
                {
                    data.UsedIntervallType = EMFData.IntervallType.Time;
                }
                else if (data.IntervallTime <= 0 && data.IntervallCount > 0)
                {
                    data.UsedIntervallType = EMFData.IntervallType.Count;
                }
                else
                {
                    this.ErrorMessage = string.Format("{0}", Language.WrongEMFType);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EMFGetIntervall: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// This function is giving back the inverval for First Article Inspection.
        /// 该功能将返回首件检查的平均值。
        /// </summary>
        /// <param name="materialNumber">Method is returning the inveral (count and/or time based) for the given material number</param>
        /// <param name="data">Output parameter to get the all the EMF data. UsedIntervallType property is for the intervall value.</param>
        /// <returns>Returned value is showing the success of the oracle database query</returns>
        public bool EMFGetIntervallFromMaterialNumber(string materialNumber, out EMFData data)
        {
            try
            {
                data = new EMFData();

                if (materialNumber == "" | materialNumber == null)
                {
                    this.ErrorMessage = Language.NoParameterValue;
                    return false;
                }

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string tRetVal = null;
                tRetVal = oracleDB.GetIntervallFromMaterialNumber(materialNumber);

                if (tRetVal.Length <= 0)
                {
                    this.ErrorMessage = string.Format("{0}", Language.NoReturnValue);
                    return false;
                }

                if (tRetVal.Contains("ORA-"))
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                string outValue = string.Empty;
                int intervall = 0;

                data.Ident = oracleDB.SPLITTER(tRetVal, delimiter, "IDENT", out outValue);
                data.Ordernumber = oracleDB.SPLITTER(tRetVal, delimiter, "ORDERNUMBER", out outValue);
                data.Materialnumber = oracleDB.SPLITTER(tRetVal, delimiter, "MATERIALNUMBER", out outValue);
                data.Versionnumber = oracleDB.SPLITTER(tRetVal, delimiter, "VERSIONNUMBER", out outValue);

                int.TryParse(oracleDB.SPLITTER(tRetVal, delimiter, "INTERVALLTIME", out outValue), out intervall);
                data.IntervallTime = intervall;

                int.TryParse(oracleDB.SPLITTER(tRetVal, delimiter, "INTERVALLCOUNT", out outValue), out intervall);
                data.IntervallCount = intervall;

                if (data.IntervallTime > 0 && data.IntervallCount > 0)
                {
                    data.UsedIntervallType = EMFData.IntervallType.Both;
                }
                else if (data.IntervallCount <= 0 && data.IntervallTime > 0)
                {
                    data.UsedIntervallType = EMFData.IntervallType.Time;
                }
                else if (data.IntervallTime <= 0 && data.IntervallCount > 0)
                {
                    data.UsedIntervallType = EMFData.IntervallType.Count;
                }
                else
                {
                    this.ErrorMessage = string.Format("{0}", Language.WrongEMFType);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EMFGetIntervall: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 检查工站是否被磁场锁定
        /// This function is for checking a workplace whether it was locked from a EMF questionnaire.
        /// </summary>
        /// <param name="mid">The workplace to be checked.</param>
        /// <param name="lockState">The lock state of the workplace.</param>
        /// <returns></returns>
        public bool EMFGetWorkPlaceLock(string mid, out EMFWorkplaceStates lockState)
        {
            try
            {
                lockState = EMFWorkplaceStates.Locked;

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string tRetVal = oracleDB.GetWorkplaceLock(mid);

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                lockState = (EMFWorkplaceStates)Convert.ToInt32(oracleDB.EMFWorkplaceState);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EMFGetWorkPlaceLock: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// After a workplace was unlocked, then the EMF is shown and if the result is true,
        /// then this function has to be executed.
        /// 工作场所解锁后，必须执行此功能。
        /// </summary>
        /// <param name="mid">The workplace.</param>
        /// <returns></returns>
        public bool EMFSetRetest(string mid)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string tRetVal = oracleDB.UpdateCheckTime(mid);

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EMFSetRetest: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        /// <summary>
        /// 该函数用于从产品参数分配表中查询指定参数的相应值。
        /// Mit dieser Funktion wird zu einem angegebenen Parameter der passende Wert aus der Produkt-Parameter-Zuordungstabelle abgefragt.
        /// </summary>
        /// <param name="materialNumber">Die Materialnummer, welche abgefragt werden soll.</param>
        /// <param name="parameter">Der Name des Parameters, welcher abgefragt werden soll.</param>
        /// <param name="value">Der Wert der diesem Parameter zugeordnet ist.</param>
        /// <returns></returns>
        public bool GetProduktParameter(string materialNumber, string parameter, out string value)
        {
            try
            {
                string errMsg = "";
                string retVal = "";
                string[] result = null;

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                value = "";
                materialNumber = materialNumber.PadLeft(10, '0');

                errMsg = oracleDB.GET_PRODUKT_PARAMETER(materialNumber, parameter, ref retVal);

                result = retVal.Split('\r');

                if (errMsg.Length > 0)
                {
                    this.ErrorMessage = errMsg;
                    return false;
                }

                if (result.Length < 2)
                {
                    this.ErrorMessage = string.Format("{0} \"Parameter={1}\"", Language.NoParameterValue, parameter);
                    return false;
                }

                if (result[0].Split(' ')[0].Split('=')[1] != materialNumber)
                {
                    this.ErrorMessage = string.Format("{0} \"ReturnMatNr={1}\"", Language.WrongMatNr, result[0].Split(' ')[0].Split('=')[1]);
                    return false;
                }

                if (result[0].Split(' ')[1].Split('=')[1] != parameter)
                {
                    this.ErrorMessage = string.Format("{0} \"Parameter={1}\"", Language.WrongParameter, result[0].Split(' ')[1].Split('=')[1]);
                    return false;
                }

                value = result[1].Split('=')[1].Split(';')[0]; // If there are more than one values in a row, just take the first
                value = value == "-" ? "" : value;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetProduktParameter: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 该函数用于查询指定检查员编号的检查员数据。
        /// Mit dieser Funktion werden Prüferdaten zu der angegebenen Prüfernummer abgefragt.
        /// </summary>
        /// <param name="operatorNumber">Die mit der FIS-Datenbank zu überprüfende Prüfernummer.</param>
        /// <returns></returns>
        public bool CheckPruefer(string operatorNumber)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // Hier wird der Meta-Befehl zusammengesetzt.
                string tInVal = string.Format("PRFNR={0}", operatorNumber);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GET_META_DATA("CHECK_PRUEFER", tInVal, out tResult);

                if (tInVal.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                this.CurrentOperatorData = new Operator();
                Operator tInspector = new Operator();

                string tRetVal = null;

                tInspector.PrueferNr = operatorNumber;
                tInspector.Vorname = oracleDB.SPLITTER(tResult, delimiter, "VORNAME", out tRetVal);
                tInspector.Nachname = oracleDB.SPLITTER(tResult, delimiter, "NACHNAME", out tRetVal);
                tInspector.Schicht = oracleDB.SPLITTER(tResult, delimiter, "SCHICHT", out tRetVal);

                this.CurrentOperatorData = tInspector;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CheckPruefer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// this function is used to export information of the type key-value to an external database/location over fis
        /// 该函数用于通过 fis 将键值类型的信息导出到外部数据库/位置。
        /// </summary>
        /// <param name="ident">ident of the pcb</param>
        /// <param name="lap">logical work place - identity of the test station</param>
        /// <param name="customer">customer name which is defined in a list in FIS</param>
        /// <param name="key">the key is defined from the client</param>
        /// <param name="value">the apropiate value to the key</param>
        /// <returns></returns>
        public bool CustomerExportStore(string ident, string lap, string customer, string key, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(ident) || ident.Length < 1) { throw new Exception(string.Format("{0}", Language.NoParameterValue)); }
                if (string.IsNullOrEmpty(lap) || lap.Length < 1) { throw new Exception(string.Format("{0}", Language.NoParameterValue)); }
                if (string.IsNullOrEmpty(customer) || customer.Length < 1) { throw new Exception(string.Format("{0}", Language.NoParameterValue)); }
                if (string.IsNullOrEmpty(key) || key.Length < 1) { throw new Exception(string.Format("{0}", Language.NoParameterValue)); }
                if (string.IsNullOrEmpty(value) || value.Length < 1) { throw new Exception(string.Format("{0}", Language.NoParameterValue)); }
                
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);
                
                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.CustomerExportStore(ident, lap, customer, key, value);
                
                if (tResult != null && (tResult.Length > 0 || tResult.Contains("ORA")))
                {
                    this.ErrorMessage = tResult;
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CheckPruefer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// get the panel definition of a specific materialnumber
        /// 获取特定材料编号的面板定义
        /// </summary>
        /// <param name="materialnumber">any materialnumber you want the panel definition for</param>
        /// <returns></returns>
        public List<PanelDefinition> GetPanelDefinition(string materialnumber)
        {
            try
            {
                if (string.IsNullOrEmpty(materialnumber) || materialnumber.Length < 1) { throw new Exception(string.Format("{0}", Language.NoParameterValue)); }

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // Hier wird die Anfrage an die Datenbank gesendet.
                List<OracleDB.PanelDefinition> tResult = oracleDB.GetPanelDefinitionForMaterial(materialnumber);
                
                if (tResult == null)
                {
                    this.ErrorMessage = Melecs.OracleDataBase.FIS.Language.NoReturnValue;
                    return null;
                }
                
                return new List<PanelDefinition>(tResult.Select<OracleDB.PanelDefinition, PanelDefinition>(x => new PanelDefinition(x)));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CheckPruefer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        #region Ident Related Functions

        /// <summary>
        /// 检查Ident在机台重复次数
        /// Gets the iteration of the given value.
        /// The iteration indicates who often a ident passes the given worplace (or workplace group).
        /// </summary>
        /// <param name="ident">The ident of which you want to know the iteration.</param>
        /// <param name="value">The value which indicates the workplace or workplace group.</param>
        /// <param name="apType">The type of the value (e.g.: worplace group).</param>
        /// <param name="iteration">The iteration of the given ident and value.</param>
        /// <returns></returns>
        public bool GetIteration(string ident, string value, ApTypeAvailable apType, out int iteration)
        {
            iteration = errorValue;


            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                Oracle.IterationTypes iterationType = (Oracle.IterationTypes)apType;

                string tResult = oracleDB.GetIteration(ident, value, iterationType, out iteration);

                if (!tResult.Contains("ORA-"))
                {
                    return true;
                }

                this.ErrorMessage = tResult;
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetIdent: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                    // Ignore the exception
                }
            }
        }

        /// <summary>
        /// 检查Ident上一层机台信息
        /// Mit dieser Funktion können die Informationen der Oberstufe abgefragt werden.
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public bool GetOberstufenInformation(string ident)
        {
            this.CurrentUnitData.OberstufenMaterialnummer = string.Empty;
            this.CurrentUnitData.OberstufenAuftragsnummer = string.Empty;
            this.CurrentUnitData.Hochgerüstet = false;

            try
            {
                DataSet dataSet = null;

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GetOberUnterStufenInformation(ref dataSet, ident);

                if (dataSet == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoReturnValue));
                }

                if (tResult.Contains("ORA-"))
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                if (dataSet.Tables[0].Rows.Count == 0)
                {
                    throw new Exception(string.Format("{0}", Language.NoSecLvlOrdNr));
                }

                for (int i = 0; i < dataSet.Tables[0].Rows[0].ItemArray.Length; i++)
                {
                    if (dataSet.Tables[0].Rows[0].ItemArray[i] == null)
                    {
                        dataSet.Tables[0].Rows[0].ItemArray[i] = "Keine Daten gefunden!";
                    }
                }

                this.CurrentUnitData.OberstufenMaterialnummer = (string)dataSet.Tables[0].Rows[0].ItemArray[2];
                this.CurrentUnitData.OberstufenAuftragsnummer = (string)dataSet.Tables[0].Rows[0].ItemArray[3];

                if (this.CurrentUnitData.OberstufenMaterialnummer == this.CurrentUnitData.G_Materialnummer)
                {
                    this.CurrentUnitData.Hochgerüstet = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetIdent: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 检查Ident上下层信息
        /// Mit dieser Funktion können die Informationen der Ober und -Unterstufe abgefragt werden.
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public bool GetOberUnterStufenInformation(string ident)
        {
            try
            {
                DataSet dataSet = null;

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GetOberUnterStufenInformation(ref dataSet, ident);

                if (dataSet == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoReturnValue));
                }

                if (tResult.Contains("ORA-"))
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                Unit tIdentData = new Unit();

                if (dataSet.Tables[0].Rows.Count == 0)
                {
                    throw new Exception(string.Format("{0}", Language.NoSecLvlOrdNr));
                }

                for (var i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    var tempSecondLevelOrdernumber = (string)dataSet.Tables[0].Rows[i].ItemArray[3];
                    var openQuantity = Convert.ToString(oracleDB.GETAUFTRAGSSTÜCKZAHL(tempSecondLevelOrdernumber));

                    // If the order is closed (no open quantity left), then we have to skip it and try the next one 
                    if (openQuantity == "CLOSE")
                    {
                        continue;
                    }

                    // If the order is open then we should have a look if we have all data needed
                    for (var j = 0; j < dataSet.Tables[0].Rows[0].ItemArray.Length; j++)
                    {
                        if (dataSet.Tables[0].Rows[i].ItemArray[j] == null)
                        {
                            dataSet.Tables[0].Rows[i].ItemArray[j] = "Keine Daten gefunden!";
                        }
                    }

                    tIdentData.G_Materialnummer = (string)dataSet.Tables[0].Rows[i].ItemArray[1];
                    tIdentData.Auftrag = (string)dataSet.Tables[0].Rows[i].ItemArray[0];
                    tIdentData.OberstufenMaterialnummer = (string)dataSet.Tables[0].Rows[i].ItemArray[2];
                    tIdentData.OberstufenAuftragsnummer = (string)dataSet.Tables[0].Rows[i].ItemArray[3];
                    break;
                }

                tIdentData.Ident = ident;

                this.CurrentUnitData = tIdentData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetIdent: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 该函数用于查询指定标识中相关的下级材料编号和下级订单。 如果标识尚未升级，该函数将返回当前材料编号和当前订单。
        /// Mit dieser Funktion kann vom angegebenen Ident die zugehörige Unterstufenmaterialnummer, sowie der Unterstufenauftrag abgefragt werden.
        /// Wurde der Ident noch nicht hochgerüstet, gibt die Funktion die aktuelle Materialnummer und den aktuellen Auftrag zurück.
        /// </summary>
        /// <param name="Ident">Der Ident, von dem die Unterstufenmaterialnummer ermittelt werden soll.</param>
        /// <param name="materialnummer">Die Unterstufenmaterialnummer.</param>
        /// <param name="auftragsnummer">Die Unterstufenauftragsnummer.</param>
        /// <returns></returns>
        public bool GetUnterstufenInformationen(string Ident, out string materialnummer, out string auftragsnummer)
        {
            try
            {
                List<string> tResult = new List<string>();

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                materialnummer = "";
                auftragsnummer = "";

                tResult = oracleDB.GetUnterstufenInformation(Ident);

                if (tResult == null)
                {
                    this.ErrorMessage = string.Format("{0}", Language.UnknownError);
                    return false;
                }

                if (tResult.Count < 2)
                {
                    this.ErrorMessage = string.Format("{0}\r\n{1}", Language.NoReturnValue, string.Join("", tResult));
                    return false;
                }

                materialnummer = tResult[0];
                auftragsnummer = tResult[1];

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}: {1}", "GetUnterstufenInformationen", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 重置Ident信息数据
        /// Mit dieser Funktion werden die Ident Daten zurück gesetzt.
        /// </summary>
        public void ResetIdentData()
        {
            this.CurrentUnitData = new Unit();
        }

        /// <summary>
        /// 该函数用于检查程序集的预处理。 如果存在预处理，则可获得程序集的特定数据和赋值数据。
        /// Mit dieser Funktion wird der Vorprozess der Baugruppe überprüft.
        /// Bei vorhandenem Vorprozess werden die Baugruppenspezifischen Daten und Zuordnungsdaten der Baugruppe verfügbar.       
        /// </summary>
        /// <param name="ident">Der mit der FIS-Datenbank zu überprüfende Ident.</param>"
        /// <returns></returns>
        public bool CheckIdent(string ident)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                this.CurrentUnitData = new Unit();
                this.CurrentUnitData.Ident = ident;

                // Hier wird der Meta-Befehl zusammengesetzt.
                string tCMD = string.Format("PROZESS={0}\rIDENT={1}", this.processID, ident);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GET_META_DATA("ALL", tCMD, out tResult);

                if (tResult.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                Unit tIdentData = new Unit();

                string tRetVal = null;

                tIdentData.Ident = ident;
                tIdentData.MachineID = "EOL1MON";

                tIdentData.G_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "G_MATERIALNUMMER", out tRetVal);
                tIdentData.MLFB = oracleDB.SPLITTER(tResult, delimiter, "MLFB", out tRetVal);
                tIdentData.MLFB_Index = oracleDB.SPLITTER(tResult, delimiter, "MLFB_INDEX", out tRetVal);
                tIdentData.BSH_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "BSH_MATERIALNUMMER", out tRetVal);
                tIdentData.BSH_Materialnummer_Kunde = oracleDB.SPLITTER(tResult, delimiter, "BSH_MATERIALNR_KUNDE", out tRetVal);
                tIdentData.Verpackungseinheit = oracleDB.SPLITTER(tResult, delimiter, "VERPACKUNGSEINHEIT", out tRetVal);
                tIdentData.SW_SYC = oracleDB.SPLITTER(tResult, delimiter, "SW_SYC", out tRetVal);
                tIdentData.EEP_File = oracleDB.SPLITTER(tResult, delimiter, "EEP-FILE", out tRetVal);
                tIdentData.SW_VER = oracleDB.SPLITTER(tResult, delimiter, "SW_VER", out tRetVal);
                tIdentData.SW_Index = oracleDB.SPLITTER(tResult, delimiter, "SW_INDEX", out tRetVal);
                tIdentData.Benennung = oracleDB.SPLITTER(tResult, delimiter, "BENENNUNG", out tRetVal);
                tIdentData.Typ = oracleDB.SPLITTER(tResult, delimiter, "TYP", out tRetVal);
                tIdentData.F_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "F_MATERIALNR", out tRetVal);
                tIdentData.Approbationstype = oracleDB.SPLITTER(tResult, delimiter, "APPROBATIONSTYPE", out tRetVal);
                tIdentData.Kommentar = oracleDB.SPLITTER(tResult, delimiter, "KOMMENTAR", out tRetVal);
                tIdentData.Verfahrenstechnik = oracleDB.SPLITTER(tResult, delimiter, "VERFAHRENSTECHNIK", out tRetVal);
                tIdentData.Eingabetext = oracleDB.SPLITTER(tResult, delimiter, "EINGABETEXT", out tRetVal);

                switch (this.CurrentProcess)
                {
                    case ProcessAvailable.ICT:
                        tIdentData.ICT_PRG = oracleDB.SPLITTER(tResult, delimiter, "ICT_PRG", out tRetVal);
                        tIdentData.ICT_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "ICT_PRG_VAR", out tRetVal);
                        tIdentData.ICT_ADAP = oracleDB.SPLITTER(tResult, delimiter, "ICT_ADAP", out tRetVal);
                        break;
                    case ProcessAvailable.EPE:
                        tIdentData.EPE_PRG = oracleDB.SPLITTER(tResult, delimiter, "EPE_PRG", out tRetVal);
                        tIdentData.EPE_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "EPE_PRG_VAR", out tRetVal);
                        tIdentData.EPE_MDATA = oracleDB.SPLITTER(tResult, delimiter, "EPE_MDATA", out tRetVal);
                        tIdentData.EPE_ADAP = oracleDB.SPLITTER(tResult, delimiter, "EPE_ADAP", out tRetVal);
                        break;
                    case ProcessAvailable.EPE2:
                        tIdentData.EPE_PRG = oracleDB.SPLITTER(tResult, delimiter, "EPE2_PRG", out tRetVal);
                        tIdentData.EPE_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "EPE2_PRG_VAR", out tRetVal);
                        tIdentData.EPE_MDATA = oracleDB.SPLITTER(tResult, delimiter, "EPE2_MDATA", out tRetVal);
                        tIdentData.EPE_ADAP = oracleDB.SPLITTER(tResult, delimiter, "EPE2_ADAP", out tRetVal);
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


                tIdentData.Auftrag = oracleDB.SPLITTER(tResult, delimiter, "AUFTRAG", out tRetVal);

                tIdentData.Durchlauf = oracleDB.SPLITTER(tResult, delimiter, "DURCHLAUF", out tRetVal);
                tIdentData.Stueckzahl = oracleDB.SPLITTER(tResult, delimiter, "STUECKZAHL", out tRetVal);
                tIdentData.Barcodetyp = oracleDB.SPLITTER(tResult, delimiter, "BARCODETYP", out tRetVal);

                tIdentData.Sachnummer = oracleDB.SPLITTER(tResult, delimiter, "SACHNUMMER", out tRetVal);

                tIdentData.GoldenSample = oracleDB.SPLITTER(tResult, delimiter, "GOLDEN_SAMPLE", out tRetVal) == "YES" ? true : false;

                tIdentData.RefSampleType = oracleDB.SPLITTER(tResult, delimiter, "REF_SAMPLE_TYPE", out tRetVal);
                tIdentData.RefSampleTypeName = oracleDB.SPLITTER(tResult, delimiter, "REF_SAMPLE_TYPE_NAME", out tRetVal);

                this.CurrentUnitData = tIdentData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CheckIdent: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }

        }

        /// <summary>
        /// Mit dieser Funktion wird der Vorprozess der Baugruppe überprüft.
        /// Bei vorhandenem Vorprozess werden die Baugruppenspezifischen Daten und Zuordnungsdaten der Baugruppe verfügbar.
        /// 该函数用于检查程序集的预处理。 如果存在预处理，则可获得程序集的特定数据和赋值数据。
        /// </summary>
        /// <param name="ident">Der mit der FIS-Datenbank zu überprüfende Ident.</param>
        /// <param name="metaData">Die Metadaten des Idents als Liste.</param>
        /// <returns></returns>
        public bool CheckIdent(string ident, out List<string> metaData)
        {
            metaData = new List<string>();

            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                this.CurrentUnitData = new Unit();
                this.CurrentUnitData.Ident = ident;

                // Hier wird der Meta-Befehl zusammengesetzt.
                string tCMD = string.Format("PROZESS={0}\rIDENT={1}", this.processID, ident);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GET_META_DATA("ALL", tCMD, out tResult);

                if (tResult.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                metaData.AddRange(tResult.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries));

                Unit tIdentData = new Unit();

                string tRetVal = null;

                tIdentData.Ident = ident;
                tIdentData.MachineID = "EOL1MON";

                tIdentData.G_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "G_MATERIALNUMMER", out tRetVal);
                tIdentData.MLFB = oracleDB.SPLITTER(tResult, delimiter, "MLFB", out tRetVal);
                tIdentData.MLFB_Index = oracleDB.SPLITTER(tResult, delimiter, "MLFB_INDEX", out tRetVal);
                tIdentData.BSH_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "BSH_MATERIALNUMMER", out tRetVal);
                tIdentData.BSH_Materialnummer_Kunde = oracleDB.SPLITTER(tResult, delimiter, "BSH_MATERIALNR_KUNDE", out tRetVal);
                tIdentData.Verpackungseinheit = oracleDB.SPLITTER(tResult, delimiter, "VERPACKUNGSEINHEIT", out tRetVal);
                tIdentData.SW_SYC = oracleDB.SPLITTER(tResult, delimiter, "SW_SYC", out tRetVal);
                tIdentData.EEP_File = oracleDB.SPLITTER(tResult, delimiter, "EEP-FILE", out tRetVal);
                tIdentData.SW_VER = oracleDB.SPLITTER(tResult, delimiter, "SW_VER", out tRetVal);
                tIdentData.SW_Index = oracleDB.SPLITTER(tResult, delimiter, "SW_INDEX", out tRetVal);
                tIdentData.Benennung = oracleDB.SPLITTER(tResult, delimiter, "BENENNUNG", out tRetVal);
                tIdentData.Typ = oracleDB.SPLITTER(tResult, delimiter, "TYP", out tRetVal);
                tIdentData.F_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "F_MATERIALNR", out tRetVal);
                tIdentData.Approbationstype = oracleDB.SPLITTER(tResult, delimiter, "APPROBATIONSTYPE", out tRetVal);
                tIdentData.Kommentar = oracleDB.SPLITTER(tResult, delimiter, "KOMMENTAR", out tRetVal);
                tIdentData.Verfahrenstechnik = oracleDB.SPLITTER(tResult, delimiter, "VERFAHRENSTECHNIK", out tRetVal);
                tIdentData.Eingabetext = oracleDB.SPLITTER(tResult, delimiter, "EINGABETEXT", out tRetVal);

                switch (this.CurrentProcess)
                {
                    case ProcessAvailable.ICT:
                        tIdentData.ICT_PRG = oracleDB.SPLITTER(tResult, delimiter, "ICT_PRG", out tRetVal);
                        tIdentData.ICT_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "ICT_PRG_VAR", out tRetVal);
                        tIdentData.ICT_ADAP = oracleDB.SPLITTER(tResult, delimiter, "ICT_ADAP", out tRetVal);
                        break;
                    case ProcessAvailable.EPE:
                        tIdentData.EPE_PRG = oracleDB.SPLITTER(tResult, delimiter, "EPE_PRG", out tRetVal);
                        tIdentData.EPE_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "EPE_PRG_VAR", out tRetVal);
                        tIdentData.EPE_MDATA = oracleDB.SPLITTER(tResult, delimiter, "EPE_MDATA", out tRetVal);
                        tIdentData.EPE_ADAP = oracleDB.SPLITTER(tResult, delimiter, "EPE_ADAP", out tRetVal);
                        break;
                    case ProcessAvailable.EPE2:
                        tIdentData.EPE_PRG = oracleDB.SPLITTER(tResult, delimiter, "EPE2_PRG", out tRetVal);
                        tIdentData.EPE_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "EPE2_PRG_VAR", out tRetVal);
                        tIdentData.EPE_MDATA = oracleDB.SPLITTER(tResult, delimiter, "EPE2_MDATA", out tRetVal);
                        tIdentData.EPE_ADAP = oracleDB.SPLITTER(tResult, delimiter, "EPE2_ADAP", out tRetVal);
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


                tIdentData.Auftrag = oracleDB.SPLITTER(tResult, delimiter, "AUFTRAG", out tRetVal);

                tIdentData.Durchlauf = oracleDB.SPLITTER(tResult, delimiter, "DURCHLAUF", out tRetVal);
                tIdentData.Stueckzahl = oracleDB.SPLITTER(tResult, delimiter, "STUECKZAHL", out tRetVal);
                tIdentData.Barcodetyp = oracleDB.SPLITTER(tResult, delimiter, "BARCODETYP", out tRetVal);

                tIdentData.Sachnummer = oracleDB.SPLITTER(tResult, delimiter, "SACHNUMMER", out tRetVal);

                tIdentData.GoldenSample = oracleDB.SPLITTER(tResult, delimiter, "GOLDEN_SAMPLE", out tRetVal) == "YES" ? true : false;

                tIdentData.RefSampleType = oracleDB.SPLITTER(tResult, delimiter, "REF_SAMPLE_TYPE", out tRetVal);
                tIdentData.RefSampleTypeName = oracleDB.SPLITTER(tResult, delimiter, "REF_SAMPLE_TYPE_NAME", out tRetVal);

                this.CurrentUnitData = tIdentData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CheckIdent: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }

        }

        /// <summary>
        /// 在不进行预处理检查的情况下，检索程序集的特定数据和分配数据。
        /// Holt die Baugruppenspezifischen Daten und Zuordnungsdaten der Baugruppe OHNE eine Vorprozessprüfung durchzuführen.
        /// </summary>
        /// <param name="ident">Der Ident zu dem die Daten geholt werden sollen.</param>
        /// <returns></returns>
        public bool GetMetaData(string ident)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                this.CurrentUnitData = new Unit();
                this.CurrentUnitData.Ident = ident;

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GET_META_DATA_ONLY(ident, this.processID, out tResult);

                if (tResult.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                Unit tIdentData = new Unit();

                string tRetVal = null;

                tIdentData.Ident = ident;
                tIdentData.MachineID = "EOL1MON";

                tIdentData.G_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "G_MATERIALNUMMER", out tRetVal);
                tIdentData.MLFB = oracleDB.SPLITTER(tResult, delimiter, "MLFB", out tRetVal);
                tIdentData.MLFB_Index = oracleDB.SPLITTER(tResult, delimiter, "MLFB_INDEX", out tRetVal);
                tIdentData.BSH_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "BSH_MATERIALNUMMER", out tRetVal);
                tIdentData.BSH_Materialnummer_Kunde = oracleDB.SPLITTER(tResult, delimiter, "BSH_MATERIALNR_KUNDE", out tRetVal);
                tIdentData.Verpackungseinheit = oracleDB.SPLITTER(tResult, delimiter, "VERPACKUNGSEINHEIT", out tRetVal);
                tIdentData.SW_SYC = oracleDB.SPLITTER(tResult, delimiter, "SW_SYC", out tRetVal);
                tIdentData.EEP_File = oracleDB.SPLITTER(tResult, delimiter, "EEP-FILE", out tRetVal);
                tIdentData.SW_VER = oracleDB.SPLITTER(tResult, delimiter, "SW_VER", out tRetVal);
                tIdentData.SW_Index = oracleDB.SPLITTER(tResult, delimiter, "SW_INDEX", out tRetVal);
                tIdentData.Benennung = oracleDB.SPLITTER(tResult, delimiter, "BENENNUNG", out tRetVal);
                tIdentData.Typ = oracleDB.SPLITTER(tResult, delimiter, "TYP", out tRetVal);
                tIdentData.F_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "F_MATERIALNR", out tRetVal);
                tIdentData.Approbationstype = oracleDB.SPLITTER(tResult, delimiter, "APPROBATIONSTYPE", out tRetVal);
                tIdentData.Kommentar = oracleDB.SPLITTER(tResult, delimiter, "KOMMENTAR", out tRetVal);
                tIdentData.Verfahrenstechnik = oracleDB.SPLITTER(tResult, delimiter, "VERFAHRENSTECHNIK", out tRetVal);
                tIdentData.Eingabetext = oracleDB.SPLITTER(tResult, delimiter, "EINGABETEXT", out tRetVal);

                switch (this.CurrentProcess)
                {
                    case ProcessAvailable.ICT:
                        tIdentData.ICT_PRG = oracleDB.SPLITTER(tResult, delimiter, "ICT_PRG", out tRetVal);
                        tIdentData.ICT_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "ICT_PRG_VAR", out tRetVal);
                        tIdentData.ICT_ADAP = oracleDB.SPLITTER(tResult, delimiter, "ICT_ADAP", out tRetVal);
                        break;
                    case ProcessAvailable.EPE:
                        tIdentData.EPE_PRG = oracleDB.SPLITTER(tResult, delimiter, "EPE_PRG", out tRetVal);
                        tIdentData.EPE_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "EPE_PRG_VAR", out tRetVal);
                        tIdentData.EPE_MDATA = oracleDB.SPLITTER(tResult, delimiter, "EPE_MDATA", out tRetVal);
                        tIdentData.EPE_ADAP = oracleDB.SPLITTER(tResult, delimiter, "EPE_ADAP", out tRetVal);
                        break;
                    case ProcessAvailable.EPE2:
                        tIdentData.EPE_PRG = oracleDB.SPLITTER(tResult, delimiter, "EPE2_PRG", out tRetVal);
                        tIdentData.EPE_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "EPE2_PRG_VAR", out tRetVal);
                        tIdentData.EPE_MDATA = oracleDB.SPLITTER(tResult, delimiter, "EPE2_MDATA", out tRetVal);
                        tIdentData.EPE_ADAP = oracleDB.SPLITTER(tResult, delimiter, "EPE2_ADAP", out tRetVal);
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


                tIdentData.Auftrag = oracleDB.SPLITTER(tResult, delimiter, "AUFTRAG", out tRetVal);

                tIdentData.Durchlauf = oracleDB.SPLITTER(tResult, delimiter, "DURCHLAUF", out tRetVal);
                tIdentData.Stueckzahl = oracleDB.SPLITTER(tResult, delimiter, "STUECKZAHL", out tRetVal);
                tIdentData.Barcodetyp = oracleDB.SPLITTER(tResult, delimiter, "BARCODETYP", out tRetVal);

                tIdentData.Sachnummer = oracleDB.SPLITTER(tResult, delimiter, "SACHNUMMER", out tRetVal);

                tIdentData.GoldenSample = oracleDB.SPLITTER(tResult, delimiter, "GOLDEN_SAMPLE", out tRetVal) == "YES" ? true : false;

                tIdentData.RefSampleType = oracleDB.SPLITTER(tResult, delimiter, "REF_SAMPLE_TYPE", out tRetVal);
                tIdentData.RefSampleTypeName = oracleDB.SPLITTER(tResult, delimiter, "REF_SAMPLE_TYPE_NAME", out tRetVal);


                this.CurrentUnitData = tIdentData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetMetaData: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }

        }

        /// <summary>
        /// 在不进行预处理检查的情况下，检索程序集的特定数据和赋值数据。 不保存数据，仅以列表形式输出。
        /// Holt die Baugruppenspezifischen Daten und Zuordnungsdaten der Baugruppe OHNE eine Vorprozessprüfung durchzuführen.
        /// Die Daten werden nicht gespeichert, sondern nur als Liste ausgegeben.
        /// </summary>
        /// <param name="ident">Der Ident zu dem die Daten geholt werden sollen.</param>
        /// <param name="metaData">Die Baugruppenspezifischen Datan.</param>
        /// <returns></returns>
        public bool GetMetaData(string ident, out List<string> metaData)
        {
            metaData = new List<string>();

            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                this.CurrentUnitData = new Unit();
                this.CurrentUnitData.Ident = ident;

                // Hier wird der Meta-Befehl zusammengesetzt.
                string tCMD = string.Format("PROZESS={0}\rIDENT={1}", this.processID, ident);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GET_META_DATA("ALL", tCMD, out tResult);

                if (tResult.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                metaData.AddRange(tResult.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries));

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetMetaData: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }

        }

        /// <summary>
        /// 该功能用于从 FIS 中检索身份识别数据。
        /// Mit dieser Funktion werden Daten zum Ident aus FIS geholt.
        /// </summary>
        /// <param name="ident">Ident zu dem Daten geholt werden.</param>"
        /// <returns></returns>
        public bool GetIdent(string ident)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                this.CurrentUnitData = new Unit();
                this.CurrentUnitData.Ident = ident;

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GETIDENTINFO(ident, "AUF", "PROP", delimiter, out tResult);

                //string tResult = oracleDB.GET_META_DATA("ALL", tCMD, out tResult);

                if (tResult.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }
                Unit tIdentData = new Unit();

                string tRetVal = null;

                tIdentData.Ident = ident;


                tIdentData.G_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "MATERIALNUMMER", out tRetVal);
                tIdentData.Auftrag = oracleDB.SPLITTER(tResult, delimiter, "AUF_ID", out tRetVal);
                tIdentData.Typ = oracleDB.SPLITTER(tResult, delimiter, "FEV_ID", out tRetVal);
                this.CurrentUnitData = tIdentData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetIdent: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Ermittelt den Identtyp (Einzel/Nutzen)
        /// 确定识别类型（单一/效益）
        /// </summary>
        /// <param name="Ident">Ident</param>
        /// <returns>Einzelident/Nutzenident</returns>
        public ObjectTypes GetIdentType(string Ident)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GETIDENTINFO(Ident, "FID", "PROP", delimiter, out tResult);

                if (tResult.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    throw new Exception(tResult);
                }

                oracleDB.SPLITTER(tResult, delimiter, "TRAEGER_FE_ID", out tResult);

                if (tResult == null)
                {
                    return ObjectTypes.NutzenIdent;
                }
                else
                {
                    return ObjectTypes.EinzelIdent;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetIdent: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 该功能用于从 FIS 中检索身份识别数据。
        /// Mit dieser Funktion werden Daten zum Ident aus FIS geholt.
        /// </summary>
        /// <param name="ident">Ident zu dem Daten geholt werden.</param>"
        /// <returns></returns>
        public bool GetIdentInfo(string ident)
        {
            this.CurrentUnitData.OberstufenMaterialnummer = string.Empty;
            this.CurrentUnitData.OberstufenAuftragsnummer = string.Empty;
            this.CurrentUnitData.Hochgerüstet = false;

            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                this.CurrentUnitData = new Unit();
                this.CurrentUnitData.Ident = ident;

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GETIDENTINFO(ident, "FID|FAZ|AUF|FEV|OLDA", "PROP", delimiter, out tResult);

                if (tResult.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }
                Unit tIdentData = new Unit();

                string tRetVal = null;

                tIdentData.Ident = oracleDB.SPLITTER(tResult, delimiter, "FE_ID", out tRetVal);

                tIdentData.Nutzenposition = oracleDB.SPLITTER(tResult, delimiter, "POSITION", out tRetVal);
                tIdentData.G_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "MATERIALNUMMER", out tRetVal);
                tIdentData.Auftrag = oracleDB.SPLITTER(tResult, delimiter, "AUF_ID", out tRetVal);
                tIdentData.OberstufenMaterialnummer = oracleDB.SPLITTER(tResult, delimiter, "MATERIALNUMMER_OBERSTUFE", out tRetVal);
                tIdentData.OberstufenAuftragsnummer = oracleDB.SPLITTER(tResult, delimiter, "AUFTRAGSNUMMER_OBERSTUFE", out tRetVal);

                if (tIdentData.OberstufenMaterialnummer == tIdentData.G_Materialnummer)
                {
                    tIdentData.Hochgerüstet = true;
                }

                this.CurrentUnitData = tIdentData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetIdent: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 检查并返回分割后的Ident
        /// Gibt den Nutzenident zu einem Einzelident zurück
        /// </summary>
        /// <param name="EinzelIdent"></param>
        /// <returns>Nutzenident</returns>
        public string GetNutzenIdent(string EinzelIdent)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GETIDENTINFO(EinzelIdent, "FID", "PROP", delimiter, out tResult);

                if (tResult.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    throw new Exception(tResult);
                }
                string NutzenIdent = oracleDB.SPLITTER(tResult, delimiter, "TRAEGER_FE_ID", out tResult);
                if (NutzenIdent == null)
                {
                    return EinzelIdent; // Nutzenident wurde übergeben
                }
                else
                {
                    return NutzenIdent;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetIdent: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 返回单边条的受益位置
        /// Gibt die Nutzenposition eines Einzelidents zurück.
        /// </summary>
        /// <param name="Ident">Ident, dessen Position ermittelt werden soll.</param>
        /// <param name="NutzenPosition">Die Position des Idents.</param>
        /// <returns></returns>
        public bool GetNutzenPosition(string Ident, out int NutzenPosition)
        {
            NutzenPosition = -1;

            try
            {
                string tRetVal = null;
                string NutzenIdent = "";
                List<string> tNutzenList = new List<string>();

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string tResult = oracleDB.GETIDENTINFO(Ident, "FID", "PROP", delimiter, out tResult);

                if (tResult.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                // If the units have been lasered with the new laser-program, then the parameter "POSITION" will be received
                if (oracleDB.SPLITTER(tResult, delimiter, "rPOSITION", out tRetVal) != "Keine Daten gefunden!")
                {
                    NutzenPosition = Convert.ToInt32(tRetVal);
                    return true;
                }

                NutzenIdent = oracleDB.SPLITTER(tResult, delimiter, "TRAEGER_FE_ID", out tResult);

                tResult = "";

                tNutzenList.AddRange(oracleDB.GETIDENTINFO(NutzenIdent, "NUTZ", "PROP", delimiter, out tResult).Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries));

                NutzenPosition = Convert.ToInt32(tNutzenList.Find(item => item.Split('=')[1] == Ident).Split('=')[0].Split('_')[1]);

                return true;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        ///  您只需向该方法提供一个 Nutzen Ident，它就会返回与其相关的所有 einzels
        ///  You just need to give an Nutzen Ident to this method and it will return all the einzels associated to it
        /// </summary>
        /// <param name="NutzenIdent">This must be a nutzen ident to get a result</param>
        /// <returns>All the Einzel idents associated to the Nutzen ident</returns>
        public List<string> GetEinzelIdentsFromNutzenIdent(string NutzenIdent)
        {
            string tResult = "";
            var nutzenIdentList = new List<string>();

            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);
                var EinzelIdents = oracleDB.GETIDENTINFO(NutzenIdent, "NUTZ", "PROP", delimiter, out tResult);
                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return nutzenIdentList;
                }

                nutzenIdentList.AddRange(EinzelIdents.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries));
                if (nutzenIdentList.Count > 1)
                {
                    nutzenIdentList.RemoveAt(1); // need to remove the first two records from the list since that is not a nutzen ident
                    nutzenIdentList.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
            return nutzenIdentList;
        }

        /// <summary>
        ///  第一个参数是包装编号 - V5000001234，结果是一堆 ID。 所有 ID 都属于给定的包装编号
        ///  The first parameter is a packaging number - V5000001234 and the result is a bunch of idents. 
        ///  All of the idents are belong to the given packaging number
        /// </summary>
        /// <param name="PackagingNumber"></param>
        /// <returns></returns>
        public List<string> GetIdentsFromPackagingNumber(string PackagingNumber)
        {
            string tResult = "";
            this.ErrorMessage = "";
            var identList = new List<string>();

            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);
                tResult = oracleDB.GetIdentsFromPackagingNumber(PackagingNumber, ref identList);
                if (string.IsNullOrWhiteSpace(tResult) == false && tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return null;
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
            return identList;
        }

        /// <summary>
        ///  第一个参数是订单号 - 70178532，结果是一堆 ID。 所有 ID 都属于给定的订单号
        ///  The first parameter is the order number - 70178532 and the result is a bunch of idents. 
        ///  All of the idents are belong to the given order number
        /// </summary>
        /// <param name="orderNumber">Order number </param>
        /// <param name="packagingFlag">Packaging flag usage: 
        /// 'T' means: packaged only,
        /// 'F' means: 'not packaged at all'
        /// Empty means: All idents
        /// </param>
        /// <returns>List of idents</returns>
        public List<string> GetIdentsFromOrder(string orderNumber, string packagingFlag = "")
        {
            string tResult = "";
            this.ErrorMessage = "";
            var identList = new List<string>();

            try
            {
                if (orderNumber.Length < 8 || orderNumber.Length > 8) throw new Exception(Language.AuftragsNummerLengthError01);

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);
                DataSet dataSet = new DataSet();
                tResult = oracleDB.GetIdentsFromOrder(ref dataSet, orderNumber, packagingFlag);

                if (dataSet == null || dataSet.Tables == null)
                {
                    this.ErrorMessage = string.Format("{0}", Language.IdentsNotFound);
                    return null;
                }

                if (string.IsNullOrWhiteSpace(tResult) == false && tResult.IndexOf("ORA") >= 0)
                {
                    this.ErrorMessage = tResult;
                    return null;
                }

                if (dataSet.Tables.Count > 0 &&
                    dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        identList.Add(row.ItemArray[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
            return identList;
        }

        /// <summary>
        ///  第一个参数是维修站（或任何其他站），结果是该站和过去 24 小时内传递的 ID 编号
        ///  The first parameter is the repair station (or any other station) and the result is the passed idents for that station
        ///  and for the last 24 hours
        /// </summary>
        /// <param name="lap">LAP is the station identifier, like VERP-972, EPE-840</param>
        /// <param name="preprocessApg">Optional parameter. If it is specified then the result 
        /// will only contain idents failed previously on that station, ie.: you would like to know passed idents
        /// for a repair station but only ICT failed idents, that you can specify preprocessApg as "2610"
        /// </param>
        /// <param name="fromDate">Optional parameter. If it's not specified then last 24 hour, this is the minimum value.
        /// By setting this parameter you can specify a date for the last 24 hour, older date should not be used.
        /// Why? To avoid performance problems.</param>
        /// <returns>List of idents</returns>
        public List<string> GetPassedIdentsForLAP(string lap, string preprocessApg = null, DateTime? fromDate = null)
        {
            string tResult = "";
            this.ErrorMessage = "";
            var identList = new List<string>();

            try
            {
                
                if (fromDate.HasValue)
                {
                    var last24hourEndDate = DateTime.Now.AddHours(-24);
                    if (fromDate < last24hourEndDate)
                    {
                        this.ErrorMessage = Language.IncorrectFromDateRange;
                        return null;
                    }
                }
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);
                DataSet dataSet = new DataSet();
                tResult = oracleDB.GetPassedIdentsForLap(ref dataSet, lap, preprocessApg, fromDate);

                if (dataSet == null || dataSet.Tables == null)
                {
                    this.ErrorMessage = string.Format("{0}", Language.IdentsNotFound);
                    return null;
                }

                if (string.IsNullOrWhiteSpace(tResult) == false && tResult.IndexOf("ORA") >= 0)
                {
                    this.ErrorMessage = tResult;
                    return null;
                }

                if (dataSet.Tables.Count > 0 &&
                    dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        identList.Add(row.ItemArray[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
            return identList;
        }

        /// <summary>
        /// 该功能用于从 FIS 中检索身份识别数据。
        /// Mit dieser Funktion werden Daten zum Ident aus FIS geholt.
        /// </summary>
        /// <param name="ident">Ident zu dem Daten geholt werden.</param>
        /// <param name="identTyp">Der Typ des Idents.</param>
        /// <param name="identInfo">Die zurückgegebenen Infos zum Ident.</param>
        /// <returns></returns>
        public bool GetIdentInfo(string ident, ObjectTypes identTyp, ref string identInfo)
        {
            try
            {
                string tObjectTypes = "";
                List<string> EinzelIdents = new List<string>();

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                switch (identTyp)
                {
                    case ObjectTypes.NutzenIdent:
                        tObjectTypes = "NUTZ";
                        break;
                    case ObjectTypes.EinzelIdent:
                        tObjectTypes = "FID";
                        break;
                    case ObjectTypes.All:
                        tObjectTypes = "FID|FAZ|AUF|FEV|OLDA|NUTZ";
                        break;
                    default:
                        break;
                }

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GETIDENTINFO(ident, tObjectTypes, "PROP", delimiter, out tResult);

                if (tResult.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                identInfo = tResult;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetIdent: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        #region Series number

        /// <summary>
        /// 检查获取序列号
        /// Mit dieser Funktion wird eine Seriennummer von ein Nummernstreck von der FIS Datenbank angefordert.
        /// </summary>
        /// <param name="fnvID">Ist die Bezeichnung der Nummernstrecke.</param>
        /// <param name="formatID">Ist die Bezeichnung für der Formatierung.</param>
        /// <param name="result">Gibt die Seriennummer zurück.</param>
        /// <param name="isTechID">Gibt an ob eine technische Seriennummer gezogen werden soll oder nicht.</param>
        /// <returns></returns>
        public bool GetSeriennummer(string fnvID, string formatID, out string result, bool isTechID = true)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (isTechID)
                {
                    this.ErrorMessage = oracleDB.CREATEFETECHID(fnvID, formatID, out result);
                }
                else
                {
                    this.ErrorMessage = oracleDB.CREATEFISIDENTX(fnvID, formatID, out result);
                }

                if (result.Length < 1) return false;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetSeriennummer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 该功能用于从 FIS 数据库的编号范围中获取序列号。
        /// Mit dieser Funktion wird eine Seriennummer von einer Nummernstrecke von der FIS Datenbank angefordert.
        /// </summary>
        /// <param name="fnvID">Ist die Bezeichnung der Nummernstrecke.</param>
        /// <param name="formatID">Ist die Bezeichnung für der Formatierung.</param>
        /// <returns>Gibt die Seriennummer zurück</returns>
        public string GetSeriennummer(string fnvID, string formatID)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string result = "";

                this.ErrorMessage = oracleDB.CREATEFETECHID(fnvID, formatID, out result);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetSeriennummer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 该功能用于从 FIS 数据库中请求号码范围的序列号。
        /// Mit dieser Funktion wird eine Seriennummer von ein Nummernstreck von der FIS Datenbank angefordert.
        /// </summary>
        /// <param name="fnvID">Ist die Bezeichnung der Nummernstrecke.</param>
        /// <param name="formatID">Ist die Bezeichnung für der Formatierung.</param>
        /// <returns>Gibt die Seriennummer zurück</returns>
        public bool GetSeriennummer(string fnvID, string formatID, out string result, ref string errorMessage)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                result = "";

                errorMessage = oracleDB.CREATEFISIDENTX(fnvID, formatID, out result);

                return result.Length == 0 ? false : true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetSeriennummer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Mit dieser Funktion wird eine Seriennummer von ein Nummernstreck von der FIS Datenbank angefordert.
        /// 该功能用于从 FIS 数据库中请求号码范围的序列号。
        /// </summary>
        /// <param name="fnvID">Ist die Bezeichnung der Nummernstrecke.</param>
        /// <param name="formatID">Ist die Bezeichnung für der Formatierung.</param>
        /// <param name="DateFlag">Bei Seriennummern die nur aus einer fortlaufenden Nummer bestehen muss hier 'X' übergeben werden.</param>
        /// <returns>Gibt die Seriennummer zurück</returns>
        public bool GetSeriennummer(string fnvID, string formatID, out string result, ref string errorMessage, string DateFlag)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                result = "";

                errorMessage = oracleDB.CREATEFISIDENTX(fnvID, formatID, out result, DateFlag);

                return result.Length == 0 ? false : true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetSeriennummer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        #region Pre Process
        /// <summary>
        /// 不清楚怎么传值
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="lap"></param>
        /// <returns></returns>
        public string GetAlleVorprozesse(string ident, string lap)
        {
            string rueckgabe;
            string fisResult = "";
            string preProcessDate = "";
            string preProcessLAp = "";
            string additionalText = "";
            string Meldung_Fehler = "";

            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (!IsConnected) return Meldung_Fehler + "Not Connected to FIS";

                rueckgabe = oracleDB.CheckPreProcess(ident, lap, ref fisResult, ref preProcessDate, ref preProcessLAp, ref additionalText);

                if (rueckgabe.StartsWith("ORA"))
                {
                    throw new Exception(rueckgabe);
                }
                if (fisResult != "1")
                {
                    throw new Exception(string.Concat(rueckgabe, additionalText, ": ", preProcessLAp));
                }

                return Meldung_Fehler;
            }
            catch (Exception ex)
            {
                return Meldung_Fehler + ex.Message.ToString();
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 检查的功能 单个工作站的预处理。
        /// Funktionen überprüft Vorprozess des einzelnen Arbeitsplatzes.
        /// </summary>
        /// <param name="Ident">Ident der geprüft werden soll.</param>
        /// <param name="AP">Abzurufender Arbeitsplatz (LAP - Logischer Arbeitsplatz) (APG - Arbeitsplatzgruppe)</param>
        /// <param name="APType">Mit APGLIST wird die Arbeitsplatzgruppenliste druchsucht und bei LAPLIST die Logischer-Arbeitsplatzliste.</param>
        /// <param name="Error">Wenn ein Fehler auftritt wird er hier ausgegeben. </param>
        /// <param name="vorprozessDatum">Datum des Prozesseintrages.</param>
        /// <param name="vorprozessLAP">Arbeitsplatz.</param>
        /// <returns></returns>
        public bool CheckVorprozess(string Ident, string AP, ApTypeAvailable APType, ref string Error, ref DateTime vorprozessDatum, ref string vorprozessLAP)
        {
            try
            {
                string result = "";
                string vorprzDatum = "";
                string vorprzLAP = "";

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // IN_VALUE (Muss mit Anführungsstrichen angegeben werden! Bsp: 'XXX-XXX' )
                // RESULT (Ergebnis des Checks (1 = IO, 0 = NIO))
                Error = oracleDB.CHECKVORPROZESS(Ident, string.Format("'{0}'", AP), (string)Enum.GetName(typeof(ApTypeAvailable), APType), ref result, ref vorprzDatum, ref vorprzLAP);

                if (vorprzDatum == "" || vorprzDatum == "null")
                {
                    vorprozessDatum = new DateTime(1, 1, 1, 0, 0, 0);
                }
                else
                {
                    vorprozessDatum = DateTime.ParseExact(vorprzDatum, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }

                this.CurrentUnitData.Ident = Ident;

                vorprozessLAP = vorprzLAP == "null" ? "" : vorprzLAP;

                return result == "1" ? true : false;
            }
            catch (Exception Except)
            {
                Error = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 检查的功能 单个工作站的预处理。
        /// Funktionen überprüft Vorprozess des einzelnen Arbeitsplatzes.
        /// </summary>
        /// <param name="Ident">Ident der geprüft werden soll.</param>
        /// <param name="AP">Abzurufender Arbeitsplatz (LAP - Logischer Arbeitsplatz) (APG - Arbeitsplatzgruppe)</param>
        /// <param name="APType">Mit APGLIST wird die Arbeitsplatzgruppenliste druchsucht und bei LAPLIST die Logischer-Arbeitsplatzliste.</param>
        /// <param name="Error">Wenn ein Fehler auftritt wird er hier ausgegeben. </param>
        /// <param name="vorprozessDatum">Datum des Prozesseintrages.</param>
        /// <returns></returns>
        public bool CheckVorprozess(string Ident, string AP, ApTypeAvailable APType, ref string Error, ref DateTime vorprozessDatum)
        {
            try
            {
                string result = "";
                string vorprzDatum = "";
                string vorprzLAP = "";

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // IN_VALUE (Muss mit Anführungsstrichen angegeben werden! Bsp: 'XXX-XXX' )
                // RESULT (Ergebnis des Checks (1 = IO, 0 = NIO))
                Error = oracleDB.CHECKVORPROZESS(Ident, string.Format("'{0}'", AP), (string)Enum.GetName(typeof(ApTypeAvailable), APType), ref result, ref vorprzDatum, ref vorprzLAP);

                if (vorprzDatum == "" || vorprzDatum == "null")
                {
                    vorprozessDatum = new DateTime(1, 1, 1, 0, 0, 0);
                }
                else
                {
                    vorprozessDatum = DateTime.ParseExact(vorprzDatum, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }

                this.CurrentUnitData.Ident = Ident;

                return result == "1" ? true : false;
            }
            catch (Exception Except)
            {
                Error = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 功能检查各个工作站的预处理。
        /// Funktion überprüft Vorprozess des einzelnen Arbeitsplatzes.
        /// </summary>
        /// <param name="Ident">Ident der geprüft werden soll.</param>
        /// <param name="AP">Abzurufender Arbeitsplatz (LAP - Logischer Arbeitsplatz) (APG - Arbeitsplatzgruppe)</param>
        /// <param name="APType">Mit APGLIST wird die Arbeitsplatzgruppenliste druchsucht und bei LAPLIST die Logischer-Arbeitsplatzliste.</param>
        /// <param name="vorprozessDatum">Datum des Prozesseintrages.</param>
        /// <param name="vorprozessLAP">Arbeitsplatz.</param>
        /// <param name="Fehlercode">Im Falle eines schlechten Eintrages wird hier der Fehlercode zurückgegeben.</param>
        /// <param name="Fehlertext">Im Falle eines schlechten Eintrages wird hier der Fehlertext zurückgegeben.</param>
        /// <returns></returns>
        public bool CheckVorprozess(string Ident, string AP, ApTypeAvailable APType, ref DateTime vorprozessDatum, ref string vorprozessLAP, ref string Fehlercode, ref string Fehlertext)
        {
            try
            {
                string result = "";
                string vorprzDatum = "";
                string vorprzLAP = "";

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // IN_VALUE (Muss mit Anführungsstrichen angegeben werden! Bsp: 'XXX-XXX' )
                // RESULT (Ergebnis des Checks (1 = IO, 0 = NIO))
                this.ErrorMessage = oracleDB.CHECKVORPROZESS(Ident, string.Format("'{0}'", AP), (string)Enum.GetName(typeof(ApTypeAvailable), APType), ref result, ref vorprzDatum, ref vorprzLAP, ref Fehlercode, ref Fehlertext);

                if (vorprzDatum == "" || vorprzDatum == "null")
                {
                    vorprozessDatum = new DateTime(1, 1, 1, 0, 0, 0);
                }
                else
                {
                    vorprozessDatum = DateTime.ParseExact(vorprzDatum, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }

                vorprozessLAP = vorprzLAP == "null" ? "" : vorprzLAP;

                return result == "1" ? true : false;
            }
            catch (Exception Except)
            {
                this.ErrorMessage = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 检查的功能 单个工作站的预处理。
        /// Funktionen überprüft Vorprozess des einzelnen Arbeitsplatzes.
        /// </summary>
        /// <param name="Ident">Ident der geprüft werden soll.</param>
        /// <param name="AP">Abzurufender Arbeitsplatz (LAP - Logischer Arbeitsplatz) (APG - Arbeitsplatzgruppe)</param>
        /// <param name="APType">Mit APGLIST wird die Arbeitsplatzgruppenliste druchsucht und bei LAPLIST die Logischer-Arbeitsplatzliste.</param>
        /// <param name="vorprozessDatum">Datum des Prozesseintrages.</param>
        /// <param name="vorprozessLAP">Arbeitsplatz.</param>
        /// <returns></returns>
        public bool CheckVorprozess(string Ident, string AP, ApTypeAvailable APType, ref DateTime vorprozessDatum, ref string vorprozessLAP)
        {
            try
            {
                string result = "";
                string vorprzDatum = "";
                string vorprzLAP = "";

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // IN_VALUE (Muss mit Anführungsstrichen angegeben werden! Bsp: 'XXX-XXX' )
                // RESULT (Ergebnis des Checks (1 = IO, 0 = NIO))
                this.ErrorMessage = oracleDB.CHECKVORPROZESS(Ident, string.Format("'{0}'", AP), (string)Enum.GetName(typeof(ApTypeAvailable), APType), ref result, ref vorprzDatum, ref vorprzLAP);

                if (vorprzDatum == "" || vorprzDatum == "null")
                {
                    vorprozessDatum = new DateTime(1, 1, 1, 0, 0, 0);
                }
                else
                {
                    vorprozessDatum = DateTime.ParseExact(vorprzDatum, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }

                vorprozessLAP = vorprzLAP == "null" ? "" : vorprzLAP;

                return result == "1" ? true : false;
            }
            catch (Exception Except)
            {
                this.ErrorMessage = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        public bool CheckVorprozess(string Ident, string AP, ApTypeAvailable APType, ref DateTime vorprozessDatum, bool Central)
        {
            try
            {
                string result = "";
                string vorprzDatum = "";
                string vorprzLAP = "";

                if (Central)
                {
                    oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, OracleDB.Oracle.DataSources.Central);
                }
                else
                {
                    oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, OracleDB.Oracle.DataSources.EWS);
                }

                // IN_VALUE (Muss mit Anführungsstrichen angegeben werden! Bsp: 'XXX-XXX' )
                // RESULT (Ergebnis des Checks (1 = IO, 0 = NIO))
                this.ErrorMessage = oracleDB.CHECKVORPROZESS(Ident, string.Format("'{0}'", AP), (string)Enum.GetName(typeof(ApTypeAvailable), APType), ref result, ref vorprzDatum, ref vorprzLAP);

                if (vorprzDatum == "" || vorprzDatum == "null")
                {
                    vorprozessDatum = new DateTime(1, 1, 1, 0, 0, 0);
                }
                else
                {
                    vorprozessDatum = DateTime.ParseExact(vorprzDatum, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }

                return result == "1" ? true : false;
            }
            catch (Exception Except)
            {
                this.ErrorMessage = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 检查的功能 单个工作站的预处理。
        /// Funktionen überprüft Vorprozess des einzelnen Arbeitsplatzes.
        /// </summary>
        /// <param name="Ident">Ident der geprüft werden soll.</param>
        /// <param name="AP">Abzurufender Arbeitsplatz (LAP - Logischer Arbeitsplatz) (APG - Arbeitsplatzgruppe)</param>
        /// <param name="APType">Mit APGLIST wird die Arbeitsplatzgruppenliste druchsucht und bei LAPLIST die Logischer-Arbeitsplatzliste.</param>
        /// <param name="vorprozessDatum">Datum des Prozesseintrages.</param>
        /// <param name="vorprozessLAP">Arbeitsplatz.</param>
        /// <returns></returns>
        public bool CheckVorprozess(string Ident, string AP, ApTypeAvailable APType)
        {
            try
            {
                string result = "";
                string vorprzDatum = "";
                string vorprzLAP = "";

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // IN_VALUE (Muss mit Anführungsstrichen angegeben werden! Bsp: 'XXX-XXX' )
                // RESULT (Ergebnis des Checks (1 = IO, 0 = NIO))
                this.ErrorMessage = oracleDB.CHECKVORPROZESS(Ident, string.Format("'{0}'", AP), (string)Enum.GetName(typeof(ApTypeAvailable), APType), ref result, ref vorprzDatum, ref vorprzLAP);

                return result == "1" ? true : false;
            }
            catch (Exception Except)
            {
                this.ErrorMessage = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 检查的功能 单个工作站的预处理。
        /// Funktionen überprüft Vorprozess des einzelnen Arbeitsplatzes.
        /// </summary>
        /// <param name="Ident">Ident der geprüft werden soll.</param>
        /// <param name="AP">Abzurufender Arbeitsplatz (LAP - Logischer Arbeitsplatz) (APG - Arbeitsplatzgruppe)</param>
        /// <param name="APType">Mit APGLIST wird die Arbeitsplatzgruppenliste druchsucht und bei LAPLIST die Logischer-Arbeitsplatzliste.</param>
        /// <param name="vorprozessDatum">Datum des Prozesseintrages.</param>
        /// <param name="vorprozessLAP">Arbeitsplatz.</param>
        /// <param name="ErrMsg">Beinhaltet die Fehlermeldung.</param>
        /// <returns></returns>
        public bool CheckVorprozess(string Ident, string AP, ApTypeAvailable APType, out string ErrMsg)
        {
            ErrMsg = "";
            this.ErrorMessage = "";

            try
            {
                string result = "";
                string vorprzDatum = "";
                string vorprzLAP = "";

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // IN_VALUE (Muss mit Anführungsstrichen angegeben werden! Bsp: 'XXX-XXX' )
                // RESULT (Ergebnis des Checks (1 = IO, 0 = NIO))
                this.ErrorMessage = oracleDB.CHECKVORPROZESS(Ident, string.Format("'{0}'", AP), (string)Enum.GetName(typeof(ApTypeAvailable), APType), ref result, ref vorprzDatum, ref vorprzLAP);

                if (this.ErrorMessage != "")
                {
                    ErrMsg = this.ErrorMessage;
                    return false;
                }

                return result == "1" ? true : false;
            }
            catch (Exception Except)
            {
                this.ErrorMessage = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        /// <summary>
        /// 该函数查询并返回指定材料编号的赋值数据。
        /// Mit dieser Funktion werden die Zuordnungsdaten zu der angegebenen Materialnummer abgefragt und zurückgegeben.
        /// </summary>
        /// <param name="materialNumber">Die mit der FIS-Datenbank zu überprüfende Materialnummer.</param>
        /// <param name="printdata">Die Daten die zum Drucken benötigt werden.</param>
        /// <returns></returns>
        public bool GetPrintDataMaterialnummer(string materialNumber, out string printdata)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // Hier wird der Meta-Befehl zusammengesetzt.
                string tCMD = string.Format("PROZESS={0}\rG_MATERIALNUMMER={1}", this.processID, materialNumber);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GET_META_DATA("ZUORD_DATA", tCMD, out tResult);

                if (tCMD.Length < 1) { throw new Exception(string.Format("{0}", Language.NoReturnValue)); }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    printdata = "";
                    return false;
                }

                printdata = string.Format("G_MATERIALNUMMER={0}\r{1}", materialNumber, tResult);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetPrintData: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        ///  The first parameter is the material number - 0010503987 and the results are open orders. 
        ///  All of the orders are belong to the given material number
        ///  第一个参数是材料编号 - 0010503987，结果是未结订单。 所有订单都属于给定的材料编号
        /// </summary>
        /// <param name="materialNumber">Material number </param>
        /// <param name="maxPlanDate">Optional parameter. If it is specified then you can filter orders for older plan date
        /// For example: maxPlanDate is 2020-05-01 then order with higher plan date is excluded from the result list
        /// </param>
        /// <returns>List of orders</returns>
        public List<string> GetOpenOrdersForMaterialNumber(string materialNumber, DateTime? maxPlanDate = null)
        {
            string tResult = "";
            this.ErrorMessage = "";
            var orderList = new List<string>();

            try
            {
                if (string.IsNullOrWhiteSpace(materialNumber) == false)
                {
                    materialNumber = materialNumber.PadLeft(10, '0');

                    oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);
                    DataSet dataSet = new DataSet();
                    tResult = oracleDB.GetOpenordersForMaterial(ref dataSet, materialNumber, maxPlanDate);

                    if (dataSet == null || dataSet.Tables == null)
                    {
                        this.ErrorMessage = string.Format("{0}", Language.OrdersNotFound);
                        return null;
                    }

                    if (string.IsNullOrWhiteSpace(tResult) == false && tResult.IndexOf("ORA") >= 0)
                    {
                        this.ErrorMessage = tResult;
                        return null;
                    }

                    if (dataSet.Tables.Count > 0 &&
                        dataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            orderList.Add(row.ItemArray[0].ToString());
                        }
                    }
                }
                else
                {
                    this.ErrorMessage = Language.MissingMaterialNumber;
                    return null;
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
            return orderList;
        }

        /// <summary>
        /// 该函数用于查询指定材料编号的分配数据。
        /// Mit dieser Funktion werden die Zuordnungsdaten zu der angegebenen Materialnummer abgefragt.
        /// </summary>
        /// <param name="sIdent">Die mit der FIS-Datenbank zu überprüfende Materialnummer.</param>"
        /// <returns></returns>
        public bool GetMaterialnummer(string materialNumber)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // Hier wird der Meta-Befehl zusammengesetzt.
                string tCMD = string.Format("PROZESS={0}\rG_MATERIALNUMMER={1}", this.processID, materialNumber);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GET_META_DATA("ZUORD_DATA", tCMD, out tResult);

                if (tCMD.Length < 1)
                {
                    throw new Exception(string.Format("{0}", Language.NoReturnValue));
                }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                this.CurrentAssignData = new AssignData();
                AssignData tAssignData = new AssignData();

                string tRetVal = null;

                //tAssignData.Auftrag = "";
                tAssignData.G_Materialnummer = materialNumber;
                tAssignData.MLFB = oracleDB.SPLITTER(tResult, delimiter, "MLFB", out tRetVal);
                tAssignData.MLFB_Index = oracleDB.SPLITTER(tResult, delimiter, "MLFB_INDEX", out tRetVal);
                tAssignData.BSH_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "BSH_MATERIALNUMMER", out tRetVal);
                tAssignData.BSH_Materialnummer_Kunde = oracleDB.SPLITTER(tResult, delimiter, "BSH_MATERIALNR_KUNDE", out tRetVal);
                tAssignData.Verpackungseinheit = oracleDB.SPLITTER(tResult, delimiter, "VERPACKUNGSEINHEIT", out tRetVal);
                tAssignData.SW_SYC = oracleDB.SPLITTER(tResult, delimiter, "SW_SYC", out tRetVal);
                tAssignData.EEP_File = oracleDB.SPLITTER(tResult, delimiter, "EEP-FILE", out tRetVal);
                tAssignData.SW_VER = oracleDB.SPLITTER(tResult, delimiter, "SW_VER", out tRetVal);
                tAssignData.SW_Index = oracleDB.SPLITTER(tResult, delimiter, "SW_INDEX", out tRetVal);
                tAssignData.Benennung = oracleDB.SPLITTER(tResult, delimiter, "BENENNUNG", out tRetVal);
                tAssignData.Typ = oracleDB.SPLITTER(tResult, delimiter, "TYP", out tRetVal);
                tAssignData.F_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "F_MATERIALNR", out tRetVal);
                tAssignData.Approbationstype = oracleDB.SPLITTER(tResult, delimiter, "APPROBATIONSTYPE", out tRetVal);
                tAssignData.Kommentar = oracleDB.SPLITTER(tResult, delimiter, "KOMMENTAR", out tRetVal);
                tAssignData.Verfahrenstechnik = oracleDB.SPLITTER(tResult, delimiter, "VERFAHRENSTECHNIK", out tRetVal);
                tAssignData.Eingabetext = oracleDB.SPLITTER(tResult, delimiter, "EINGABETEXT", out tRetVal);

                switch (this.CurrentProcess)
                {
                    case ProcessAvailable.ICT:
                        tAssignData.ICT_PRG = oracleDB.SPLITTER(tResult, delimiter, "ICT_PRG", out tRetVal);
                        tAssignData.ICT_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "ICT_PRG_VAR", out tRetVal);
                        tAssignData.ICT_ADAP = oracleDB.SPLITTER(tResult, delimiter, "ICT_ADAP", out tRetVal);
                        break;
                    case ProcessAvailable.EPE:
                        tAssignData.EPE_PRG = oracleDB.SPLITTER(tResult, delimiter, "EPE_PRG", out tRetVal);
                        tAssignData.EPE_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "EPE_PRG_VAR", out tRetVal);
                        tAssignData.EPE_MDATA = oracleDB.SPLITTER(tResult, delimiter, "EPE_MDATA", out tRetVal);
                        tAssignData.EPE_ADAP = oracleDB.SPLITTER(tResult, delimiter, "EPE_ADAP", out tRetVal);
                        break;
                    case ProcessAvailable.EPE2:
                        tAssignData.EPE_PRG = oracleDB.SPLITTER(tResult, delimiter, "EPE2_PRG", out tRetVal);
                        tAssignData.EPE_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "EPE2_PRG_VAR", out tRetVal);
                        tAssignData.EPE_MDATA = oracleDB.SPLITTER(tResult, delimiter, "EPE2_MDATA", out tRetVal);
                        tAssignData.EPE_ADAP = oracleDB.SPLITTER(tResult, delimiter, "EPE2_ADAP", out tRetVal);
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

                this.CurrentAssignData = tAssignData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetMaterialnummer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 该功能用于从 FIS 检索订单数据。无需为此维护分配数据。
        /// Mit dieser Funktion werden Daten zum Auftrag aus FIS geholt. Die Zuordnungsdaten müssen dafür nicht gepflegt sein
        /// </summary>
        /// <param name="orderNumber">Abzufragende Auftragsnummer</param>
        /// <returns></returns>
        public bool GetAuftragInfo(string orderNumber)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GETAUFTRAGSINFO(orderNumber, "1", "AUF|FEV", "PROP", ";", out tResult);

                if (tResult.Length < 1)
                {
                    throw new Exception(string.Format("{0}", Language.NoReturnValue));
                }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                this.CurrentAssignData = new AssignData();
                AssignData tAssignData = new AssignData();

                string tRetVal = null;
                tAssignData.Auftrag = orderNumber;
                tAssignData.G_Materialnummer = oracleDB.SPLITTER(tResult, ";", "MATERIALNUMMER", out tRetVal);
                tAssignData.Typ = oracleDB.SPLITTER(tResult, ";", "PGRP_NAME", out tRetVal);

                int tmpint = 0;
                oracleDB.SPLITTER(tResult, ";", "AUF_NUTZEN_ANZ", out tRetVal);
                if (int.TryParse(tRetVal, out tmpint))
                    tAssignData.NutzenAnzahl = tmpint;
                else
                    tAssignData.NutzenAnzahl = 0;

                this.CurrentAssignData = tAssignData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetAuftragInfo: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 该功能用于从 FIS 检索订单数据。无需为此维护分配数据。
        /// Mit dieser Funktion werden Daten zum Auftrag aus FIS geholt. Die Zuordnungsdaten müssen dafür nicht gepflegt sein
        /// </summary>
        /// <param name="orderNumber">Abzufragende Auftragsnummer</param>
        /// <returns></returns>
        public bool GetAuftragInfo(string orderNumber, out string materialnumber)
        {
            try
            {
                materialnumber = "";

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GETAUFTRAGSINFO(orderNumber, "1", "AUF|FEV", "PROP", ";", out tResult);

                if (tResult.Length < 1)
                {
                    throw new Exception(string.Format("{0}", Language.NoReturnValue));
                }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                string tRetVal = null;

                materialnumber = oracleDB.SPLITTER(tResult, ";", "MATERIALNUMMER", out tRetVal);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetAuftragInfo: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 该函数用于查询指定订单的分配数据。
        /// Mit dieser Funktion werden die Zuordnungsdaten zu dem angegebenen Auftrag abgefragt.
        /// </summary>
        /// <param name="sIdent">Der mit der FIS-Datenbank zu überprüfende Auftrag.</param>"
        /// <returns></returns>
        public bool GetAuftrag(string orderNumber)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                // Hier wird der Meta-Befehl zusammengesetzt.
                string tCMD = string.Format("PROZESS={0}\rAUFTRAG={1}", this.processID, orderNumber);

                // Hier wird die Anfrage an die Datenbank gesendet.
                string tResult = oracleDB.GET_META_DATA("GET_AUFTRAG_ZUORD", tCMD, out tResult);

                if (tCMD.Length < 1)
                {
                    throw new Exception(string.Format("{0}", Language.NoReturnValue));
                }

                if (tResult.Substring(0, 3) == "ORA")
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                this.CurrentAssignData = new AssignData();
                AssignData tAssignData = new AssignData();

                string tRetVal = null;

                tAssignData.Auftrag = orderNumber;
                tAssignData.G_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "G_MATERIALNUMMER", out tRetVal);
                tAssignData.MLFB = oracleDB.SPLITTER(tResult, delimiter, "MLFB", out tRetVal);
                tAssignData.MLFB_Index = oracleDB.SPLITTER(tResult, delimiter, "MLFB_INDEX", out tRetVal);
                tAssignData.BSH_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "BSH_MATERIALNUMMER", out tRetVal);
                tAssignData.BSH_Materialnummer_Kunde = oracleDB.SPLITTER(tResult, delimiter, "BSH_MATERIALNR_KUNDE", out tRetVal);
                tAssignData.Verpackungseinheit = oracleDB.SPLITTER(tResult, delimiter, "VERPACKUNGSEINHEIT", out tRetVal);
                tAssignData.SW_SYC = oracleDB.SPLITTER(tResult, delimiter, "SW_SYC", out tRetVal);
                tAssignData.EEP_File = oracleDB.SPLITTER(tResult, delimiter, "EEP-FILE", out tRetVal);
                tAssignData.SW_VER = oracleDB.SPLITTER(tResult, delimiter, "SW_VER", out tRetVal);
                tAssignData.SW_Index = oracleDB.SPLITTER(tResult, delimiter, "SW_INDEX", out tRetVal);
                tAssignData.Benennung = oracleDB.SPLITTER(tResult, delimiter, "BENENNUNG", out tRetVal);
                tAssignData.Typ = oracleDB.SPLITTER(tResult, delimiter, "TYP", out tRetVal);
                tAssignData.F_Materialnummer = oracleDB.SPLITTER(tResult, delimiter, "F_MATERIALNR", out tRetVal);
                tAssignData.Approbationstype = oracleDB.SPLITTER(tResult, delimiter, "APPROBATIONSTYPE", out tRetVal);
                tAssignData.Kommentar = oracleDB.SPLITTER(tResult, delimiter, "KOMMENTAR", out tRetVal);
                tAssignData.Verfahrenstechnik = oracleDB.SPLITTER(tResult, delimiter, "VERFAHRENSTECHNIK", out tRetVal);
                tAssignData.Eingabetext = oracleDB.SPLITTER(tResult, delimiter, "EINGABETEXT", out tRetVal);

                switch (this.CurrentProcess)
                {
                    case ProcessAvailable.ICT:
                        tAssignData.ICT_PRG = oracleDB.SPLITTER(tResult, delimiter, "ICT_PRG", out tRetVal);
                        tAssignData.ICT_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "ICT_PRG_VAR", out tRetVal);
                        tAssignData.ICT_ADAP = oracleDB.SPLITTER(tResult, delimiter, "ICT_ADAP", out tRetVal);
                        break;
                    case ProcessAvailable.EPE:
                        tAssignData.EPE_PRG = oracleDB.SPLITTER(tResult, delimiter, "EPE_PRG", out tRetVal);
                        tAssignData.EPE_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "EPE_PRG_VAR", out tRetVal);
                        tAssignData.EPE_MDATA = oracleDB.SPLITTER(tResult, delimiter, "EPE_MDATA", out tRetVal);
                        tAssignData.EPE_ADAP = oracleDB.SPLITTER(tResult, delimiter, "EPE_ADAP", out tRetVal);
                        break;
                    case ProcessAvailable.EPE2:
                        tAssignData.EPE_PRG = oracleDB.SPLITTER(tResult, delimiter, "EPE2_PRG", out tRetVal);
                        tAssignData.EPE_PRG_VAR = oracleDB.SPLITTER(tResult, delimiter, "EPE2_PRG_VAR", out tRetVal);
                        tAssignData.EPE_MDATA = oracleDB.SPLITTER(tResult, delimiter, "EPE2_MDATA", out tRetVal);
                        tAssignData.EPE_ADAP = oracleDB.SPLITTER(tResult, delimiter, "EPE2_ADAP", out tRetVal);
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

                tAssignData.Sachnummer = oracleDB.SPLITTER(tResult, delimiter, "SACHNUMMER", out tRetVal);

                this.CurrentAssignData = tAssignData;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetAuftrag: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 检查订单数量的功能。
        /// Funktion zum prüfen der Auftragsstückzahl.
        /// </summary>
        /// <param name="AuftragsNummer">Auftragsnummer die zu prüfen ist.</param>
        /// <returns>true, wenn der Auftrag noch genug offene Stück hat. false, wenn der Auftrag voll ist.</returns>
        public bool CheckAuftragsStückzahl(string AuftragsNummer)
        {
            string tmessage = "";

            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                tmessage = oracleDB.GETAUFTRAGSSTÜCKZAHL(AuftragsNummer).ToString();
                if (tmessage.Contains("ORA"))
                {
                    this.ErrorMessage = string.Format("Fehler beim prüfen der Auftragsstückzahl!\r\n", tmessage);
                    return false;
                }

                if (tmessage.Contains("CLOSE"))
                {
                    this.ErrorMessage = string.Format("Auftragsstückzahl des Auftrages: {0} wurde erreicht", AuftragsNummer);
                    return false;
                }

                int tOffeneStückzahl = Convert.ToInt32(tmessage);
                if (tOffeneStückzahl <= 0)
                {
                    this.ErrorMessage = string.Format("Auftragsstückzahl des Auftrages: {0} wurde erreicht", AuftragsNummer);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }

            return true;
        }

        /// <summary>
        /// 检查未完成的订单编号，订单编号存储在CurrentUnitData中
        /// Diese Funktion eruiert die offene Auftragsstückzahl des eingegebenen Auftrags.
        /// Die Auftragsstückzahl wird in dem CurrentUnitData hinterlegt.
        /// </summary>
        /// <param name="AuftragsNummer">Auftragsnummer die geprüft werden soll.</param>
        /// <returns></returns>
        public bool AuftragsStückzahl(string AuftragsNummer)
        {
            try
            {
                if (AuftragsNummer.Length < 8 || AuftragsNummer.Length > 8) throw new Exception(Language.AuftragsNummerLengthError01);

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                this.ErrorMessage = oracleDB.GETAUFTRAGSSTÜCKZAHL(AuftragsNummer).ToString();

                if (this.ErrorMessage.Contains("ORA")) return false;
                else
                {
                    this.CurrentUnitData.OffeneAuftragsStückzahl = this.ErrorMessage;
                    return true;
                }
            }
            catch (Exception Except)
            {
                this.ErrorMessage = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Every order has a specific size and can only carry a specific amount of idents. 
        /// This function returns the target and the actual amount of these idents and also the splitted values for panel and single. 
        /// Additional you will also get the remaining count of idents which can be added/assigned to the order.
        /// 每个订单都有特定的尺寸，只能携带特定数量的 ID。 此函数将返回这些 ID 的目标值和实际值，以及面板和单个的分割值。 此外，您还将获得可添加/分配给订单的剩余 ID 数量。
        /// </summary>
        /// <param name="orderNr">the number of the order to look up for</param>
        /// <param name="panelSize">the size of the panel (amount of pcbs which forms a panel)</param>
        /// <param name="targetIdentCount">maximum amount of idents assignable to an order (single/sub and panel idents)</param>
        /// <param name="targetPanelCount">the amount of panels assigned to the order (panel idents)</param>
        /// <param name="actualIdentCount">the amount of idents assigned to the order (single/sub and panel idents)</param>
        /// <param name="actualSingleCount">the amount of singles/subs assigned to the order (single/sub idents)</param>
        /// <param name="remainingIdentCount">remaining amount of idents which can be assigned to the order</param>
        /// <returns>show the status of the fis request. Returns false if there was an error while requesting fis and true if there wasn't.</returns>
        public bool IdentCountOfOrder(string orderNr, ref long panelSize, ref long targetIdentCount, ref long targetPanelCount, ref long actualIdentCount, ref long actualSingleCount, ref long remainingIdentCount)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string retval = oracleDB.GETAUFIDENTANZAHL(orderNr, 1, ref panelSize, ref targetIdentCount, ref targetPanelCount, ref actualIdentCount, ref actualSingleCount, ref remainingIdentCount);

                if (retval.Contains("ORA"))
                {
                    this.ErrorMessage = retval;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception Except)
            {
                this.ErrorMessage = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Every order has a specific size and can only carry a specific amount of idents. 
        /// This function returns the remaining amount of idents which can be added/assigned to the order.
        /// 每个订单都有特定的大小，只能携带特定数量的 idents。 此函数返回可添加/分配给订单的剩余 ID 数量。
        /// </summary>
        /// <param name="orderNr">the number of the order to look up for</param>
        /// <param name="remainingIdentCount">remaining amount of idents which can be assigned to the order</param>
        /// <returns>show the status of the fis request. Returns false if there was an error while requesting fis and true if there wasn't.</returns>
        public bool IdentCountOfOrder(string orderNr, ref long remainingIdentCount)
        {
            long panelSize = 0;
            long targetIdentCount = 0;
            long targetCarrierCount = 0;
            long actualIdentCount = 0;
            long actualSingleCount = 0;

            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string retval = oracleDB.GETAUFIDENTANZAHL(orderNr, 1, ref panelSize, ref targetIdentCount, ref targetCarrierCount, ref actualIdentCount, ref actualSingleCount, ref remainingIdentCount);

                if (retval.Contains("ORA"))
                {
                    this.ErrorMessage = retval;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception Except)
            {
                this.ErrorMessage = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 将标识分配给指定的订单号。
        /// Ordnet den Ident der angegebenen Auftragsnummer zu.
        /// </summary>
        /// <param name="Ident">Ident der geprüft werden soll.</param>
        /// <param name="AuftragsNummer">Die Auftragsnummer auf die zugeordnet werden soll</param>
        /// <param name="LAP">LAP ist der Logische-Arbeitsplatz wo die Zuordnung zu dem Auftrag erledigt wird.</param>
        /// <param name="Error">Wenn ein Fehler auftritt wird er hier ausgegeben. </param>
        /// <returns>True wenn alles in Ordnung ist und False wenn ein Fehler auftritt.</returns>
        public bool AuftragsZuordnung(string Ident, string AuftragsNummer, string LAP, ref string Error)
        {
            try
            {
                if (Ident.Length < 7) throw new Exception(Language.IdentLengthError01);

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                Error = oracleDB.AUFTRAGSZUORDNUNG(AuftragsNummer, 1, Ident, LAP);

                return Error == "" ? true : false;
            }
            catch (Exception Except)
            {
                Error = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 将标识分配给指定的订单号。
        /// Ordnet den Ident der angegebenen Auftragsnummer zu.
        /// </summary>
        /// <param name="Ident">Ident der geprüft werden soll.</param>
        /// <param name="AuftragsNummer">Die Auftragsnummer auf die zugeordnet werden soll</param>
        /// <param name="LAP">LAP ist der Logische-Arbeitsplatz wo die Zuordnung zu dem Auftrag erledigt wird.</param>
        /// <returns>True wenn alles in Ordnung ist und False wenn ein Fehler auftritt.</returns>
        public bool AuftragsZuordnung(string Ident, string AuftragsNummer, string LAP)
        {
            try
            {
                if (Ident.Length < 7) throw new Exception(Language.IdentLengthError01);

                if (AuftragsNummer.Length < 8 || AuftragsNummer.Length > 8) throw new Exception(Language.AuftragsNummerLengthError01);

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                this.ErrorMessage = oracleDB.AUFTRAGSZUORDNUNG(AuftragsNummer, 1, Ident, LAP);

                return this.ErrorMessage == "" ? true : false;
            }
            catch (Exception Except)
            {
                this.ErrorMessage = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Ordnet den Ident der angegebenen Auftragsnummer zu.
        /// 将标识分配给指定的订单号。
        /// </summary>
        /// <param name="identToAssign">Ident der dem Auftrag und falls vorhanden dem Master zugeordnet werden soll. Ist kein Master vorhanden ist dieser auf null zu setzen.</param>
        /// <param name="masterIdent">Zu diesem Master Ident wird der zuzuordnente Ident zugewiesen. Gibt es diesen Master nicht ist er auf null zu setzen.</param>
        /// <param name="AuftragsNummer">Die Auftragsnummer auf die zugeordnet werden soll</param>
        /// <param name="LAP">LAP ist der Logische-Arbeitsplatz wo die Zuordnung zu dem Auftrag erledigt wird.</param>
        /// <returns>True wenn alles in Ordnung ist und False wenn ein Fehler auftritt.</returns>
        public bool AuftragsZuordnung(string identToAssign, string masterIdent, string AuftragsNummer, string LAP)
        {
            try
            {
                if (AuftragsNummer.Length < 8 || AuftragsNummer.Length > 8) throw new Exception(Language.AuftragsNummerLengthError01);

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                this.ErrorMessage = oracleDB.AUFTRAGSZUORDNUNG(identToAssign, masterIdent, AuftragsNummer, LAP);

                return this.ErrorMessage == "" ? true : false;
            }
            catch (Exception Except)
            {
                this.ErrorMessage = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Ordnet den Ident der angegebenen Auftragsnummer zu.
        /// 将标识分配给指定的订单号。
        /// </summary>
        /// <param name="identToAssign">Ident der dem Auftrag und falls vorhanden dem Master zugeordnet werden soll. Ist kein Master vorhanden ist dieser auf null zu setzen.</param>
        /// <param name="masterIdent">Zu diesem Master Ident wird der zuzuordnente Ident zugewiesen. Gibt es diesen Master nicht ist er auf null zu setzen.</param>
        /// <param name="AuftragsNummer">Die Auftragsnummer auf die zugeordnet werden soll</param>
        /// <param name="LAP">LAP ist der Logische-Arbeitsplatz wo die Zuordnung zu dem Auftrag erledigt wird.</param>
        /// <param name="nutzenPos">Die Position des Subs im Nutzen.</param>
        /// <returns>True wenn alles in Ordnung ist und False wenn ein Fehler auftritt.</returns>
        public bool AuftragsZuordnung(string identToAssign, string masterIdent, string AuftragsNummer, string LAP, int nutzenPos)
        {
            try
            {
                if (AuftragsNummer.Length < 8 || AuftragsNummer.Length > 8) throw new Exception(Language.AuftragsNummerLengthError01);

                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                this.ErrorMessage = oracleDB.AUFTRAGSZUORDNUNG(identToAssign, masterIdent, AuftragsNummer, LAP, NUTZEN_POS: nutzenPos);

                return this.ErrorMessage == "" ? true : false;
            }
            catch (Exception Except)
            {
                this.ErrorMessage = Except.Message;
                return false;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        #region Trace Functions


        public bool WriteTraceData(string line, string orderNr, string packageUnitIdent)
        {
            /*  Barcode definition (packageUnitIdent)
             * 
             *  Name            |   Count   |   Optional
             *  ----------------------------------------
             *  UniqueID        |   13      |
             *  OrderNumber     |   10      |
             *  OrderPosition   |   5       |
             *  Component MatNr |   10      |
             *  Quantity        |   8       |
             *  DateCode        |   4       |
             *  Moisturelevel   |   2       |   X
             *  
             */

            try
            {
                var uid = packageUnitIdent.Substring(0, 13);
                var purchaseOrderNo = packageUnitIdent.Substring(13, 10);
                var purchaseOrderPos = packageUnitIdent.Substring(23, 5);
                var componentMaterialNo = packageUnitIdent.Substring(28, 10);
                var quantity = packageUnitIdent.Substring(38, 8);
                var dateCode = packageUnitIdent.Substring(46, 4);
                var moistureLevel = string.Empty;

                if (packageUnitIdent.Length > 50)
                {
                    moistureLevel = packageUnitIdent.Substring(50, packageUnitIdent.Length - 50);
                }

                var filename = "DBCom_" + line;


                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string result = "";

                result = oracleDB.Store_Load_Store_Rst_Load(
                    "05", "1083", line, "", "", "", "", DateTime.Now.ToString("yyyyMMddHHmmss"), 
                    purchaseOrderNo, purchaseOrderPos, componentMaterialNo, dateCode, "", componentMaterialNo, "", orderNr,
                    "", filename, "", uid, moistureLevel, quantity);
                
                if (!string.IsNullOrEmpty(result))
                {
                    this.ErrorMessage = result;
                    return false;
                }

                result = oracleDB.Verteil_Rst(filename);

                if (!string.IsNullOrEmpty(result))
                {
                    this.ErrorMessage = result;
                    return false;
                }

                this.ErrorMessage = "";

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("WriteTraceData: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 查询数据追踪编号
        /// Mit dieser Funktion wird eine Tracenummer von der FIS Datenbank angefordert.
        /// </summary>
        /// <returns>Gibt die Tracenummer zurück</returns>
        public bool GetTracenummer(out string result)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                result = "";

                this.ErrorMessage = oracleDB.CREATE_VERPACK_ID(out result);

                if (result.Length > 13) // Die Tracenummer ist 13 Zeichen lang
                    return false; // Ist der zurückgegebene String länger, handelt es sich um einen Fehler!

                this.ErrorMessage = "";

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetTracenummer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Mit dieser Funktion wird eine Tracenummer von der FIS Datenbank angefordert.
        /// 该功能用于向 FIS 数据库申请跟踪号码。
        /// </summary>
        /// <returns>Gibt die Tracenummer zurück</returns>
        public string GetTracenummer()
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string result = "";

                this.ErrorMessage = oracleDB.CREATE_VERPACK_ID(out result);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetTracenummer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 绑定追踪标识符
        /// Diese Funktion baut den Ident in die angegebene Tracenummer ein.
        /// </summary>
        /// <param name="ident">Ident der eingebaut werden soll.</param>
        /// <param name="Tracenummer">Tracenummer in die eingebaut werden soll.</param>
        /// <param name="mid">Die ID der Anlage.</param>
        /// <returns></returns>
        public bool EinbauTracenummer(string ident, string Tracenummer, string mid)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }
                if (Tracenummer == "" | Tracenummer == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = 0;
                tRetVal = oracleDB.EVENT(ref tAGID, "ANF", ident, "", mid, this.CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now, VERP_ID: Tracenummer, VERP_FLAG: "E");

                this.AGID = tAGID;

                if (tRetVal.Contains("IDENT_IN_PKG"))
                {
                    if (!this.AusbauTracenummer(ident, tRetVal.Substring(46, 13), mid))
                    {
                        this.ErrorMessage = tRetVal;
                        return false;
                    }

                    // Reopen connection, because it will be closed in function "AusbauTracenummer"
                    oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                    tRetVal = null;
                    tAGID = 0;
                    tRetVal = oracleDB.EVENT(ref tAGID, "ANF", ident, "", mid, this.CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now, VERP_ID: Tracenummer, VERP_FLAG: "E");

                    this.AGID = tAGID;

                    if (tRetVal.Length > 0)
                    {
                        this.ErrorMessage = tRetVal;
                        return false;
                    }
                }
                else if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                tRetVal = null;
                tAGID = 0;
                tRetVal = oracleDB.EVENT(ref tAGID, "ENDE", ident, "", mid, this.CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now);

                this.AGID = tAGID;

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EinbauTracenummer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 创建追踪标识符
        /// Diese Funktion baut den Ident aus der angegebenen Tracenummer aus.
        /// </summary>
        /// <param name="ident">Ident der ausgebaut werden soll.</param>
        /// <param name="Tracenummer">Tracenummer aus der ausgebaut werden soll.</param>
        /// <param name="mid">Die ID der Anlage.</param>
        /// <returns></returns>
        public bool AusbauTracenummer(string ident, string Tracenummer, string mid)
        {
            try
            {
                // Check connection, because function is used internally
                if (!oracleDB.IsConnected())
                {
                    oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);
                }

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }
                if (Tracenummer == "" | Tracenummer == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = 0;
                tRetVal = oracleDB.EVENT(ref tAGID, "ANF", ident, "", mid, this.CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now, VERP_ID: Tracenummer, VERP_FLAG: "A");

                this.AGID = tAGID;

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                tRetVal = null;
                tAGID = 0;
                tRetVal = oracleDB.EVENT(ref tAGID, "FEHL", ident, "", mid, this.CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now, "", "", "", "Auspacken");

                this.AGID = tAGID;

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                tRetVal = null;
                tAGID = 0;
                tRetVal = oracleDB.EVENT(ref tAGID, "ENDE", ident, "", mid, this.CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now);

                this.AGID = tAGID;

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("AusbauTracenummer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// This function will give you the tracenumber for the current ident.
        /// 该功能将提供当前标识的轨迹编号。
        /// </summary>
        /// <param name="ident">Key parameter, you would like to know what is the packaging number for this ident</param>
        /// <param name="traceNumber">The trance number what you would like to know.</param>
        /// <returns></returns>
        public bool GetTracenummer(string ident, ref string traceNumber)
        {
            try
            {
                traceNumber = "";
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = 0;
                tRetVal = oracleDB.GetVerpackIdFromIdent(ident, ref traceNumber);

                this.AGID = tAGID;

                if (string.IsNullOrWhiteSpace(tRetVal) == false)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EinbauTracenummer: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// 创建FIS条目的起始事件
        /// Mit dieser Funktion wird der Anfangseventeintrag in die FIS-Datenbank geschrieben.
        /// Wird am Anfang einer Prüfung geschrieben.
        /// </summary>
        /// <param name="ident">Der in die FIS-Datenbank einzutragende Ident.</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <returns></returns>
        public bool EventStart(string ident, string mid)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = 0;
                tRetVal = oracleDB.EVENT(ref tAGID, "ANF", ident, "", mid, this.CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now);
                this.AGID = tAGID;

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventStart: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 创建FIS条目的起始事件，并附加一段文本
        /// Mit dieser Funktion wird der Anfangseventeintrag in die FIS-Datenbank geschrieben.
        /// Zusätzlich wird ein Text an den Eintrag angehängt
        /// Wird am Anfang einer Prüfung geschrieben.
        /// </summary>
        /// <param name="ident">Der in die FIS-Datenbank einzutragende Ident.</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <param name="kommentar">Der zusätzliche Text (zB. Messwerte)</param>
        /// <returns></returns>
        public bool EventStart(string ident, string mid, string kommentar)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = 0;
                tRetVal = oracleDB.EVENT(ref tAGID, "ANF", ident, "", mid, this.CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now, "", "", "", kommentar);
                this.AGID = tAGID;

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventStart: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 创建FIS条目的起始事件，并附加时间戳和程序名
        /// Schreibt ein Start Event ins FIS
        /// </summary>
        /// <param name="ident">Der Ident für den Eintrag</param>
        /// <param name="mid">Der Arbeitsplatz (LAP) für den Eintrag</param>
        /// <param name="Zeit">Der Zeitstempel</param>
        /// <param name="Program">Der Name des Prüfprogramms</param>
        /// <returns></returns>
        public bool EventStart(string ident, string mid, DateTime Zeit, string Program)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = 0;
                tRetVal = oracleDB.EVENT(ref tAGID, "ANF", ident, "", mid, this.CurrentOperatorData.PrueferNr, Zeit, Zeit, Program);
                this.AGID = tAGID;

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventStart: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 创建FIS条目，附带时间戳
        /// Mit dieser Funktion wird der Anfangseventeintrag in die FIS-Datenbank geschrieben.
        /// Wird am Anfang einer Prüfung geschrieben.
        /// </summary>
        /// <param name="ident">Der in die FIS-Datenbank einzutragende Ident.</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <param name="Zeit">Der Zeitstempel des Eintrags</param>
        /// <returns></returns>
        public bool EventStart(string ident, string mid, DateTime Zeit)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = 0;
                tRetVal = oracleDB.EVENT(ref tAGID, "ANF", ident, "", mid, this.CurrentOperatorData.PrueferNr, Zeit, Zeit);
                this.AGID = tAGID;

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventStart: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 创建有错误的FIS条目，附带时间戳
        /// Mit dieser Funktion wird der Fehleventeintrag in die FIS-Datenbank geschrieben.
        /// Dies sollte nur gemacht werden wenn ein Fehler bei einer Prüfung aufgetreten ist.
        /// </summary>
        /// <param name="ident">Der in die FIS-Datenbank einzutragende Ident.</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <param name="errorText">Der Fehler wird in Form eines Textes hinterlegt.</param>
        /// <param name="errorNumber">Die Nummer des Testschrittes bei welchem die Baugruppe ausgefallen ist bzw. wo ein Fehler erkannt wurde.</param>
        /// <param name="Zeit">Der Zeitstempel des Eintrags</param>
        /// <returns></returns>
        public bool EventError(string ident, string mid, string errorText, string errorNumber, DateTime Zeit)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = this.AGID;
                tRetVal = oracleDB.EVENT(ref tAGID, "FEHL", ident, "", mid, this.CurrentOperatorData.PrueferNr, Zeit, Zeit, "", "", "", errorText, "", "", 0, "", "", errorNumber);
                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventError: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 创建有错误的FIS条目，附带错误ID
        /// Mit dieser Funktion wird der Fehleventeintrag in die FIS-Datenbank geschrieben.
        /// Dies sollte nur gemacht werden wenn ein Fehler bei einer Prüfung aufgetreten ist.
        /// </summary>
        /// <param name="ident">Der in die FIS-Datenbank einzutragende Ident.</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <param name="errorText">Der Fehler wird in Form eines Textes hinterlegt.</param>
        /// <param name="errorNumber">Die Nummer des Testschrittes bei welchem die Baugruppe ausgefallen ist bzw. wo ein Fehler erkannt wurde.</param>
        /// <param name="Fehl_ID">Fehler ID</param>
        /// <param name="DIAG_BE_ID">Bauteil ID</param>
        /// <returns></returns>
        public bool EventError(string ident, string mid, string errorText, string errorNumber, string Fehl_ID, string DIAG_BE_ID)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = this.AGID;
                tRetVal = oracleDB.EVENT(ref tAGID, "FEHL", ident, "", mid, this.CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now, "", "", "", errorText, "", Fehl_ID, 0, "", "", errorNumber, DIAG_BE_ID);
                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventError: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }
        /// <summary>
        /// 创建有错误的FIS条目，附带错误ID
        /// Mit dieser Funktion wird der Fehleventeintrag in die FIS-Datenbank geschrieben.
        /// Die Parameter sind bei dieser Funktion richtiggestellt.
        /// </summary>
        /// <param name="ident">Der in die FIS-Datenbank einzutragende Ident.</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <param name="Meld_Fehl_Text">Der Fehler wird in Form eines Textes hinterlegt.</param>
        /// <param name="Meld_Be_ID">Die Nummer des Testschrittes/Bauteils bei welchem die Baugruppe ausgefallen ist bzw. wo ein Fehler erkannt wurde.</param>
        /// <param name="Diag_Fehl_ID">ID des diagnostizierten Fehlers. zB. "0072" für Pseudo</param>
        /// <param name="DIAG_BE_ID">Die Nummer des diagnostizierten Bauteils</param>
        /// <param name="Diag_Fehl_Text">Beschreibungstext des diagnostizierten Fehlers</param>
        /// <param name="Zeit">Zeitstempel des Eintrags</param>
        /// <returns></returns>
        public bool EventError(string ident, string mid, string Meld_Fehl_Text, string Meld_Be_ID, string Diag_Fehl_ID, string DIAG_BE_ID, string Diag_Fehl_Text, DateTime Zeit)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = this.AGID;
                tRetVal = oracleDB.EVENT(ref tAGID, "FEHL", ident, "", mid, this.CurrentOperatorData.PrueferNr, Zeit, Zeit, "", "", "", Meld_Fehl_Text, Diag_Fehl_Text, Diag_Fehl_ID, 0, "", "", Meld_Be_ID, DIAG_BE_ID);
                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventError: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
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
        public bool EventError(string ident, string mid, string errorText, string errorNumber)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = this.AGID;
                tRetVal = oracleDB.EVENT(ref tAGID, "FEHL", ident, "", mid, this.CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now, "", "", "", errorText, "", "", 0, "", "", errorNumber);
                //tRetVal = oracleDB.EVENT(ref tAGID, "FEHL", ident, "", mid, CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now, "", "", "", errorText, "", errorNumber, 0, "", "", errorNumber);
                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventError: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 创建FIS结束条目
        /// Mit dieser Funktion wird der Endeventeintrag in die FIS-Datenbank geschrieben.
        /// Wird am Ende eine Prüfung geschrieben um das Event zu schließen.
        /// </summary>
        /// <param name="ident">Der in die FIS-Datenbank einzutragende Ident.</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <returns></returns>
        public bool EventEnd(string ident, string mid)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = this.AGID;
                tRetVal = oracleDB.EVENT(ref tAGID, "ENDE", ident, "", mid, this.CurrentOperatorData.PrueferNr, DateTime.Now, DateTime.Now);

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventEnd: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 创建FIS结束条目，带时间戳
        /// Mit dieser Funktion wird der Endeventeintrag in die FIS-Datenbank geschrieben.
        /// Wird am Ende eine Prüfung geschrieben um das Event zu schließen.
        /// </summary>
        /// <param name="ident">Der in die FIS-Datenbank einzutragende Ident.</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <param name="Zeit">Der Zeitstempel des Eintrags</param>
        /// <returns></returns>
        public bool EventEnd(string ident, string mid, DateTime Zeit)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                if (ident == "" | ident == null)
                {
                    throw new Exception(string.Format("{0}", Language.NoIdent));
                }

                string tRetVal = null;
                Int64 tAGID = this.AGID;
                tRetVal = oracleDB.EVENT(ref tAGID, "ENDE", ident, "", mid, this.CurrentOperatorData.PrueferNr, Zeit, Zeit);

                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("EventEnd: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        #region Synonym, Mounting, Booking

        /// <summary>
        /// Create a Synonym in the Database. For Exampel Ident BBFSKD and Synonym 203049494
        /// 在数据库中创建同义词。Exampel Ident BBFSKD 和同义词 203049494
        /// </summary>
        /// <param name="ident">Melecs Ident</param>
        /// <param name="synonym">Synonym which build a connection with the ident</param>
        /// <returns>true if the synonym building was succesfull and in other case false</returns>
        public bool CreateSynonym(string ident, string synonym)
        {
            string synonymResult = string.Empty;

            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string tResult = oracleDB.SYNONYM_INP(ident, synonym, ref  synonymResult);

                if (tResult.Contains("ORA-"))
                {
                    this.ErrorMessage = tResult;
                    return false;
                }

                if (synonymResult.Contains("ORA-"))
                {
                    this.ErrorMessage = synonymResult;
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetIdent: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                    // Ignore the exception
                }
            }
        }

        /// <summary>
        /// Anonym booking of units.
        /// 匿名预订单位。
        /// </summary>
        /// <param name="auftrag">Auftragsnummer</param>
        /// <param name="mid">Die ID der überprüfenden Anlage.</param>
        /// <param name="total">Total number of PCBs (good + bad)</param>
        /// <param name="failed">Number of bad PCBs</param>
        /// <returns></returns>
        public bool AnonymBooking(string auftrag, string mid, int total, int failed)
        {
            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string tRetVal = null;
                Int64 tAGID = this.AGID;

                tRetVal = oracleDB.EVENT(AG_OID: ref tAGID, FE_ID: "", HT_ID: "ANF", AUF_ID: auftrag, LAP_MASCH_ID: mid, MA_ID: this.CurrentOperatorData.PrueferNr, GESAMT_ANZ: total.ToString(), SCHLECHT_ANZ: failed.ToString(), AP_DATUM: DateTime.Now, CLIENT_DATUM: DateTime.Now);
                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }
                tRetVal = oracleDB.EVENT(AG_OID: ref tAGID, FE_ID: "", HT_ID: "ENDE", AUF_ID: auftrag, LAP_MASCH_ID: mid, MA_ID: this.CurrentOperatorData.PrueferNr, AP_DATUM: DateTime.Now, CLIENT_DATUM: DateTime.Now);
                if (tRetVal.Length > 0)
                {
                    this.ErrorMessage = tRetVal;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("AnonymBooking: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 该函数将一个标识符构建到另一个标识符中，从而将子标识符构建到主标识符中。
        /// Diese Funktion baut einen Ident in einen anderen ein.
        /// Hierbei wird der Subident in den Mainident eingebaut.
        /// </summary>
        /// <param name="mainIdent">Der Ident in den eingebaut werden soll.</param>
        /// <param name="subIdent">Der Ident, der eingebaut werden soll.</param>
        /// <param name="mID">Physikalischer Arbeitsplatz.</param>
        /// <returns></returns>
        public bool ProcessMounting(string mainIdent, string subIdent, string mID)
        {
            int UNTER_WUID_OID = 0;
            string existingMainIdent = "";

            try
            {
                oracleDB.OPEN_DATABASE_CONNECTION(this.DBUserName, this.DBPassword, this.CurrentDataSource);

                string tResult = oracleDB.PROCESSMONTAGE(OBER_FE_ID: mainIdent, UNTER_FE_ID: subIdent, MASCH_ID: mID, MONTAGE_TYP: "EIN", MONTAGE_FILE: "", UNTER_WUID_OID: out UNTER_WUID_OID);

                if (tResult.Contains("ORA-06512"))
                {
                    System.Threading.Thread.Sleep(50);

                    tResult = oracleDB.GETIDENTFROMOBERSTUFE(MASCH_ID: mID, UNTER_FE_ID: subIdent, OBER_FE_ID: out existingMainIdent, UNTER_WUID_OID: out UNTER_WUID_OID);
                    if (tResult != "")
                    {
                        this.ErrorMessage = tResult;
                        return false;
                    }

                    System.Threading.Thread.Sleep(50);

                    tResult = oracleDB.PROCESSMONTAGE(OBER_FE_ID: existingMainIdent, UNTER_FE_ID: subIdent, MASCH_ID: mID, MONTAGE_TYP: "AUS", MONTAGE_FILE: "", UNTER_WUID_OID: out UNTER_WUID_OID);
                    if (tResult != "")
                    {
                        this.ErrorMessage = tResult;
                        return false;
                    }

                    System.Threading.Thread.Sleep(1000);

                    tResult = oracleDB.PROCESSMONTAGE(OBER_FE_ID: mainIdent, UNTER_FE_ID: subIdent, MASCH_ID: mID, MONTAGE_TYP: "EIN", MONTAGE_FILE: "", UNTER_WUID_OID: out UNTER_WUID_OID);
                    if (tResult != "")
                    {
                        this.ErrorMessage = tResult;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    oracleDB.CLOSE_DATABASE_CONNECTION();
                }
                catch (Exception)
                {
                }
            }

            return true;
        }

        #endregion


        /// <summary>
        /// 该函数用于指定 Oracle.dll 的版本。
        /// Diese Funktion gibt die Version der Oracle.dll an.
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            return oracleDB.GETVERSION();
        }

        // ##############################################################
        // ###########      Konstruktor und Destruktor     ##############
        // ##############################################################

        public DBCom(ProcessAvailable processName, string userName, string password, DataSource datasource)
        {
            this.DBUserName = userName;
            this.DBPassword = password;

            if (oracleDB == null)
            {
                oracleDB = new Oracle();
                this.IsConnected = false;
            }

            this.CurrentAssignData = new AssignData();

            this.CurrentUnitData = new Unit();

            this.CurrentOperatorData = new Operator();

            this.CurrentProcess = processName;

            Oracle.DataSources datasource_parsed = Oracle.DataSources.EWS;

            if (!Enum.TryParse<Oracle.DataSources>(Enum.GetName(typeof(DataSource), datasource), out datasource_parsed))
            {
                throw new Exception($"The data source {Enum.GetName(typeof(DataSource), datasource)} is not implemented in the OracleDB!");
            }

            this.CurrentDataSource = datasource_parsed;

            this.UseTestDB = false;
        }

        public DBCom(ProcessAvailable processName, string userName, string password)
            : this(processName, userName, password, DataSource.EWS)
        {
        }

        public DBCom(ProcessAvailable processName)
            : this(processName, "fisfema", "fis1fe2+ma", DataSource.EWS)
        {
        }

        public DBCom(ProcessAvailable processName, DataSource dataSource)
            : this(processName, "fisfema", "fis1fe2+ma", dataSource)
        {
        }

        public DBCom(string userName, string password, DataSource dataSource)
            : this(ProcessAvailable.MONT, userName, password, dataSource)
        {
        }

        public DBCom(string userName, string password)
            : this(ProcessAvailable.MONT, userName, password)
        {
        }

        public DBCom()
            : this(ProcessAvailable.EPE)
        {
        }
    }
}
