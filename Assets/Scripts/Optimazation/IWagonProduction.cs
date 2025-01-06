public interface IWagonProduction
{
    void ProcessProduction(float deltaTime);
    bool CanProduce(); // Checks conditions like durability, workers, etc.
    float GetProductionInterval(); // Returns how often this wagon should process (customizable per wagon type)
}