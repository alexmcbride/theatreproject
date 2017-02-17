using System.Configuration;
using System.Web.Configuration;

namespace TheatreProject
{
    /// <summary>
    /// Config class for reading the Secret.config file.
    /// </summary>
    public sealed class SecretConfig : ConfigurationSection
    {
        private static SecretConfig config = null;

        public static SecretConfig Config
        {
            get
            {
                if (config == null)
                {
                    config = (SecretConfig)WebConfigurationManager.GetSection("secret");
                }

                return config;
            }
        }

        [ConfigurationProperty("sendGridApiKey", IsRequired = true)]
        public string SendGridApiKey
        {
            get { return (string)base["sendGridApiKey"]; }
            set { base["sendGridApiKey"] = value; }
        }
    }
}