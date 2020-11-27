using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

using Riff.Framework;

namespace Riff.Components
{
    abstract public class AbstractApplicationContext : AbstractSpeechHandler
    {
        #region Private Data
        private bool m_commonBasePath = false;
        private string m_commonBasePathName = "";
        private string m_executableName = "";
        private string m_applicationPath = "";
        #endregion

        #region Protected Data
        protected string m_applicationName = "";
        protected string m_additionalAppStartMessage = "";
        protected IDictionary<string, string> m_supportedApplications = null;
        protected bool m_applciationOpened = false;
        protected IList<string> m_alternateApplicationAlias = null;
        #endregion

        #region Private Windows methods
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_CLOSE = 0xF060;
        private const int SW_SHOW = 5;
        private const int SW_RESTORE = 9;

        #endregion

        #region Constructor(s)
        public AbstractApplicationContext(IRiffConfigurableSettings riffConfigurableSettings, ISpeechContext speechContext)
            : base(speechContext)
        {
            m_supportedApplications = riffConfigurableSettings.SupportedApplications();
            m_alternateApplicationAlias = new List<string>();
        }
        #endregion

        #region Protected Method(s)
        protected void SetApplicationPath()
        {
            if (m_commonBasePath)
            {
                m_applicationPath = m_supportedApplications[m_commonBasePathName] + "/" + m_executableName;
            }
            else
            {
                m_applicationPath = m_supportedApplications[m_applicationName];
            }
        }

        protected virtual void SetAlternateAliasForApplciation()
        {
            // Default implementation - No Operation
        }

        protected virtual bool HandleApplicationSpecificOperation(string speech)
        {
            // Default implementation
            return false;
        }

        protected void SetCommonApplicationBasePathOverride(string commonBasePathName, string executableName)
        {
            m_commonBasePath = true;
            m_commonBasePathName = commonBasePathName;
            m_executableName = executableName;
        }
        #endregion

        #region Public method(s)
        public virtual void OpenApplication()
        {
            var responseMessage = "";
            if (String.IsNullOrEmpty(m_applicationName))
            {
                m_applicationName = "application";
            }

            if (File.Exists(m_applicationPath))
            {
                if (!IsApplicationRunning())
                {
                    Process.Start(m_applicationPath);
                    responseMessage = "Starting " + m_applicationName + ", " + m_additionalAppStartMessage;
                    m_applciationOpened = true;
                }
                else
                {
                    responseMessage = "Switching to " + m_applicationName;
                    m_applciationOpened = true;
                }
            }
            else
            {
                responseMessage = "Application path invalid, cannot start " + m_applicationName;
            }

            m_speechContext.Speak(responseMessage);
        }

        public bool IsApplicationRunning()
        {
            var fileName = Path.GetFileNameWithoutExtension(m_applicationPath).ToLower();
            bool isRunning = false;

            var processes = Process.GetProcessesByName(fileName);

            if (processes.Length > 0)
            {
                foreach (var process in processes)
                {
                    var handle = process.MainWindowHandle;
                    ShowWindow(handle, SW_RESTORE);
                    ShowWindow(handle, SW_SHOW);
                    SetForegroundWindow(handle);
                    SetForegroundWindow(handle);
                }
                isRunning = true;
            }

            return isRunning;
        }

        public override void HandleSpeechRequest(string speech)
        {
            if ((speech.Contains("OPEN") || speech.Contains("SHOW")) && 
                (speech.Contains(m_applicationName.ToUpper()) || HasAlternateApplicationName(speech)))
            {
                OpenApplication();
            }
            else
            if ((speech.Contains("CLOSE") || speech.Contains("QUIT") || speech.Contains("EXIT")) && 
                (speech.Contains(m_applicationName.ToUpper()) || HasAlternateApplicationName(speech)))
            {
                CloseApplication();
            }
            else
            if (HandleApplicationSpecificOperation(speech))
            {
                // no op - Dervied Application will do the required operation
            }
            else
            {
                this.PassRequestHandling(speech);
            }
        }
        #endregion

        #region Protected method(s)
        protected bool HasAlternateApplicationName(string speech)
        {
            var result = false;
            if (null != m_alternateApplicationAlias)
            {
                foreach (var name in m_alternateApplicationAlias)
                {
                    result = speech.Contains(name.ToUpper());
                }
            }
            return result;
        }
        #endregion

        #region Private method(s)
        private void CloseApplication()
        {
            var filePath = Path.GetDirectoryName(m_applicationPath);
            var fileName = Path.GetFileNameWithoutExtension(m_applicationPath).ToLower();
            Process.Start(new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = $"/im {fileName + ".exe"} /f /t",
                CreateNoWindow = true,
                UseShellExecute = false
            }).WaitForExit();
            m_speechContext.Speak(fileName + " closed");
            m_applciationOpened = false;
        }
        #endregion
    }
}
