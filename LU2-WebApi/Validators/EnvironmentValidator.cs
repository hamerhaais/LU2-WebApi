using LU2_WebApi.Models;

namespace LU2_WebApi.Validators
{
    public class EnvironmentValidator
    {
        public void Validate(Environment2D env)
        {
            if (string.IsNullOrWhiteSpace(env.Name))
                throw new ArgumentException("Naam is verplicht.");

            if (env.Name.Length > 100)
                throw new ArgumentException("Naam is te lang.");
        }
    }
}
