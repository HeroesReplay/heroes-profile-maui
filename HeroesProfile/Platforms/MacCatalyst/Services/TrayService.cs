using System;
using Foundation;
using HeroesProfile.UI.Platforms.Native;
using HeroesProfile.UI.Services;

namespace HeroesProfile.UI.Platforms.Services;

[Preserve]
	public class TrayService : ITrayService
	{
		public TrayService() : base()
		{
			serviceNative = new TrayServiceNative();
			serviceNative.ClickHandler = () => ClickHandler?.Invoke();
		}

		TrayServiceNative serviceNative;

		public void Initialize()
			=> serviceNative.Initialize();

		public Action ClickHandler { get; set; }
	}