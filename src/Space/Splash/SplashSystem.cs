using System.Numerics;
using Space.Assets;
using Space.Events;
using Space.Game;
using Titan.Assets;
using Titan.Audio;
using Titan.BuiltIn.Components;
using Titan.Core;
using Titan.Core.Maths;
using Titan.ECS;
using Titan.ECS.Entities;
using Titan.Events;
using Titan.Input;
using Titan.Systems;

namespace Space.Splash;

internal struct SplashSystem : ISystem
{
    private EntityManager _entityManager;
    private ComponentManager _componentManager;
    private AudioManager _audioManager;
    private InputManager _input;
    private AssetsManager _assetsManager;

    private MutableResource<GameState> _gameState;
    private MutableStorage<Transform2D> _transform;
    private EventsWriter<GameStartEvent> _gameStartEvent;

    private Handle<Asset> _musicAsset;
    private Entity _splashEntity;
    private Handle<Audio> _music;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _componentManager = init.GetComponentManager();
        _assetsManager = init.GetAssetsManager();

        _input = init.GetInputManager();

        _gameState = init.GetMutableResource<GameState>();
        _transform = init.GetMutableStorage<Transform2D>();
        _gameStartEvent = init.GetEventsWriter<GameStartEvent>();
        _audioManager = init.GetAudioManager();
    }
    public void Update()
    {
        if (_splashEntity.IsInvalid)
        {
            _splashEntity = _entityManager.Create();
            _componentManager.AddComponent(_splashEntity, Transform2D.Default);
            _componentManager.AddComponent(_splashEntity, Sprite.Default with
            {
                Layer = 1,
                Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
                Color = Color.Red,
                Pivot = Vector2.One * 0.5f,
                SourceRect = SpriteRectangles.PressE
            });
            _musicAsset = _assetsManager.Load(AssetRegistry.Manifest.Textures.SplashScreenMusic);
            _music = _audioManager.CreateAndPlay(_musicAsset, PlaybackSettings.Default with { Volume = 1f, Frequency = 1f, Loop = true });
        }

        ref var transform = ref _transform[_splashEntity];
        transform.Position = new Vector2(_gameState.Get().BoardSize.Width / 2f, 100);

        if (_input.IsKeyPressed(KeyCode.E))
        {
            _gameState.Get().CurrentState = GameStateTypes.Startup;
            _entityManager.Destroy(_splashEntity);
            _splashEntity = Entity.Invalid;

            _gameStartEvent.Send(new GameStartEvent());
        }

        if (_input.IsKeyPressed(KeyCode.Q))
        {
            _audioManager.Stop(_music);
        }

        if (_input.IsKeyPressed(KeyCode.F))
        {
            _audioManager.Pause(_music);
        }
        if (_input.IsKeyPressed(KeyCode.G))
        {
            _audioManager.Resume(_music);
        }
    }
    public bool ShouldRun() => _gameState.Get().CurrentState is GameStateTypes.Splash;
}
