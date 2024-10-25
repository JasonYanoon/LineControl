using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melecs.OracleDataBase.FIS
{
    public enum ProcessAvailable
    {
        ICT = 1,
        EPE = 2,
        EPE2 = 3,
        VIS1 = 4,
        VIS2 = 5,
        MONT = 6,
        VERP = 7,
        BEST1 = 8,
        BEST2 = 9
    }

    public enum ObjectTypes
    {
        NutzenIdent = 0,
        EinzelIdent = 1,
        All = 2
    }

    /// <summary>
    /// The available Workplace types.
    /// </summary>
    public enum ApTypeAvailable
    {
        /// <summary>
        /// The workplace group.
        /// </summary>
        APGLIST = 1,

        /// <summary>
        /// The logic workplace.
        /// </summary>
        LAPLIST = 2
    }

    /// <summary>
    /// Indicates which data source should be used to connect to FIS.
    /// </summary>
    public enum DataSource
    {
        /// <summary>
        /// EWS DB.
        /// </summary>
        EWS,

        /// <summary>
        /// Györ DB.
        /// </summary>
        EWG,

        /// <summary>
        /// Wuxi DB.
        /// </summary>
        EWW,

        /// <summary>
        /// The central DB.
        /// </summary>
        Central,

        /// <summary>
        /// The ews test DB.
        /// </summary>
        EWSTest,

        /// <summary>
        /// The central test DB.
        /// </summary>
        CentralTest,

        /// <summary>
        /// The ewg test DB.
        /// </summary>
        EWGTest,

        /// <summary>
        /// The ewq DB.
        /// </summary>
        EWQ

    }

    /// <summary>
    /// The Values of the WorkplaceStates from the EMF.
    /// </summary>
    public enum EMFWorkplaceStates
    {
        /// <summary>
        /// The workplace is locked.
        /// </summary>
        Locked,

        /// <summary>
        /// The workplace is free.
        /// </summary>
        Free,

        /// <summary>
        /// The workplace has been unlocked but has to be tested again.
        /// </summary>
        Retest
    }

    /// <summary>
    /// The Values of the WcTypes.
    /// </summary>
    public enum WcType
    {
        /// <summary>
        /// Automatic Packaging Type. Only Idents will be sent to the packaging Station
        /// </summary>
        Normal,

        /// <summary>
        /// Automatic Packaging Type. Idents and Tracenumber will be sent to the packaging Station
        /// </summary>
        AutomaticTracenumber
    }

}
