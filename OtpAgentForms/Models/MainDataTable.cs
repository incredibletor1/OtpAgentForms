using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtpAgentForms.Models
{
    public class MainDataTable
    {
        public DataTable DataTable() => new DataTable()
        {
            Columns =
            {
                {
                  "COM",
                  typeof (string)
                },
                {
                  "Operator",
                  typeof (string)
                },
                {
                  "Numberphone",
                  typeof (string)
                },
                {
                  "Rssi",
                  typeof (string)
                },
                {
                  "Hub name",
                  typeof (string)
                },
                {
                  "Port number",
                  typeof (string)
                },
                {
                  "Information",
                  typeof (string)
                }
            }
        };
    }
}
