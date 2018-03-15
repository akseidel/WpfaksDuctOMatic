namespace WpfaksDuctOMatic {
    internal class TableSolutionArgs {
        public double Cfm { get; set; }
        public double Lph { get; set; }
        public double Surfe { get; set; }
        public bool Colebrook { get; set; }
        public bool Limitvelocity { get; set; }
        public double Vellimit { get; set; }
        public double MaxAr { get; set; }
        public double DLiner { get; set; }
        public double LphMargin { get; set; }
        public int Dtype { get; set; }
        public bool ChkHRange { get; set; }
        public double HtLL { get; set; }
        public double HtUL { get; set; }
        public bool ChkWRange { get; set; }
        public double WtLL { get; set; }
        public double WtUL { get; set; }
    }
}