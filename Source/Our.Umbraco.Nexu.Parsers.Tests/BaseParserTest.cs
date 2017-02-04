namespace Our.Umbraco.Nexu.Parsers.Tests
{
    using global::Umbraco.Tests.TestHelpers;

    using NUnit.Framework;

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
    }
}