(() => {
  // 素材预览无需 Unity 云遥测，保持截图录屏入口可离线运行。
  const originalFetch = window.fetch.bind(window);
  window.fetch = (input, options) => {
    const url = new URL(typeof input === 'string' ? input : input.url, location.href);
    if (url.hostname === 'config.uca.cloud.unity3d.com' || url.hostname === 'cdp.cloud.unity3d.com')
      return Promise.reject(new TypeError('Unity telemetry disabled in preview'));
    return originalFetch(input, options);
  };
  const mode = new URLSearchParams(location.search).get('ads') || 'success';
  const unavailable = () => { if (mode === 'nofill') throw new Error('NO_FILL (local preview)'); };
  const createAd = rewarded => ({
    loadAsync: async () => { unavailable(); },
    showAsync: async () => {
      unavailable();
      const completed = confirm(rewarded
        ? '本地模拟激励广告：确定 = 完成并发奖；取消 = 不发奖。'
        : '本地模拟插屏广告，点击确定关闭。');
      if (mode === 'cancel' || !completed) throw new Error('USER_INPUT (local preview)');
    }
  });
  window.FBInstant = {
    initializeAsync: async () => {},
    startGameAsync: async () => {},
    setLoadingProgress: () => {},
    player: { getID: () => 'local-preview-player' },
    getSupportedAPIs: () => ['getInterstitialAdAsync', 'getRewardedVideoAsync'],
    getInterstitialAdAsync: async () => { unavailable(); return createAd(false); },
    getRewardedVideoAsync: async () => { unavailable(); return createAd(true); },
    onPause: () => {}
  };
  console.info('[StrongmanBoost] Local preview: simulated SDK and ads; local-only save.');
})();
