using System.Data;

namespace WpfaksDuctOMatic
{
    internal class SolutionsTable : DataTable {
        public SolutionsTable() {
            Columns.Add("Width", typeof(double));
            Columns.Add("X", typeof(string));
            Columns.Add("Height", typeof(double));
            Columns.Add("Type", typeof(string));
            Columns.Add("PLPH", typeof(string));
            Columns.Add("VFPM", typeof(string));
            Columns.Add("AR", typeof(string));
            Columns.Add("PFT", typeof(string));
        }
    }
}
