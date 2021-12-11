namespace login_web_api.SettingsModels
{
    public class HashingConfiguration
    {
        public short Iterations { get; set; }
        public short SaltLength { get; set; }
        public string Algorithm { get; set; }
        public short AlgorithmLength { get; set; }
    }
}
