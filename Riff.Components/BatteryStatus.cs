using Riff.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace Riff.Components
{
    public class BatteryStatus : AbstractSpeechHandler
    {
        #region Private Data
        private PowerStatus m_powerStatus = null;
        private Dictionary<BatteryChargeStatus, string> m_powerStatusMap = null;
        private const int m_highBatteryThreshold = 95;
        #endregion

        #region Constructor(s)
        public BatteryStatus(ISpeechContext speechContext)
            : base(speechContext)
        {
            m_powerStatus = SystemInformation.PowerStatus;
            PopulatePowerStatusMap();
            BatteryStatusCheckScheduler();
        }
        #endregion

        #region Public method(s)
        public override void HandleSpeechRequest(string speech)
        {
            if (speech.Contains("BATTERY"))
            {
                CurrentBatteryStatus();
            }
            else
            {
                this.PassRequestHandling(speech);
            }
        }
        #endregion

        #region Private method(s)
        private void CurrentBatteryStatus()
        {
            var batteryPercent = (int)(m_powerStatus.BatteryLifePercent * 100);
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(ChargeStatus());
            messageBuilder.AppendLine("Current battery percent is " + batteryPercent);

            m_speechContext.Speak(messageBuilder.ToString());
        }

        private string ChargeStatus(bool timerTriggered = false)
        {
            var batteryStatus = m_powerStatus.BatteryChargeStatus;
            var result = "";
            if (m_powerStatusMap.ContainsKey(batteryStatus))
            {
                result = m_powerStatusMap[batteryStatus];
                if (timerTriggered)
                {
                    if (batteryStatus == BatteryChargeStatus.Charging ||
                        batteryStatus == BatteryChargeStatus.Unknown ||
                        batteryStatus == BatteryChargeStatus.NoSystemBattery ||
                        batteryStatus == BatteryChargeStatus.High ||
                        batteryStatus == (BatteryChargeStatus.Low | BatteryChargeStatus.Charging) ||
                        batteryStatus == (BatteryChargeStatus.Low | BatteryChargeStatus.Critical | BatteryChargeStatus.Charging) ||
                        batteryStatus == 0)
                    {
                        result = "";
                    }
                    else
                    if (batteryStatus == (BatteryChargeStatus.High | BatteryChargeStatus.Charging))
                    {
                        var batteryPercent = (int)(m_powerStatus.BatteryLifePercent * 100);
                        if (batteryPercent < m_highBatteryThreshold)
                        {
                            result = "";
                        }
                    }
                }
            }
            return result;
        }

        private void PopulatePowerStatusMap()
        {
            m_powerStatusMap = new Dictionary<BatteryChargeStatus, string>();
            m_powerStatusMap.Add(BatteryChargeStatus.Charging, "Plugged in, charging now.");
            m_powerStatusMap.Add(BatteryChargeStatus.Low, "Battery low, I recommed you find a charging spot.");
            m_powerStatusMap.Add(BatteryChargeStatus.Low | BatteryChargeStatus.Charging, "Battery low, charging up now");
            m_powerStatusMap.Add(0, "Battery is good for now.");
            m_powerStatusMap.Add(BatteryChargeStatus.High, "Battery high.");
            m_powerStatusMap.Add(BatteryChargeStatus.High | BatteryChargeStatus.Charging, "Battery high, feel free to remove the charger.");
            m_powerStatusMap.Add(BatteryChargeStatus.Critical, "Battery critical, just plug in the charger already.");
            m_powerStatusMap.Add(BatteryChargeStatus.Critical | BatteryChargeStatus.Charging, "Battery critical, charging up now.");
            m_powerStatusMap.Add(BatteryChargeStatus.Low | BatteryChargeStatus.Critical | BatteryChargeStatus.Charging, "Battery Critical, charging up now");
            m_powerStatusMap.Add(BatteryChargeStatus.NoSystemBattery, "No battery detected.");
            m_powerStatusMap.Add(BatteryChargeStatus.Unknown, "Battery status unknown.");
        }

        private void BatteryStatusCheckScheduler()
        {
            System.Timers.Timer timer = new System.Timers.Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnBatteryStatusCheckInvoked);
            timer.Start();
        }

        private void OnBatteryStatusCheckInvoked(object sender, ElapsedEventArgs e)
        {
            m_speechContext.Speak(ChargeStatus(true));
        }
        #endregion
    }
}
