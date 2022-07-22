using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest
{
    public class TestResult
    {
        private string _error;

        public bool Success { get; set; } = true;
        public string Error
        {
            get => _error != null ? _error : "";
            set => _error = value;
        }
    }
}
