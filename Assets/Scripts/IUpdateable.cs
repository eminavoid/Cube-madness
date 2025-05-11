// Interfaz que define un contrato: cualquier clase que la implemente debe tener un método Tick().
// Esto reemplaza al Update de Unity, dándote control total sobre qué se actualiza y cuándo.

public interface IUpdatable
{
    void Tick(float deltaTime); // Se llama una vez por frame desde el CustomUpdateManager
}