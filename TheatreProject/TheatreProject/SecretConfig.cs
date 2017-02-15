using System.Configuration;
using System.Web.Configuration;

namespace TheatreProject
{
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

        [ConfigurationProperty("emailPassword", IsRequired = true)]
        public string EmailPassword
        {
            get { return (string)base["emailPassword"]; }
            set { base["emailPassword"] = value; }
        }
    }
}