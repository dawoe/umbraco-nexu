namespace Our.Umbraco.Nexu.Parsers.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Umbraco.Core;
    using global::Umbraco.Tests.TestHelpers;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.ObjectResolution;

    /// <summary>
    /// The base parser test.
    /// </summary>
    [TestFixture]
    public class BaseParserTest : BaseDatabaseFactoryTest
    {
        /// <summary>
        /// Override initialize method
        /// </summary>
        [SetUp]
        public override void Initialize()
        {
            base.Initialize();
            SettingsForTests.ConfigureSettings(SettingsForTests.GenerateMockSettings());
        }

        protected override void FreezeResolution()
        {
            var assembly = Assembly.Load("Our.Umbraco.Nexu.Parsers");
            var propertyParsers =
                TypeFinder.FindClassesOfType<IPropertyParser>(new List<Assembly> { assembly }).ToList();

            var gridEditorParsers =
               TypeFinder.FindClassesOfType<IGridEditorParser>(new List<Assembly> { assembly }).ToList();

            // set up property  parser resolver
            PropertyParserResolver.Current = new PropertyParserResolver(
                new ActivatorServiceProvider(),
                this.Logger,
                propertyParsers);

            // setup grid editor parser resolver
            GridEditorParserResolver.Current = new GridEditorParserResolver(
                new ActivatorServiceProvider(),
                this.Logger,
                gridEditorParsers);


            base.FreezeResolution();
        }
    }
}