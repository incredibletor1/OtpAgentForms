namespace OtpAgentForms.Dto.Base
{
    /// <summary>
    /// Response errors
    /// </summary>
    public enum ResponseError
    {
        /// <summary>
        /// Invalid request
        /// </summary>
        //InvalidRequest = 0x1baaa,
        InvalidRequest = 400,

        /// <summary>
        /// Internal error
        /// </summary>
        //InternalError = 0x1baab,
        InternalError = 500,

        /// <summary>
        /// No content
        /// </summary>
        //NoContent = 0x1baac,
        NoContent = 404,

        /// <summary>
        /// Forbidden
        /// </summary>
        //Forbidden = 0x1baad,
        Forbidden = 403,

        /////////////////////
        // business errors //
        /////////////////////

        /// <summary>
        /// User name already exist
        /// </summary>
        UserNameExist = 1501,

        /// <summary>
        /// User not exists
        /// </summary>
        UserNotExists = 1502,

        /// <summary>
        /// User already deleted
        /// </summary>
        UserAlreadyDeleted = 1503,

        /// <summary>
        /// User already restored
        /// </summary>
        UserAlreadyRestored = 1504,

        /// <summary>
        /// Agent name already exist
        /// </summary>
        AgentNameExist = 1511,

        /// <summary>
        /// Agent not exists
        /// </summary>
        AgentNotExists = 1512,

        /// <summary>
        /// Agent already deleted
        /// </summary>
        AgentAlreadyDeleted = 1513,

        /// <summary>
        /// Agent already restored
        /// </summary>
        AgentAlreadyRestored = 1514,

        /// <summary>
        /// Agent address in use
        /// </summary>
        AgentAddressInUse = 1515,

        /// <summary>
        /// Service code already exist
        /// </summary>
        ServiceCodeExist = 1521,

        /// <summary>
        /// Service not exists
        /// </summary>
        ServiceNotExists = 1522,

        /// <summary>
        /// Service already deleted
        /// </summary>
        ServiceAlreadyDeleted = 1523,

        /// <summary>
        /// Service already restored
        /// </summary>
        ServiceAlreadyRestored = 1524,

        /// <summary>
        /// Service code not exist
        /// </summary>
        ServiceCodeNotExist = 1525,

        /// <summary>
        /// Service sender already exist
        /// </summary>
        ServiceSenderExist = 1526,

        /// <summary>
        /// Service sender not exist
        /// </summary>
        ServiceSenderNotExist = 1527,

        /// <summary>
        /// Country code already exist
        /// </summary>
        CountryCodeExist = 1531,

        /// <summary>
        /// Country not exists
        /// </summary>
        CountryNotExists = 1532,

        /// <summary>
        /// Country already deleted
        /// </summary>
        CountryAlreadyDeleted = 1533,

        /// <summary>
        /// Country already restored
        /// </summary>
        CountryAlreadyRestored = 1534,

        /// <summary>
        /// Currency abbriviature already exist
        /// </summary>
        CurrencyAbbriviatureExist = 1541,

        /// <summary>
        /// Currency not exists
        /// </summary>
        CurrencyNotExists = 1542,

        /// <summary>
        /// Currency already deleted
        /// </summary>
        CurrencyAlreadyDeleted = 1543,

        /// <summary>
        /// Currency already restored
        /// </summary>
        CurrencyAlreadyRestored = 1544,

        /// <summary>
        /// Provider code already exist
        /// </summary>
        ProviderCodeExist = 1551,

        /// <summary>
        /// Provider not exists
        /// </summary>
        ProviderNotExists = 1552,

        /// <summary>
        /// Provider already deleted
        /// </summary>
        ProviderAlreadyDeleted = 1553,

        /// <summary>
        /// Provider already restored
        /// </summary>
        ProviderAlreadyRestored = 1554,

        /// <summary>
        /// Client name already exist
        /// </summary>
        ClientNameExist = 1561,

        /// <summary>
        /// Client not exists
        /// </summary>
        ClientNotExists = 1562,

        /// <summary>
        /// Client already deleted
        /// </summary>
        ClientAlreadyDeleted = 1563,

        /// <summary>
        /// Client already restored
        /// </summary>
        ClientAlreadyRestored = 1564,

        /// <summary>
        /// Service already allowed for client
        /// </summary>
        ClientServiceAlreadyAllowd = 1565,

        /// <summary>
        /// Service already denyed for client
        /// </summary>
        ClientServiceAlreadyDenyed = 1566,

        /// <summary>
        /// Agent already allowed for client
        /// </summary>
        AgentServiceAlreadyAllowd = 1567,

        /// <summary>
        /// Agent already denyed for client
        /// </summary>
        AgentServiceAlreadyDenyed = 1568,

        /// <summary>
        /// Undefined error
        /// </summary>
        UndefinedError = 503,
    }
}