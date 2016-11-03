namespace Edubase.Common.Spatial
{
    internal class RefEll
    {
        public double maj, min, ecc;
        public RefEll(double major, double minor)
        {
            maj = major;
            min = minor;
            ecc = ((major * major) - (minor * minor)) / (major * major);
        }
    }
}
