
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

            // Injection: Context(s) 
            var speechContext = new SpeechContext();
            builder.RegisterInstance(speechContext)
                .As<ISpeechContext>()
                .SingleInstance();

            // Injection: Config
            var riffConfigurableSettings = new RiffConfigurableSettings();
            builder.RegisterInstance(riffConfigurableSettings)
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
            var webRequest = new WebRequest();
            builder.RegisterInstance(webRequest)
                .As<WebRequest>()
                .SingleInstance();

            // Injection: System operations
            builder.RegisterType<RiffSystemOperations>()
               .As<RiffSystemOperations>()
               .SingleInstance();

            // Injection: Applications
            RegisterApplications(builder, riffConfigurableSettings, speechContext);

            // Injection: RiffApplication components 
            builder.RegisterType<Greetings>()
                .As<Greetings>()
                .WithParameter(new TypedParameter(typeof(ISpeechContext), speechContext))
                .SingleInstance();

            builder.RegisterType<Email>()
                .As<Email>()
                .SingleInstance();

            /*builder.RegisterType<Search>()
                .As<Search>()
                .SingleInstance();
                */
                
            builder.RegisterType<Clock>()
                .As<Clock>()
                .WithParameter(new TypedParameter(typeof(ISpeechContext), speechContext))
                .SingleInstance();

            builder.RegisterType<Weather>()
                .As<Weather>()
                .WithParameter(new TypedParameter(typeof(ISpeechContext), speechContext))
                .WithParameter(new TypedParameter(typeof(WebRequest), webRequest))
                .SingleInstance();

            builder.RegisterType<BatteryStatus>()
                .As<BatteryStatus>()
                .WithParameter(new TypedParameter(typeof(ISpeechContext), speechContext))
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
        private static void RegisterApplications(ContainerBuilder builder, 
            IRiffConfigurableSettings riffConfigurableSettings, 
            ISpeechContext speechContext)
        {
            builder.RegisterType<Chrome>()
                .As<Chrome>()
                .WithParameter(new TypedParameter(typeof(IRiffConfigurableSettings), riffConfigurableSettings))
                .WithParameter(new TypedParameter(typeof(ISpeechContext), speechContext))
                .AsSelf();
             
            builder.RegisterType<Word>()
                .As<Word>()
                .WithParameter(new TypedParameter(typeof(IRiffConfigurableSettings), riffConfigurableSettings))
                .WithParameter(new TypedParameter(typeof(ISpeechContext), speechContext))
                .AsSelf();

            builder.RegisterType<Slack>()
                .As<Slack>()
                .WithParameter(new TypedParameter(typeof(IRiffConfigurableSettings), riffConfigurableSettings))
                .WithParameter(new TypedParameter(typeof(ISpeechContext), speechContext))
                .AsSelf();

            var outlook = new Outlook(riffConfigurableSettings, speechContext);
            builder.RegisterInstance(outlook)
                .As<Outlook>()
                .AsSelf();

            builder.RegisterType<Calendar>()
                .As<Calendar>()
                .WithParameter(new TypedParameter(typeof(ISpeechContext), speechContext))
                .WithParameter(new TypedParameter(typeof(Outlook), outlook))
                .SingleInstance();

            //  m_appContainer = builder.Build();
        }
        #endregion

    }
}