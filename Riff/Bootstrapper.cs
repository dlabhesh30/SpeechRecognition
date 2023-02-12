using Autofac;

using Riff.Components;
using Riff.Components.Google;
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
        public IContainer Bootstrap()
        {
            // Injection: Controller Registration
            var builder = new ContainerBuilder();

            // Injection: Context(s) 
            builder.RegisterType<SpeechContext>()
                .As<ISpeechContext>()
                .SingleInstance();

            // Injection: Config
            builder.RegisterType<RiffConfigurableSettings>()
                .As<IRiffConfigurableSettings>()
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
                .SingleInstance();

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
                
            builder.RegisterType<Clock>()
                .As<Clock>()
                .SingleInstance();

            builder.RegisterType<Weather>()
                .As<Weather>()
                .SingleInstance();

            builder.RegisterType<GoogleSearch>()
                .As<GoogleSearch>()
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
        private void RegisterApplications(ContainerBuilder builder)
        {
            builder.RegisterType<Chrome>()
                .As<Chrome>()
                .AsSelf();
             
            builder.RegisterType<Word>()
                .As<Word>()
                .AsSelf();

            builder.RegisterType<Powerpoint>()
                .As<Powerpoint>()
                .AsSelf();

            builder.RegisterType<Excel>()
                .As<Excel>()
                .AsSelf();

            builder.RegisterType<Slack>()
                .As<Slack>()
                .AsSelf();

            builder.RegisterType<Outlook>()
                .As<Outlook>()
                .AsSelf();

            builder.RegisterType<Calendar>()
                .As<Calendar>()
                .SingleInstance();
        }
        #endregion

    }
}