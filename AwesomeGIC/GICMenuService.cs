using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGIC
{
    public interface IGICProcess
    {

        public string Process();
    }



    public class QuitProcess : IGICProcess
    {
        private bool _keepRunning;

        public QuitProcess(ref bool KeepRunning) 
        {
            _keepRunning = KeepRunning;
        }
        public string Process()
        {
            _keepRunning = false;

            // do nothing
            return string.Empty;
        }
    }
}
