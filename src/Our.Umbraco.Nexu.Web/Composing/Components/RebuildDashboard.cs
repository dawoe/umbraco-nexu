namespace Our.Umbraco.Nexu.Web.Composing.Components
{
    using global::Umbraco.Core.Dashboards;

    /// <summary>
    /// Represents the rebuild dashboard.
    /// </summary>
    public class RebuildDashboard : IDashboard
    {
        /// <inheritdoc />
        public string Alias => "Our.Umbraco.Nexu.RebuildDashboard";

        /// <inheritdoc />
        public string View => "/App_Plugins/Nexu/views/dashboard.html";

        /// <inheritdoc />
        public string[] Sections => new string[] { new global::Umbraco.Web.Sections.SettingsSection().Alias};

        /// <inheritdoc />
        public IAccessRule[] AccessRules => new IAccessRule[0];
    }
}
