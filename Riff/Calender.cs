using System;
using Microsoft.Office.Interop.Outlook;
using System.Threading;

namespace Riff
{
    public class Calendar : SpeechHandler
    {
        #region Private Data
        private Outlook m_outlook = null;
        private MAPIFolder m_calendar = null;
        #endregion

        #region Constructor(s)
        public Calendar()
        {
            m_outlook = Bootstrapper.ResolveType<Outlook>();
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
                        var sayAppointment = new Thread(new ThreadStart(() => m_speechContext.Appointment(appointmentContent)));
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
            var noAppointment = new Thread(new ThreadStart(() => m_speechContext.AppointmentError()));
            noAppointment.IsBackground = true;
            noAppointment.Start();
        }

        private void CurrentDate()
        {
            m_speechContext.CurrentDate();
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
        #endregion
    }
}
