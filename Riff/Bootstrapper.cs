
using Autofac;

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
            
            // Injection: Speech Context 
            builder.RegisterType<SpeechContext>()
                .As<SpeechContext>()
                .SingleInstance();
            
            builder.RegisterType<SpeechHandlerChain>()
                .As<SpeechHandlerChain>()
                .SingleInstance();

            // Injection: Applications
            RegisterApplications(builder);

            // Injection: WebRequest
            builder.RegisterType<WebRequest>()
                .As<WebRequest>()
                .InstancePerDependency();
            
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

            builder.RegisterType<RiffSystemOperations>()
                .As<RiffSystemOperations>()
                .SingleInstance();
            
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