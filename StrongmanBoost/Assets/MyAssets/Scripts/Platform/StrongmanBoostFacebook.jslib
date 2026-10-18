mergeInto(LibraryManager.library, {
  StrongmanBoost_ConfigValue: function (keyPtr) {
    var value = window.strongmanBoostConfig[UTF8ToString(keyPtr)];
    var size = lengthBytesUTF8(value) + 1, buffer = _malloc(size);
    stringToUTF8(value, buffer, size); return buffer;
  },
  StrongmanBoost_PlatformReady: function () { return window.strongmanBoostPlatformReady ? 1 : 0; },
  StrongmanBoost_RegisterLifecycle: function (namePtr) {
    var name = UTF8ToString(namePtr);
    FBInstant.onPause(function () { SendMessage(name, 'OnPlatformPause', ''); });
    document.addEventListener('visibilitychange', function () {
      SendMessage(name, document.hidden ? 'OnPageHidden' : 'OnPageVisible', '');
    });
  },
  StrongmanBoost_PrefsHas: function (keyPtr) {
    try { return localStorage.getItem(window.strongmanBoostSaveKey + ':' + UTF8ToString(keyPtr)) !== null ? 1 : 0; }
    catch (error) { console.warn('[StrongmanBoost] Storage unavailable', error); return 0; }
  },
  StrongmanBoost_PrefsGet: function (keyPtr, fallbackPtr) {
    var value = UTF8ToString(fallbackPtr);
    try {
      var saved = localStorage.getItem(window.strongmanBoostSaveKey + ':' + UTF8ToString(keyPtr));
      if (saved !== null) value = saved;
    } catch (error) { console.warn('[StrongmanBoost] Storage unavailable', error); }
    var buffer = _malloc(lengthBytesUTF8(value) + 1);
    stringToUTF8(value, buffer, lengthBytesUTF8(value) + 1);
    return buffer;
  },
  StrongmanBoost_PrefsSet: function (keyPtr, valuePtr) {
    try { localStorage.setItem(window.strongmanBoostSaveKey + ':' + UTF8ToString(keyPtr), UTF8ToString(valuePtr)); }
    catch (error) { console.warn('[StrongmanBoost] Save failed', error); }
  },
  StrongmanBoost_PrefsDelete: function (keyPtr) {
    try { localStorage.removeItem(window.strongmanBoostSaveKey + ':' + UTF8ToString(keyPtr)); }
    catch (error) { console.warn('[StrongmanBoost] Delete failed', error); }
  },
  StrongmanBoost_PrefsClear: function () {
    // 只清理本游戏当前玩家，不能清空站点的其他游戏存档。
    try {
      var prefix = window.strongmanBoostSaveKey + ':';
      Object.keys(localStorage).filter(function (key) { return key.indexOf(prefix) === 0; })
        .forEach(function (key) { localStorage.removeItem(key); });
    } catch (error) { console.warn('[StrongmanBoost] Clear failed', error); }
  }
});
