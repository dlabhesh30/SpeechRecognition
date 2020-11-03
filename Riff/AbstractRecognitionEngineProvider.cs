﻿
using System.Threading.Tasks;

namespace Riff
{
    internal interface IRecognitionEngineProvider
    {
        /// <summary>
        /// Method to start recognizing speech from default system device
        /// </summary>
        /// <returns>Task object that returns a integer value</returns>
        Task<int> RecognizeSpeech();
        
    }
}
