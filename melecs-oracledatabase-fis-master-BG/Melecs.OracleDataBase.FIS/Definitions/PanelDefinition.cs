using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melecs.OracleDataBase.FIS
{
    public class PanelDefinition
    {
        public PanelDefinition(string name, int subPos, string matNr, string matNrZFER, int xPos, int yPos, int rotation, string connectedTo)
        {
            PanelName = name;
            BoardPosition = subPos;
            MaterialNumber = matNr;
            MaterialNumber_ZFER = matNrZFER;
            X_Position = xPos;
            Y_Position = yPos;
            Rotation = rotation;
            ConnectedTo = connectedTo;
        }

        public PanelDefinition() : this(string.Empty, 0, string.Empty, string.Empty, 0, 0, 0, string.Empty)
        {

        }

        public PanelDefinition(OracleDB.PanelDefinition panelDefinition) : this(panelDefinition.PanelName, panelDefinition.BoardPosition, panelDefinition.MaterialNumber,
            panelDefinition.MaterialNumber_ZFER, panelDefinition.X_Position, panelDefinition.Y_Position, panelDefinition.Rotation, panelDefinition.ConnectedTo)
        {
            
        }
        
        public string PanelName { get; set; }

        public int BoardPosition { get; set; }

        public string MaterialNumber { get; set; }

        public string MaterialNumber_ZFER { get; set; }

        public int X_Position { get; set; }
        
        public int Y_Position { get; set; }

        public int Rotation { get; set; }

        public string ConnectedTo { get; set; }
    }
}
