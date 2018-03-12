using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfaksDuctOMatic {

    public partial class MainWindow : Window {
        public DuctOMaticSession sessionModel = new DuctOMaticSession();
        bool Silent = false;
        public double Dinc;
        public double d_inc = 1;
        public double PI = Math.PI;  //3.141592654
        static double DFwidthLimitLow = 6;
        static double DFwidthLimitHigh = 60;
        static double DFheightLimitLow = 6;
        static double DFheightLimitHigh = 60;
        public int DFPictWidth = 50;

        public MainWindow() {
            InitializeComponent();
            DataContext = sessionModel;
        }

        private void SetToNullState(bool includeDV) {
            if (includeDV) {
                sessionModel.StrEquivCircDiameter = "Diameter (IN): N/A";
                sessionModel.StrEquivCircDuctPLPH = "PLPH (IN): N/A";
                sessionModel.StrEquivCircDuctVel = "Velocity (FPM): N/A";
                sessionModel.StrMaterialUse = "Material/FT (SF): N/A";
                sessionModel.SolTable.Clear();
                sessionModel.DesignMsg = string.Empty;
                sessionModel.SolutionsMsg = string.Empty;
                sessionModel.GraphicMsg = string.Empty;
            }
        }

        /// <summary>
        /// calculates the ashre circular soultion 
        /// </summary>
        public void CalcCircularSolution() {
            if (!IsInitialized | !IsLoaded) { return; }
            if (FlunkCriticals()) { return; };
            double req_airstream_CirDctDiam = 0;
            double area_sf = 0;
            double plphft_inH2O = 0;
            double sfPft = 0;
            double ductouterdiam = 0;
            double ductLinerInches = sessionModel.Dliner;
            double cfm = sessionModel.CFM;
            double lossperhundred = sessionModel.Lossperhundred;
            double surfe = sessionModel.Surfe;
            bool colebrook = sessionModel.ChkEnHanced;
            bool limitthevelocity = sessionModel.ChkVelLimit;
            double vellimit = sessionModel.VelLimit;
            // sf of duct per ft of duct, i.e. material use
            if (!(Silent)) {
                // try {
                SetToNullState(true);
                req_airstream_CirDctDiam = ReqEqvCirDuct(cfm, lossperhundred, surfe, colebrook, limitthevelocity, vellimit);
                ductouterdiam = req_airstream_CirDctDiam + 2 * ductLinerInches;
                plphft_inH2O = CircDuctPLPH(cfm, req_airstream_CirDctDiam, surfe, colebrook);
                area_sf = PI * (req_airstream_CirDctDiam / 12) * (req_airstream_CirDctDiam / 12) / 4;
                sfPft = PI * ductouterdiam / 12;

                sessionModel.StrEquivCircDiameter = "Diameter: " + ductouterdiam.ToString() + " IN";
                sessionModel.StrEquivCircDuctPLPH = "PLPH: " + plphft_inH2O.ToString("0.000") + " IN H2O";
                sessionModel.StrEquivCircDuctVel = "Velocity: " + Vel(cfm, area_sf).ToString("#,##0") + " FPM";
                sessionModel.StrMaterialUse = "Material/FT (SF): " + sfPft.ToString("#,##0.00");
                RectSolu(cfm, lossperhundred, surfe, colebrook, limitthevelocity, vellimit);
                //} catch (Exception) {
                //    sessionModel.StrEquivCircDiameter = "Diameter (IN)= N/A";
                //}
                return;
            }
        }

        public void CalcManualSolution() {
            double REqCirDct = 0;
            double area_sf = 0;
            string msg = null;
            double cfm = sessionModel.CFM;
            double surfe = sessionModel.Surfe;
            bool colebrook = sessionModel.ChkEnHanced;
            int dtype = sessionModel.Dtype;
            double manualWidth = sessionModel.ManualWidth - (2 * sessionModel.Dliner);
            double manualHeight = sessionModel.ManualHeight - (2 * sessionModel.Dliner);
            if (!(Silent)) {
                try {
                    REqCirDct = DhEqCircRO(manualHeight, manualWidth, dtype);
                    area_sf = DAreaRO(manualHeight, manualWidth, dtype);
                    double drPFT = DuctPFT(manualWidth, manualHeight, dtype);
                    msg = "PLPH: " + CircDuctPLPH(cfm, REqCirDct, surfe, colebrook).ToString("0.000");
                    msg = msg + " IN  Velocity: " + Vel(cfm, area_sf).ToString("#,##0") + " FPM";
                    msg = msg + "  AR: " + (Math.Max(manualWidth, manualHeight) / Math.Min(manualWidth, manualHeight)).ToString("#.00");
                    msg = msg + "  P: " + drPFT.ToString("0.00");
                    sessionModel.StrmanRecPROP = msg;
                } catch (Exception) {
                    sessionModel.StrmanRecPROP = "PLPH: N/A  Velocity: N/A  AR: N/A  P:N/A";
                }
            }
            return;
        }

        private void GetLiner() {
            if (sessionModel.Dliner == 1.0) {
                Dinc = 2;
                sessionModel.DuctshapeTHK = 3;
                return;
            }
            if (sessionModel.Dliner == 0.5) {
                Dinc = 1;
                sessionModel.DuctshapeTHK = 2;
                return;
            }
            if (sessionModel.Dliner == 0.0) {
                Dinc = 2;
                sessionModel.DuctshapeTHK = 1;
            }
        }

        /// <summary>
        /// Returns true is critical values are invalid and would cause trouble.
        /// </summary>
        /// <returns></returns>
        private bool FlunkCriticals() {
            SetToNullState(true);
            if (sessionModel.CFM <= 0) {
                sessionModel.DesignMsg = "No go! Check the CFM.";
                return true;
            }
            if (sessionModel.Lossperhundred <= 0) {
                sessionModel.DesignMsg = "No go! Check the LPH.";
                return true;
            }
            if (sessionModel.Surfe <= 0) {
                sessionModel.DesignMsg = "No go! Check the Roughness.";
                return true;
            }
            return false;
        }

        //   Main function
        //....................................................
        public double ReqEqvCirDuct(double _cfm, double _lossperhundred, double _surfe, bool _colebrook, bool _limitvelocity, double _vellimit) {
            double _functionReturnValue = 0;
            double _duct_Dh = 0;
            double _new_PD = 0;
            double _delta_pd = 0;
            double _olddelta_pd = 0;
            double _aprox = 0.34;

            // double check - we want to round down??
            _duct_Dh = (int)(Math.Pow(_cfm, _aprox));
            // first guess
            _new_PD = CircDuctPLPH(_cfm, _duct_Dh, _surfe, _colebrook);
            _delta_pd = _new_PD - _lossperhundred;
            _olddelta_pd = _delta_pd;
            // loops guesses until delta changes sign
            while (Math.Sign(_delta_pd) == Math.Sign(_olddelta_pd)) {
                _new_PD = CircDuctPLPH(_cfm, _duct_Dh, _surfe, _colebrook);
                _olddelta_pd = _delta_pd;
                _delta_pd = _new_PD - _lossperhundred;
                if ((_new_PD < _lossperhundred)) {
                    _duct_Dh = _duct_Dh - d_inc;
                } else {
                    _duct_Dh = _duct_Dh + d_inc;
                }
            }
            _duct_Dh = _duct_Dh + d_inc;
            // At this point the solution satisfies the PLPH requirement but it may
            // not satisfy any velocity limit if there is one.
            if (_limitvelocity && _vellimit > 0) {
                while ((_cfm / ((PI / (4) * Math.Pow((_duct_Dh / 12), 2)))) > _vellimit) {
                    _duct_Dh = _duct_Dh + d_inc;
                }
            }
            _functionReturnValue = _duct_Dh;
            return _functionReturnValue;
        }

        /// returns circular duct pressure loss per hundred feet given cfm
        /// hyd diameter, and surf roughness
        public double CircDuctPLPH(double _cfm, double _duct_Dh, double _surfe, bool _colebrook) {
            double _Vel = 0;
            double _Vp = 0;
            double _Re = 0;
            double _Dff = 0;
            double _chk = 0;
            _Vel = _cfm / ((PI / (4) * Math.Pow((_duct_Dh / 12), 2)));
            _Vp = Math.Pow((_Vel / 4005), 2);
            _Re = 8.56 * _duct_Dh * _Vel;
            _chk = 0.11 * Math.Pow((12 * _surfe / (_duct_Dh) + 68 / _Re), 0.25);
            if ((_chk >= 0.018)) {
                _Dff = _chk;
            } else {
                _Dff = 0.85 * _chk + 0.0028;
            }
            if (_colebrook) { _Dff = MoodyF(_Dff, _surfe, _duct_Dh, _Vel); }
            return _Vp * _Dff * 100 / (_duct_Dh / 12);
        }

        public double MoodyF(double seedF, double surfe, double Dh, double Vel) {
            double guessF = seedF;
            double guessCrit = 5E-06;
            double newGuess = 0;
            double twoLogArg = 0;
            double delta = 0;
            double re = re = 8.56 * Dh * Vel;
            // .5%
            double factor = 12 / (3.7 * Dh);
            do {
                //twoLogArg = -2 * Math.Log((12 * surfe / (3.7 * Dh)) + (2.51 / (re * Math.Sqrt(guessF)))) / Math.Log(10.0);
                twoLogArg = -2 * Math.Log((surfe * factor) + (2.51 / (re * Math.Sqrt(guessF)))) / Math.Log(10.0);
                // newGuess = (1 / twoLogArg) * (1 / twoLogArg);
                newGuess = 1 / (twoLogArg * twoLogArg);
                delta = Math.Abs((guessF - newGuess) / guessF);
                guessF = newGuess;
            } while (delta >= guessCrit);
            return guessF;
        }

        // returns ret or oval duct pressure loss per hundred feet given cfm
        // hyd diameter, and surf roughness, and duct type
        //////////public double DRlph(double _cfm, double _duct_wt, double _duct_ht, double _surfe, int t, bool _colebrook) {
        //////////    double _DeqC = 0;
        //////////    //DeqC = DhCircR(duct_wt, duct_ht)
        //////////    _DeqC = DhEqCircRO(_duct_wt, _duct_ht, t);
        //////////    return CircDuctPLPH(_cfm, _DeqC, _surfe, _colebrook);
        //////////}

        // DhRect returns hydraulic diameter
        public double DhRect(double _ht, double _wt) {
            return 4 * _ht * _wt / (2 * (_ht + _wt));
        }

        // DhCircR returns equilavent circular duct
        // ASHRE 32 1993 eq 25
        public double DhCircR(double _a, double _b) {
            double _N = 0;
            double _D = 0;
            _N = Math.Pow((_a * _b), 0.625);
            _D = Math.Pow((_a + _b), 0.25);
            return 1.3 * _N / _D;
        }

        // DhCircRO returns equilavent circular duct for rect or oval
        // ASHRE 32 1993 eq 25 and eq 26 (these are actually the same)
        public double DhEqCircRO(double _wt, double _ht, int _t) {
            double _a = 0;
            double _P = 0;
            double _temp = 0;
            double _lht = 0;
            double _lwt = 0;
            _lht = _ht;
            _lwt = _wt;
            switch (_t) {
                case 0:
                    // rect
                    _a = _lwt * _lht;
                    _P = 2 * (_lwt + _lht);
                    break;
                case 1:
                    // flat oval
                    // arguments are backwards
                    if (_lht > _lwt) {
                        _temp = _lwt;
                        _lwt = _lht;
                        _lht = _temp;
                    }
                    _a = (PI * _lht * _lht / 4) + _lht * (_lwt - _lht);
                    _P = PI * _lht + 2 * (_lwt - _lht);
                    break;
            }
            return 1.55 * (Math.Pow(_a, 0.625)) / (Math.Pow(_P, 0.25));
        }

        // DAreaRO returns duct area in ft*ft for rect or oval
        // t = 0 rect t= 1 flat oval
        public double DAreaRO(double wt, double ht, int t) {
            double a = 0;
            double temp = 0;
            double lht = 0;
            double lwt = 0;
            lht = ht;
            lwt = wt;
            switch (t) {
                case 0:
                    // rect
                    a = lwt * lht;
                    break;
                case 1:
                    // flat oval
                    // arguments are backwards
                    if (lht > lwt) {
                        temp = lwt;
                        lwt = lht;
                        lht = temp;
                    }
                    a = (PI * lht * lht / 4) + lht * (lwt - lht);
                    break;
            }
            return a / 144;
        }

        public double Vel(double cfm, double area_sf) {
            return cfm / area_sf;
        }

        public string StrSoluT() {
            return Convert.ToString((sessionModel.Dtype == 0 ? "Rectangular" : "Flat Oval"));
        }

        private void RunSolutions() {
            if (FlunkCriticals()) { return; }
            CalcCircularSolution();
            CalcManualSolution();
        }

        /// <summary>
        /// Calculate the solutions table
        /// </summary>
        /// <param CFM in duct="cfm"></param>
        /// <param pressure loss per hundred feet="lph"></param>
        /// <param surface e="surfe"></param>
        /// <param use colebrook="colebrook"></param>
        /// <param limit velocity="limitvelocity"></param>
        /// <param fpm velocity limit="vellimit"></param>
        public void RectSolu(double cfm, double lph, double surfe, bool colebrook, bool limitvelocity, double vellimit) {
            double TryLPH = 0;
            double REqCirDct = 0;
            int MaxDuct = 0;
            int MaxDuctH = 0;
            int MaxDuctW = 0;
            double MaxAr = sessionModel.MaxAR;
            double DLiner = sessionModel.Dliner;
            double lphMargin = sessionModel.LphMargin;
            int Dtype = sessionModel.Dtype;
            double area_sf = 0;
            string Typ = null;
            double dVel = 0;
            double HtBot = 0;
            double WtBot = 0;
            string solTypeText = StrSoluT();
            MaxDuct = Convert.ToInt32(Math.Truncate((MaxAr - 1) * ReqEqvCirDuct(cfm, lph, surfe, colebrook, limitvelocity, vellimit)));
            MaxDuct = Convert.ToInt32(Math.Max(MaxDuct, 6));
            // limit lower size
            MaxDuctH = MaxDuct;
            MaxDuctW = MaxDuct;
            HtBot = 6;
            WtBot = 6;
            if (sessionModel.ChkHRange) {
                HtBot = sessionModel.HtLL;
                MaxDuctH = Convert.ToInt32(sessionModel.HtUL);
            }
            if (sessionModel.ChkWRange) {
                WtBot = sessionModel.WtLL;
                MaxDuctW = Convert.ToInt32(sessionModel.WtUL);
            }

            if (HtBot == 0 | WtBot == 0 | MaxDuctH <= 0 | MaxDuctW <= 0) { return; }
            sessionModel.SolutionsMsg = "Calculating " + solTypeText + " Solutions .......";

            Typ = (sessionModel.Dtype == 0 ? " R" : " FO");
            double DDLiner = 2 * DLiner;
            sessionModel.SolTable.Clear();
            sessionModel.Ductshapevis = Visibility.Hidden;
            for (double ductHeight = HtBot; ductHeight <= MaxDuctH; ductHeight += Dinc) {
                for (double wt = WtBot; wt <= MaxDuctW; wt += Dinc) {
                    // check ar and even size first
                    if ((Math.Max(wt, ductHeight) / Math.Min(wt, ductHeight) <= MaxAr) && ((wt + DDLiner) % 2 == 0) && ((ductHeight + DDLiner) % 2 == 0)) {
                        REqCirDct = DhEqCircRO(ductHeight, wt, Dtype);
                        TryLPH = CircDuctPLPH(cfm, REqCirDct, surfe, colebrook);
                        // margin check
                        if ((TryLPH <= lph) && (TryLPH > lphMargin * lph)) {
                            area_sf = DAreaRO(wt, ductHeight, Dtype);
                            dVel = Vel(cfm, area_sf);
                            // velocity limit check, if fails then skip this for
                            if ((sessionModel.ChkVelLimit) && (dVel > vellimit)) { continue; }
                            double drWt = wt + DDLiner;
                            double drHt = ductHeight + DDLiner;
                            double drPFT = DuctPFT(drWt, drHt, Dtype);
                            // inlist check, if already in list as reverse size then skip this solution
                            //    if (InList(drWt, drHt)) { continue; }
                            if (InList(drHt, drWt)) { continue; }
                            DataRow dr = sessionModel.SolTable.NewRow();
                            dr[0] = drWt;
                            dr[1] = "x";
                            dr[2] = drHt;
                            dr[3] = Typ;
                            dr[4] = TryLPH.ToString("0.000");
                            dr[5] = dVel.ToString("#,##0");
                            dr[6] = (Math.Max(wt, ductHeight) / Math.Min(wt, ductHeight)).ToString("0.00");
                            dr[7] = drPFT.ToString("0.00");
                            sessionModel.SolTable.Rows.Add(dr);
                        }
                    }
                    if (sessionModel.SolTable.Rows.Count > 60) { break; }
                }
            }
            // sort by aspect ratio first, then by pressure loss
            // This puts the most efficient section at the list top. 
            // sessionModel.SolTable.DefaultView.Sort = "PFT ASC, AR ASC, PLPH DESC";
            sessionModel.SolTable.DefaultView.Sort = "AR ASC, PLPH ASC";

            if (sessionModel.SolTable.Rows.Count > 0) {
                // When DataTables are bound to DataGrids, the items seen in the window are
                // DataRowViews that correspond to the DataTable's DataRows.
                DataRowView sdrv = sessionModel.SolTable.DefaultView[0] as DataRowView;
                sessionModel.SelrowSolTable = sdrv;
                UpDateDuctGraphic();
            }

            if (sessionModel.SolTable.Rows.Count == 0 && sessionModel.ChkVelLimit) {
                string msg = "There are no solutions that meet all the criteria.";
                msg = msg + "\n\n" + "Duct sizes can have an energy loss peformance much";
                msg = msg + " better than how close the 'LPH Boundary' factor allows for";
                msg = msg + " a selection when the 'LPH Boundary' value is high and the CFM is small.";
                msg = msg + "\n\n" + "Duct sizes can also result in air velocities larger";
                msg = msg + " than the velocity limit.";
                 
                sessionModel.SolutionsMsg = msg;
                sessionModel.Ductshapevis = Visibility.Hidden;
            } else {
                sessionModel.SolutionsMsg = solTypeText + " Solutions: " + sessionModel.SolTable.Rows.Count.ToString();
                sessionModel.Ductshapevis = Visibility.Visible;
            }

        }

        /// <summary>
        /// Duct perimeter in feet, The longest side is assumed to be
        /// the width.
        /// </summary>
        /// <param duct width="drWt"></param>
        /// <param duct height="drHt"></param>
        /// <param duct type="dtype"></param>
        /// <returns></returns>
        private double DuctPFT(double drWt, double drHt, int dtype) {
            double _drWT = Math.Max(drWt, drHt);
            double _drHT = Math.Min(drWt, drHt);
            if (dtype == 0) {
                // rect
                return (_drWT + _drWT + _drHT + _drHT) / 12;
            } else {
                // flat oval
                return (_drWT + _drWT - _drHT - _drHT + PI * _drHT) / 12;
            }
        }

        /// <summary>
        /// Returns true if solution table already contains an ht wt entry
        /// </summary>
        /// <param width="wt"></param>
        /// <param height="ht"></param>
        /// <returns></returns>
        public bool InList(double wt, double ht) {
            string expression = "Width = " + ht + " and Height = " + wt;
            DataRow drinlist = sessionModel.SolTable.Select(expression).FirstOrDefault();
            if (drinlist != null) {
                return true;
            }
            return false;
        }

        private void UpDateDuctGraphic() {
            if (sessionModel.SelrowSolTable == null) {
                sessionModel.Ductshapevis = Visibility.Hidden;
                return;
            }
            int orgDH = sessionModel.DuctshapeHT; //   RectangleShapeDuct.Size.Height;
            int orgDW = sessionModel.DuctshapeWT; //   RectangleShapeDuct.Size.Width;
            double ar = Convert.ToDouble(sessionModel.SelrowSolTable.Row[6]);
            int newDh = Convert.ToInt32(DFPictWidth / ar);
            if ((double)sessionModel.SelrowSolTable.Row[0] >= (double)sessionModel.SelrowSolTable.Row[2]) {
                sessionModel.CanvasLeft = 5;
                sessionModel.CanvasTop = 5 + Convert.ToInt32((DFPictWidth - newDh) / 2);
                sessionModel.DuctshapeWT = DFPictWidth;
                sessionModel.DuctshapeHT = newDh;
            } else {
                sessionModel.CanvasLeft = 5 + Convert.ToInt32((DFPictWidth - newDh) / 2);
                sessionModel.CanvasTop = 5;
                sessionModel.DuctshapeWT = newDh;
                sessionModel.DuctshapeHT = DFPictWidth;
            }
            sessionModel.Ductshapevis = Visibility.Visible;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                Close();
            }
        }

        public void DragWindow(object sender, MouseButtonEventArgs args) {
            // Watch out. Fatal error if not primary button!
            if (args.LeftButton == MouseButtonState.Pressed) { DragMove(); }
        }

        private void Button_Resets_MouseDown(object sender, MouseButtonEventArgs e) {
            RangeResets(sender);
        }

        private void Button_Resets_MouseDown(object sender, RoutedEventArgs e) {
            RangeResets(sender);
        }

        private void RangeResets(object sender) {
            if (sender is Button btn) {
                switch (btn.Name) {
                    case "BtnResetHRange":
                        sessionModel.StrHtLL = DFheightLimitLow.ToString();
                        sessionModel.StrHtUL = DFheightLimitHigh.ToString();
                        break;
                    case "BtnResetWRange":
                        sessionModel.StrWtLL = DFwidthLimitLow.ToString();
                        sessionModel.StrWtUL = DFwidthLimitHigh.ToString();
                        break;
                    default:
                        break;
                }
                RunSolutions();
            }
        }

        private void ComboSurfeSelections_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            RunSolutions();
        }

        private void TB_CFM_KeyUp(object sender, KeyEventArgs e) {
            RunSolutions();
        }

        private void LinerButton_Checked(object sender, RoutedEventArgs e) {
            GetLiner();
            RunSolutions();
        }

        private void ComboBoxDuctClass_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            RunSolutions();
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e) {
            CalcCircularSolution();
        }

        private void ACheckBoxWasChanged_Click(object sender, RoutedEventArgs e) {
            RunSolutions();
        }

        private void TB_VELLIIMIT_TextChanged(object sender, TextChangedEventArgs e) {
            if (sessionModel.ChkVelLimit && sessionModel.VelLimit > 0) { CalcCircularSolution(); }
        }

        private void ManualRectSizeChanged_TextChanged(object sender, TextChangedEventArgs e) {
            CalcManualSolution();
        }

        private void TB_MAXAR_TextChanged(object sender, TextChangedEventArgs e) {
            CalcCircularSolution();
        }

        private void DuctTypeSelection_Checked(object sender, RoutedEventArgs e) {
            RunSolutions();
        }

        private void DoubleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            RunSolutions();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            RunSolutions();
        }

        private void SolutionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            UpDateDuctGraphic();
        }

        private void ButtonClearDevList_Click(object sender, RoutedEventArgs e) {
            sessionModel.AdvTable.Clear();
            sessionModel.DeviceTally = 0.0;
        }

        private void CalcAirDevices() {
            double deviceSum = 0.0;
            foreach (DataRow dr in sessionModel.AdvTable.Rows) {
                if ((Double.TryParse(dr[0].ToString(), out double devCFM)) && (Double.TryParse(dr[1].ToString(), out double devQty))) {
                    deviceSum += devCFM * devQty;
                }
            }
            sessionModel.DeviceTally = deviceSum;
            if (sessionModel.ChkUseTally) {
                sessionModel.StrCFM = deviceSum.ToString();
                RunSolutions();
            }
        }

        private void TallyGridEdit(object sender, EventArgs e) {
            CalcAirDevices();
        }

        private void ChkUseTally_Click(object sender, RoutedEventArgs e) {
            CalcAirDevices();
        }

        private void PressKey(Key key) {
            KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key) {
                RoutedEvent = Keyboard.KeyDownEvent
            };
            InputManager.Current.ProcessInput(args);
        }

        /// <summary>
        /// Moves position in table to the right except at last entry
        /// where it allows the row to be added and cursor flows naturally
        /// down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TallyGrid_PreviewKeyDown(object sender, KeyEventArgs e) {
            var uiElement = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter && uiElement != null) {
                if (sender is DataGrid dgc) {
                    int ccindx = dgc.CurrentColumn.DisplayIndex;
                    if (ccindx < dgc.Columns.Count - 1) {
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        e.Handled = true;
                    }
                }
            }
        }
       
    } /// main window class
}
