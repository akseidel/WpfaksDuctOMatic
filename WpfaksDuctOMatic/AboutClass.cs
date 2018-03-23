using System.ComponentModel;
using System.Reflection;

namespace WpfaksDuctOMatic
{
    class AboutClass : INotifyPropertyChanged {
        static Assembly  app =  Assembly.GetExecutingAssembly();
      
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string propName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        //AssemblyTitleAttribute title = (AssemblyTitleAttribute)app.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0];
        private string title = ((AssemblyTitleAttribute)app.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0]).Title;
        public string Title { get { return title; } set { title = value; OnPropertyChanged("Title"); } }

        //AssemblyProductAttribute product = (AssemblyProductAttribute)app.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0];
        private string product = ((AssemblyProductAttribute)app.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
        public string Product { get { return product; } set { product = value; OnPropertyChanged("Product"); } }
        
        //AssemblyCopyrightAttribute copyright = (AssemblyCopyrightAttribute)app.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0];
        private string copyright = ((AssemblyCopyrightAttribute)app.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
        public string Copyright { get { return copyright; } set { copyright = value; OnPropertyChanged("Copyright"); } }

        //AssemblyCompanyAttribute company = (AssemblyCompanyAttribute)app.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0];
        private string company = ((AssemblyCompanyAttribute)app.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0]).Company;
        public string Company { get { return company; } set { company = value; OnPropertyChanged("Company"); } }

        //AssemblyDescriptionAttribute description = (AssemblyDescriptionAttribute)app.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0];
        private string description = ((AssemblyDescriptionAttribute)app.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
        public string Description { get { return description; } set { description = value; OnPropertyChanged("Description"); } }

        //Version version = app.GetName().Version;
        private string version = app.GetName().Version.ToString();
        public string Version { get { return version; } set { version = value; OnPropertyChanged("Version"); } }
    }
}
