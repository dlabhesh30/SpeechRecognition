using Riff.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Riff
{
    public class Greetings : AbstractSpeechHandler
    {
        #region Private Data        
        private Random m_random;
        private int m_randomNumber = 0;
        private List<String> m_welcomeGreetingList = null;
        private List<String> m_exitGreetingList = null;
        private List<String> m_smallTalkList = null;
        private List<String> m_thanksResponseList = null;
        private String m_userName = "";
        #endregion

        #region Constructor(s)   
        public Greetings(ISpeechContext speechContext)
            : base(speechContext)
        {
            PopulateWelcomeGreetings();
            PopulateExitGreetings();
            PopulateSmallTalkList();
            PopulateThanksResponseList();
            m_random = new Random();
            m_userName = "Labhesh";
        }
        
        #endregion

        #region Public Methods   
        public void Welcome()
        {
            m_randomNumber = m_random.Next(0, m_welcomeGreetingList.Count);
            var welcomeMessage = m_welcomeGreetingList[m_randomNumber] + " " + m_userName;
            m_speechContext.Speak(welcomeMessage);
        }

        public void Exit()
        {
            m_randomNumber = m_random.Next(0, m_exitGreetingList.Count);
            var goodbyeMessage = m_exitGreetingList[m_randomNumber] + m_userName;
            m_speechContext.Speak(goodbyeMessage);
        }

        public override void HandleSpeechRequest(string speech)
        {
            switch (speech)
            {
                case ("HELLO"):
                case ("HEY RIFF"):
                case ("HEY"):
                case ("SUP"):
                case ("GOOD MORNING"):
                case ("GOOD AFTERNOON"):
                case ("GOOD EVENING"):
                    StartHelloResponse();
                    break;

                case ("HOW ARE YOU"):
                case ("HOW'S IT GOING"):
                case ("HOW IS IT GOING"):
                case ("HOW ARE THINGS"):
                case ("HOW IS THE DAY GOING"):
                    StartSmallTalk();
                    break;

                case ("THANKS"):
                case ("THANKS FOR YOUR HELP"):
                case ("APPRECIATE YOUR HELP"):
                    StartThanksResponse();
                    break;

                default:
                    this.PassRequestHandling(speech);
                    break;
            }
        }
        #endregion

        #region Private Methods        
        private void PopulateWelcomeGreetings()
        {
            m_welcomeGreetingList = new List<String>();
            m_welcomeGreetingList.Add("Welcome");
            m_welcomeGreetingList.Add("Hello");
            m_welcomeGreetingList.Add("hi");
            m_welcomeGreetingList.Add("How is it going");
            m_welcomeGreetingList.Add("What's up");
            m_welcomeGreetingList.Add("Good to see you");
        }
        private void PopulateExitGreetings()
        {
            m_exitGreetingList = new List<String>();
            m_exitGreetingList.Add("Good bye");
            m_exitGreetingList.Add("See you");
            m_exitGreetingList.Add("Have a nice time");
            m_exitGreetingList.Add("Adieu");
            m_exitGreetingList.Add("Enjoy");
            m_exitGreetingList.Add("Later");
        }

        private void PopulateSmallTalkList()
        {
            m_smallTalkList = new List<String>();
            m_smallTalkList.Add("I'm Good, thanks for asking");
            m_smallTalkList.Add("I'm having the most pleasant day");
            m_smallTalkList.Add("Chilling, and you");
            m_smallTalkList.Add("It's going alright, how about yourself");
            m_smallTalkList.Add("you know me, i'm a workaholic, keeping busy");
            m_smallTalkList.Add("I Wish for something interesting to do");
        }

        private void PopulateThanksResponseList()
        {
            m_thanksResponseList = new List<String>();
            m_thanksResponseList.Add("Happy to help always");
            m_thanksResponseList.Add("no problem at all");
            m_thanksResponseList.Add("you know this my job right");
            m_thanksResponseList.Add("no worries");
            m_thanksResponseList.Add("you got it");
            m_thanksResponseList.Add("Thats what I am here for");
        }

        private void StartHelloResponse()
        {
            var hello = new Thread(new ThreadStart(this.Hello));
            hello.IsBackground = true;
            hello.Start();
        }

        private void StartSmallTalk()
        {
            var smallTalk = new Thread(new ThreadStart(this.SmallTalk));
            smallTalk.IsBackground = true;
            smallTalk.Start();
        }

        private void StartThanksResponse()
        {
            var thanks = new Thread(new ThreadStart(this.Thanks));
            thanks.IsBackground = true;
            thanks.Start();
        }


        private void Hello()
        {
            DateTime timeNow = DateTime.Now;
            var greeting = m_userName + ", how can I help";
            if (timeNow.Hour >= 0 && timeNow.Hour < 12)
            {
                m_speechContext.Speak("Good Morning " + greeting);
            }
            else if (timeNow.Hour >= 12 && timeNow.Hour < 18)
            {
                m_speechContext.Speak("Good afternoon " + greeting);
            }
            else if (timeNow.Hour >= 18 && timeNow.Hour < 24)
            {
                m_speechContext.Speak("Good evening " + greeting);
            }
        }

        private void SmallTalk()
        {
            m_randomNumber = m_random.Next(0, m_smallTalkList.Count);
            var smallTalkMessage = m_smallTalkList[m_randomNumber];
            m_speechContext.Speak(smallTalkMessage);
        }

        private void Thanks()
        {
            m_randomNumber = m_random.Next(0, m_thanksResponseList.Count);
            var thanksResponseMessage = m_thanksResponseList[m_randomNumber];
            m_speechContext.Speak(thanksResponseMessage);
        }
        #endregion

    }
}
