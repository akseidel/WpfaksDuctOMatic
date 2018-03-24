using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace WpfaksDuctOMatic {

    public class DuctOMaticSession : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string propName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private Visibility ductshapevis = Visibility.Hidden;
        public Visibility Ductshapevis { get { return ductshapevis; } set { ductshapevis = value; OnPropertyChanged("Ductshapevis"); } }

        private int ductshapeTHK = 2;
        public int DuctshapeTHK { get { return ductshapeTHK; } set { ductshapeTHK = value; OnPropertyChanged("DuctshapeTHK"); } }

        private int canvasLeft = 5;
        public int CanvasLeft { get { return canvasLeft; } set { canvasLeft = value; OnPropertyChanged("CanvasLeft"); } }

        private int canvasTop = 5;
        public int CanvasTop { get { return canvasTop; } set { canvasTop = value; OnPropertyChanged("CanvasTop"); } }

        private DataTable solTable = new SolutionsTable();
        public DataTable SolTable { get { return solTable; } set { solTable = value; OnPropertyChanged("SolTable"); } }

        private DataRowView selrowSolTable;
        public DataRowView SelrowSolTable {
            get { return selrowSolTable; }
            set {
                selrowSolTable = value;
                OnPropertyChanged("SelrowSolTable");
                if (SelrowSolTable != null) {
                    GraphicMsg = SelrowSolTable.Row[0].ToString() + " x " + SelrowSolTable.Row[2].ToString();
                } else {
                    GraphicMsg = "";
                }
            }
        }

        private DataTable advTable = new AirDeviceTable();
        public DataTable AdvTable { get { return advTable; } set { advTable = value; OnPropertyChanged("AdvTable"); } }

        private DataRowView selrowadvTable;
        public DataRowView SelrowadvTable {
            get { return selrowadvTable; }
            set {
                selrowadvTable = value;
                OnPropertyChanged("SelrowadvTable");
            }
        }

        private double deviceTally = 0.0;
        public double DeviceTally {
            get { return deviceTally; }
            set {
                deviceTally = value;
                OnPropertyChanged("DeviceTally");
            }
        }

        private string designMsg = string.Empty;
        public string DesignMsg { get { return designMsg; } set { designMsg = value; OnPropertyChanged("DesignMsg"); } }

        private string solutionsMsg = string.Empty;
        public string SolutionsMsg { get { return solutionsMsg; } set { solutionsMsg = value; OnPropertyChanged("SolutionsMsg"); } }

        private string graphicMsg = "W x H";
        public string GraphicMsg { get { return graphicMsg; } set { graphicMsg = value; OnPropertyChanged("GraphicMsg"); } }

        private double cFM = 1000;
        public double CFM {
            get { return cFM; }
            set {
                cFM = value;
                OnPropertyChanged("CFM");
                strcFM = value.ToString();
            }
        }
        private string strcFM = "1000";
        public string StrCFM {
            get { return strcFM; }
            set {
                CFM = TextEntry2Dbl(value);
                strcFM = TextEntry2Str(value);
                OnPropertyChanged("StrCFM");
                //DesignMsg = "Changed StrCFM to " + StrCFM;
            }
        }

        private double velLimit = 1500.0;
        public double VelLimit { get { return velLimit; } set { velLimit = value; OnPropertyChanged("VelLimit"); } }
        private string strvelLimit = "1500";
        public string StrVelLimit {
            get { return strvelLimit; }
            set {
                VelLimit = TextEntry2Dbl(value);
                strvelLimit = TextEntry2Str(value);
                OnPropertyChanged("StrVelLimit");
                //DesignMsg = "Changed StrVelLimit to " + StrVelLimit;
            }
        }

        /// <summary>
        /// Turns input into a double if it looks like a number, zero if not a number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double TextEntry2Dbl(string value) {
            if (Double.TryParse(value, out double result)) {
                return result;
            } else {
                // if blank return 0
                if (value.ToUpper() == String.Empty) {
                    return 0.0;
                }
            }
            return 0.0;
        }

        /// <summary>
        /// Turns input into text as number if it looks like a number, blank if not a number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string TextEntry2Str(string value) {
            if (Double.TryParse(value, out double result)) {
                return value;
            } else {
                if (value.ToUpper() == String.Empty) {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        private double wtLL = 6.0;
        public double WtLL { get { return wtLL; } set { wtLL = value; OnPropertyChanged("WtLL"); } }
        private string strwtLL = "6";
        public string StrWtLL {
            get { return strwtLL; }
            set {
                WtLL = TextEntry2Dbl(value);
                strwtLL = TextEntry2Str(value);
                OnPropertyChanged("StrWtLL");
                //DesignMsg = "Changed WtLL " + WtLL.ToString();
            }
        }

        private double wtUL = 60.0;
        public double WtUL { get { return wtUL; } set { wtUL = value; OnPropertyChanged("WtUL"); } }
        private string strwtUL = "60";
        public string StrWtUL {
            get { return strwtUL; }
            set {
                WtUL = TextEntry2Dbl(value);
                strwtUL = TextEntry2Str(value);
                OnPropertyChanged("StrWtUL");
                //DesignMsg = "Changed WtUL " + WtUL.ToString();
            }
        }

        private double htLL = 6.0;
        public double HtLL { get { return htLL; } set { htLL = value; OnPropertyChanged("HtLL"); } }
        private string strhtLL = "6";
        public string StrHtLL {
            get { return strhtLL; }
            set {
                HtLL = TextEntry2Dbl(value);
                strhtLL = TextEntry2Str(value);
                OnPropertyChanged("StrHtLL");
                //DesignMsg = "Changed HtLL " + HtLL.ToString();
            }
        }

        private double htUL = 60.0;
        public double HtUL { get { return htUL; } set { htUL = value; OnPropertyChanged("HtUL"); } }
        private string strhtUL = "60";
        public string StrHtUL {
            get { return strhtUL; }
            set {
                HtUL = TextEntry2Dbl(value);
                strhtUL = TextEntry2Str(value);
                OnPropertyChanged("StrHtUL");
                //DesignMsg = "Changed HtUL " + HtUL.ToString();
            }
        }

        private double maxAR = 4.00;
        public double MaxAR { get { return maxAR; } set { maxAR = value; OnPropertyChanged("MaxAR"); } }
        private string strmaxAR = "4.00";
        public string StrMaxAR {
            get { return strmaxAR; }
            set {
                MaxAR = TextEntry2Dbl(value);
                strmaxAR = TextEntry2Str(value);
                OnPropertyChanged("StrMaxAR");
                //DesignMsg = "Changed MaxAR " + MaxAR.ToString();
            }
        }

        private double lossperhundred = 0.10;
        public double Lossperhundred { get { return lossperhundred; } set { lossperhundred = value; OnPropertyChanged("Lossperhundred"); } }
        private string strlossperhundred = "0.10";
        public string StrLossperhundred {
            get { return strlossperhundred; }
            set {
                Lossperhundred = TextEntry2Dbl(value);
                strlossperhundred = TextEntry2Str(value);
                OnPropertyChanged("StrLossperhundred");
                //DesignMsg = "Changed pressure loss/hundred feet to " + Lossperhundred.ToString();
            }
        }

        private double manualWidth = 12;
        public double ManualWidth { get { return manualWidth; } set { manualWidth = value; OnPropertyChanged("ManualWidth"); } }
        private string strmanualWidth = "12";
        public string StrmanualWidth {
            get { return strmanualWidth; }
            set {
                ManualWidth = TextEntry2Dbl(value);
                strmanualWidth = TextEntry2Str(value);
                OnPropertyChanged("StrmanualWidth");
                //DesignMsg = "Changed ManualWidth to " + ManualWidth.ToString();
            }
        }

        private double manualHeight = 12;
        public double ManualHeight { get { return manualHeight; } set { manualHeight = value; OnPropertyChanged("ManualHeight"); } }
        private string strmanualHeight = "12";
        public string StrmanualHeight {
            get { return strmanualHeight; }
            set {
                ManualHeight = TextEntry2Dbl(value);
                strmanualHeight = TextEntry2Str(value);
                OnPropertyChanged("StrmanualHeight");
                //DesignMsg = "Changed ManualHeight to " + ManualHeight.ToString();
            }
        }

        private string strmanRecPROP = "PLPH = N/A, Velocity = N/A, AR = N/A";
        public string StrmanRecPROP {
            get { return strmanRecPROP; }
            set {
                strmanRecPROP = value;
                OnPropertyChanged("StrmanRecPROP");
            }
        }

        private double lphMargin = 0.86;
        public double LphMargin {
            get { return lphMargin; }
            set {
                lphMargin = Math.Round(value * 50) / 50;
                OnPropertyChanged("LphMargin");
            }
        }

        private bool chkVelLimit = true;
        public bool ChkVelLimit { get { return chkVelLimit; } set { chkVelLimit = value; OnPropertyChanged("ChkVelLimit"); } }

        private bool chkusedevTally = false;
        public bool ChkusedevTally { get { return chkusedevTally; } set { chkusedevTally = value; OnPropertyChanged("ChkusedevTally"); } }

        private bool chkEnHanced = true;
        public bool ChkEnHanced { get { return chkEnHanced; } set { chkEnHanced = value; OnPropertyChanged("ChkEnHanced"); } }

        private bool chkWRange = true;
        public bool ChkWRange { get { return chkWRange; } set { chkWRange = value; OnPropertyChanged("ChkWRange"); } }

        private bool chkHRange = true;
        public bool ChkHRange { get { return chkHRange; } set { chkHRange = value; OnPropertyChanged("ChkHRange"); } }

        private double dliner = 0.0;
        public double Dliner { get { return dliner; } set { dliner = value; OnPropertyChanged("Dliner"); } }

        private bool obLiner1 = true;
        public bool ObLiner1 {
            get { return obLiner1; }
            set {
                obLiner1 = value;
                OnPropertyChanged("ObLiner1");
                if (obLiner1) {
                    dliner = 1.0;
                    OnPropertyChanged("Dliner");
                    //DesignMsg = "Changed ObLiner1 to " + obLiner1.ToString();
                }
            }
        }

        private bool obLiner5 = true;
        public bool ObLiner5 {
            get { return obLiner5; }
            set {
                obLiner5 = value;
                OnPropertyChanged("ObLiner5");
                if (obLiner5) {
                    dliner = 0.5;
                    OnPropertyChanged("Dliner");
                    //DesignMsg = "Changed ObLiner5 to " + obLiner5.ToString();
                }
            }
        }

        private bool obLinerN = true;
        public bool ObLinerN {
            get { return obLinerN; }
            set {
                obLinerN = value;
                OnPropertyChanged("ObLinerN");
                if (obLinerN) {
                    dliner = 0.0;
                    OnPropertyChanged("Dliner");
                    //DesignMsg = "Changed ObLinerN to " + obLinerN.ToString();
                }
            }
        }

        private string strmanualcalcHeader = "Width and Height Manual Entry (Rectangular)";
        public string StrManualcalcHeader { get { return strmanualcalcHeader; } set { strmanualcalcHeader = value; OnPropertyChanged("StrManualcalcHeader"); } }

        private bool obdtrect = true;
        public bool ObDTRect {
            get { return obdtrect; }
            set {
                obdtrect = value;
                OnPropertyChanged("ObDTRect");
                if (obdtrect) {
                    dtype = 0;
                    OnPropertyChanged("Dtype");
                    strmanualcalcHeader = "Width and Height Manual Entry (Rectangular)";
                    OnPropertyChanged("StrManualcalcHeader");
                    strGroupBoxSolutions = "Rectangular Sizes Meeting the Design Criteria";
                    OnPropertyChanged("StrGroupBoxSolutions");
                    //DesignMsg = "Changed DType to " + dtype.ToString();
                    ductshaperadius = 0;
                    OnPropertyChanged("DuctshapeRadius");
                }
            }
        }

        private bool obdtfo = false;
        public bool ObDTFO {
            get { return obdtfo; }
            set {
                obdtfo = value;
                OnPropertyChanged("ObDTFO");
                if (obdtfo) {
                    dtype = 1;
                    OnPropertyChanged("Dtype");
                    strmanualcalcHeader = "Width and Height Manual Entry (Flat Oval)";
                    OnPropertyChanged("StrManualcalcHeader");
                    strGroupBoxSolutions = "Flat Oval Sizes Meeting the Design Criteria";
                    OnPropertyChanged("StrGroupBoxSolutions");
                    //DesignMsg = "Changed DType to " + dtype.ToString();
                    ductshaperadius = Math.Min(ductshapewt, ductshapeht) / 2;
                    OnPropertyChanged("DuctshapeRadius");
                }
            }
        }

        private int dtype;
        public int Dtype {
            get { return dtype; }
            set {
                dtype = value; // 0 = rect 1 = flat oval
                OnPropertyChanged("Dtype");
            }
        }

        private void SetDuctShapeRadius() {
            if (dtype == 0) {
                ductshaperadius = 0;
            } else {
                ductshaperadius = Math.Min(ductshapewt, ductshapeht) / 2;
                OnPropertyChanged("DuctshapeRadius");
            }
        }

        //Rectangle ShapeDuct Size Height
        private int ductshapeht = 50;
        public int DuctshapeHT {
            get { return ductshapeht; }
            set {
                ductshapeht = value;
                OnPropertyChanged("DuctshapeHT");
                SetDuctShapeRadius();
            }
        }

        //Rectangle ShapeDuct Size Width
        private int ductshapewt = 50;
        public int DuctshapeWT {
            get { return ductshapewt; }
            set {
                ductshapewt = value;
                OnPropertyChanged("DuctshapeWT");
                SetDuctShapeRadius();
            }
        }

        //Rectangle ShapeDuct Corner Radius
        private int ductshaperadius = 0;
        public int DuctshapeRadius { get { return ductshaperadius; } set { ductshaperadius = value; OnPropertyChanged("DuctshapeRadius"); } }

        private bool showTips = false;
        public bool ShowTips { get { return showTips; } set { showTips = value; OnPropertyChanged("ShowTips"); } }

        //CheckBoxUseTally
        private bool chkUseTally = false;
        public bool ChkUseTally { get { return chkUseTally; } set { chkUseTally = value; OnPropertyChanged("ChkUseTally"); } }

        private double tallyTotal = 0;
        public double TallyTotal { get { return tallyTotal; } set { tallyTotal = value; OnPropertyChanged("TallyTotal"); } }

        //public double savSurfe;
        //private double saveSurfe;
        //public double SaveSurfe { get { return saveSurfe; } set { saveSurfe = value; OnPropertyChanged("SaveSurfe"); } }

        private double surfe = 0.0005;
        public double Surfe { get { return surfe; } set { surfe = value; OnPropertyChanged("Surfe"); } }

        //public int ductPictCenterX;
        //private double ductPictCenterX;
        //public double DuctPictCenterX { get { return ductPictCenterX; } set { ductPictCenterX = value; OnPropertyChanged("DuctPictCenterX"); } }

        ////public int ductPictCenterY;
        //private double ductPictCenterY;
        //public double DuctPictCenterY { get { return ductPictCenterY; } set { ductPictCenterY = value; OnPropertyChanged("DuctPictCenterY"); } }

        //private double surfMR = 0.003;
        //public double SurfMR { get { return surfMR; } set { surfMR = value; OnPropertyChanged("SurfMR"); } }

        //private double surfAv = 0.0005;
        //public double SurfAv { get { return surfAv; } set { surfAv = value; OnPropertyChanged("SurfAv"); } }

        //////////static double DFwidthLimitLow = 6;
        ////////private double dFwidthLimitLow = 6;
        ////////public double DFwidthLimitLow { get { return dFwidthLimitLow; } set { dFwidthLimitLow = value; OnPropertyChanged("DFwidthLimitLow"); } }
        //////////static double DFwidthLimitHigh = 120;
        ////////private double dFwidthLimitHigh = 120;
        ////////public double DFwidthLimitHigh { get { return dFwidthLimitHigh; } set { dFwidthLimitHigh = value; OnPropertyChanged("DFwidthLimitHigh"); } }
        //////////static double DFheightLimitLow = 6;
        ////////private double dFheightLimitLow = 6;
        ////////public double DFheightLimitLow { get { return dFheightLimitLow; } set { dFheightLimitLow = value; OnPropertyChanged("DFheightLimitLow"); } }
        //////////static double DFheightLimitHigh = 120;
        ////////private double dFheightLimitHigh = 120;
        ////////public double DFheightLimitHigh { get { return dFheightLimitHigh; } set { dFheightLimitHigh = value; OnPropertyChanged("DFheightLimitHigh"); } }


        //private int dFPictWidth = 50;
        //public int DFPictWidth { get { return dFPictWidth; } set { dFPictWidth = value; OnPropertyChanged("DFPictWidth"); } }
        ////////////public double widthLimitLow = DFwidthLimitLow;
        //////////private double widthLimitLow = 6;
        //////////public double WidthLimitLow { get { return widthLimitLow; } set { widthLimitLow = value; OnPropertyChanged("WidthLimitLow"); } }
        ////////////public double widthLimitHigh = DFwidthLimitHigh;
        //////////private double widthLimitHigh = 120;
        //////////public double WidthLimitHigh { get { return widthLimitHigh; } set { widthLimitHigh = value; OnPropertyChanged("WidthLimitHigh"); } }
        ////////////public double heightLimitLow = DFheightLimitLow;
        //////////private double heightLimitLow = 6;
        //////////public double HeightLimitLow { get { return heightLimitLow; } set { heightLimitLow = value; OnPropertyChanged("HeightLimitLow"); } }
        ////////////public double heightLimitHigh = DFheightLimitHigh;
        //////////private double heightLimitHigh = 120;
        //////////public double HeightLimitHigh { get { return heightLimitHigh; } set { heightLimitHigh = value; OnPropertyChanged("HeightLimitHigh"); } }


        //lbEquivCircDiameter.Text = "Diameter: " + _REqCirDct + 2 * DLiner + " IN";
        private string strEquivCircDiameter = "Diameter (IN)= N/A";
        public string StrEquivCircDiameter { get { return strEquivCircDiameter; } set { strEquivCircDiameter = value; OnPropertyChanged("StrEquivCircDiameter"); } }
        //     lbEqivCircDuctPLPH.Text = "PLPH: " + Strings.Format(_DDia_ft, "0.000") + " IN H2O";
        private string strEquivCircDuctPLPH = "PLPH:";
        public string StrEquivCircDuctPLPH { get { return strEquivCircDuctPLPH; } set { strEquivCircDuctPLPH = value; OnPropertyChanged("StrEquivCircDuctPLPH"); } }
        //lbEquivCircDuctVel.Text = "Velocity: " + Strings.Format(Vel(cfm, _area_sf), "#,##0") + " FPM";
        private string strEquivCircDuctVel = "Velocity:";
        public string StrEquivCircDuctVel { get { return strEquivCircDuctVel; } set { strEquivCircDuctVel = value; OnPropertyChanged("StrEquivCircDuctVel"); } }
        //LabelMaterialUse.Text = "Material/FT (SF): " + Strings.Format(_sfPft, "#,##0.00");
        private string strMaterialUse = "Velocity:";
        public string StrMaterialUse { get { return strMaterialUse; } set { strMaterialUse = value; OnPropertyChanged("StrMaterialUse"); } }

        private string strGroupBoxSolutions = "Rectangular Sizes Meeting the Design Criteria";
        public string StrGroupBoxSolutions { get { return strGroupBoxSolutions; } set { strGroupBoxSolutions = value; OnPropertyChanged("StrGroupBoxSolutions"); } }

        private bool airdevExpanded = false;
        public bool AirdevExpanded { get { return airdevExpanded; } set { airdevExpanded = value; OnPropertyChanged("AirdevExpanded"); } }

        private bool recsolExpanded = true;
        public bool RecsolExpanded { get { return recsolExpanded; } set { recsolExpanded = value; OnPropertyChanged("RecsolExpanded"); } }

        #region This is all about binding the duct smoothness combobox
        // This is all about binding the duct smoothness combobox list and
        // the selected value (an object) in that list.
        public class DuctSmoothnessType {
            private double d_surfe;
            public double D_Surfe {
                get { return d_surfe; }
                set { d_surfe = value; }
            }

            private string s_surfedescription;
            public string S_Surfedescription {
                get { return s_surfedescription; }
                set { s_surfedescription = value; }
            }
        }

        // private part
        private List<DuctSmoothnessType> surfeChoices = new List<DuctSmoothnessType>(){
            new DuctSmoothnessType(){ D_Surfe=0.0001, S_Surfedescription=  "0.0001 | Baby Ass Smooth - uncoated steel, PVC, Aluminum"}
            ,new  DuctSmoothnessType(){ D_Surfe=0.0003, S_Surfedescription="0.0003 | Med. Smooth - Galvanized steel, spiral seam"}
            ,new  DuctSmoothnessType(){ D_Surfe=0.0005, S_Surfedescription="0.0005 | Average - Galvanized steel, 2.5 ft joints"}
            ,new  DuctSmoothnessType(){ D_Surfe=0.003, S_Surfedescription= "0.0030 | Med. Rough - Lined"}
            ,new  DuctSmoothnessType(){ D_Surfe=0.01, S_Surfedescription=  "0.0100 | Turkey Neck Rough - Flex duct"}
        };
        // public pasrt
        public List<DuctSmoothnessType> SurfeChoices {
            get { return surfeChoices; }
            set {
                surfeChoices = value;
                OnPropertyChanged("SurfeChoices");
            }
        }

        // The selected duct smoothness type
        private DuctSmoothnessType surfeSelected = new DuctSmoothnessType() { D_Surfe = 0.0005, S_Surfedescription = "Average - Galvanized steel, 2.5 ft joints" }; // default
        public DuctSmoothnessType SurfeSelected {
            get { return surfeSelected; }
            set {
                surfeSelected = value;
                surfe = surfeSelected.D_Surfe;
                OnPropertyChanged("SurfeSelected");
                OnPropertyChanged("Surfe");
                //DesignMsg = "Selected surfe of " + surfe.ToString();
            }
        }
        #endregion


        #region This is all about binding the duct pressureclass combobox
        //// This is all about binding the duct smoothness combobox list and
        //// the selected value (an object) in that list.
        public class DuctPClassType {
            private double d_ptypePLPH;
            public double D_PTypePLPH {
                get { return d_ptypePLPH; }
                set { d_ptypePLPH = value; }
            }

            private string s_ptypedescription;
            public string S_PTypedescription {
                get { return s_ptypedescription; }
                set { s_ptypedescription = value; }
            }

            private double d_vellimit;
            public double D_Vellimit {
                get { return d_vellimit; }
                set { d_vellimit = value; }
            }

        }

        //// private part
        private List<DuctPClassType> ptypeChoices = new List<DuctPClassType>(){
            new DuctPClassType(){ D_PTypePLPH=0.10, D_Vellimit= 1500, S_PTypedescription=  "Low_Pressure_Ducts  0.10 IN  1500 FPM"}
            ,new  DuctPClassType(){ D_PTypePLPH=0.20, D_Vellimit= 2500, S_PTypedescription="Medium_Pressure_Ducts  0.20 IN  2500 FPM"}
            ,new  DuctPClassType(){ D_PTypePLPH=0.40, D_Vellimit= 3500, S_PTypedescription="High_Pressure_Ducts  0.40 IN  3500 FPM"}
            ,new  DuctPClassType(){ D_PTypePLPH=0.03, D_Vellimit= 500, S_PTypedescription= "Transfer_Ducts  0.03 IN  500 FPM"}
        };
        //// public part
        public List<DuctPClassType> PTypeChoices {
            get { return ptypeChoices; }
            set {
                ptypeChoices = value;
                OnPropertyChanged("PTypeChoices");
            }
        }

        //// The selected duct pressure class type PLPH
        private DuctPClassType ptypeSelected = new DuctPClassType() { D_PTypePLPH = 0.10, D_Vellimit = 1500, S_PTypedescription = "Low_Pressure_Ducts  0.10 IN  1500 FPM" }; // default
        public DuctPClassType PTypeSelected {
            get { return ptypeSelected; }
            set {
                ptypeSelected = value;
                OnPropertyChanged("PTypeSelected");
                lossperhundred = value.D_PTypePLPH;
                strlossperhundred = value.D_PTypePLPH.ToString();
                OnPropertyChanged("StrLossperhundred");
                velLimit = value.D_Vellimit;
                strvelLimit = value.D_Vellimit.ToString();
                OnPropertyChanged("StrVelLimit");
                //DesignMsg = "Changed pressure loss/hundred feet to " + Lossperhundred.ToString();
            }
        }
        #endregion

        public static Boolean IsNumeric(String input) {
            Boolean result = Double.TryParse(input, out double temp);
            return result;
        }

    }
}
