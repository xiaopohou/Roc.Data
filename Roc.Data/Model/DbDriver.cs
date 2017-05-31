using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    public class DbDriver
    {
        public ProviderType Type { get; set; }
        public string Name { get; set; }
        public string AssemblyName { get; set; }
        public string Path { get; set; }

        public string ProviderName { get; set; }

        public DbDriver()
        {

        }

        public DbDriver(ProviderType type, string name, string pname)
        {
            this.Type = type;
            this.Name = name;
            this.ProviderName = pname;
            if (!string.IsNullOrEmpty(name))
                this.Path = GetPath(name);
        }

        private string GetPath(string name)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(basePath, "dll", name);
        }
    }
}
