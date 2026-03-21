using InfinitePorker.Logic;
using KszUtil.Utilities;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using AudioSettings = KszUtil.Utilities.AudioSettings;

public class RootLifetimeScope : LifetimeScope
{
    [SerializeField] private AudioSettings _audioSettings;
    [SerializeField] private PorkerSetting _porkerSetting;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<ChapterProgressManager>(Lifetime.Singleton);

        builder.Register<AudioManager>(Lifetime.Singleton).AsSelf();

        builder.RegisterInstance(_porkerSetting).AsSelf();
        builder.RegisterInstance(_audioSettings).AsSelf();

        builder.RegisterBuildCallback(resolver => resolver.Resolve<AudioManager>());
    }
}