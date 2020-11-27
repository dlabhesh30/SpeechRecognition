using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Outlook;

using Riff.Framework;

namespace Riff.Components
{
    public class Outlook : AbstractApplicationContext
    {
        #region Private Data
        private Application m_outlookApp;
        private NameSpace m_outlookNamespace;
        private MAPIFolder m_inbox;
        private bool m_outlookInitialized = false;
        #endregion

        #region Constructor(s)
        public Outlook(IRiffConfigurableSettings riffConfigurableSettings, ISpeechContext speechContext)
            : base(riffConfigurableSettings, speechContext)
        {
            m_applicationName = "Outlook";
            this.SetApplicationPath();
            SetAlternateAliasForApplciation();
        }
        #endregion

        #region Public Properties(s)
        public Application OutlookApp
        {
            get
            {
                return m_outlookApp;
            }
        }

        public NameSpace OutlookNamespace
        {
            get
            {
                return m_outlookNamespace;
            }
        }

        public bool OutlookInitialized
        {
            get
            {
                return m_outlookInitialized;
            }
        }
        #endregion

        #region Public method(s)
        public override void OpenApplication()
        {
            base.OpenApplication();
            OutlookInitialize();
        }

        public void OutlookInitialize()
        {
            if (!IsApplicationRunning())
            {
                this.OpenApplication();
            }
            m_outlookApp = new Application();

            m_outlookNamespace = m_outlookApp.GetNamespace("MAPI");
            m_outlookNamespace.Logon();
            m_outlookNamespace.SendAndReceive(false);
            m_inbox = m_outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);


            var newMail = m_inbox.UnReadItemCount;
            var emailMessage = "";
            if (newMail == 0)
            {
                emailMessage = ("You have no new emails");
            }
            else if (newMail == 1)
            {
                emailMessage = ("You have one new m_email");
            }
            else
            {
                emailMessage = ("You have " + newMail + " new  emails");
            }

            m_speechContext.Speak(emailMessage);
            m_outlookInitialized = true;
        }
        #endregion

        #region Protected method(s)
        protected override void SetAlternateAliasForApplciation()
        {
            m_alternateApplicationAlias.Add("EMAIL");
            m_alternateApplicationAlias.Add("EMAILS");
            m_alternateApplicationAlias.Add("MAILS");
            m_alternateApplicationAlias.Add("MAIL");
        }

        protected override bool HandleApplicationSpecificOperation(string speech)
        {
            var result = false;
            if (speech.Contains("READ") && HasAlternateApplicationName(speech))
            {
                ReadMail();
            }
            else
            if (speech.Contains("SEND") && HasAlternateApplicationName(speech))
            {
                SendMail();
            }
            return result;
        }
        #endregion

        #region Private method(s)
        private void ReadMail()
        {
            if (!m_applciationOpened)
            {
                OutlookInitialize();
            }

            try
            {
                int amountToRead = 5;
                int amountOfMail = m_inbox.Items.Count;

                if (amountOfMail != 0)
                {
                    if (amountOfMail < amountToRead)
                    {
                        amountToRead = amountOfMail;
                        Console.WriteLine(amountToRead.ToString());
                    }

                    for (int i = 0; i < amountToRead; i++)
                    {
                        MailItem email = m_inbox.Items[amountToRead - i];
                        string sender = email.SenderEmailAddress;

                        string subject = email.Subject;

                        string body = email.Body;
                        if (body.Length >= 100)
                        {
                            m_speechContext.Speak("Message From: " + sender + ", Message Subject: " + subject + ", Email too long to repeat");
                        }
                        else
                        {
                            m_speechContext.Speak("Message From: " + sender + ", Message Subject: " + subject + ", Message: " + body);
                        }

                    }
                }
                else
                {
                    m_speechContext.Speak("No mail in inbox");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("DONE");
        }

        private void SendMail()
        {
        }
        #endregion
    }
}
