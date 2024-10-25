using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melecs.OracleDataBase.FIS
{
    public class Operator
    {
        public string Vorname { get; set; }

        public string Nachname { get; set; }

        public string Schicht { get; set; }

        public string PrueferNr { get; set; }

        public Operator()
        {
            this.Vorname = "";
            this.Nachname = "";
            this.Schicht = "";
            this.PrueferNr = "";
        }
    }
}
