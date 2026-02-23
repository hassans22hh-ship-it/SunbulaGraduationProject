namespace PlantDomain.Exceptions
{
    public sealed class PlantNotFoundException:Exception
    {
        public PlantNotFoundException(Guid id)
       : base($"Plant with ID '{id}' was not found.") { }
    }
}
