using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGIC
{

    public interface IGICDataAccess
    {
        public void SetRunningState(bool newRunningState);
        public bool GetRunningState();

    }

    public class GICData: IGICDataAccess
    {
        /// <summary>
        /// true = running; false = stopping
        /// </summary>
        private bool _currentRunningState;

        public GICData()
        {
            _currentRunningState = true;
        }

        public bool GetRunningState()
        {
            return _currentRunningState;
        }

        public void SetRunningState(bool newstate)
        {
            _currentRunningState = newstate;
        }
    }
}
