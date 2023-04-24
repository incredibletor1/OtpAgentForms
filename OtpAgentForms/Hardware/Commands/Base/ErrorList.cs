namespace OtpAgentForms.Hardware.Commands.Base
{
    /// <summary>
    /// At error list
    /// </summary>
    public enum ErrorList
    {
        // Generic errors //

        /// <summary>
        /// At error
        /// </summary>
        Error = 0x0100,

        /// <summary>
        /// Empty response
        /// </summary>
        NoResponse = 0x0101,

        /// <summary>
        /// Serial ports error
        /// </summary>
        SerialPortError = 0x0102,

        /// <summary>
        /// Timeout error
        /// </summary>
        Timeout = 0x0103,

        /// <summary>
        /// Undefined error
        /// </summary>
        GenericUndefined = 0x0104,

        // Network errors //



        // Equipment errors //

        /// <summary>
        /// Phone failure (0)
        /// </summary>
        PhoneFailure = 0x0200,

        /// <summary>
        /// No connection to phone (1)
        /// </summary>
        NoConnectionToPhone = 0x0201,

        /// <summary>
        /// Phone adapter link reserved (2)
        /// </summary>
        PhoneAdapterLinkReserved = 0x0202,

        /// <summary>
        /// Operation not allowed (3)
        /// </summary>
        OperationNotAllowed = 0x0203,

        /// <summary>
        /// Operation not supported (4)
        /// </summary>
        OperationNotSupported = 0x0204,

        /// <summary>
        /// PH_SIM PIN required (5)
        /// </summary>
        PHSimPinRequired = 0x0205,

        /// <summary>
        /// PH_FSIM PIN required (6)
        /// </summary>
        PHFSimPinRequired = 0x0206,

        /// <summary>
        /// PH_FSIM PUK required (7)
        /// </summary>
        PHFSimPukRequired = 0x0207,

        /// <summary>
        /// SIM not inserted (10)
        /// </summary>
        SimNotInserted = 0x020A,

        /// <summary>
        /// SIM PIN required (11)
        /// </summary>
        SimPinRequired = 0x020B,

        /// <summary>
        /// SIM PUK required (12)
        /// </summary>
        SimPukRequired = 0x020C,

        /// <summary>
        /// SIM failure (13)
        /// </summary>
        SimFailure = 0x020D,

        /// <summary>
        /// SIM busy (14)
        /// </summary>
        SimBusy = 0x020E,

        /// <summary>
        /// Sim wrong (15)
        /// </summary>
        SimWrong = 0x020F,

        /// <summary>
        /// Incorrect password (16)
        /// </summary>
        IncorrectPassword = 0x0210,

        /// <summary>
        /// SIM PIN2 required (17)
        /// </summary>
        SimPin2Required = 0x0211,

        /// <summary>
        /// SIM PUK2 required (18)
        /// </summary>
        SimPuk2Required = 0x0212,

        /// <summary>
        /// Memory full (20)
        /// </summary>
        MemoryFull = 0x0214,

        /// <summary>
        /// Invalid index (21)
        /// </summary>
        InvalidIndex = 0x0215,

        /// <summary>
        /// Not found (22)
        /// </summary>
        NotFound = 0x0216,

        /// <summary>
        /// Memory failure (23)
        /// </summary>
        MemoryFailure = 0x0217,

        /// <summary>
        /// Text string too long (24)
        /// </summary>
        TextStringTooLong = 0x0218,

        /// <summary>
        /// Invalid characters in text string (25)
        /// </summary>
        InvalidCharactersInTextString = 0x0219,

        /// <summary>
        /// Dial string too long (26)
        /// </summary>
        DialStringTooLong = 0x021A,

        /// <summary>
        /// Invalid characters in dial string (27)
        /// </summary>
        InvalidCharactersInDialString = 0x021b,

        /// <summary>
        /// No network service (30)
        /// </summary>
        NoNetworkService = 0x021e,

        /// <summary>
        /// Network timeout (31)
        /// </summary>
        NetworkTimeout = 0x021f,

        /// <summary>
        /// Network not allowed, emergency calls only (32)
        /// </summary>
        NetworkNotAllowedEmergencyCallsOnly = 0x0220,

        /// <summary>
        /// Network personalization PIN required (40)
        /// </summary>
        NetworkPersonalizationPinRequired = 0x0228,

        /// <summary>
        /// Network personalization PUK required (41)
        /// </summary>
        NetworkPersonalizationPukRequired = 0x0229,

        /// <summary>
        /// Network subset personalization PIN required (42)
        /// </summary>
        NetworkSubsetPersonalizationPinRequired = 0x022a,

        /// <summary>
        /// Network subset personalization PUK required (43)
        /// </summary>
        NetworkSubsetPersonalizationPukRequired = 0x022b,

        /// <summary>
        /// Service provider personalization PIN required (44)
        /// </summary>
        ServiceProviderPersonalizationPinRequired = 0x022c,

        /// <summary>
        /// Service provider personalization PUK required (45)
        /// </summary>
        ServiceProviderPersonalizationPukRequired = 0x022d,

        /// <summary>
        /// Corporate personalization PIN required (46)
        /// </summary>
        CorporatePersonalizationPinRequired = 0x022e,

        /// <summary>
        /// Corporate personalization PUK required (47)
        /// </summary>
        CorporatePersonalizationPukRequired = 0x022f,

        /// <summary>
        /// PH-SIM PUK required (48)
        /// </summary>
        PHSimPukRequired = 0x0230,

        /// <summary>
        /// Unknown error (100)
        /// </summary>
        UnknownError = 0x0264,

        /// <summary>
        /// Illegal MS (103)
        /// </summary>
        IllegalMS = 0x0267,

        /// <summary>
        /// Illegal ME (106)
        /// </summary>
        IllegalME = 0x026a,

        /// <summary>
        /// GPRS services not allowed (107)
        /// </summary>
        GPRSServicesNotAllowed = 0x026b,

        /// <summary>
        /// PLMN not allowed (111)
        /// </summary>
        PLMNNotAllowed = 0x026f,

        /// <summary>
        /// Location area not allowed (112)
        /// </summary>
        LocationAreaNotAllowed = 0x0270,

        /// <summary>
        /// Roaming not allowed in this location area (113)
        /// </summary>
        RoamingNotAllowedInThisLocationArea = 0x0271,

        /// <summary>
        /// Operation temporary not allowed (126)
        /// </summary>
        OperationTemporaryNotAllowed = 0x027e,

        /// <summary>
        /// Service operation not supported (132)
        /// </summary>
        ServiceOperationNotSupported = 0x0284,

        /// <summary>
        /// Requested service option not subscribed (133)
        /// </summary>
        RequestedServiceOptionNotSubscribed = 0x0285,

        /// <summary>
        /// Service option temporary out of order (134)
        /// </summary>
        ServiceOptionTemporaryOutOfOrder = 0x0286,

        /// <summary>
        /// Unspecified GPRS error (148)
        /// </summary>
        UnspecifiedGprsError = 0x0294,

        /// <summary>
        /// PDP authentication failure (149)
        /// </summary>
        PdpAuthenticationFailure = 0x0295,

        /// <summary>
        /// Invalid mobile class (150)
        /// </summary>
        InvalidMobileClass = 0x0296,

        /// <summary>
        /// Operation temporarily not allowed (256)
        /// </summary>
        OperationTemporarilyNotAllowed = 0x02100,

        /// <summary>
        /// Call barred (257)
        /// </summary>
        CallBarred = 0x02101,

        /// <summary>
        /// Phone is busy (258)
        /// </summary>
        PhoneIsBusy = 0x02102,

        /// <summary>
        /// User abort (259)
        /// </summary>
        UserAbort = 0x02103,

        /// <summary>
        /// Invalid dial string (260)
        /// </summary>
        InvalidDialString = 0x02104,

        /// <summary>
        /// SS not executed (261)
        /// </summary>
        SsNotExecuted = 0x02105,

        /// <summary>
        /// SIM blocked (262)
        /// </summary>
        SIMBlocked = 0x02106,

        /// <summary>
        /// Invalid block (263)
        /// </summary>
        InvalidBlock = 0x02107,

        /// <summary>
        /// SIM powered down (772)
        /// </summary>
        SimPoweredDown = 0x02304
    }
}