
namespace Melecs.OracleDataBase.FIS
{
    public class Unit : AssignData
    {

        public string Ident { get; set; }

        public string OberstufenMaterialnummer { get; set; }

        public string OberstufenAuftragsnummer { get; set; }

        public bool Hochgerüstet { get; set; }

        public string Durchlauf { get; set; }

        public string Nutzenposition { get; set; }

        public string Stueckzahl { get; set; }

        public string Barcodetyp { get; set; }

        public string OffeneAuftragsStückzahl { get; set; }

        public bool GoldenSample { get; set; }

        /// <summary>
        /// Reference Sample Type: this is the acronym for the reference type e.g. G for golden sample
        /// </summary>
        public string RefSampleType { get; set; }

        /// <summary>
        /// Reference Sample Type Name: this is the full name of the reference type
        /// </summary>
        public string RefSampleTypeName { get; set; }

        public Unit() : base()
        {
            this.Ident = "";
            this.OberstufenMaterialnummer = "";
            this.OberstufenAuftragsnummer = "";
            this.Hochgerüstet = false;
            this.Durchlauf = "";
            this.Nutzenposition = "";
            this.Stueckzahl = "";
            this.Barcodetyp = "";
            this.OffeneAuftragsStückzahl = "";
            this.GoldenSample = false;
            this.RefSampleType = string.Empty;
            this.RefSampleTypeName = string.Empty;
        }

    }
}
