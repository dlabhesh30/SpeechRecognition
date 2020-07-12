using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Office.Interop.Outlook;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.IO;
using System.Threading;

namespace Riff
{
    public class Email
    {
        Application outLookApp = new Application();
        
        //load sendMailItem form
        //grab info
        //say send
        //SpeechRecognitionEngine waitForSend = new SpeechRecognitionEngine();
        /*sendMailInput mailPreview = new sendMailInput();
        public void sendMail()
        {
            mailPreview.Show();
            RiffApplication.m_speechEngine.RecognizeAsyncStop();
            waitForSend.SetInputToDefaultAudioDevice();
            waitForSend.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(RiffApplication.getPhrases().ToArray()))));
            waitForSend.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(waitForSend_SpeechRecognized);
            waitForSend.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void waitForSend_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text.ToUpper().Equals("SEND"))
            {
                MailItem myMail = (MailItem)outLookApp.CreateItem(OlItemType.olMailItem);
                string[] recipients = new string[20];
                string subject = "";
                string body = "";
                string[] attachmentPaths = new string[20];

                using (SpeechSynthesizer sendMail = new SpeechSynthesizer())
                {

                    recipients = mailPreview.sendTo();

                    subject = mailPreview.subject();

                    body = mailPreview.getMessage();

                    attachmentPaths = mailPreview.getAttachments();

                    sendMail.Speak("Sending Mail");

                    int attachmentsIndex = 0;
                    while(attachmentPaths[attachmentsIndex] != null)
                    {
                        myMail.Attachments.Add(attachmentPaths[attachmentsIndex], Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue,1,attachmentPaths[attachmentsIndex]);
                        attachmentsIndex++;
                    }


                    int recipientsIndex = 0;
                    if (recipients[recipientsIndex] != null)
                    {
                        myMail.To = recipients[recipientsIndex];
                        recipientsIndex++;

                        while (recipients[recipientsIndex] != null)
                        {                            
                            myMail.Recipients.Add(recipients[recipientsIndex]);
                            recipientsIndex++;
                        }

                        myMail.Subject = subject;
                        myMail.Body = body;
                        ((Microsoft.Office.Interop.Outlook._MailItem)myMail).Send();
                        sendMail.Speak("Sent");
                        sendMail.Volume = 0;
                    }
                    else
                    {
                        sendMail.Speak("No e-mail address specified");
                    }

                    for (int index = 0; index <= recipients.Length - 1; index++)
                    {
                        recipients[index] = null;
                    }
                    subject = "";
                    body = "";
                    myMail = null;
                }
                try
                {
                    RiffApplication.m_speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                }
                catch(System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                mailPreview.Close();

                waitForSend.RecognizeAsyncStop();
            }
            
        }*/
    }
}
