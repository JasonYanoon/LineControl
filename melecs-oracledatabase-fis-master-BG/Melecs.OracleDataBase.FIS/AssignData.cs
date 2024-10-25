using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melecs.OracleDataBase.FIS
{
    public class AssignData
    {

        public string Auftrag { get; set; }

        public string G_Materialnummer { get; set; }

        public string MLFB { get; set; }

        public string MLFB_Index { get; set; }

        public string BSH_Materialnummer { get; set; }

        public string BSH_Materialnummer_Kunde { get; set; }

        public string Verpackungseinheit { get; set; }

        public string SW_SYC { get; set; }

        public string EEP_File { get; set; }

        public string SW_VER { get; set; }

        public string SW_Index { get; set; }

        public string Benennung { get; set; }

        public string Typ { get; set; }

        public string F_Materialnummer { get; set; }

        public string Approbationstype { get; set; }

        public string Kommentar { get; set; }

        public string Verfahrenstechnik { get; set; }

        public string Eingabetext { get; set; }

        public string EPE_PRG { get; set; }

        public string EPE_PRG_VAR { get; set; }

        public string EPE_MDATA { get; set; }

        public string EPE_ADAP { get; set; }

        public string ICT_PRG { get; set; }

        public string ICT_PRG_VAR { get; set; }

        public string ICT_ADAP { get; set; }

        public string Sachnummer { get; set; }

        public string MachineID { get; set; }

        public int NutzenAnzahl { get; set; }

        public AssignData()
        {
            this.Approbationstype = "";
            this.Auftrag = "";
            this.Benennung = "";
            this.BSH_Materialnummer = "";
            this.BSH_Materialnummer_Kunde = "";
            this.EEP_File = "";
            this.Eingabetext = "";
            this.EPE_ADAP = "";
            this.EPE_MDATA = "";
            this.EPE_PRG = "";
            this.EPE_PRG_VAR = "";
            this.F_Materialnummer = "";
            this.G_Materialnummer = "";
            this.ICT_ADAP = "";
            this.ICT_PRG = "";
            this.ICT_PRG_VAR = "";
            this.Kommentar = "";
            this.MachineID = "";
            this.MLFB = "";
            this.MLFB_Index = "";
            this.Sachnummer = "";
            this.SW_Index = "";
            this.SW_SYC = "";
            this.SW_VER = "";
            this.Typ = "";
            this.Verfahrenstechnik = "";
            this.Verpackungseinheit = "";
            this.NutzenAnzahl = 0;
        }
    }
}
