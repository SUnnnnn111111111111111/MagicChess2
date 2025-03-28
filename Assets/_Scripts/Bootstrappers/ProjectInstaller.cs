using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Биндим глобальные штуки
        Container.Bind<GameStateManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TilesRepository>().FromComponentInHierarchy().AsSingle();
        Container.Bind<FiguresRepository>().FromComponentInHierarchy().AsSingle();

        // и т.п.
    }
}