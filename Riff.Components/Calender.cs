using System;
using Microsoft.Office.Interop.Outlook;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using Riff.Framework;

namespace Riff.Components
{
    public class Calendar : AbstractSpeechHandler
    {
        #region Private Data
        private Outlook m_outlook = null;
        private MAPIFolder m_calendar = null;
        #endregion

        #region Constructor(s)
        public Calendar(ISpeechContext speechContext, Outlook outlook)
            : base(speechContext)
        {
            m_outlook = outlook;
        }
        #endregion

        #region Public method(s)
        public override void HandleSpeechRequest(string speech)
        {
            if (speech.Contains("DAY") || speech.Contains("DATE"))
            {
                CurrentDate();
            }
            else
            if (speech.Contains("CALENDER") || speech.Contains("TASKS") || speech.Contains("APPOINTMENTS"))
            {
                CalendarAppointments();
            }
            else
            {
                this.PassRequestHandling(speech);
            }
        }
        #endregion

        #region Private method(s)
        private void CalendarAppointments()
        {
            EnsureOutlookIsInitialized();
            m_calendar = m_outlook.OutlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderCalendar);

            try
            {
                var start = DateTime.Now;
                var end = start.AddDays(7);
                var index = 1;
                var appointmentFound = false;
                
                var calenderItems = GetAppointmentsInRange(start, end);
                if (calenderItems != null)
                {
                    Console.WriteLine("Appointments Found");
                    //set bool here change in foreach ask after foreach for change
                    foreach (AppointmentItem appointment in calenderItems)
                    {
                        Console.WriteLine("Appointment " + index.ToString());
                        string[] appointmentContent = { index.ToString(), appointment.Subject, appointment.Start.Date.ToString("d"), appointment.Start.TimeOfDay.ToString()};
                        var sayAppointment = new Thread(new ThreadStart(() => Appointment(appointmentContent)));
                        sayAppointment.IsBackground = true;
                        sayAppointment.Start();
                        Thread.Sleep(8000);
                        index++;
                        appointmentFound = true;
                    }

                    if (!appointmentFound)
                    {
                        Console.WriteLine("Nothing found");
                        NoAppointments();
                    }
                }
                else
                {
                    Console.WriteLine("Nothing found");
                    NoAppointments();

                }
            }
            catch
            {
                NoAppointments();
            }

        }

        private Items GetAppointmentsInRange(DateTime start, DateTime end)
        {
            var filter = "[Start] >= '" + start.ToString("g") + "' AND [End] <= '" + end.ToString("g") + "'";

            try
            {
                var calItems = m_calendar.Items;
                calItems.IncludeRecurrences = true;
                calItems.Sort("[Start]", Type.Missing);
                var restictItems = calItems.Restrict(filter);
                if (restictItems.Count > 0)
                {
                    return restictItems;
                }
                else
                {
                    return null;
                }
            }
            catch(System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }

        private void NoAppointments()
        {
            var noAppointment = new Thread(new ThreadStart(() => AppointmentError()));
            noAppointment.IsBackground = true;
            noAppointment.Start();
        }

        private void CurrentDate()
        {
            string dateString = DateTime.Now.ToShortDateString();
            var datePrefix = "The date is: ";
            m_speechContext.Speak(datePrefix, new List<String>() { " <say-as interpret-as=\"date_md\">" + dateString + "</say-as>" });
        }

        private void EnsureOutlookIsInitialized()
        {
            if (!m_outlook.IsApplicationRunning())
            {
                m_outlook.OpenApplication();
            }
            else
            if (!m_outlook.OutlookInitialized)
            {
                m_outlook.OutlookInitialize();
            }
        }

        private void Appointment(string[] whatToSay)
        {
            var date = whatToSay[2].Split('/');
            var newDate = date[1] + "/" + date[0];
            var time = whatToSay[3].Split(':');
            var newTime = time[0] + ":" + time[1];

            var appointmentText = new StringBuilder();
            appointmentText.Append("Appointment " + whatToSay[0]);
            appointmentText.Append(", subject is: " + whatToSay[1]);
            appointmentText.Append(", at " + whatToSay[3]);
            appointmentText.Append(", on");

            var timeString = new List<String>();
            timeString.Add("<say-as interpret-as=\"date_md\">" + newDate + "</say-as>");
            timeString.Add(" <say-as interpret-as=\"time\">" + newTime + "</say-as>");

            m_speechContext.Speak(appointmentText.ToString(), timeString);

        }

        private void AppointmentError()
        {
            m_speechContext.Speak("No Appointments found for the next 7 days.");
        }
        #endregion
    }
}
