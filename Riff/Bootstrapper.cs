﻿
using Autofac;
using Riff.Framework;
using Riff.RecognitionProvider;

namespace Riff
{
    public class Bootstrapper
    {
        #region Private Data
        private static IContainer m_appContainer;
        #endregion

        #region Public methods
        public static IContainer Bootstrap()
        {
            // Injection: Controller Registration
            var builder = new ContainerBuilder();

            // Injection: Config
            builder.RegisterType<RiffConfigurableSettings>()
                .As<RiffConfigurableSettings>()
                .SingleInstance();
            
            // Injection: Context(s) 
            builder.RegisterType<SpeechContext>()
                .As<SpeechContext>()
                .SingleInstance();
            
            builder.RegisterType<MicrophoneContext>()
                .As<IMicrophoneContext>()
                .SingleInstance();

            builder.RegisterType<SpeechHandlerChain>()
                .As<ISpeechHandlerChain>()
                .SingleInstance();

            // Injection: RecognitionEngineProvider
            builder.RegisterType<GoogleRecognitionEngineProvider>()
                .As<IRecognitionEngineProvider>()
                .InstancePerDependency();

            // Injection: WebRequest
            builder.RegisterType<WebRequest>()
                .As<WebRequest>()
                .InstancePerDependency();

            // Injection: System operations
            builder.RegisterType<RiffSystemOperations>()
               .As<RiffSystemOperations>()
               .SingleInstance();

            // Injection: Applications
            RegisterApplications(builder);
           
            // Injection: RiffApplication components 
            builder.RegisterType<Greetings>()
                .As<Greetings>()
                .SingleInstance();

            builder.RegisterType<Email>()
                .As<Email>()
                .SingleInstance();

            builder.RegisterType<Search>()
                .As<Search>()
                .SingleInstance();

            builder.RegisterType<Calendar>()
                .As<Calendar>()
                .SingleInstance();

            builder.RegisterType<Clock>()
                .As<Clock>()
                .SingleInstance();

            builder.RegisterType<Weather>()
                .As<Weather>()
                .SingleInstance();

            builder.RegisterType<BatteryStatus>()
                .As<BatteryStatus>()
                .InstancePerDependency();
            
            // Injection: Build container / integrate with MVC
            m_appContainer = builder.Build();
            return m_appContainer;
        }

        public static T ResolveType<T>()
        {
            T result = m_appContainer.Resolve<T>();
            return result;
        }
        #endregion

        #region Private Methods
        private static void RegisterApplications(ContainerBuilder builder)
        {
            builder.RegisterType<Chrome>()
                .As<AbstractApplicationContext>()
                .AsSelf();

            builder.RegisterType<Outlook>()
                .As<AbstractApplicationContext>()
                .AsSelf();

            builder.RegisterType<Word>()
                .As<AbstractApplicationContext>()
                .AsSelf();

            builder.RegisterType<Slack>()
                .As<AbstractApplicationContext>()
                .AsSelf();
        }
        #endregion

    }
}