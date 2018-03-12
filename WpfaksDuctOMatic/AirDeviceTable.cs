using System.Data;

namespace WpfaksDuctOMatic
{
    internal class AirDeviceTable : DataTable {
        public AirDeviceTable() {
            Columns.Add("ADCFM", typeof(double));
            Columns.Add("QTY", typeof(double));
            Columns.Add("NOTES", typeof(string));
        }
    }
}
