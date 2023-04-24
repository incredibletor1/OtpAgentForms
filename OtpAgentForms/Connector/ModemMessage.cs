namespace OtpAgentForms.Connector
{
    /// <summary>
    /// Dto for serial data request and response
    /// </summary>
    internal class ModemMessage
    {
        /// <summary>
        /// Message id
        /// </summary>
        public Guid MessageId { get; private set; }

        /// <summary>
        /// Request
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// Is request has been send to port
        /// True if send
        /// </summary>
        public bool RequestHasBeenSend { get; set; }

        /// <summary>
        /// Received response
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Is response has been send from port
        /// True if recived
        /// </summary>
        public bool ResponseHasBeenReceived { get; set; }

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="request"></param>
        public ModemMessage(string request)
        {
            MessageId = Guid.NewGuid();
            Request = request;
            Response = null;
            RequestHasBeenSend = false;
            ResponseHasBeenReceived = false;
        }
    }
}